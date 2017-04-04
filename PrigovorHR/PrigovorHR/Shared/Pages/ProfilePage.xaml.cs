using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private Controllers.TAPController _TAPController;
        public static byte[] ProfileImageByte;
        public delegate void ProfileUpdatedDelegate();
        public static event ProfileUpdatedDelegate ProfileUpdatedEvent;

        public ProfilePage()
        {
            InitializeComponent();

            _TAPController = new Controllers.TAPController(_imgProfilePicture);

            _btnSaveChanges.Clicked += _btnSaveChanges_Clicked;
            _EMailEntry.Completed += _EntryEMail_Completed;
            _PasswordAgainEntry.Completed += _EntryPasswordAgain_Completed;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            _TAPController.SingleTaped += _TAPController_SingleTaped;
            LoadData();
        }

        protected override bool OnBackButtonPressed()
        {
            NavigationBar.InitBackButtonPressed();
            return true;
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }

        public void LoadData()
        {
            _NameEntry.Text = Controllers.LoginRegisterController.LoggedUser.name;
            _SurnameEntry.Text = Controllers.LoginRegisterController.LoggedUser.surname;
            _TelephoneEntry.Text = Controllers.LoginRegisterController.LoggedUser.telephone;
            _EMailEntry.Text = Controllers.LoginRegisterController.LoggedUser.email;
            _EMailEntry.IsEnabled = false;
            _PasswordAgainEntry.Text = Controllers.LoginRegisterController.LoggedUser.password;
            _PasswordEntry.Text = Controllers.LoginRegisterController.LoggedUser.password;

            try
            {
                if (!string.IsNullOrEmpty(Controllers.LoginRegisterController.LoggedUser.profileimage))
                {
                    ProfileImageByte = Convert.FromBase64String(Controllers.LoginRegisterController.LoggedUser.profileimage);
                    _imgProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(ProfileImageByte));
                }
                else
                    _imgProfilePicture.Source = "person.png";
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "public void LoadData() " + "Greška u učitavanju profilne slike");
                _imgProfilePicture.Source = "person.png";
            }
        }

        private async void _TAPController_SingleTaped(string viewId, View view)
        {
            var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
            if (!string.IsNullOrEmpty(Picker.FileName))
            {
                ProfileImageByte = Picker.DataArray;
                Controllers.LoginRegisterController.LoggedUser.profileimage = Convert.ToBase64String(ProfileImageByte);
                _imgProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(ProfileImageByte));
            }

            Picker = null;
        }

        public void ResizeCircleControl()
        {
            _imgProfilePicture.WidthRequest = AppGlobal._screenWidth / 6;
            _imgProfilePicture.HeightRequest = AppGlobal._screenHeight / 6;
        }

        #region entrycontrols
        private void _EntryPasswordAgain_Completed(object sender, EventArgs e)
        {
            if (_PasswordAgainEntry.Text != _PasswordEntry.Text)
            {
                _lblPassword.Text = "Lozinka i ponovljena lozinka trebaju biti jednake!";
                _lblPassword.TextColor = Color.Red;
            }
            else
            {
                _lblPassword.Text = "Ostavite prazno ako ne želite mjenjati lozinku";
                _lblPassword.TextColor = Color.Black;

            }
        }

        private void _EntryEMail_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_EMailEntry.Text))
            {
                if (!Android.Util.Patterns.EmailAddress.Matcher(_EMailEntry.Text).Matches())
                {
                    _EMailEntry.TextColor = Color.Red;
                    _lblIncorrectEMail.IsVisible = true;
                    _EMailEntry.Focus();
                    return;
                }
                else
                {
                    _EMailEntry.TextColor = Color.Black;
                    _lblIncorrectEMail.IsVisible = false;
                }
            }
        }
        #endregion

        private void _btnSaveChanges_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_NameEntry.Text))
            {
                _NameEntry.PlaceholderColor = Color.Red;
                _NameEntry.Placeholder = "Molimo vas upišite vaše ime!";
                return;
            }
            if (string.IsNullOrEmpty(_SurnameEntry.Text))
            {
                _SurnameEntry.PlaceholderColor = Color.Red;
                _SurnameEntry.Placeholder = "Molimo vas upišite vaše prezime!";
                return;
            }
            if (string.IsNullOrEmpty(_EMailEntry.Text))
            {
                _EMailEntry.PlaceholderColor = Color.Red;
                _EMailEntry.Placeholder = "Molimo vas upišite vaš e-mail";
                return;
            }

            if (string.IsNullOrEmpty(_PasswordEntry.Text))
            {
                _PasswordEntry.PlaceholderColor = Color.Red;
                _PasswordEntry.Placeholder = "Molimo vas upišite lozinku";
                return;
            }
            else
            {
                if (_PasswordEntry.Text.Length > 5 &
                   _PasswordEntry.Text.ToCharArray().Any(chars => char.IsDigit(chars)))
                {
                    _PasswordEntry.PlaceholderColor = Color.White;
                    _lblPassword.Text = "Ostavite prazno ako ne želite mjenjati lozinku";
                }
                else
                {
                    _lblPassword.TextColor = Color.Red;
                    _lblPassword.Text = "Vaša lozinka treba sadržavati minimalno 6 znakova od čega jedan mora biti broj";
                    return;
                }

                if (_PasswordAgainEntry.Text != _PasswordEntry.Text)
                {
                    _lblPassword.Text = "Lozinka i ponovljena lozinka trebaju biti jednake!";
                    _lblPassword.TextColor = Color.Red;
                    return;
                }
                else
                {
                    _lblPassword.Text = "Ostavite prazno ako ne želite mjenjati lozinku";
                    _lblPassword.TextColor = Color.Black;
                }

                if (_PasswordAgainEntry.Text == _PasswordEntry.Text && _PasswordEntry.Text != Controllers.LoginRegisterController.LoggedUser.password)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Confirm(
                       new Acr.UserDialogs.ConfirmConfig()
                       {
                           Title = "Promjena lozinke",
                           CancelText = "Odustani",
                           OkText = "Promjeni",
                           Message = "Unijeli ste lozinku koja je drugačija od trenutne što će rezultirati promjenom lozinke" + Environment.NewLine + "Jeste li sigurni u promjenu lozinke?",
                           OnAction = (Confirm) => { if (!Confirm) { return; } else SaveChanges(); }
                       });
                }
                else SaveChanges();
            }
        }

        private async void SaveChanges()
        {
            var NewUser = new User()
            {
                email = _EMailEntry.Text,
                isLogged = true,
                isNotificationEnabled = true,
                LoginType = LoginTypeModel.eLoginType.email,
                name = _NameEntry.Text,
                profileimage = Convert.ToBase64String(ProfileImageByte),
                surname = _SurnameEntry.Text,
                telephone = _TelephoneEntry.Text,
                token = UserToken.token,
                password = _PasswordEntry.Text,
                usertype = UserType.eUserType.Basic
            };

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Ažuriram podatke");

            if (!await Controllers.LoginRegisterController.SaveUserData(NewUser, LoginTypeModel.eLoginType.email, true))
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške tijekom ažuriranja podataka!");
            else ProfileUpdatedEvent?.Invoke();

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }
    }
}
