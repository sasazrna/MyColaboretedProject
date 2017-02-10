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

namespace PrigovorHR.Shared.Views
{
    public partial class CompanysStoreFoundListView : ContentView, IDisposable
    {
        private List<string> _LastSearchResults = new List<string>();
        private List<View> _CompaniesStoreFoundViews = new List<View>();
        private List<Models.CompanyElementModel> _CompaniesStoresFoundInfo = new List<Models.CompanyElementModel>();
        public CompanysStoreFoundListView()
        {
            InitializeComponent();
            MainNavigationBar._RefToView.SearchActivated += _RefToView_SearchActivated;
        }

        // Acr.UserDialogs.UserDialogs.Instance.Toast("Imate novu poruku: " + result);
        // Acr.Notifications.Notification N = new Acr.Notifications.Notification() { Date = DateTime.Now, Message = "Hej imaš odgovor na prigovor", Title = "Prigovor.hr", Vibrate = true, When = new TimeSpan(0, 0, 0, 0, 100) };
        //  Acr.Notifications.Notifications.Instance.Badge = 1;
        // Acr.Notifications.Notifications.Instance.Send(N);

        //await Task.Delay(5000);
        //var notificator = DependencyService.Get<IToastNotificator>();

        //var options = new NotificationOptions()
        //{
        //    Title = "Prigovor.hr" + Environment.NewLine +  "Novi odgovor na vaš prigovor",
        //    Description = "Poštovani primili ste novi odgovor od tvrtke Ja sam mali zeko d.o.o.",
        //    IsClickable = true
        //};

        //var res = await notificator.Notify(options);
        //if(res.Action == NotificationAction.Clicked)
        //{
        //    //   Acr.UserDialogs.ToastAction a = new Acr.UserDialogs.ToastAction();
        // //   Acr.UserDialogs.UserDialogs.Instance.Prompt(new Acr.UserDialogs.PromptConfig() { InputType = Acr.UserDialogs.InputType.Phone, IsCancellable = true, OkText = "OK", Message = "WTF", Placeholder = "Upiši telefon", Text = "Upis telefona", Title = "Prigovor.hr" });
        //    //

        //var notification = new LocalNotification
        //{
        //    Text = "Hello from Plugin",
        //    Title = "Notification Plugin",
        //    Id = 2,
        //    NotifyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 500)
        //};

        //var notifier = CrossLocalNotifications.CreateLocalNotifier();
        //notifier.Notify(notification);   


        private async void _RefToView_SearchActivated(string searchtext, bool isQRCoded)
        {
            if (!isQRCoded)
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Pretražujem " + searchtext, Acr.UserDialogs.MaskType.Clear);
                await Task.Delay(20);
            }

            await Task.Run(async () =>
            {
                var Result = await DataExchangeServices.GetSearchResults(searchtext);

                if (Result == "Greška")
                {
                    Controllers.VibrationController.Vibrate();
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                    return;
                }

                _CompaniesStoresFoundInfo = JsonConvert.DeserializeObject<List<Models.CompanyElementModel>>(Result);

                Device.BeginInvokeOnMainThread(() =>
               {
                   DisplayData(_CompaniesStoresFoundInfo);
               });

                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                if (isQRCoded)
                {
                    if (_CompaniesStoresFoundInfo.Any())
                    {
                        //await Navigation.PushAsync(new WriteNewComplaintView(SearchResult.First().slug, SearchResult.First().name), true);
                        return;
                    }
                    else
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");
                }
            });

            if (_CompaniesStoresFoundInfo.Any() && isDifferentResult(_CompaniesStoresFoundInfo.Select(sr => sr.slug).ToList(), _LastSearchResults))
            {
                _LastSearchResults.Clear();
                _LastSearchResults.AddRange(_CompaniesStoresFoundInfo.Select(sr => sr.slug));
            }
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
                _CompaniesStoreFoundViews.Add(CompanyStoreFound);
                CompanyStoreFound.SingleClicked += CompanyStoreFound__SingleTaped;
                _StackLayout.Children.Add(CompanyStoreFound);
            }

            // _StackLayout.Children.LastOrDefault()?.Focus();
        }

        private async void CompanyStoreFound__SingleTaped(int ElementId)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading();
            await Task.Delay(100);
            var CompanyElement = JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(
               await DataExchangeServices.GetCompanyElementData(_CompaniesStoresFoundInfo.First(com => com.id == ElementId).slug));
            
            if (!MainNavigationBar._RefToView._QuickComplaintRequested)
                await Navigation.PushModalAsync(new Pages.Company_ElementInfoPage(CompanyElement) { Title = "Natrag na pretragu" }, true);
            else
            {
                var QuickComplaintPage = new QuickComplaintPage(CompanyElement.element);
                QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView_BasicUser.RefToView.LoadComplaints(); });
                await Navigation.PushPopupAsync(QuickComplaintPage);
            }

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        public void Dispose()
        {
            _LastSearchResults.Clear();
            _CompaniesStoreFoundViews.Clear();
            _CompaniesStoresFoundInfo.Clear();
            _ScrollView = null;
            _StackLayout = null;
        }
    }
}
