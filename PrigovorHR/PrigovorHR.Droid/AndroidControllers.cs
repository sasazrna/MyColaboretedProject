using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PrigovorHR.Droid;
using PrigovorHR.Shared.Controllers;

using System.IO;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using static PrigovorHR.Droid.AndroidCallers;
using Xamarin.Auth;
using Xamarin.Facebook.Login;
using PrigovorHR.Shared.Views;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Widget;
using System.Net;
using Android.Webkit;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidCallers))]
//[assembly: Xamarin.Forms.Dependency(typeof(LoginPageRenderer))]
//[assembly: ExportRenderer(typeof(LoginPageRenderer), typeof(PrigovorHR.Shared.Pages.RegisterLoginPage))]

namespace PrigovorHR.Droid
{
    class AndroidCallers : IAndroidCallers
    {
        public AndroidCallers() { }

        public void OpenGPSSettings()
        {
            Intent gpsSettingIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            Xamarin.Forms.Forms.Context.StartActivity(gpsSettingIntent);
        }

        public void OpenNetworkSettings()
        {
            Intent gpsSettingIntent = new Intent(Android.Provider.Settings.ActionWifiSettings);
            Xamarin.Forms.Forms.Context.StartActivity(gpsSettingIntent);
        }

        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public void SaveFile(string FileName, byte[] FileData)
        {
            if (!Directory.Exists(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/"))
                Directory.CreateDirectory(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/");

            FileStream FS = new FileStream(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/" + FileName, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS);
            BW.Write(FileData);
            //FS.Flush();
            //FS.Close();
            BW.Close();
            FS.Close();
            BW.Dispose();
            FS.Dispose();
        }
            
        


        public void OpenFile(string FileName)
        {
         //   var path = System.IO.Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.Path, "doc" + FixedDocumentationLanguageCode + ".pdf");

            var FullAddress = Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/" + FileName;
            var FileInfo = new System.IO.FileInfo(FullAddress);
             var intent = new Intent(Intent.ActionView);
           Android.Net.Uri pdfFile = Android.Net.Uri.FromFile(new Java.IO.File(FullAddress));
            var MimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(FileInfo.Extension.Replace(".",""));
            intent.SetDataAndType(pdfFile, MimeType );
            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
            intent.SetFlags(ActivityFlags.NewTask);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset);

           new ContextWrapper(Forms.Context).StartActivity(intent);
            //   Device.OpenUri(new Uri(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/" + FileName));
        }

       
        #region sound recording
        private Android.Media.MediaRecorder _recorder = new Android.Media.MediaRecorder();
        private Android.Media.MediaPlayer _player = new Android.Media.MediaPlayer();
        private string _pathtolastsavedrecording = string.Empty;

        public void RecordSound(string nameofelement)
        {
            _recorder.SetAudioSource(Android.Media.AudioSource.Mic);
            _recorder.SetOutputFormat(Android.Media.OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(Android.Media.AudioEncoder.HeAac);
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, nameofelement + ".mp3");
            _pathtolastsavedrecording = filePath;
            _recorder.SetOutputFile(filePath);
            _recorder.Prepare();
            _recorder.Start();
        }

        public void StopRecordingSound()
        {
            _recorder.Stop();
            _recorder.Reset();
        }

        public void PlayRecordedSound()
        {
            _player.SetDataSource(_pathtolastsavedrecording);
            _player.Prepare();
            _player.Start();
        }

        #endregion

        ImageSource IAndroidCallers.ConvertUrlToImage(string url)
        {
            var webClient = new WebClient();
            return ImageSource.FromStream(() => new MemoryStream(webClient.DownloadData(new Uri(url))));
        }


    }

    public class DroidGoogleOAuth2Authenticator : OAuth2Authenticator
    {
        public DroidGoogleOAuth2Authenticator() : base("16014409697-pta8q7f8jvkp120jes2jcd6bq53nkqdp.apps.googleusercontent.com",
                                                 "4cXHbgpEuN3WBCUxufsoxG1k",
                                                 "email",
                                                 new Uri("https://accounts.google.com/o/oauth2/auth"),
                                                 new Uri("https://prigovor.hr/auth/google/callback"),
                                                 new Uri("https://accounts.google.com/o/oauth2/token"))
        {

        }
        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            //Remove state from dictionaries.
            //We are ignoring request state forgery status
            // as we're hitting an ASP.NET service which forwards 
            // to a third - party OAuth service itself
            if (query.ContainsKey("state"))
                query.Remove("state");

            if (fragment.ContainsKey("state"))
                fragment.Remove("state");

            base.OnPageEncountered(url, query, fragment);
        }
    }

    public class DroidFaceBookOAuth2Authenticator : OAuth2Authenticator
    {
        public DroidFaceBookOAuth2Authenticator() : base("234937896889074",
                                                          "8e3d12d2796cebaf81e1dd70f78f5ff7",
                                                         "",
                                                         new Uri("https://m.facebook.com/dialog/oauth/"), 
                                                         new Uri("https://www.facebook.com/connect/login_success.html"), 
                                                         new Uri("https://graph.facebook.com/oauth/access_token"))
        // new Uri("http://www.facebook.com/connect/login_success.html"))
        {

        }
        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            //Remove state from dictionaries.
            //We are ignoring request state forgery status
            // as we're hitting an ASP.NET service which forwards 
            // to a third - party OAuth service itself
            if (query.ContainsKey("state"))
                query.Remove("state");

            if (fragment.ContainsKey("state"))
                fragment.Remove("state");

            base.OnPageEncountered(url, query, fragment);
        }
    }

}
