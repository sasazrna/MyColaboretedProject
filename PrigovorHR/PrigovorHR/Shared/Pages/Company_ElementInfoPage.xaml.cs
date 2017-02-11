using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PrigovorHR.Shared.Pages
{
    public partial class Company_ElementInfoPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private CompanyElementRootModel CompanyElement;
        public Company_ElementInfoPage(CompanyElementRootModel companyElement)
        {
            InitializeComponent();
            CompanyElement = companyElement;
            SetData(CompanyElement);
            TAPController = new Controllers.TAPController(lytPreviousElementControl, lytCompanyInfo, imgCompanyDetails, NavigationBar.imgBack);
            TAPController.SingleTaped += TAPController_SingleTaped;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            CompanyElementsListView.ElementSelectedEvent += CompanyElementsListView_ElementSelectedEvent;
        }

        private void CompanyElementsListView_ElementSelectedEvent(int ElementId)
        {
            SetData(CompanyElement,
                CompanyElement.element.id == ElementId ?
                CompanyElement.element :
                CompanyElement.siblings.Single(sib => sib.id == ElementId));

             //CompanyElement = JsonConvert.DeserializeObject<CompanyElementRootModel>(
             //await DataExchangeServices.GetCompanyElementData(_CompaniesStoresFoundInfo.First(com => com.id == ElementId).slug));
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await NavigationBar.imgBack.RotateTo(90, 75);
                await Navigation.PopModalAsync(true);
            });
            return true;
        }

        public async void SetData(CompanyElementRootModel companyElement, CompanyElementModel OtherElement=null)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje poslovnice", Acr.UserDialogs.MaskType.Clear);
            var CompanyElement = OtherElement ?? companyElement.element;
         
            NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height;
            lblCompanyAddress.Text = CompanyElement.root_business.address;
            lblCompanyCity.Text = CompanyElement.root_business.city?.name;
            lblCompanyDescription.Text = CompanyElement.root_business.description;
            lblCompanyName.Text = CompanyElement.root_business.name;
            lblCompanyWebAddress.Text = "CompanyStore.root_business.web";

            lblElementAddress.Text = CompanyElement.address;
            lblElementDescription.Text = CompanyElement.description;
            lblElementType.Text = CompanyElement.type?.name;
            lblLocation.Text = CompanyElement.location_tag;
            lblStoreName.Text = CompanyElement.name;
            lblWorkTime.Text = CompanyElement.working_hours;

            if (!string.IsNullOrEmpty(CompanyElement.address))
            {
                string LongLat = await DataExchangeServices.GetLongLatFromAddress(CompanyElement.address);
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
                        Label = CompanyElement.name,
                        Address = CompanyElement.address
                    };
                    ElementMap.Pins.Clear();
                    ElementMap.Pins.Add(pin);
                }
                else ElementMap.IsVisible = false;
            }
            else ElementMap.IsVisible = false;

            if (companyElement.siblings != null)
                CompanyElementsListView.DisplayData(companyElement.siblings);
            else
                CompanyElementsListView.IsVisible = false;

            CompanyMap.HeightRequest = 200;
            ElementMap.HeightRequest = 200;
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == lytPreviousElementControl)
            {
                //going back controller
            }
            else if (view == lytCompanyInfo | view == imgCompanyDetails)
            {
                lytCompanyDescription.IsVisible = !lytCompanyDescription.IsVisible;

                if (!CompanyMap.Pins.Any())
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam mapu", Acr.UserDialogs.MaskType.Clear);
                    string LongLat = await DataExchangeServices.GetLongLatFromAddress(CompanyElement.element.root_business.address);
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
                            Label = CompanyElement.element.root_business.name,
                            Address = CompanyElement.element.root_business.address
                        };
                        CompanyMap.Pins.Add(pin);
                    }
                    else CompanyMap.IsVisible = false;
                }
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height;
        }
    }
}
