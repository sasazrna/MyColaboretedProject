using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Complio.Shared.Views;
using System.Reflection;
using Complio.Shared.Pages;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace Complio
{
    public class App : Application
    {
        public App()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            MainPage = new NavigationPage(new SplashScreenPage());
            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () => { SetMainPage(); return false; });
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
        }

        private void SetMainPage()
        {
            try
            {
                MainPage = new NavigationPage(new LandingPage())
                {
                    BarBackgroundColor = Color.White,
                    BarTextColor = Color.Black
                };
                MainPage.SizeChanged += MainPage_SizeChanged1;
            }catch
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
        }

        private void MainPage_SizeChanged1(object sender, EventArgs e)
        {
            Shared.AppGlobal._screenHeight = (int)MainPage.Height;
            Shared.AppGlobal._screenWidth = (int)MainPage.Width;
        }

        private void NavigationPage_PoppedToRoot(object sender, NavigationEventArgs e)
        {
          //  navigationPage.BackgroundColor = Color.FromHex("#30343f");
        }

        public static Action<string> PostSuccessFacebookAction { get; set; }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
