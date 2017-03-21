using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PrigovorHR.Shared.Views
{
    public partial class CompanyInfoView : ContentView
    {
        public CompanyInfoView()
        {
            InitializeComponent();
        }

        public CompanyInfoView(Models.CompanyModel Company)
        {
            InitializeComponent();
            lblCompanyAddress.Text = Company.address;
            lblCompanyCity.Text = Company.city?.name;
            lblCompanyDescription.Text = Company.description;
            lblCompanyName.Text = Company.name;
          //  lblCompanyWebAddress.Text = "CompanyStore.root_business.web";
           LoadMap(Company.address, Company.name);
        }

        private async void LoadMap(string address, string name)
        {
            if (!CompanyMap.Pins.Any())
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam mapu", Acr.UserDialogs.MaskType.Clear);
                string LongLat = await DataExchangeServices.GetLongLatFromAddress(address);
                JObject Jobj = JObject.Parse(LongLat);
                if (Jobj["results"].Count() > 0)
                {
                    double Lat = (double)Jobj["results"][0]["geometry"]["location"]["lat"];
                    double Long = (double)Jobj["results"][0]["geometry"]["location"]["lng"];

                    var position = new Position(Lat, Long); // Latitude, Longitude
                    CompanyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(300)));

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = name,
                        Address = address
                    };
                    CompanyMap.Pins.Add(pin);
                }
                else CompanyMap.IsVisible = false;
            }
        }
    }
}

