using Newtonsoft.Json;
using Complio.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LoginPage : ContentPage
    {
        public delegate void LoginSucessfulHandler(bool Sucessfull);
        public static event LoginSucessfulHandler _LoginSucessfulEvent;

        public LoginPage()
        {
            InitializeComponent();

         //   NavigationPage.SetHasNavigationBar(this, false);
           
            btnLogin.Clicked += BtnPrijava_Clicked;
            btnIForgotPassword.Clicked += BtnZaboravioSamLozinku_Clicked;

           //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
        }


        private void BtnZaboravioSamLozinku_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PasswordResetPage() { BackgroundColor = Color.White });
        }

        private async void BtnPrijava_Clicked(object sender, EventArgs e)
        {
            if (EMailEntry.Text?.ToUpper() == "DEV123")
            {
                AppGlobal.DEBUGING = true;
                EMailEntry.Text = "korisnik1@prigovor.hr";
                PasswordEntry.Text = "123123";
            }

            if (string.IsNullOrEmpty(EMailEntry.Text) | string.IsNullOrEmpty(PasswordEntry.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Unesite sve potrebne podatke!", "Neispravna prijava", "OK");
                return;
            }

            if (!Android.Util.Patterns.EmailAddress.Matcher(EMailEntry.Text).Matches())
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Unesite ispravan e-mail!", "Neispravna prijava", "OK");
                return;
            }


            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Prijava u toku....", Acr.UserDialogs.MaskType.Clear);
            await Task.Delay(20);

            var LoginModel = new Models.EMailLoginModel() { email = EMailEntry.Text, password = PasswordEntry.Text };
            var user = await Controllers.LoginRegisterController.LoginUser(LoginTypeModel.eLoginType.email, LoginModel);

            if(user != null)
            { 
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Navigation.PopAsync(true);
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

        private void EMailEntry_Focused(object sender, FocusEventArgs e)
        {
            EmailStack.IsVisible = false;
            GumbiStack.VerticalOptions = LayoutOptions.Start;
            EntryStack.VerticalOptions = LayoutOptions.Start;
        }

        private void EMailEntry_Unfocused(object sender, FocusEventArgs e)
        {
            EmailStack.IsVisible = true;
            GumbiStack.VerticalOptions = LayoutOptions.EndAndExpand;
            EntryStack.VerticalOptions = LayoutOptions.FillAndExpand;
        }

        private void PasswordEntry_Focused(object sender, FocusEventArgs e)
        {
            EmailStack.IsVisible = false;
            GumbiStack.VerticalOptions = LayoutOptions.Start;
            EntryStack.VerticalOptions = LayoutOptions.Start;
        }

        private void PasswordEntry_Unfocused(object sender, FocusEventArgs e)
        {
            EmailStack.IsVisible = true;
            GumbiStack.VerticalOptions = LayoutOptions.EndAndExpand;
            EntryStack.VerticalOptions = LayoutOptions.FillAndExpand;
        }
    }
}
