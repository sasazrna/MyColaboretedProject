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
        private Controllers.TAPController TAPController;
        public static byte[] ProfileImageByte;
        public delegate void ProfileUpdatedDelegate();
        public static event ProfileUpdatedDelegate ProfileUpdatedEvent;

        public ProfilePage()
        {
            InitializeComponent();

            TAPController = new Controllers.TAPController(imgProfilePicture);

            btnSaveChanges.Clicked += btnSaveChanges_Clicked;
            EMailEntry.Completed += EntryEMail_Completed;
            PasswordAgainEntry.Completed += EntryPasswordAgain_Completed;
            //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            TAPController.SingleTaped += TAPController_SingleTaped;
            LoadData();
        }

        public void LoadData()
        {
            NameEntry.Text = Controllers.LoginRegisterController.LoggedUser.name;
            SurnameEntry.Text = Controllers.LoginRegisterController.LoggedUser.surname;
            TelephoneEntry.Text = Controllers.LoginRegisterController.LoggedUser.telephone;
            EMailEntry.Text = Controllers.LoginRegisterController.LoggedUser.email;
            EMailEntry.IsEnabled = false;
            //_PasswordAgainEntry.Text = Controllers.LoginRegisterController.LoggedUser.password;
            //_PasswordEntry.Text = Controllers.LoginRegisterController.LoggedUser.password;

            try
            {
                if (!string.IsNullOrEmpty(Controllers.LoginRegisterController.LoggedUser.profileimage))
                {
                    ProfileImageByte = Convert.FromBase64String(Controllers.LoginRegisterController.LoggedUser.profileimage);
                    imgProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(ProfileImageByte));
                }
                else
                    imgProfilePicture.Source = "person.png";
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "public void LoadData() " + "Greška u učitavanju profilne slike");
                imgProfilePicture.Source = "person.png";
            }
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
            if (!string.IsNullOrEmpty(Picker?.FileName))
            {
                ProfileImageByte = Picker.DataArray;
                Controllers.LoginRegisterController.LoggedUser.profileimage = Convert.ToBase64String(ProfileImageByte);
                imgProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(ProfileImageByte));
            }

            Picker = null;
        }

        #region entrycontrols
        private void EntryPasswordAgain_Completed(object sender, EventArgs e)
        {
            if (PasswordAgainEntry.Text != PasswordEntry.Text)
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

        private void EntryEMail_Completed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(EMailEntry.Text))
            {
                if (!Android.Util.Patterns.EmailAddress.Matcher(EMailEntry.Text).Matches())
                {
                    EMailEntry.TextColor = Color.Red;
                    lblIncorrectEMail.IsVisible = true;
                    EMailEntry.Focus();
                    return;
                }
                else
                {
                    EMailEntry.TextColor = Color.Black;
                    lblIncorrectEMail.IsVisible = false;
                }
            }
        }
        #endregion

        private void btnSaveChanges_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                NameEntry.PlaceholderColor = Color.Red;
                NameEntry.Placeholder = "Molimo vas upišite vaše ime!";
                return;
            }
            if (string.IsNullOrEmpty(SurnameEntry.Text))
            {
                SurnameEntry.PlaceholderColor = Color.Red;
                SurnameEntry.Placeholder = "Molimo vas upišite vaše prezime!";
                return;
            }
            if (string.IsNullOrEmpty(EMailEntry.Text))
            {
                EMailEntry.PlaceholderColor = Color.Red;
                EMailEntry.Placeholder = "Molimo vas upišite vaš e-mail";
                return;
            }

            if (!string.IsNullOrEmpty(PasswordEntry.Text))
            {
                if (PasswordEntry.Text.Length > 5 &
                      PasswordEntry.Text.ToCharArray().Any(chars => char.IsDigit(chars)))
                {
                    PasswordEntry.PlaceholderColor = Color.White;
                    _lblPassword.Text = "Ostavite prazno ako ne želite mjenjati lozinku";
                }
                else
                {
                    _lblPassword.TextColor = Color.Red;
                    _lblPassword.Text = "Vaša lozinka treba sadržavati minimalno 6 znakova od čega jedan mora biti broj";
                    return;
                }

                if (PasswordAgainEntry.Text != PasswordEntry.Text)
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

                if (PasswordAgainEntry.Text == PasswordEntry.Text && PasswordEntry.Text != Controllers.LoginRegisterController.LoggedUser.password)
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
            else SaveChanges();
        }

        private async void SaveChanges()
        {
            var NewUser = new User()
            {
                email = EMailEntry.Text,
                isLogged = true,
                isNotificationEnabled = true,
                LoginType = LoginTypeModel.eLoginType.email,
                name = NameEntry.Text,
                profileimage = Controllers.LoginRegisterController.LoggedUser.profileimage,
                surname = SurnameEntry.Text,
                telephone = TelephoneEntry.Text,
                token = UserToken.token,
                password = !string.IsNullOrEmpty(PasswordEntry.Text) ? PasswordEntry.Text : Controllers.LoginRegisterController.LoggedUser.password,
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
