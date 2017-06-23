using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    public class NetworkController
    {
        public delegate void InternetStatusChangedHandler(bool IsAvailable);
        public static event InternetStatusChangedHandler InternetStatusChanged;
        public static bool IsInternetAvailable { get { return CrossConnectivity.Current.IsConnected; } }

        public static void OpenNetworkSettings()
        {
            DependencyService.Get<IAndroidCallers>().OpenNetworkSettings();
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private static void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            InternetStatusChanged?.Invoke(e.IsConnected);
            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }
    }
}
