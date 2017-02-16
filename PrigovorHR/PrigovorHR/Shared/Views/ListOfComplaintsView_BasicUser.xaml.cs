using Newtonsoft.Json;
using PrigovorHR.Shared.Models;
using Refractored.XamForms.PullToRefresh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ListOfComplaintsView_BasicUser : ContentView, IDisposable
    {
        private PullToRefreshModel PullToRefreshModel = new PullToRefreshModel();
        public delegate void ComplaintClickedHandler(ComplaintModel Complaint);
        public static ListOfComplaintsView_BasicUser ReferenceToView;
        private bool DisplayClosedComplaints=false;
        private RootComplaintModel _DataSource;

        public ListOfComplaintsView_BasicUser()
        {
            InitializeComponent();
            BindingContext = PullToRefreshModel;

            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy);
            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshCommand);
            PullToRefreshModel.Pulled += PullToRefreshModel_Pulled;
            ComplaintListTabView.SelectedTabChangedEvent += ComplaintListTabView_SelectedTabChangedEvent;
            ReferenceToView = this; 
            LoadComplaints();
        }

        private void ComplaintListTabView_SelectedTabChangedEvent(ComplaintListTabView.enumTabs SelectedTab)
        {
            DisplayClosedComplaints = SelectedTab == ComplaintListTabView.enumTabs.ClosedComplaints;
            DataSource = DataSource;
        }

        public void LoadComplaints()
        {
            PullToRefreshModel_Pulled();
        }

        private async void PullToRefreshModel_Pulled()
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje prigovora");
            _StackLayout.Children.Clear();
            await Task.Delay(19);
            try
            {
                //var RE = await DataExchangeServices.GetMyComplaints();
                DataSource = ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                    (await DataExchangeServices.GetMyComplaints());

                MainNavigationBar._RefToView.NumOfUnreadedComplaints = ComplaintModel.RefToAllComplaints.user.unread_complaints.Count;
            }
            catch (Exception err) { Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške u dohvaćanju vaših prigovora!" + System.Environment.NewLine + "Provjerite internet konekciju","Greška", "OK"); }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            PullToRefreshModel.IsBusy = false;
        }

        public void Dispose()
        {
            PullToRefreshModel = null;
            _pullLayout = null;
            _StackLayout = null;
        }

        public Models.RootComplaintModel DataSource
        {
            private get { return _DataSource; }
            set
            {
                _DataSource = value;
                _StackLayout.Children.Clear();
                foreach (var Complaint in value?.user?.complaints.OrderByDescending(c => DateTime.Parse(c.last_event))
                                                                 .Where(c=>c.closed == DisplayClosedComplaints))
                {
                    var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                    _StackLayout.Children.Add(ComplaintListView);
                }
            }
        }
    }
}
