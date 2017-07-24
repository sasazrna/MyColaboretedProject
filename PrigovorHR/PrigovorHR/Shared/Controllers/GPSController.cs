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
using Xamarin.Forms.Maps;

namespace Complio.Shared.Controllers
{
    class GPSController
    {
        public delegate void GPSEnabledChangedHandler(bool state);
        public static event GPSEnabledChangedHandler GPSEnabledChangedEvent;
        private static bool GPSEnabledLastState = false;
        private static bool TrackingOfGPSStateStarted = false;
        public enum AddressOrCityenum { Address=0, City=2 };

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
                if (IsGPSEnabled)
                    return await CrossGeolocator.Current.GetPositionAsync(new TimeSpan(0, 0, 15));
                else throw new Exception("GPSIsOff");
            }
            catch (Exception Err)
            {
                ExceptionController.HandleException(Err, "Došlo je do greške na public static async Task<Plugin.Geolocator.Abstractions.Position> GetPosition()");

                if(numofretrys==0 & IsGPSEnabled)
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

        public static async Task<string> GetAddressOrCityFromPosition(AddressOrCityenum AddressOrCity, double Latitude, double Longitude)
        {
            if (IsGPSEnabled)
            {
                Geocoder G = new Geocoder();

                Plugin.Geolocator.Abstractions.Position Pos1 = null;

                if (Latitude == 0)
                    Pos1 = await GetPosition();
                else
                    Pos1 = new Plugin.Geolocator.Abstractions.Position() { Latitude = Latitude, Longitude = Longitude };

                Position Pos2 = new Position(Pos1.Latitude, Pos1.Longitude);
                var adr = await G.GetAddressesForPositionAsync(Pos2);

                if (adr.ToList().Any())
                {
                    if (AddressOrCity == AddressOrCityenum.City)
                    {
                        var AllCities = Models.CityModel.AllCities;
                        foreach (var AdrSplit in adr.ToList()[1].Split(','))
                            if (AllCities.Contains(AdrSplit.Replace(" ", "")))
                                return AdrSplit;

                        return string.Empty;
                    }
                    else return adr.ToList()[0];
                }
                else return string.Empty;
                //return adr.ToList().Any() ? 
                //    adr.ToList()[(int)AddressOrCity].Substring(0, adr.ToList()[(int)AddressOrCity].IndexOf("\n")) : string.Empty;
            }
            else return "GPSIsOff";
        }

        public static void OpenGPSOptions()
        {
            DependencyService.Get<IAndroidCallers>().OpenGPSSettings();
        }
    }
}
