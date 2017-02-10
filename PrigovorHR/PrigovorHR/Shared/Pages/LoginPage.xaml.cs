using Newtonsoft.Json;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class LoginPage : ContentPage
    {
        public delegate void LoginSucessfulHandler(bool Sucessfull);
        public static event LoginSucessfulHandler _LoginSucessfulEvent;

        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _EMailEntry.Completed += EmailEntry_Completed;
            btnLogin.Clicked += BtnPrijava_Clicked;
            btnIForgotPassword.Clicked += BtnZaboravioSamLozinku_Clicked;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
           await Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await NavigationBar.imgBack.RotateTo(90, 75);
                await Navigation.PopModalAsync();
            });
            return true;
        }

        private void BtnZaboravioSamLozinku_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PasswordResetPage());
        }

        private async void BtnPrijava_Clicked(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Prijava u toku....", Acr.UserDialogs.MaskType.Clear);
            await Task.Delay(20);

            var LoginModel = new Models.EMailLoginModel() { email = _EMailEntry.Text, password = _PasswordEntry.Text };
            var user = await Controllers.LoginRegisterController.LoginUser(LoginTypeModel.eLoginType.email, LoginModel);

            if(user != null)
            { 
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Navigation.PopModalAsync(true);
                _LoginSucessfulEvent?.Invoke(true);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                if (AppGlobal._lastError.Contains("442"))
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Neispravna prijava!" + Environment.NewLine + "Provjerite ispravnost unesenih podataka", "Neispravna prijava", "OK");
                else
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Greška kod prijave!" + Environment.NewLine + "Provjerite ispravnost unesenih podataka", "Neispravna prijava", "OK");
            }
        }

        private void EmailEntry_Completed(object sender, EventArgs e)
        {
            switch (((Entry)sender).Placeholder)
            {
                case "Upišite vaš email":
                    if (string.IsNullOrEmpty(_PasswordEntry.Text))
                        _PasswordEntry.Focus();
                    break;
            }
        }
    }
}
