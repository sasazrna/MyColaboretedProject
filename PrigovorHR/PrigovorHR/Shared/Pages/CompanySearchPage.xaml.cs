using PrigovorHR.Shared.Views;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanySearchPage : PopupPage
    {
        public delegate void SearchActivatedHandler(string searchtext);
        public delegate void SearchDeactivatedHandler();
        private Controllers.SearchController SearchController;
        private Controllers.TAPController TAPController;

        public CompanySearchPage()
        {
            InitializeComponent();
            imgClose.Text = Views.FontAwesomeLabel.Images.FAClose;
            imgClose.TextColor = Color.FromHex("#aaa4a4");
            SearchOptionsLayout.IsVisible = Models.ComplaintModel.RefToAllComplaints.user.complaints.Count < 4;

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () => { entrySearch.Focus(); return false; });

            SearchController = new Controllers.SearchController(null, entrySearch);
            SearchController.SearchActivated += SearchController_SearchActivated;
            CompanyStoreFoundListView.CompanyStoreClickedEvent += CompanyStoreFoundListView_CompanyStoreClickedEvent;
            TAPController = new Controllers.TAPController(imgClose);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (!string.IsNullOrEmpty(entrySearch.Text))
            {
                entrySearch.Text = string.Empty;
                entrySearch.Focus();
            }
            else Navigation.PopPopupAsync(true);
        }

        private async void CompanyStoreFoundListView_CompanyStoreClickedEvent(Models.CompanyElementRootModel CompanyElement)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam....", Acr.UserDialogs.MaskType.Clear);
            if (!entrySearch.Text.Contains("#"))
            {
                await Navigation.PopPopupAsync(true);
                await Navigation.PushAsync(new Company_ElementInfoPage(CompanyElement, true) { BackgroundColor = APPMasterDetailPage.ReferenceToView.Detail.BackgroundColor }, true);
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
