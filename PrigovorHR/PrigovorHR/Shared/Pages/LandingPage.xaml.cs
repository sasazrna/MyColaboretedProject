
using Newtonsoft.Json;
using PrigovorHR.Shared.Controllers;
using PrigovorHR.Shared.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LandingPage 
    {
        private LandingViewNoLogin LandingViewNoLogin=null;
        private LandingPageWithLogin LandingViewWithLogin=null;
        public LandingPage()
        {
            InitializeComponent();
            Content = new SplashScreenView();
            NavigationPage.SetHasNavigationBar(this, false);
            LoginRegisterController._UserLoggedInOutEvent += LoginRegisterController__UserLoggedInOutEvent;
            Task.Run(() => { LoadUser(); });
        }

        private async void LoadUser()
        {
            if (NetworkController.IsInternetAvailable)
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Prijava u toku", Acr.UserDialogs.MaskType.Clear);
                await Task.Delay(100);

                if (!await LoginRegisterController.LoadUser())
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                       LandingViewNoLogin = new LandingViewNoLogin();
                        Content = LandingViewNoLogin;
                        BackgroundColor = Color.FromHex("#30343f");
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    });
                }
            }
            else
                Acr.UserDialogs.UserDialogs.Instance.ActionSheet(new Acr.UserDialogs.ActionSheetConfig()
                {
                    Cancel = new Acr.UserDialogs.ActionSheetOption("Odustani", (() =>
                    {
                        AppGlobal.CloseApp();
                    })),

                    Title = "Internet konekcija nije dostupna, želite li aktivirati internet konekciju?",
                    Options = new List<Acr.UserDialogs.ActionSheetOption>()
                    { new Acr.UserDialogs.ActionSheetOption("DA", (()=> { OpenNetworkConnectionSettingsAndWait(); })),
                      new Acr.UserDialogs.ActionSheetOption("NE", (()=> { AppGlobal.CloseApp();})) }
                });
        }

        private void OpenNetworkConnectionSettingsAndWait()
        {
            NetworkController.OpenNetworkSettings();
            NetworkController.InternetStatusChanged += (bool IsAvailable) =>
            {
                if (IsAvailable)
                    Task.Run(() => { LoadUser(); });
            };
        }

        private void LoginRegisterController__UserLoggedInOutEvent(bool isLogged)
        {
            Device.BeginInvokeOnMainThread(() =>
           {
               if (!isLogged)
               {
                   LandingViewNoLogin = new LandingViewNoLogin();
                   Content = LandingViewNoLogin;
                   BackgroundColor = Color.FromHex("#30343f");
                   Navigation.PopToRootAsync();
                }
               else
               {
                   //LandingViewWithLogin = new LandingViewWithLogin();
                   //Content = LandingViewWithLogin;
                   App.Current.MainPage = new NavigationPage(new APPMasterDetailPage()) { BarBackgroundColor = Color.Blue };

                   BackgroundColor = Color.White;
               }
           });
        }

        protected override bool OnBackButtonPressed()
        {
                return base.OnBackButtonPressed();
        }
    }
}
