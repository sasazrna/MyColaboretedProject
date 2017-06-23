using Android.Content;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    class GPSController
    {
        public delegate void GPSEnabledChangedHandler(bool state);
        public static event GPSEnabledChangedHandler GPSEnabledChangedEvent;
        private static bool GPSEnabledLastState = false;
        private static bool TrackingOfGPSStateStarted = false;

        public static bool IsGPSEnabled
        {
            get
            {
                try
                {
                    if (!TrackingOfGPSStateStarted)
                    {
                        Device.StartTimer(new TimeSpan(0, 0, 0, 5), () =>
                        {
                            var GPSLastState = CrossGeolocator.Current.IsGeolocationEnabled;
                            if (GPSLastState != GPSEnabledLastState)
                            {
                                GPSEnabledLastState = GPSLastState;
                                GPSEnabledChangedEvent?.Invoke(GPSLastState);
                            }
                            TrackingOfGPSStateStarted = true;
                            return true;
                        });
                    }
                    return CrossGeolocator.Current.IsGeolocationEnabled;
                }
                catch(Exception Err)
                {
                    ExceptionController.HandleException(Err, "Došlo je do greške na public static bool IsGPSEnabled");
                    return false;
                }
            }
        }

        public GPSController()
        {
           
        }

        private static int numofretrys = 0;
        public delegate void GetLocationExceptionHandler();
        public static event GetLocationExceptionHandler GetLocationExceptionEvent;

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetPosition()
        {
            retry:
            try
            {
                return await CrossGeolocator.Current.GetPositionAsync(15000);
            }
            catch (Exception Err)
            {
                ExceptionController.HandleException(Err, "Došlo je do greške na public static async Task<Plugin.Geolocator.Abstractions.Position> GetPosition()");

                if(numofretrys==0)
                {
                    numofretrys++;
                    goto retry;
                }
                else
                {
                    numofretrys = 0;
                    GetLocationExceptionEvent?.Invoke();
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
