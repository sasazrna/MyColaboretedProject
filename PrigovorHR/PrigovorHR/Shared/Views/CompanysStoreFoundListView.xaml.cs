using LocalNotifications.Plugin;
using LocalNotifications.Plugin.Abstractions;
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

        public async void DoSearch(string searchtext)
        {

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Pretražujem " + searchtext, Acr.UserDialogs.MaskType.Clear);
            await Task.Delay(20);

            await Task.Run(async () =>
            {
                var Result = await DataExchangeServices.GetSearchResults(searchtext);

                if (Result == "Error:")
                {
                    Controllers.VibrationController.Vibrate();
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                    return;
                }

                CompaniesStoresFoundInfo = JsonConvert.DeserializeObject<List<Models.CompanyElementModel>>(Result);

                Device.BeginInvokeOnMainThread(() =>
               {
                   DisplayData(CompaniesStoresFoundInfo);
               });

                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            });

            //if (_CompaniesStoresFoundInfo.Any() && isDifferentResult(_CompaniesStoresFoundInfo.Select(sr => sr.slug).ToList(), _LastSearchResults))
            //{
            //    _LastSearchResults.Clear();
            //    _LastSearchResults.AddRange(_CompaniesStoresFoundInfo.Select(sr => sr.slug));
            //}
        }

        private bool isDifferentResult(List<string> old, List<string> _new)
        {
            if (old.Count != _new.Count) return true;

            foreach (var o in old)
                if (_new.Contains(o)) return false;

            return true;
        }

        public void DisplayData(List<Models.CompanyElementModel> _data)
        {
            _StackLayout.Children.Clear();
            foreach (var data in _data)
            {
                var CompanyStoreFound = new CompanyStoreFoundView(data);
                CompanyStoreFound.SingleClicked += CompanyStoreFound_SingleClicked;
                _StackLayout.Children.Add(CompanyStoreFound);
            }

            // _StackLayout.Children.LastOrDefault()?.Focus();
        }

        private async void CompanyStoreFound_SingleClicked(Models.CompanyElementModel element)
        {
            var CompanyElement = JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(element.slug));
            CompanyStoreClickedEvent?.Invoke(CompanyElement);
        }
    }
}
