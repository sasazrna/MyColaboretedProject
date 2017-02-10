using Newtonsoft.Json;
using PrigovorHR.Shared.Controllers;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            EmailEntry.Completed += Entry_Completed;
            NameEntry.Completed += Entry_Completed;
            SurnameEntry.Completed += Entry_Completed;
            PasswordEntry.Completed += Entry_Completed;
            btnRegister.Clicked += btnRegister_Clicked;
        }

        private void BtnZaboravioSamLozinku_Clicked(object sender, EventArgs e)
        {
         //otvori prozor za povrat lozinke
        }

        private async void btnRegister_Clicked(object sender, EventArgs e)
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
            if (string.IsNullOrEmpty(EmailEntry.Text))
            {
                EmailEntry.PlaceholderColor = Color.Red;
                EmailEntry.Placeholder = "Molimo vas upišite vaš e-mail";
                return;
            }
            if (string.IsNullOrEmpty(PasswordEntry.Text))
            {
                PasswordEntry.PlaceholderColor = Color.Red;
                PasswordEntry.Placeholder = "Molimo vas upišite lozinku";
                return;
            }
            else
            {
                if (PasswordEntry.Text.Length > 5 &
                   PasswordEntry.Text.ToCharArray().Any(chars => char.IsDigit(chars)))
                {
                    PasswordEntry.PlaceholderColor = Color.White;
                    lblPassword.Text = "Upišite lozinku:";
                }
                else
                {
                    lblPassword.TextColor = Color.Red;
                    lblPassword.Text = "Vaša lozinka treba sadržavati minimalno 6 znakova od čega jedan mora biti broj";
                    return;
                }
            }

            var NewUser = new User()
            {
                email = EmailEntry.Text,
                isLogged = true,
                isNotificationEnabled = true,
                LoginType = LoginTypeModel.eLoginType.email,
                name = NameEntry.Text,
                profileimage = null,
                surname = SurnameEntry.Text,
                telephone = "",
                token = string.Empty,
                password = PasswordEntry.Text,
                usertype = UserType.eUserType.Basic
            };
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registracija u toku....", Acr.UserDialogs.MaskType.Clear);
            await Task.Delay(20);


            var user = await LoginRegisterController.RegisterUser(NewUser);
            if (user != null)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Navigation.PopModalAsync(true);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                if (AppGlobal._lastError.Contains("422"))
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Navedeni e-mail već postoji!" + Environment.NewLine + "Odaberite prijavu s tim e-mailom ili registraciju s drugim e-mailom", "E-mail već postoji", "OK");
            }
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            NameEntry.Placeholder = "Upišite vaše ime";
            SurnameEntry.Placeholder = "Upišite vaše prezime";
            EmailEntry.Placeholder = "Upišite vaš email";
            PasswordEntry.Placeholder = "Upišite lozinku";

            switch (((Entry)sender).Placeholder)
            {
                case "Upišite vaše ime":
                    if (string.IsNullOrEmpty(SurnameEntry.Text) & !string.IsNullOrEmpty(NameEntry.Text))
                    {
                        NameEntry.BackgroundColor = Color.White;
                        SurnameEntry.Focus();
                    }
                    break;

                case "Upišite vaše prezime":
                    if (string.IsNullOrEmpty(EmailEntry.Text) & !string.IsNullOrEmpty(SurnameEntry.Text))
                    {
                        SurnameEntry.BackgroundColor = Color.White;
                        EmailEntry.Focus();
                    }
                    break;

                case "Upišite vaš email":
       
                    if (!string.IsNullOrEmpty(EmailEntry.Text))
                    {
                        if (!Android.Util.Patterns.EmailAddress.Matcher(EmailEntry.Text).Matches())
                        {
                            EmailEntry.TextColor = Color.Red;
                            lblEmail.Text = "Molim vas upišite ispravan e-mail";
                            lblEmail.TextColor = Color.Red;
                            EmailEntry.Focus();
                            return;
                        }
                        else
                        {
                            EmailEntry.TextColor = Color.Black;
                            lblEmail.TextColor = Color.Silver;
                            lblEmail.Text = "upišite e-mail";
                        }

                        if (string.IsNullOrEmpty(PasswordEntry.Text) & !string.IsNullOrEmpty(EmailEntry.Text))
                        {
                            EmailEntry.BackgroundColor = Color.White;
                            PasswordEntry.Focus();
                        }
                    }
                    break;

                case "Odaberite lozinku":
                    if (!string.IsNullOrEmpty(PasswordEntry.Text))
                        btnRegister.Focus();
                    else
                        EmailEntry.BackgroundColor = Color.White;
                    break;
            }
        }
    }
}
