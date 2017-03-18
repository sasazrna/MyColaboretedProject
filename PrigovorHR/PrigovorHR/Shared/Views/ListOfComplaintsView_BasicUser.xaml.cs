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
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListOfComplaintsView_BasicUser : ContentView
    {
        private PullToRefreshModel PullToRefreshModel = new PullToRefreshModel();
        public delegate void ComplaintClickedHandler(ComplaintModel Complaint);
        public static ListOfComplaintsView_BasicUser ReferenceToView;
        private RootComplaintModel _DataSource;
        private Dictionary<string, int> DisplayedComplaints = new Dictionary<string, int>();
        private const int MaximumDisplayedComplaintsPerRequest = 6;
        private double CurrentMaximumScrollValue = 0;
        private bool AllComplaintsVisible = false;
        private Dictionary<ComplaintListTabView.Tabs, StackLayout> Layouts = new Dictionary<ComplaintListTabView.Tabs, StackLayout>();
        private StackLayout VisibleLayout;
        private ComplaintListTabView.Tabs SelectedTab;
        public ListOfComplaintsView_BasicUser()
        {
            InitializeComponent();
            BindingContext = PullToRefreshModel;

            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy);
            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshCommand);
            PullToRefreshModel.Pulled += PullToRefreshModel_Pulled;

            DisplayedComplaints.Add(lytActiveComplaints.Id.ToString(), 0);
            DisplayedComplaints.Add(lytClosedComplaints.Id.ToString(), 0);
            DisplayedComplaints.Add(lytStoredComplaints.Id.ToString(), 0);
            DisplayedComplaints.Add(lytUnsentComplaints.Id.ToString(), 0);
            Layouts.Add(ComplaintListTabView.Tabs.ActiveComplaints, lytActiveComplaints);
            Layouts.Add(ComplaintListTabView.Tabs.ClosedComplaints, lytClosedComplaints);
            Layouts.Add(ComplaintListTabView.Tabs.DraftComplaints, lytStoredComplaints);
            Layouts.Add(ComplaintListTabView.Tabs.UnsentComplaints, lytUnsentComplaints);
            SelectedTab = ComplaintListTabView.Tabs.ActiveComplaints;
            VisibleLayout = Layouts[ComplaintListTabView.Tabs.ActiveComplaints];
            LoadComplaints();
            scrview.Scrolled += Scrview_Scrolled;
            ReferenceToView = this;
        }

        public void ChangeVisibleLayout(ComplaintListTabView.Tabs selectedTab)
        {
            SelectedTab = selectedTab;
            VisibleLayout = Layouts[selectedTab];
            VisibleLayout.IsVisible = true;
            foreach (var Layout in Layouts.Where(l => l.Key != selectedTab))
                Layout.Value.IsVisible = false;

            DisplayData();
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
            if (VisibleLayout.Children.Any())
            {
                new Task(async () =>
                {
                    scrview.Scrolled -= Scrview_Scrolled;
                    await scrview.ScrollToAsync(VisibleLayout.Children.LastOrDefault(), ScrollToPosition.Start, false);
                    CurrentMaximumScrollValue = scrview.ScrollY;
                    await scrview.ScrollToAsync(VisibleLayout.Children.FirstOrDefault(), ScrollToPosition.Start, false);
                    scrview.Scrolled += Scrview_Scrolled;
                }).RunSynchronously();
            }
        }

        public void LoadComplaints()
        {
            PullToRefreshModel_Pulled();
        }

        private async void PullToRefreshModel_Pulled()
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje prigovora");
            VisibleLayout.Children.Clear();
            await Task.Delay(19);
            try
            {
                DisplayedComplaints[VisibleLayout.Id.ToString()] = 0;
                var Json = await DataExchangeServices.GetMyComplaints();
                DataSource = ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                    (await DataExchangeServices.GetMyComplaints());
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

        public Models.RootComplaintModel DataSource
        {
            private get { return _DataSource; }
            set
            {
                _DataSource = value;
                VisibleLayout.Children.Clear();
                DisplayData();
                CalculateMaximumScroll();
                MainNavigationBar.ReferenceToView.HasUnreadedReplys = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any();
            }
        }

        private async void DisplayData()
        {
            var displayedComplaints = DisplayedComplaints[VisibleLayout.Id.ToString()];
            var ClosedComplaintsVisible = VisibleLayout == lytClosedComplaints;

            if (VisibleLayout == lytClosedComplaints | VisibleLayout == lytActiveComplaints)
            {
                var MaxOfVisibleComplaints = _DataSource.user?.complaints.Count(c => c.closed == ClosedComplaintsVisible) - 1;
                if (displayedComplaints != MaxOfVisibleComplaints | displayedComplaints == 0)
                {
                    foreach (var Complaint in _DataSource.user?.complaints.OrderByDescending(c => DateTime.Parse(c.last_event))
                                                               .Where(c => c.closed == ClosedComplaintsVisible)
                                                               .Skip(displayedComplaints)
                                                               .Take(MaximumDisplayedComplaintsPerRequest))
                    {
                        Complaint.typeOfComplaint =
                            (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                        var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                        VisibleLayout.Children.Add(ComplaintListView);
                        displayedComplaints++;
                        AllComplaintsVisible = displayedComplaints >= MaxOfVisibleComplaints;
                    }

                    DisplayedComplaints[VisibleLayout.Id.ToString()] = displayedComplaints;
                }
            }
            else
            {
                foreach (var Draft in ComplaintDraftController.LoadDrafts())
                {
                    var Complaint = new Models.ComplaintModel();
                    Complaint.attachments = Draft.attachments;
                    Complaint.closed = false;
                    Complaint.complaint = Draft.complaint;
                    Complaint.id = Draft.complaint_id;
                    Complaint.element = JsonConvert.DeserializeObject<CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(Draft.element_slug)).element;
                    Complaint.element_id = Complaint.element.id;
                    Complaint.typeOfComplaint =
                     (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                    Complaint.user_id = Draft.user_id;
                    var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                    VisibleLayout.Children.Add(ComplaintListView);
                }
                AllComplaintsVisible = true;
            }
        }
    }
}
