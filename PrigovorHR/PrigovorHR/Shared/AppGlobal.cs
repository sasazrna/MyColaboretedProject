using PrigovorHR.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared
{
    public class AppGlobal
    {
        public static int _screenWidth;
        public static int _screenHeight;
        public static string _lastError { get; set; }
        public static bool AppLoaded { get; set; } = false;
        public static bool DEBUGING = false;

        public static void CloseApp()
        {
            DependencyService.Get<IAndroidCallers>().CloseApp();
        }

        public static int GetAndroidSDKVersion()
        {
            return DependencyService.Get<Controllers.IAndroidCallers>().GetSDKVersion();
        }
    }
}

