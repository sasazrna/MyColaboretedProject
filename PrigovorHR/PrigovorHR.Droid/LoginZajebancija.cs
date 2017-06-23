using System;

using Xamarin.Forms;

//#if __ANDROID__
using Xamarin.Facebook;
using LoginManager = Xamarin.Facebook.Login.LoginManager;
using LoginResult = Xamarin.Facebook.Login.LoginResult;
using Complio.Shared.Controllers;
using System.ComponentModel.Design;
[assembly: Dependency(typeof(Complio.Droid.Login))]

namespace Complio.Droid
{
    public class FacebookCallback : Java.Lang.Object, IFacebookCallback
    {
        private MainActivity mainActivity;
        Login outer;

        public FacebookCallback(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public FacebookCallback(Login outer)
        {
            this.outer = outer;
        }

        public void OnCancel()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                outer.result.Text = "You canceled.";
            });
        }

        public void OnError(FacebookException error)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                outer.result.Text = error.Message;
            });
        }

        public void OnSuccess(Java.Lang.Object p0)
        {
            LoginResult result = p0 as LoginResult;
            Console.WriteLine("TOKEN: " + result.AccessToken.Token);
            Device.BeginInvokeOnMainThread(() =>
            {
                outer.result.Text = result.AccessToken.Token;
            });
        }
    }

    public class Login : ContentPage, IAndroidFacebookCallers
    {
        ICallbackManager callbackManager;
        Button loginButton = new Button { Text = "Sign in" };
        public Label result = new Label { Text = "Nothing happened yet" };

        public Login()
        {
            callbackManager = CallbackManagerFactory.Create();
           var callBacks = new FacebookCallback(this);
            LoginManager.Instance.RegisterCallback(callbackManager, callBacks);
      
            Content = new StackLayout
            {
                Children = {
                    loginButton,
                    result
                }
            };
            LoginManager.Instance.SetLoginBehavior(Xamarin.Facebook.Login.LoginBehavior.NativeWithFallback);
            loginButton.Clicked += (sender, args) => LoginManager.Instance.LogInWithReadPermissions((Android.App.Activity)Forms.Context, new string[] { });
        }

        ContentPage IAndroidFacebookCallers.Login()
        {
           return new Login();
        }
    }
}