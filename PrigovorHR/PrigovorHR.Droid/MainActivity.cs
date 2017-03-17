using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Views.InputMethods;
using Android.Content;
using Xamarin.Forms;
//using XamSvg.Shared.Cross;
using ZXing.Mobile;
using Xamarin.Facebook;
using System.Threading.Tasks;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Support.V7.App;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Toasts;
using PrigovorHR.Shared;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using ScnViewGestures.Plugin.Forms.Droid.Renderers;

namespace PrigovorHR.Droid
{
    [Activity(Label = "Prigovor.hr", Icon = "@drawable/logo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static ICallbackManager CallbackManager = CallbackManagerFactory.Create();
        public static readonly string[] PERMISSIONS = new[] { "publish_actions" };
        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;
     

            base.OnCreate(bundle);
            //var Intent = new Intent(this, typeof(Activity1));
            //var Activity = new Activity1();
           
            Forms.Init(this, bundle);
            ViewGesturesRenderer.Init();

            //StartActivity(Intent);
            //await Task.Delay(500);

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FF6A00"));
               // KeyBoardOverlayFix.assistActivity(this, WindowManager);
            }
            else
            {
                Window.SetSoftInputMode(SoftInput.AdjustResize);
            }

            switch (Xamarin.Forms.Device.Idiom)
            {
                case TargetIdiom.Phone:
                    RequestedOrientation = ScreenOrientation.Portrait;
                    break;
                case TargetIdiom.Tablet:
                    RequestedOrientation = ScreenOrientation.Landscape;
                    break;
            }

            //Act:
            //try
            //{
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init(this);
            Acr.UserDialogs.UserDialogs.Init(() => (Activity)Forms.Context);
            FacebookSdk.SdkInitialize(Forms.Context);
            MobileBarcodeScanner.Initialize(Application);
            ImageCircleRenderer.Init();
            //Activity1.refActivity1.FinishAndRemoveTask();
            //await Task.Delay(100);
            Xamarin.FormsMaps.Init(this, bundle);
            MobileCenter.Configure("9837e567-5433-43da-90ae-309b3bfa36c1");

            LoadApplication(new App());
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
               global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
        }
    }

    [Activity(Label = "Prigovor.hr", Icon = "@drawable/logo", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class Activity1 : Activity
    {
        public static Activity1 refActivity1;
        protected override void OnCreate(Bundle bundle)
        {
           base.OnCreate(bundle);
            refActivity1 = this;
            SetContentView(Resource.Layout.StartScreen);
        }

        protected override void OnResume()
        {
            base.OnResume();

            //    await Task.Delay(1);
            // Lets start from the beginning again
        }
    }
}
