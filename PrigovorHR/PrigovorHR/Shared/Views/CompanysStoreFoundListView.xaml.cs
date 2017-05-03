//using LocalNotifications.Plugin;
//using LocalNotifications.Plugin.Abstractions;
using Newtonsoft.Json;
//using Plugin.Toasts;
using PrigovorHR.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CompanysStoreFoundListView : ContentView
    {
        private List<Models.CompanyElementModel> CompaniesStoresFoundInfo = new List<Models.CompanyElementModel>();
        public delegate void CompanyStoreClickedHandler(Models.CompanyElementRootModel element);
        public event CompanyStoreClickedHandler CompanyStoreClickedEvent;

        public CompanysStoreFoundListView()
        {
            InitializeComponent();
        }

        public async void DoSearch(string searchtext, bool IsDirectTag, Entry entrySearch)
        {
            //Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Pretražujem " + searchtext, Acr.UserDialogs.MaskType.Clear);
            //await Task.Delay(20);

            string Result;

            if (!IsDirectTag)
                Result = await DataExchangeServices.GetSearchResults(searchtext);
            else
                Result = await DataExchangeServices.GetDirectTagResult(searchtext);

            if (Result.Contains("Error:"))
            {
                Controllers.VibrationController.Vibrate();
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                return;
            }

            if (!IsDirectTag)
            {
                CompaniesStoresFoundInfo = JsonConvert.DeserializeObject<List<Models.CompanyElementModel>>(Result);
                CompaniesStoresFoundInfo = CompaniesStoresFoundInfo.Where(c => c.type_id <2).ToList();
                DisplayData(CompaniesStoresFoundInfo);
                if (!CompaniesStoresFoundInfo.Any()) { entrySearch.Focus(); }
            }
            else
            {
                try
                {
                    var CompanyElement = JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(Result);
                    var NewCompanyElementInfoPage = new Company_ElementInfoPage(CompanyElement, true);
                    await Navigation.PushAsync(new NavigationPage(NewCompanyElementInfoPage) { BackgroundColor = Color.White });

                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopPopupAsync(true));
                }
                catch (Exception ex)
                {
                    entrySearch.Focus();
                }
            }
            //Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private bool isDifferentResult(List<string> old, List<string> _new)
        {
            if (old.Count != _new.Count) return true;

            foreach (var o in old)
                if (_new.Contains(o)) return false;

            return true;
        }

        public async Task DisplayData(List<Models.CompanyElementModel> _data)
        {
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _StackLayout.Children.Clear();
                    foreach (var data in _data)
                    {
                        var CompanyStoreFound = new CompanyStoreFoundView(data);
                        CompanyStoreFound.SingleClicked += CompanyStoreFound_SingleClicked;
                        _StackLayout.Children.Add(CompanyStoreFound);
                    }

                    if (!_data.Any())
                        _StackLayout.Children.Add(new Label() { Text = "Nema rezultata", FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), TextColor=Color.Black, VerticalOptions= LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand, Margin= new Thickness(20, 20, 20, 20) });
                });
            });
            // _StackLayout.Children.LastOrDefault()?.Focus();
        }

        private async void CompanyStoreFound_SingleClicked(Models.CompanyElementModel element)
        {
            var CompanyElement = JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(element.slug));
            CompanyStoreClickedEvent?.Invoke(CompanyElement);
        }
    }
}
