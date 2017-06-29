using Newtonsoft.Json;
using Complio.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    class LoginRegisterController
    {
        public static User LoggedUser=null;
        public delegate void UserLoggedInOutHandler(bool isLogged);
        public static event UserLoggedInOutHandler _UserLoggedInOutEvent;

        public static bool IsLoggedIn { get { return LoggedUser != null; } }

        public static async Task<bool> SaveUserData(User Data, LoginTypeModel.eLoginType LoginType, bool PushToServer)
        {
            if (PushToServer)
            {
                var User = Data;
                var result = await DataExchangeServices.ChangeUserInfo(JsonConvert.SerializeObject(new EditProfileModel(User)), Convert.FromBase64String( User.profileimage));
                if (result.Contains("Error"))
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom izmjene vaših podataka!", "Izmjena podataka", "OK");
                    return false;
                }
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaši podaci su uspješno izmjenjeni", "Izmjena podataka", "OK");
            }

            LoggedUser = Data;
          //  LoggedUser.profileimage = await DataExchangeServices.GetUserAvatar(JsonConvert.SerializeObject(UserToken.token));
            SaveUserData();
            return true;
        }

        private  static async void SaveUserData()
        {
            Application.Current.Properties.Remove("User");
            Application.Current.Properties.Add("User", JsonConvert.SerializeObject(LoggedUser));
            await Application.Current.SavePropertiesAsync();
        }

        private static async void DeleteUserData()
        {
            Application.Current.Properties.Remove("User");
            Application.Current.Properties.Remove("AllComplaints");
            Application.Current.Properties.Remove("DraftComplaints");
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Shared.Models.ComplaintModel.RefToAllComplaints = null;
            await Application.Current.SavePropertiesAsync();
            LoggedUser = null;
        }

        public static async Task<bool> LoadUser()
        {
            object objuser;
            if (Application.Current.Properties.TryGetValue("User", out objuser))
                LoggedUser = JsonConvert.DeserializeObject<User>((string)objuser);

            if (LoggedUser != null)
            {
                var Logged = await LoginUser(LoginTypeModel.eLoginType.email, LoggedUser) != null;
                if (!Logged) DeleteUserData(); else ExceptionController.CheckAndSendUnsentExceptions();
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
                    if(loginData.email == "korisnik1@prigovor.hr")
                        AppGlobal.DEBUGING = true;

                    EMailLoginModel = new EMailLoginModel() { email = loginData.email, password = loginData.password };
                    ReturnedData = await DataExchangeServices.LoginUser_EMail(JsonConvert.SerializeObject(EMailLoginModel));
                    //var res= await DataExchangeServices.ResetPassword(JsonConvert.SerializeObject(EMailLoginModel));
                    break;
            }

            try
            {
                if (!ReturnedData.Contains("Error"))
                {
                    string City = LoggedUser.City;
                    LoggedUser = new User();
                    LoggedUser = JsonConvert.DeserializeObject<User>(ReturnedData);
                    LoggedUser.password = EMailLoginModel.password;
                    UserToken.token = LoggedUser.token;
                    LoggedUser.profileimage = await DataExchangeServices.GetUserAvatar(JsonConvert.SerializeObject(UserToken.token));
                    LoggedUser.City = City;
                    await SaveUserData(LoggedUser, LoginTypeModel.eLoginType.email, false);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    _UserLoggedInOutEvent?.Invoke(true);
                    return LoggedUser;
                }
                else { AppGlobal._lastError = ReturnedData; return null; }
            }
            catch (Exception ex)
            {
                ExceptionController.HandleException(ex, "private static async Task<User> ExecuteLogin(LoginTypeModel.eLoginType LoginType, dynamic loginData)");
                return null;
            }
        }

        public static void UserLogOut()
        {
            AppGlobal.DEBUGING = false;
            DeleteUserData();
            _UserLoggedInOutEvent?.Invoke(false);
        }
    }
}
