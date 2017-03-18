using Newtonsoft.Json.Linq;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PrigovorHR.Shared.Views
{
    public partial class CompanyElementInfoView : ContentView
    {
        public CompanyElementInfoView()
        {
        }

        public CompanyElementInfoView(CompanyElementModel CompanyElement)
        {
            InitializeComponent();
            lblElementName.Text = CompanyElement.name;
            lblElementAddress.Text = CompanyElement.address;
            lblElementDescription.Text = CompanyElement.description;
            lblElementType.Text = CompanyElement.type?.name;
            lblLocation.Text = CompanyElement.location_tag;
            lblWorkTime.Text = CompanyElement.working_hours;
            LoadMap(CompanyElement.address, CompanyElement.name);
            LoadCompanySections(CompanyElement);
        }

        private async void LoadMap(string address, string name)
        {
            if (!string.IsNullOrEmpty(address))
            {
                string LongLat = await DataExchangeServices.GetLongLatFromAddress(address);
                JObject rss = JObject.Parse(LongLat);
                if (rss["results"].Count() > 0)
                {
                    double Lat = (double)rss["results"][0]["geometry"]["location"]["lat"];
                    double Long = (double)rss["results"][0]["geometry"]["location"]["lng"];

                    var position = new Position(Lat, Long); // Latitude, Longitude
                    ElementMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMeters(300)));

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = name,
                        Address = address
                    };
                    ElementMap.Pins.Clear();
                    ElementMap.Pins.Add(pin);
                    ElementMap.IsVisible = true;
                }
                else ElementMap.IsVisible = false;
            }
            else ElementMap.IsVisible = false;
        }

        private void LoadCompanySections(CompanyElementModel CompanyElement)
        {
            if (CompanyElement.children.Any())
                CompanyElementSectionViewList = new CompanyElementSectionViewList(CompanyElement);
            else CompanyElementSectionViewList.IsVisible = false;
        }
    }
}
