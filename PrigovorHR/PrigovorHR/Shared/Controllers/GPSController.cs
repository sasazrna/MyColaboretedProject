using Android.Content;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Controllers
{
    class GPSController
    {
        public delegate void GPSEnabledChangedHandler(bool state);
        public static event GPSEnabledChangedHandler _GPSEnabledChangedEvent;
        private static bool _GPSEnabledLastState = false;
        private static bool _TrackingOfGPSStateStarted = false;
        public static bool _GPSEnabled
        {
            get
            {
                try
                {
                    if (!_TrackingOfGPSStateStarted)
                    {
                        Device.StartTimer(new TimeSpan(0, 0, 0, 5), () =>
                        {
                            var GPSLastState = CrossGeolocator.Current.IsGeolocationEnabled;
                            if (GPSLastState != _GPSEnabledLastState)
                            {
                                _GPSEnabledLastState = GPSLastState;
                                _GPSEnabledChangedEvent?.Invoke(GPSLastState);
                            }
                            _TrackingOfGPSStateStarted = true;
                            return true;
                        });
                    }
                    return CrossGeolocator.Current.IsGeolocationEnabled;
                }
                catch
                {
                    return false;
                }
            }
        }

        public GPSController()
        {
           
        }

        private static int numofretrys = 0;
        public delegate void GetLocationExceptionHandler();
        public static event GetLocationExceptionHandler _GetLocationExceptionEvent;

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetPosition()
        {
            retry:
            try
            {
               return await CrossGeolocator.Current.GetPositionAsync(10000);
            }
            catch
            {
                if(numofretrys==0)
                {
                    numofretrys++;
                    goto retry;
                }
                else
                {
                    numofretrys = 0;
                    _GetLocationExceptionEvent?.Invoke();
                    return null;
                }
            }
        }

        public static void OpenGPSOptions()
        {
            DependencyService.Get<IAndroidCallers>().OpenGPSSettings();
        }
    }
}
