using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Models;
using PrigovorHR.Shared.Pages;
using Xamarin.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingViewWithLogin : ContentView
    {
        // private CompanysStoreFoundListView CompanysFoundListView;
        //private ListOfComplaintsView_BasicUser ListOfComplaints;
        private bool DisplayClosedComplaints = false;

        public LandingViewWithLogin()
        {
            InitializeComponent();
            _TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");
            _TopNavigationBar.SearchActivated += _TopNavigationBar_SearchActivated;
            _TopNavigationBar._ShowProfileEvent += MenuPage_ShowProfileEvent;
            _TopNavigationBar._ShowComplaintsEvent += _TopNavigationBar__ShowComplaintsEvent;
            //ComplaintListTabView.SelectedTabChangedEvent += ComplaintListTabView_SelectedTabChangedEvent;
            //CompanysFoundListView = new CompanysStoreFoundListView();
            //ListOfComplaints = new ListOfComplaintsView_BasicUser();

            //_StackLayout.Children.Add(_TopNavigationBar);
            //_StackLayout.Children.Add(ListOfComplaints);
            //_StackLayout.Children.Add(CompanysFoundListView);
            //_StackLayout.Children.Add(ProfileView);
           // CompanysFoundListView.IsVisible = false;
            Content = _StackLayout;

            //When logged in, check if there is complaint that wasnt sent for some reason.
            object objuser;
            ComplaintModel.WriteNewComplaintModel WriteNewComplaintModel = null;
            if (Application.Current.Properties.TryGetValue("WriteComplaintAutoSave", out objuser))
                WriteNewComplaintModel = JsonConvert.DeserializeObject<ComplaintModel.WriteNewComplaintModel>((string)objuser);

            if (objuser != null && WriteNewComplaintModel.QuickComplaint)
            {
                var QuickComplaintPage = new QuickComplaintPage(null, WriteNewComplaintModel);
            //    QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView.LoadComplaints(); });
                Navigation.PushPopupAsync(QuickComplaintPage);
            }
        }

        private void ComplaintListTabView_SelectedTabChangedEvent(ComplaintListTabView.Tabs SelectedTab)
        {
            //scrview.Scrolled -= Scrview_Scrolled;
            //DisplayedComplaints = 0;
            //DisplayClosedComplaints = SelectedTab == ComplaintListTabView.enumTabs.ClosedComplaints;
            //scrview.Scrolled += Scrview_Scrolled;
            //DataSource = DataSource;
        }
    
        private void _TopNavigationBar_SearchActivated(string searchtext, bool isQRCoded)
        {
            //ListOfComplaints.IsVisible = false;
            //CompanysFoundListView.IsVisible = true;
        }

        private void _TopNavigationBar__ShowComplaintsEvent()
        {
            //ListOfComplaints.IsVisible = true;
            //CompanysFoundListView.IsVisible = false;
            //ListOfComplaints.LoadComplaints();
        }

        private async void MenuPage_ShowProfileEvent()
        {
            await Navigation.PushModalAsync(new ProfilePage(), true);
        }
    }
}
