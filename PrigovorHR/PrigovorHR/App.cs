using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PrigovorHR.Shared.Views;
using System.Reflection;
using PrigovorHR.Shared.Pages;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace PrigovorHR
{
    public class App : Application
    {
        public delegate void OrientationChangedHandler();
        public static event OrientationChangedHandler _OrientationChanged;
        public static string _ApplicationFont = Xamarin.Forms.Device.OnPlatform("MarkerFelt-Thin", "Droid Sans Mono", "Comic Sans MS");
        public App()
        {
            //   MainPage = new NavigationPage((Page)new Shared.Pages.MasterDetail());
            //    MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);

            //   MainPage = navigationPage;
            //  MainPage = new WellComePage();
            //  navigationPage.PoppedToRoot += NavigationPage_PoppedToRoot;
            //  NavigationPage.SetHasNavigationBar(navigationPage, true);
            var assembly = typeof(App).GetTypeInfo().Assembly;

            MainPage = new NavigationPage(new SplashScreenPage());
            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () => { SetMainPage(); return false; });
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
        }

        private void SetMainPage()
        {
            MainPage = new NavigationPage(new LandingPage())
            {
                BarBackgroundColor = Color.FromHex("#7dbbe6"),
                BarTextColor = Color.Black
            };
            MainPage.SizeChanged += MainPage_SizeChanged1;
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

        private void MainPage_SizeChanged(object sender, EventArgs e)
        {
            _OrientationChanged?.Invoke();
           // MainActivity();
        }

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
