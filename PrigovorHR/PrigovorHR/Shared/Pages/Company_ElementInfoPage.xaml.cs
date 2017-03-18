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
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Company_ElementInfoPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private CompanyElementRootModel CompanyElement;
        public static Company_ElementInfoPage ReferenceToView;

        public Company_ElementInfoPage(CompanyElementRootModel companyElement)
        {
            InitializeComponent();
            CompanyElement = companyElement;
            SetData(CompanyElement);
            TAPController = new Controllers.TAPController(lytPreviousElementControl, lytCompanyInfo, imgCompanyDetails, NavigationBar.imgBack, lblOtherCompanyElements);
            TAPController.SingleTaped += TAPController_SingleTaped;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            btnWriteComplaint.Clicked += BtnWriteComplaint_Clicked;
            imgCompanyDetails.Text = Views.FontAwesomeLabel.Images.FAMapMarker;
            ReferenceToView = this;
            //lblStore.Text = Views.FontAwesomeLabel.Images.FACube;
            //lblStore.TextColor = Color.FromHex("FF7e65");

            //lblCompany.Text = Views.FontAwesomeLabel.Images.FACubes;
            //lblCompany.TextColor = Color.FromHex("FF7e65");

            //lblOtherStores.Text = Views.FontAwesomeLabel.Images.FAList;
            //lblOtherStores.TextColor = Color.FromHex("FF7e65");
            StoreUnder.IsVisible = true;
            CompanyUnder.IsVisible = false;
            OtherStoresUnder.IsVisible = false;

            LogoStack.IsVisible = true;
            ElementMap.IsVisible = false;



            OtherStoresStack.IsVisible = false;
            StoreStack.IsVisible = true;
            CompanyDescriptionStack.IsVisible = false;

            imgElementDetails.Text = Views.FontAwesomeLabel.Images.FAMapMarker;


            var StoreGestureRecognizer = new TapGestureRecognizer();
            StoreGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = true;
                CompanyUnder.IsVisible = false;
                OtherStoresUnder.IsVisible = false;
                StoreStack.IsVisible = true;
                CompanyDescriptionStack.IsVisible = false;
                OtherStoresStack.IsVisible = false;
                LogoStack.IsVisible = true;
            };
            lblStore.GestureRecognizers.Add(StoreGestureRecognizer);


            //Paljenje taba zatvorenih prigovora, gašenje taba aktivnih prigovora

            var CompanyGestureRecognizer = new TapGestureRecognizer();
            CompanyGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = false;
                CompanyUnder.IsVisible = true;
                OtherStoresUnder.IsVisible = false;
                CompanyDescriptionStack.IsVisible = true;
                StoreStack.IsVisible = false;
                OtherStoresStack.IsVisible = false;
                LogoStack.IsVisible = true;
            };
            lblCompany.GestureRecognizers.Add(CompanyGestureRecognizer);

            var OtherStoresGestureRecognizer = new TapGestureRecognizer();
            OtherStoresGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = false;
                CompanyUnder.IsVisible = false;
                OtherStoresUnder.IsVisible = true;
                StoreStack.IsVisible = false;
                CompanyDescriptionStack.IsVisible = false;
                OtherStoresStack.IsVisible = true;
                LogoStack.IsVisible = false;
            };
            lblOtherStores.GestureRecognizers.Add(OtherStoresGestureRecognizer);

        }

        public Company_ElementInfoPage()

        {

        }



        private void BtnWriteComplaint_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NewComplaintPage(), true);
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

        public void ChangeCompanyElement(int ElementId)
        {
            SetData(CompanyElement, ElementId);
        }

        private async void SetData(CompanyElementRootModel companyElement, int ElementId = 0)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje poslovnice", Acr.UserDialogs.MaskType.Clear);

            var CompanyElement = ElementId == 0 ?
                companyElement.element :
                companyElement.siblings?.SingleOrDefault(sib => sib.id == ElementId) ??
                companyElement.element.children?.SingleOrDefault(sib => sib.id == ElementId);


            NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
            lblCompanyAddress.Text = CompanyElement.root_business.address;
            lblCompanyCity.Text = CompanyElement.root_business.city?.name;
            lblCompanyDescription.Text = CompanyElement.root_business.description;
            lblCompanyName.Text = CompanyElement.root_business.name;
            lblCompanyWebAddress.Text = "CompanyStore.root_business.web";

            lblElementAddress.Text = CompanyElement.address;
            lblCompanyDescription.Text = CompanyElement.description;
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
                    ElementMap.IsVisible = true;
                }
                else ElementMap.IsVisible = false;
            }
            else ElementMap.IsVisible = false;

            CompanyMap.HeightRequest = 200;
            ElementMap.HeightRequest = 200;
            lytOtherCompanyElements.IsVisible = Convert.ToBoolean(companyElement.siblings?.Any() | companyElement.element.children?.Any());
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == lytCompanyInfo | view == imgCompanyDetails)
            {
                CompanyMapStack.IsVisible = !CompanyMapStack.IsVisible;

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
            else if (view == lblOtherCompanyElements)
            {
                await Navigation.PushModalAsync(new OtherCompanyStoresPage(CompanyElement));
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
        }
    }
}
