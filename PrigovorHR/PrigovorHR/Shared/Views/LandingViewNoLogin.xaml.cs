using PrigovorHR.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LandingViewNoLogin : ContentView
    {
        public LandingViewNoLogin()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            btnCreateAccount.Clicked += BtnCreateAccount_Clicked;
            _btnLogin.Clicked += _btnLogin_Clicked;
        }

        private async void _btnLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private async void BtnCreateAccount_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new RegisterPage()) { BackgroundColor = Color.White });
        }
    }
}
