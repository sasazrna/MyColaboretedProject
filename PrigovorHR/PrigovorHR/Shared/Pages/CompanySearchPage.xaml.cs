using Complio.Shared.Views;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Pages
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanySearchPage : PopupPage
    {
        public delegate void SearchActivatedHandler(string searchtext);
        public delegate void SearchDeactivatedHandler();
        private Controllers.SearchController SearchController;
        private Controllers.TAPController TAPController;
        private static string MyCity;

        public CompanySearchPage()
        {
            InitializeComponent();
            imgClose.Text = Views.FontAwesomeLabel.Images.FAClose;
            imgClose.TextColor = Color.FromHex("#aaa4a4");
            imgGPS.Text = Views.FontAwesomeLabel.Images.FALocationArrow;
            imgGPS.TextColor = Color.White;
            SearchOptionsLayout.IsVisible = Models.ComplaintModel.RefToAllComplaints.user.complaints.Count < 4 & AppGlobal.AppIsComplio;
            lblHash.IsVisible = lblAt.IsVisible = AppGlobal.AppIsComplio;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () => { entrySearch.Focus(); return false; });

            SearchController = new Controllers.SearchController(null, entrySearch);
            SearchController.SearchActivated += SearchController_SearchActivated;
            CompanyStoreFoundListView.CompanyStoreClickedEvent += CompanyStoreFoundListView_CompanyStoreClickedEvent;
            TAPController = new Controllers.TAPController(imgClose, imgGPS);
            TAPController.SingleTaped += TAPController_SingleTapedAsync;

            if (!AppGlobal.AppIsComplio)
                GetMyCity();
        }

        private async Task<bool> GetMyCity()
        {
            if (string.IsNullOrEmpty(MyCity))
            {
                if (!string.IsNullOrEmpty(Controllers.LoginRegisterController.LoggedUser.City))
                {
                    MyCity = Controllers.LoginRegisterController.LoggedUser.City;
                    entrySearch.Text = MyCity;
                }

                MyCity = await Controllers.GPSController.GetAddressOrCityFromPosition(Controllers.GPSController.AddressOrCityenum.City, 0,0);
                if (MyCity != "GPSIsOff")
                {
                    MyCity = MyCity.Split(',').Last();

                    if (entrySearch.Text != MyCity)
                    {
                        entrySearch.Text = MyCity;
                        Controllers.LoginRegisterController.LoggedUser.City = MyCity;
                        await Controllers.LoginRegisterController.SaveUserData(Controllers.LoginRegisterController.LoggedUser, Models.LoginTypeModel.eLoginType.email, false);
                    }
                }
                else MyCity = string.Empty;
            }
            else entrySearch.Text = MyCity;

            return true;
        }

        private async void TAPController_SingleTapedAsync(string viewId, View view)
        {
            if (view == imgClose)
            {
                if (!string.IsNullOrEmpty(entrySearch.Text))
                {
                    entrySearch.Text = string.Empty;
                    entrySearch.Focus();
                }
                else await Navigation.PopPopupAsync(true);
            }
            else
            {
                imgGPS.TextColor = imgGPS.TextColor == Color.White ? Color.FromHex("#FF7e65") : Color.White;
                if (imgGPS.TextColor == Color.FromHex("#FF7e65"))
                {
                    MyCity = string.Empty;
                    Controllers.LoginRegisterController.LoggedUser.City = string.Empty;
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Dohvaćam vašu lokaciju...");
                    await GetMyCity();
                    imgGPS.TextColor = Color.White;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
            }
        }

        private async void CompanyStoreFoundListView_CompanyStoreClickedEvent(Models.CompanyElementRootModel CompanyElement)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam....", Acr.UserDialogs.MaskType.Clear);
            if (!entrySearch.Text.Contains("#"))
            {
                await Navigation.PopPopupAsync(true);
                if (AppGlobal.AppIsComplio)
                    await Navigation.PushAsync(new Company_ElementInfoPage(CompanyElement, true) { BackgroundColor = APPMasterDetailPage.ReferenceToView.Detail.BackgroundColor }, true);
                else
                {
                    var NewComplaintPage = new NewComplaintPage(CompanyElement.element, 0);
                    await Navigation.PushAsync(NewComplaintPage, true);
                    NewComplaintPage.ToolbarItems.Add(new ToolbarItem("tbiSendComplaint", "awsomeSend2.png", (() => { NewComplaintPage.SendComplaint(); }), ToolbarItemOrder.Primary, 10));
                    NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopAsync(true); Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); };
                }
            }
            else
            {
                await Navigation.PopPopupAsync(true);
                var QuickComplaintPage = new QuickComplaintPage(CompanyElement.element);
                QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); });
                await Navigation.PushPopupAsync(QuickComplaintPage);
            }

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private void SearchController_SearchActivated(string searchtext, bool IsDirectTag)
        {
            entrySearch.Unfocus();
            CompanyStoreFoundListView.DoSearch(searchtext, IsDirectTag,  entrySearch);
        }
    }
}
