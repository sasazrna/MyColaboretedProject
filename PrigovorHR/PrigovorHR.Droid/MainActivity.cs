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
using Plugin.Permissions;
using Plugin.Media;
using PrigovorHR.Shared.Views;

namespace PrigovorHR.Droid
{
    [Activity(Label = "Prigovor.hr", Icon = "@drawable/logo", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTop, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static ICallbackManager CallbackManager = CallbackManagerFactory.Create();
        public static readonly string[] PERMISSIONS = new[] { "publish_actions" };
        public static Intent BackgroundService = null;
        public static bool IsUserActive = false;

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

            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init(this);
            Acr.UserDialogs.UserDialogs.Init(() => (Activity)Forms.Context);
            FacebookSdk.SdkInitialize(Forms.Context);
            MobileBarcodeScanner.Initialize(Application);
            ImageCircleRenderer.Init();
            Xamarin.FormsMaps.Init(this, bundle);
            MobileCenter.Configure("9837e567-5433-43da-90ae-309b3bfa36c1");

            AppGlobal.AppLoaded = false;
            IsUserActive = true;

            LoadApplication(new App());
            //if (!AndroidServices.GetNewComplaintsBackgroundService.IsRunning)
            //{
              BackgroundService = new Intent(this, typeof(AndroidServices.GetNewComplaintsBackgroundService));
              StartService(BackgroundService);
          
            //}
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
               //global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
               PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
          base.OnDestroy();
          IsUserActive = false;
          //BackgroundService = new Intent(this, typeof(AndroidServices.GetNewComplaintsBackgroundService));
          //StartService(BackgroundService);
        }

        protected override void OnPause()
        {
            base.OnPause();
            IsUserActive = false;
        }
        protected override void OnResume()
        {
            base.OnResume();
            IsUserActive = true;
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            IsUserActive = true;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (ListOfComplaintsView_BasicUser.ReferenceToView != null)
            {
                MessagingCenter.Subscribe<ListOfComplaintsView_BasicUser, int>(this, "OpenComplaint", (sender, arg) => ListOfComplaintsView_BasicUser.ReferenceToView.FindAndOpenComplaint(arg));
                MessagingCenter.Send(ListOfComplaintsView_BasicUser.ReferenceToView, "OpenComplaint", intent.GetIntExtra("ComplaintId", 0));
                MessagingCenter.Unsubscribe<ListOfComplaintsView_BasicUser, int>(this, "OpenComplaint");
            }
        }
    }
}
