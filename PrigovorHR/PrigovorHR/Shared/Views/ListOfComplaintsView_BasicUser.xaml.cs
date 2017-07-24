using Newtonsoft.Json;
using Plugin.Connectivity;
using Complio.Shared.Controllers;
using Complio.Shared.Models;
using Complio.Shared.Pages;
using Refractored.XamForms.PullToRefresh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListOfComplaintsView_BasicUser : ContentView
    {
        private PullToRefreshModel PullToRefreshModel = new PullToRefreshModel();
        public static ListOfComplaintsView_BasicUser ReferenceToView;
        private RootComplaintModel _DataSource;
        private Dictionary<string, Tuple<int, bool>> DisplayedComplaints = new Dictionary<string, Tuple<int, bool>>();
        private const int MaximumLoadedComplaintsPerRequest = 6;
        private double CurrentMaximumScrollValue = 0;
       // private bool AllComplaintsVisible = false;//a di je array za closed?
        private Dictionary<int, StackLayout> Layouts = new Dictionary<int, StackLayout>();
        private StackLayout VisibleLayout;
        private int SelectedTab;
        private bool LoadOnlyNewComplaints = false;
        public delegate void ListScrolledHandler();
        public event ListScrolledHandler ListScrolledEvent;
        public bool IsFilterAndSearchActivated = false;
        public int NumOfDisplayedComplaints { get { return DisplayedComplaints[VisibleLayout.Id.ToString()].Item1; } }

        public ListOfComplaintsView_BasicUser()
        {
            InitializeComponent();
            BindingContext = PullToRefreshModel;

            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy);
            _pullLayout.SetBinding<PullToRefreshModel>(PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshCommand);
            PullToRefreshModel.Pulled += PullToRefreshModel_Pulled;

            DisplayedComplaints.Add(lytActiveComplaints.Id.ToString(), new Tuple<int, bool>(0, false));
            DisplayedComplaints.Add(lytClosedComplaints.Id.ToString(), new Tuple<int, bool>(0, false));
            DisplayedComplaints.Add(lytStoredComplaints.Id.ToString(), new Tuple<int, bool>(0, false));
            DisplayedComplaints.Add(lytUnsentComplaints.Id.ToString(), new Tuple<int, bool>(0, false));
            Layouts.Add(1, lytActiveComplaints);
            Layouts.Add(2, lytClosedComplaints);
            Layouts.Add(3, lytStoredComplaints);
            Layouts.Add(4, lytUnsentComplaints);
            SelectedTab = 1;
            VisibleLayout = Layouts[1];
            LoadComplaints();
            scrview.Scrolled += Scrview_Scrolled;
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            ReferenceToView = this;
        }

        public async void FilterAndFindMessages(string CompanyOrElement, DateTime DateFrom, DateTime DateTo, bool Active, bool Closed, string Text)
        {
            if (DataSource == null) return;

            await Task.Delay(100);

            VisibleLayout.Children.Clear();
            var SearchableData = DataSource.user?.complaints;
            var SearchableDataWithTextFilter = new List<ComplaintModel>();

            if (!string.IsNullOrEmpty(CompanyOrElement))
                SearchableDataWithTextFilter = SearchableData.Where(c => c.element.root_business.name.Contains(CompanyOrElement) |
                                                                         c.element.name.Contains(CompanyOrElement)).ToList();
            foreach (var t in Text.Split(' '))
                if (!string.IsNullOrEmpty(t))
                    SearchableDataWithTextFilter = 
                        SearchableDataWithTextFilter.Concat(SearchableData.Where(c => c.complaint.Contains(t))).Distinct().ToList();

            if (!string.IsNullOrEmpty(CompanyOrElement) | !string.IsNullOrEmpty(Text))
                SearchableData = SearchableDataWithTextFilter;

             SearchableData = SearchableData.OrderByDescending(c => DateTime.Parse(c.updated_at))
                                                            .Where(c =>
                                                             (Convert.ToDateTime(c.created_at) >= DateFrom &
                                                             Convert.ToDateTime(c.created_at) <= DateTo)).ToList();

            if (Active != Closed & Active)
                SearchableData = SearchableData.Where(c => !c.closed).ToList();
            else if (Active != Closed & Closed)
                SearchableData = SearchableData.Where(c => c.closed).ToList();
            else if (Active == Closed & !Active)
                SearchableData = new List<ComplaintModel>();

                foreach (var Complaint in SearchableData)
            {
                Complaint.typeOfComplaint =
                        (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                VisibleLayout.Children.Add(ComplaintListView);
            }
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
                    await Navigation.PushAsync(new Pages.ComplaintPage(Complaint));
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

                    //MainNavigationBar.ReferenceToView.HasUnreadedReplys = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any();

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

        public void ChangeVisibleLayout(int selectedTab, bool ChangedByControl)
        {
            SelectedTab = selectedTab;
            VisibleLayout = Layouts[selectedTab];
            VisibleLayout.IsVisible = true;
            foreach (var Layout in Layouts.Where(l => l.Key != selectedTab))
                Layout.Value.IsVisible = false;

            CurrentMaximumScrollValue = 0;
            DisplayData();
        }

        private void Scrview_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (IsFilterAndSearchActivated) return;

            if (e.ScrollY >= CurrentMaximumScrollValue & !DisplayedComplaints[VisibleLayout.Id.ToString()].Item2)
            {
                DisplayData();
                CalculateMaximumScroll();
            }
            ListScrolledEvent?.Invoke();

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
            DisplayedComplaints[lytActiveComplaints.Id.ToString()] = new Tuple<int, bool>(0, false);
            DisplayedComplaints[lytClosedComplaints.Id.ToString()] = new Tuple<int, bool>(0, false);
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

                        foreach (var Review in NewComplaintReplys.user.element_reviews)
                            if (!ComplaintModel.RefToAllComplaints.user.element_reviews.Any(er => er.id == Review.id))
                                ComplaintModel.RefToAllComplaints.user.element_reviews.Add(Review);

                        Application.Current.Properties.Remove("AllComplaints");
                        Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                        await Application.Current.SavePropertiesAsync();

                        DisplayedComplaints[VisibleLayout.Id.ToString()] = new Tuple<int, bool>(0, false);
                        LandingPageWithLogin.ReferenceToView.firstTimeLoginView.IsVisible = false;
                        LandingPageWithLogin.ReferenceToView.listOfComplaintsView.IsVisible = true;
                        //     LandingPageWithLogin.ReferenceToView.complaintListTabView.IsVisible = true;
                    }
                    else
                    {
                        DisplayedComplaints[VisibleLayout.Id.ToString()] = new Tuple<int, bool>(0, false);
                        ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                            (await DataExchangeServices.GetMyComplaints());
                        Application.Current.Properties.Remove("AllComplaints");
                        Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                        await Application.Current.SavePropertiesAsync();

                        if (!ComplaintModel.RefToAllComplaints.user.complaints.Any())
                        {
                            LandingPageWithLogin.ReferenceToView.firstTimeLoginView.IsVisible = true;
                            LandingPageWithLogin.ReferenceToView.listOfComplaintsView.IsVisible = false;
                            //  LandingPageWithLogin.ReferenceToView.complaintListTabView.IsVisible = false;
                        }
                    }
                }
                else
                {
                    DisplayedComplaints[VisibleLayout.Id.ToString()] = new Tuple<int, bool>(0, false);
                    ComplaintModel.RefToAllComplaints = JsonConvert.DeserializeObject<RootComplaintModel>
                        (await DataExchangeServices.GetMyComplaints());
                    Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints));
                    await Application.Current.SavePropertiesAsync();
                }

                DataSource = ComplaintModel.RefToAllComplaints;
                DependencyService.Get<IAndroidCallers>().UpdateComplaintsListFromPortableToNative(JsonConvert.SerializeObject(ComplaintModel.RefToAllComplaints), LoginRegisterController.LoggedUser.token);
            }
            catch
            (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške u dohvaćanju vaših prigovora!" +
                    Environment.NewLine + "Provjerite internet konekciju", "Greška", "OK");
                ExceptionController.HandleException(ex, "Došlo je do greške na  private async void PullToRefreshModel_Pulled()");
            }

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
            }
        }

        private void DisplayData()
        {
            var displayedComplaints = DisplayedComplaints[VisibleLayout.Id.ToString()].Item1;
            var ClosedComplaintsVisible = VisibleLayout == lytClosedComplaints;

            if (VisibleLayout == lytClosedComplaints | VisibleLayout == lytActiveComplaints)
            {
                var MaxOfVisibleComplaints = DataSource?.user?.complaints.Count(c => c.closed == ClosedComplaintsVisible) - 1;
                if (displayedComplaints != MaxOfVisibleComplaints | displayedComplaints == 0)
                {
                    foreach (var Complaint in DataSource.user?.complaints.OrderByDescending(c => DateTime.Parse(c.updated_at))
                                                               .Where(c => c.closed == ClosedComplaintsVisible)
                                                               .Skip(displayedComplaints)
                                                               .Take(MaximumLoadedComplaintsPerRequest))
                    {
                            Complaint.typeOfComplaint =
                                (ComplaintModel.TypeOfComplaint)Enum.Parse(typeof(Models.ComplaintModel.TypeOfComplaint), Convert.ToString((int)SelectedTab));
                            var ComplaintListView = new ComplaintListView_BasicUser(Complaint);
                            VisibleLayout.Children.Add(ComplaintListView);
                        displayedComplaints++;
                    }
                    DisplayedComplaints[VisibleLayout.Id.ToString()] = new Tuple<int, bool>(displayedComplaints, displayedComplaints >= MaxOfVisibleComplaints);

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
               // AllComplaintsVisible = true;
            }

            //Ovo pozivam iako se scroling ne dešava ali zato što se promjenila lista pa da updejta na landing pageu što treba
            ListScrolledEvent?.Invoke();
        }
    }
}
