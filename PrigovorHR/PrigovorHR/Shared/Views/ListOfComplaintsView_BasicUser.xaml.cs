using Newtonsoft.Json;
using Plugin.Connectivity;
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
        public static ListOfComplaintsView_BasicUser ReferenceToView;
        private RootComplaintModel _DataSource;
        private Dictionary<string, int> DisplayedComplaints = new Dictionary<string, int>();
        private const int MaximumDisplayedComplaintsPerRequest = 6;
        private double CurrentMaximumScrollValue = 0;
        private bool AllComplaintsVisible = false;
        private Dictionary<ComplaintListTabView.Tabs, StackLayout> Layouts = new Dictionary<ComplaintListTabView.Tabs, StackLayout>();
        private StackLayout VisibleLayout;
        private ComplaintListTabView.Tabs SelectedTab;
        private bool LoadOnlyNewComplaints = false;

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
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            ReferenceToView = this;
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
           if( e.IsConnected) LoadComplaints();
        }

        public async void FindAndOpenComplaint(int ComplaintId)
        {
            try
            {
                if (ComplaintId > 0)
                {
                    var ComplaintLastEvent = ComplaintModel.RefToAllComplaints.user.complaints.Select(c => DateTime.Parse(c.last_event)).Max().ToString("dd.MM.yyyy. H:mm");
                    var NewComplaintReplys = JsonConvert.DeserializeObject<RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                    var Complaint = NewComplaintReplys.user.complaints.Single(c => c.id == ComplaintId);
                    await Navigation.PushModalAsync(new Pages.ComplaintPage(Complaint), true);
                    await DataExchangeServices.ComplaintReaded(JsonConvert.SerializeObject(new { complaint_id = Complaint.id }));
                    var UnreadComplaint = ComplaintModel.RefToAllComplaints.user.unread_complaints.FirstOrDefault(uc => uc.id == Complaint.id);

                    if (UnreadComplaint != null)
                    {
                        ComplaintModel.RefToAllComplaints.user.unread_complaints =
                                 ComplaintModel.RefToAllComplaints.user.unread_complaints.Where(uc => uc.id != Complaint.id).ToList();

                        Application.Current.Properties.Remove("AllComplaints");
                        Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                        await Application.Current.SavePropertiesAsync();
                    }

                    MainNavigationBar.ReferenceToView.HasUnreadedReplys = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any();

                    foreach (var lyt in VisibleLayout.Children)
                    {
                        Complaint = ((ComplaintListView_BasicUser)lyt).Complaint;
                        if (Complaint.id == ComplaintId)
                        {
                            ((ComplaintListView_BasicUser)lyt).MarkAsReaded();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { Controllers.ExceptionController.HandleException(ex, "public async void FindAndOpenComplaint(int ComplaintId)"); }
        }

        public void ChangeVisibleLayout(ComplaintListTabView.Tabs selectedTab, bool ChangedByControl)
        {
            SelectedTab = selectedTab;
            VisibleLayout = Layouts[selectedTab];
            VisibleLayout.IsVisible = true;
            foreach (var Layout in Layouts.Where(l => l.Key != selectedTab))
                Layout.Value.IsVisible = false;

            if (!ChangedByControl)
                ComplaintListTabView.ReferenceToView.InvokeSelectedTabChanged(SelectedTab);

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
            Device.BeginInvokeOnMainThread(() => PullToRefreshModel_Pulled());
        }

        private async void PullToRefreshModel_Pulled()
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje prigovora");
            VisibleLayout.Children.Clear();
            DisplayedComplaints[lytActiveComplaints.Id.ToString()] = 0;
            DisplayedComplaints[lytClosedComplaints.Id.ToString()] = 0;
            lytActiveComplaints.Children.Clear();
            lytClosedComplaints.Children.Clear();

            await Task.Delay(19);
            try
            {
                object objallcomplaints;
                if (Application.Current.Properties.TryGetValue("AllComplaints", out objallcomplaints))
                {
                    ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>((string)objallcomplaints);

                    var Complaints = ComplaintModel.RefToAllComplaints?.user.complaints;
                    if (Complaints.Any())
                    {
                        var UnreadComplaints = ComplaintModel.RefToAllComplaints?.user.unread_complaints;

                        var ComplaintLastEvent = Complaints.Any() ?
                            Complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm") :
                            DateTime.Now.ToString("dd.MM.yyyy. H:mm");

                        var NewComplaintReplys = JsonConvert.DeserializeObject<RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                        foreach (var Complaint in NewComplaintReplys.user.complaints)
                        {
                            Complaints.Remove(Complaints.FirstOrDefault(c => c.id == Complaint.id));
                            Complaints.Add(Complaint);

                            foreach (var UnreadedComplaint in NewComplaintReplys.user.unread_complaints.Where(uc => uc.id == Complaint.id))
                                if (!UnreadComplaints.Select(uc => uc.id).Contains(UnreadedComplaint.id))
                                    UnreadComplaints.Add(UnreadedComplaint);
                        }

                        foreach(var Review in NewComplaintReplys.user.element_reviews)
                            if (!ComplaintModel.RefToAllComplaints.user.element_reviews.Any(er => er.id == Review.id))
                                ComplaintModel.RefToAllComplaints.user.element_reviews.Add(Review);

                        Application.Current.Properties.Remove("AllComplaints");
                        Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                        await Application.Current.SavePropertiesAsync();

                        DisplayedComplaints[VisibleLayout.Id.ToString()] = 0;
                        LandingViewWithLogin.ReferenceToView.firstTimeLoginView.IsVisible = false;
                        LandingViewWithLogin.ReferenceToView.listOfComplaintsView.IsVisible = true;
                        LandingViewWithLogin.ReferenceToView.complaintListTabView.IsVisible = true;
                    }
                    else
                    {
                        DisplayedComplaints[VisibleLayout.Id.ToString()] = 0;
                        ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                            (await DataExchangeServices.GetMyComplaints());
                        Application.Current.Properties.Remove("AllComplaints");
                        Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                        await Application.Current.SavePropertiesAsync();

                        if (!ComplaintModel.RefToAllComplaints.user.complaints.Any())
                        {
                            LandingViewWithLogin.ReferenceToView.firstTimeLoginView.IsVisible = true;
                            LandingViewWithLogin.ReferenceToView.listOfComplaintsView.IsVisible = false;
                            LandingViewWithLogin.ReferenceToView.complaintListTabView.IsVisible = false;
                        }
                    }
                }
                else
                {
                    DisplayedComplaints[VisibleLayout.Id.ToString()] = 0;
                    ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                        (await DataExchangeServices.GetMyComplaints());
                    Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch
            (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške u dohvaćanju vaših prigovora!" +
                    Environment.NewLine + "Provjerite internet konekciju", "Greška", "OK");
                ExceptionController.HandleException(ex, "Došlo je do greške na  private async void PullToRefreshModel_Pulled()");
            }
            DataSource = ComplaintModel.RefToAllComplaints;
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            PullToRefreshModel.IsBusy = false;
            AppGlobal.AppLoaded = true;
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

        public void DisplayData()
        {
            var displayedComplaints = DisplayedComplaints[VisibleLayout.Id.ToString()];
            var ClosedComplaintsVisible = VisibleLayout == lytClosedComplaints;

            if (VisibleLayout == lytClosedComplaints | VisibleLayout == lytActiveComplaints)
            {
                var MaxOfVisibleComplaints = _DataSource.user?.complaints.Count(c => c.closed == ClosedComplaintsVisible) - 1;
                if (displayedComplaints != MaxOfVisibleComplaints | displayedComplaints == 0)
                {
                    foreach (var Complaint in _DataSource.user?.complaints.OrderByDescending(c => DateTime.Parse(c.updated_at))
                                                               .Where(c => c.closed == ClosedComplaintsVisible)
                                                               .Skip(displayedComplaints)
                                                               .Take(MaximumDisplayedComplaintsPerRequest))
                    {
                        //new Task(() =>
                        //{
                            Complaint.typeOfComplaint =
                                (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                            var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                            VisibleLayout.Children.Add(ComplaintListView);
                        //}).Start();
                        displayedComplaints++;
                        AllComplaintsVisible = displayedComplaints >= MaxOfVisibleComplaints;
                    }

                    DisplayedComplaints[VisibleLayout.Id.ToString()] = displayedComplaints;
                }
            }
            else
            {
                //foreach (var Draft in ComplaintDraftController.LoadDrafts())
                //{
                //    var Complaint = new Models.ComplaintModel();
                //    Complaint.attachments = Draft.attachments;
                //    Complaint.closed = false;
                //    Complaint.complaint = Draft.complaint;
                //    Complaint.id = Draft.complaint_id;
                //    Complaint.element = JsonConvert.DeserializeObject<CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(Draft.element_slug)).element;
                //    Complaint.element_id = Complaint.element.id;
                //    Complaint.typeOfComplaint =
                //     (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                //    Complaint.user_id = Draft.user_id;
                //    var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                //    VisibleLayout.Children.Add(ComplaintListView);
                //}
                AllComplaintsVisible = true;
            }
        }
    }
}
