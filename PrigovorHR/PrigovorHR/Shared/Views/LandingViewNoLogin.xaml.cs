﻿using PrigovorHR.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
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
            await Navigation.PushModalAsync(new LoginPage());
        }

        private async void BtnCreateAccount_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegisterPage());
        }
    }
}
