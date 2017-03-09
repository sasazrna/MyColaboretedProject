using Newtonsoft.Json;
using PrigovorHR.Shared.Controllers;
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
        private int DisplayedComplaints = 0;
        private const int MaximumDisplayedComplaintsPerRequest = 6;
        private double CurrentMaximumScrollValue = 0;
        private bool AllComplaintsVisible = false;
        public ListOfComplaintsView_BasicUser()
        {
            InitializeComponent();
            BindingContext = PullToRefreshModel;

            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy);
            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshCommand);
            PullToRefreshModel.Pulled += PullToRefreshModel_Pulled;
            ComplaintListTabView.SelectedTabChangedEvent += ComplaintListTabView_SelectedTabChangedEvent;
            LoadComplaints();
            scrview.Scrolled += Scrview_Scrolled;
            ReferenceToView = this;
        }

        private void Scrview_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY >= CurrentMaximumScrollValue & !AllComplaintsVisible)
            {
                DisplayData();
                CalculateMaximumScroll();
            }
        }

        private void CalculateMaximumScroll()
        {
            if (DisplayedComplaints > 0)
            {
                new Task(async () =>
                {
                    scrview.Scrolled -= Scrview_Scrolled;
                    await scrview.ScrollToAsync(_StackLayout.Children.Last(), ScrollToPosition.Start, false);
                    CurrentMaximumScrollValue = scrview.ScrollY;
                    await scrview.ScrollToAsync(_StackLayout.Children.First(), ScrollToPosition.Start, false);
                    scrview.Scrolled += Scrview_Scrolled;
                }).RunSynchronously();
            }
        }

        private void ComplaintListTabView_SelectedTabChangedEvent(ComplaintListTabView.enumTabs SelectedTab)
        {
            scrview.Scrolled -= Scrview_Scrolled;
            DisplayedComplaints = 0;
            DisplayClosedComplaints = SelectedTab == ComplaintListTabView.enumTabs.ClosedComplaints;
            scrview.Scrolled += Scrview_Scrolled;
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
                DisplayedComplaints = 0;
                DataSource = ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                    (await DataExchangeServices.GetMyComplaints());

                MainNavigationBar._RefToView.NumOfUnreadedComplaints = ComplaintModel.RefToAllComplaints.user.unread_complaints.Count;
            }
            catch
            (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške u dohvaćanju vaših prigovora!" +
                    System.Environment.NewLine + "Provjerite internet konekciju", "Greška", "OK");
                ExceptionController.HandleException(ex, "Došlo je do greške na  private async void PullToRefreshModel_Pulled()");
            }

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
                DisplayData();
                CalculateMaximumScroll();
            }
        }

        private void DisplayData()
        {
            if (DisplayedComplaints != _DataSource.user?.complaints.Count - 1 | DisplayedComplaints == 0)
            {
                foreach (var Complaint in _DataSource.user?.complaints.OrderByDescending(c => DateTime.Parse(c.last_event))
                                                           .Where(c => c.closed == DisplayClosedComplaints)
                                                           .Skip(DisplayedComplaints)
                                                           .Take(MaximumDisplayedComplaintsPerRequest))
                {
                    var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                    _StackLayout.Children.Add(ComplaintListView);
                    DisplayedComplaints++;
                    AllComplaintsVisible = DisplayedComplaints >= _DataSource.user?.complaints.Count - 1;
                }
            }
        }
    }
}
