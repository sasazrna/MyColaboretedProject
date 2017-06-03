﻿using System;

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
//using Plugin.Toasts;
using PrigovorHR.Shared;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using ScnViewGestures.Plugin.Forms.Droid.Renderers;
using Plugin.Permissions;
using Plugin.Media;
using PrigovorHR.Shared.Views;
using static PrigovorHR.Droid.AndroidServices;
using System.Globalization;
using System.Collections.Specialized;

namespace PrigovorHR.Droid
{
    [Activity(Label = "Prigovor.hr", Icon = "@drawable/logo", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTop, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static ICallbackManager CallbackManager = CallbackManagerFactory.Create();
        public static readonly string[] PERMISSIONS = new[] { "publish_actions" };
        public static Intent BackgroundService = null;
        public static bool IsUserActive = false;
        public static bool Restarted = false;
        public static int SDKVersion { get { return (int)Build.VERSION.SdkInt; } }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            ViewGesturesRenderer.Init();

            if ((int)Build.VERSION.SdkInt >= 21)
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FF6A00"));
            else
                Window.SetSoftInputMode(SoftInput.AdjustResize);

            switch (Xamarin.Forms.Device.Idiom)
            {
                case TargetIdiom.Phone:
                    RequestedOrientation = ScreenOrientation.Portrait;
                    break;
                case TargetIdiom.Tablet:
                    RequestedOrientation = ScreenOrientation.Landscape;
                    break;
            }

            //DependencyService.Register<ToastNotification>(); // Register your dependency
            //ToastNotification.Init(this);
            Acr.UserDialogs.UserDialogs.Init(() => (Activity)Forms.Context);
            FacebookSdk.SdkInitialize(Forms.Context);
            MobileBarcodeScanner.Initialize(Application);
            ImageCircleRenderer.Init();
            Xamarin.FormsMaps.Init(this, bundle);
            MobileCenter.Configure("9837e567-5433-43da-90ae-309b3bfa36c1");

            AppGlobal.AppLoaded = false;
            IsUserActive = true;

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("hr-HR");

            LoadApplication(new App());
            //BackgroundService = new Intent(this, typeof(AndroidServices.GetNewComplaintsBackgroundService));
            //StartService(BackgroundService);

            var TimeZone = Convert.ToInt32(Convert.ToDouble(DateTime.Now.Hour) / 6D);
            var RefreshTime = (1000 * 6); /*int)(RefreshValues[MainActivity.IsUserActive][TimeZone]*/

            //if (MainActivity.Restarted)
            //{
            //    MainActivity.Restarted = false;
            //    RefreshTime = 2000;
            //}

            //Intent alarmIntent = new Intent(this, typeof(AlarmReceiver));

            //var pendingIntent = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            //var alarmManager = (AlarmManager)this.GetSystemService(Context.AlarmService);

            ////TODO: For demo set after 5 seconds.
            //alarmManager.SetInexactRepeating(AlarmType.RtcWakeup,5000, 5000, pendingIntent);
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
            Restarted = true;
            if (BackgroundService != null)
                StopService(BackgroundService);

            //BackgroundService = new Intent(this, typeof(AndroidServices.GetNewComplaintsBackgroundService));
            //StartService(BackgroundService);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            IsUserActive = true;
            Restarted = true;
            if (BackgroundService != null)
                StopService(BackgroundService);

            //BackgroundService = new Intent(this, typeof(AndroidServices.GetNewComplaintsBackgroundService));
            //StartService(BackgroundService);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (ListOfComplaintsView_BasicUser.ReferenceToView != null)
            {
                MessagingCenter.Subscribe<ListOfComplaintsView_BasicUser, int>(this, "OpenComplaint", (sender, arg) => ListOfComplaintsView_BasicUser.ReferenceToView.FindAndOpenComplaint(arg));
                MessagingCenter.Send(ListOfComplaintsView_BasicUser.ReferenceToView, "OpenComplaint", intent.GetIntExtra("ComplaintId", 0));
                MessagingCenter.Unsubscribe<ListOfComplaintsView_BasicUser, int>(this, "OpenComplaint");
            }

            base.OnNewIntent(intent);
        }
    }
}
