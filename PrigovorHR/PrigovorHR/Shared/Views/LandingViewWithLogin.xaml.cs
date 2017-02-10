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

namespace PrigovorHR.Shared.Views
{
    public partial class LandingViewWithLogin : ContentView, IDisposable
    {
        private CompanysStoreFoundListView CompanysFoundListView;
        private ListOfComplaintsView_BasicUser ListOfComplaints;
        private ProfileView ProfileView;

        public LandingViewWithLogin()
        {
            InitializeComponent();
            _TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");
            _TopNavigationBar.SearchActivated += _TopNavigationBar_SearchActivated;
            _TopNavigationBar._ShowProfileEvent += MenuPage_ShowProfileEvent;
            _TopNavigationBar._ShowComplaintsEvent += _TopNavigationBar__ShowComplaintsEvent;

            CompanysFoundListView = new CompanysStoreFoundListView();
            ListOfComplaints = new ListOfComplaintsView_BasicUser();
            ProfileView = new ProfileView();

            _StackLayout.Children.Add(_TopNavigationBar);
            _StackLayout.Children.Add(ListOfComplaints);
            _StackLayout.Children.Add(CompanysFoundListView);
            _StackLayout.Children.Add(ProfileView);
            CompanysFoundListView.IsVisible = false;
            ProfileView.IsVisible = false;
            Content = _StackLayout;


            //When logged in, check if there is complaint that wasnt sent for some reason.
            object objuser;
            ComplaintModel.WriteNewComplaintModel WriteNewComplaintModel = null;
            if (Application.Current.Properties.TryGetValue("WriteComplaintAutoSave", out objuser))
                WriteNewComplaintModel = JsonConvert.DeserializeObject<ComplaintModel.WriteNewComplaintModel>((string)objuser);

            if (objuser != null && WriteNewComplaintModel.QuickComplaint)
            {
                var QuickComplaintPage = new QuickComplaintPage(null, WriteNewComplaintModel);
                QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaints.LoadComplaints(); });
                Navigation.PushPopupAsync(QuickComplaintPage);
            }
        }

        private void _TopNavigationBar_SearchActivated(string searchtext, bool isQRCoded)
        {
            ListOfComplaints.IsVisible = false;
            ProfileView.IsVisible = false;
            CompanysFoundListView.IsVisible = true;
        }

        private void _TopNavigationBar__ShowComplaintsEvent()
        {
            ListOfComplaints.IsVisible = true;
            CompanysFoundListView.IsVisible = false;
            ProfileView.IsVisible = false;
            ListOfComplaints.LoadComplaints();
        }

        private void MenuPage_ShowProfileEvent()
        {
            ListOfComplaints.IsVisible = false;
            CompanysFoundListView.IsVisible = false;
            ProfileView.IsVisible = true;
            ProfileView.ResizeCircleControl();
            ProfileView.LoadData();
        }

        public void Dispose()
        {
            CompanysFoundListView.Dispose();
            ListOfComplaints.Dispose();
            CompanysFoundListView = null;
            ListOfComplaints = null;
            ProfileView = null;
        }
    }
}
