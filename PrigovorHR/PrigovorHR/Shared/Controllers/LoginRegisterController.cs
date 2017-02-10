using Newtonsoft.Json;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Controllers
{
    class LoginRegisterController
    {
        public static User _LoggedUser=null;
        public delegate void UserLoggedInOutHandler(bool isLogged);
        public static event UserLoggedInOutHandler _UserLoggedInOutEvent;

        public static bool IsLoggedIn { get { return _LoggedUser != null; } }

        public static async Task<bool> SaveUserData(object Data, LoginTypeModel.eLoginType LoginType, bool PushToServer)
        {
            if (PushToServer)
            {
                var result = await DataExchangeServices.ChangeUserInfo(JsonConvert.SerializeObject(new EditProfileModel((User)Data)));
                if (result.Contains("Error"))
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom izmjene vaših podataka!", "Izmjena podataka", "OK");
                    return false;
                }
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaši podaci su uspješno izmjenjeni", "Izmjena podataka", "OK");
            }
            _LoggedUser = (User)Data;
            SaveUserData();
            return true;
        }

        private  static async void SaveUserData()
        {
            Application.Current.Properties.Remove("User");
            Application.Current.Properties.Add("User", JsonConvert.SerializeObject(_LoggedUser));
            await Application.Current.SavePropertiesAsync();
        }

        private static async void DeleteUserData()
        {
            Application.Current.Properties.Remove("User");
            await Application.Current.SavePropertiesAsync();
            _LoggedUser = null;
        }

        public static async Task<bool> LoadUser()
        {
            object objuser;
            if (Application.Current.Properties.TryGetValue("User", out objuser))
                _LoggedUser = JsonConvert.DeserializeObject<User>((string)objuser);

            if (_LoggedUser != null)
            {
                var Logged = await LoginUser(LoginTypeModel.eLoginType.email, _LoggedUser) != null;
                if (!Logged) DeleteUserData();
                return Logged;
            }
            else
                return false;
        }

        public static async Task<User> LoginUser(LoginTypeModel.eLoginType LoginType, dynamic loginData)
        {
            return await ExecuteLogin(LoginType, loginData);
        }

        public static async Task<User> RegisterUser(User RegisterData)
        {
            var RegisterUserModel = new RegisterUserModel() { name = RegisterData.name, surname = RegisterData.surname, email = RegisterData.email, password = RegisterData.password, phone = string.Empty };
            var ReturnedData = await DataExchangeServices.RegisterUser_EMail(JsonConvert.SerializeObject(RegisterUserModel));

            if(!ReturnedData.Contains("Error"))
            {
                return await LoginUser(LoginTypeModel.eLoginType.email, RegisterData);
            }
            else
            {
                AppGlobal._lastError = ReturnedData;
                return null;
            }
        }

        private static async Task<User> ExecuteLogin(LoginTypeModel.eLoginType LoginType, dynamic loginData)
        {
            var EMailLoginModel = new EMailLoginModel();
            var ReturnedData = string.Empty;

            switch (LoginType)
            {
                case LoginTypeModel.eLoginType.email:
                    EMailLoginModel = new EMailLoginModel() { email = loginData.email, password = loginData.password };
                    ReturnedData = await DataExchangeServices.LoginUser_EMail(JsonConvert.SerializeObject(EMailLoginModel));
                    //var res= await DataExchangeServices.ResetPassword(JsonConvert.SerializeObject(EMailLoginModel));
                    break;
            }

            if (!ReturnedData.Contains("Error"))
            {
                _LoggedUser = new User();
                _LoggedUser = JsonConvert.DeserializeObject<User>(ReturnedData);
                _LoggedUser.password = EMailLoginModel.password;
                UserToken.token = _LoggedUser.token;
                _LoggedUser.profileimage = await DataExchangeServices.GetUserAvatar(JsonConvert.SerializeObject(UserToken.token));
                await SaveUserData(_LoggedUser, LoginTypeModel.eLoginType.email, false);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                _UserLoggedInOutEvent?.Invoke(true);
                return _LoggedUser;
            }
            else { AppGlobal._lastError = ReturnedData; return null; }
        }

        public static void UserLogOut()
        {
            DeleteUserData();
            _UserLoggedInOutEvent?.Invoke(false);
        }
    }
}
