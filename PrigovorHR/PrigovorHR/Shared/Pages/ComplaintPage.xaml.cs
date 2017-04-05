using FAB.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using PrigovorHR.Shared.Controllers;
using PrigovorHR.Shared.Models;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComplaintPage : ContentPage
    {
        private int _clickedTotal=1;
        private Models.ComplaintModel Complaint;
        private bool ComplaintClosed;
        private bool ComplaintEvaluated;
        private Dictionary<string, View> Fabs = new Dictionary<string, View>();
        private Controllers.TAPController TAPController;

        Dictionary<string, string> FabImages = new Dictionary<string, string>{ { "fabReply", "chat.png" }, { "fabCloseComplaint", "clear.png" },
               {"fabRateComplaint", "rate_star.png" } , {"fabOpenOptions", "fab_add.png" }, };
        Dictionary<string, string> FabIcons = new Dictionary<string, string>{ { "fabReply", Views.FontAwesomeLabel.Images.FAReply},
               { "fabCloseComplaint", Views.FontAwesomeLabel.Images.FAClose },
               {"fabRateComplaint", Views.FontAwesomeLabel.Images.FAStar } , {"fabOpenOptions", Views.FontAwesomeLabel.Images.FAPlus }, };

        public ComplaintPage()
        {
            InitializeComponent();
        }

        public ComplaintPage(Models.ComplaintModel complaint)
        {
            InitializeComponent();
            Complaint = complaint;
            lytAllResponses.Children.Clear();
            lytOriginalComplaint.Children.Clear();
            scrView.IsVisible = false;

            ComplaintClosed = complaint.closed;
            ComplaintEvaluated = ComplaintModel.RefToAllComplaints.user.element_reviews?.SingleOrDefault(er => er.complaint_id == Complaint.id) != null;

            ComplaintCoversationHeaderView.SetHeaderInfo(Complaint.replies.Any() ?
                Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? "nepoznato" :
                "nepoznato", Complaint.element.name);

            NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
            NavigationBar.lblNavigationTitle.Text = "Otvaram prigovor...";

            scrView.Scrolled += ScrView_Scrolled;
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam vaš prigovor");

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), () =>
                {
                    DisplayData(Complaint);
                    return false;
                });

            SetFABS();
        }

        private void SetFABS()
        {
            for (int i = 0; i < 4; i++)
                    SetFAB(i);

            foreach (var FAB in Fabs.Values)
            {
                lytRelative.Children.Add(
                    FAB,
                    xConstraint: Constraint.RelativeToParent((parent) => { return (parent.Width - FAB.Width) - 16; }),
                    yConstraint: Constraint.RelativeToParent((parent) => { return (parent.Height - FAB.Height) - 16; }));
            }

            if (AppGlobal.GetAndroidSDKVersion() >= 21)
            {
                TAPController = new TAPController(Fabs.Values.ToArray());
                TAPController.SingleTaped += TAPController_SingleTaped;
            }

            Fabs["fabCloseComplaint"].IsVisible = !ComplaintClosed;
            Fabs["fabCloseComplaint"].TranslateTo(0, 0, 100);
            Fabs["fabReply"].TranslateTo(0, 0, 100);
            Fabs["fabRateComplaint"].TranslateTo(0, 0, 100);
            Fabs["fabRateComplaint"].IsVisible = ComplaintClosed & !ComplaintEvaluated;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            FabButton_Click(view, null);
        }

        private void SetFAB(int i)
        {
        
            if (AppGlobal.GetAndroidSDKVersion() >= 21)
            {
                var FAB = new FloatingActionButton();
                FAB.Source = FabImages.Values.ToList()[i];
                FAB.Size = FabSize.Normal;
                FAB.NormalColor = Color.FromHex("#FF7e65");
                FAB.RippleColor = Color.Blue;
                FAB.Clicked += FabButton_Click;
                FAB.AutomationId = FabImages.Keys.ToList()[i];
                Fabs.Add(FabImages.Keys.ToList()[i], FAB);
            }
            else
            {
                var FAB = new Views.FontAwesomeLabel();
                FAB.Text = FabIcons.Values.ToList()[i];
                FAB.TextColor = Color.FromHex("#FF7e65");
                FAB.BackgroundColor = Color.Gray;
                FAB.FontSize = 50;
                
                FAB.AutomationId = FabImages.Keys.ToList()[i];
                Fabs.Add(FabImages.Keys.ToList()[i], FAB);
            }
        }

        private void SetRegularButton()
        {

        }

        private void ScrView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (_clickedTotal % 2 == 0)
                FabButton_Click(Fabs["fabOpenOptions"], new EventArgs());
        }

        private void FabButton_Click(object sender, EventArgs e)
        {
            var ClickedButton = ((View)sender);

            switch (ClickedButton.AutomationId)
            {
                case "fabOpenOptions":
                    OpenFabs();
                    break;

                case "fabReply":
                    InitReply();
                    break;

                case "fabCloseComplaint":
                case "fabRateComplaint":
                    InitCloseComplaint();
                    break;
            }
        }

        private async void OpenFabs()
        {
            _clickedTotal += 1;
            var FabsToAnimate = Fabs.Where(fab => fab.Value.IsVisible & fab.Key != "fabOpenOptions").ToList();

            if (_clickedTotal % 2 == 0)
            {
                for (int i = 0; i < FabsToAnimate.Count; i++)
                   await FabsToAnimate[i].Value.TranslateTo(0, -60 * (i + 1), 70);
                //await Fabs["fabReplay"].TranslateTo(0, -60, 70);
                //await Fabs["fabCloseComplaint"].TranslateTo(0, -120, 70);
            }
            else
            {
                for (int i = 0; i < FabsToAnimate.Count; i++)
                    await FabsToAnimate[i].Value.TranslateTo(0, 0, 70);

                //await Fabs["fabCloseComplaint"].TranslateTo(0, 0, 70);
                //await Fabs["fabReplay"].TranslateTo(0, 0, 70);
            }
        }

        private async void InitReply()
        {
            var NewComplaintReplyPage = new NewComplaintReplyPage(Complaint);
            await Navigation.PushModalAsync(NewComplaintReplyPage);
            NewComplaintReplyPage.ReplaySentEvent += (int id) =>
            {
                Navigation.PopModalAsync(true);
                Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(Views.ComplaintListTabView.Tabs.ActiveComplaints, false);
            };
        }
    
        private async void InitCloseComplaint()
        {
            if (!Complaint.complaint_events.Any(ce => ce.closed) |
                Complaint.complaint_events.Any(ce => ce.closed) && !ComplaintEvaluated)
            {
                var CloseComplaintPage = new CloseComplaintPage(Complaint);
                await Navigation.PushModalAsync(CloseComplaintPage);

                CloseComplaintPage.ComplaintClosed += (int id) =>
                {
                    Navigation.PopModalAsync(true);
                    Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                    Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(Views.ComplaintListTabView.Tabs.ClosedComplaints, false);
                };
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                     new Acr.UserDialogs.ConfirmConfig()
                     {
                         Title = "Zatvaranje prigovora",
                         CancelText = "NE",
                         OkText = "DA",
                         Message = "Jeste li sigurni u zatvaranje prigovora?",
                         OnAction = async (Confirm) =>
                         {
                             if (Confirm)
                             {
                                 if (await CloseComplaintNoReview())
                                 {
                                     await Navigation.PopModalAsync(true);
                                     Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                                     Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(Views.ComplaintListTabView.Tabs.ClosedComplaints, false);
                                 }
                             }
                         }
                     });
            }
        }

        private async Task<bool> CloseComplaintNoReview()
        {
            var CloseComplaintData = new
            {
                rate_element = 1,
                complaint_id = Complaint.id
            };

            if (await DataExchangeServices.CloseComplaint(JsonConvert.SerializeObject(CloseComplaintData)))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Vaš prigovor je uspješno zatvoren!", 3500);
                await Task.Delay(3500);
                return true;
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom zatvaranja prigovora!" + System.Environment.NewLine + "Pokušajte ponovno", "Greška", "OK");
                return false;
            }
        }

        private void DisplayData(Models.ComplaintModel Complaint)
        {
            lytOriginalComplaint.Children.Add(new Views.ComplaintOriginalView(Complaint, null));
            var ListOfComplaintReplyListView = new List<Views.ComplaintReplyListView>();

            foreach (var Reply in Complaint.replies.OrderByDescending(r => DateTime.Parse(r.updated_at)))
                ListOfComplaintReplyListView.Add(new Views.ComplaintReplyListView(Complaint, Reply, null));

            foreach (var ComplaintEvent in Complaint.complaint_events.OrderByDescending(ce => DateTime.Parse(ce.updated_at))
                                                                     .Where(ce => !string.IsNullOrEmpty(ce.message) & ce.closed))
            {
                ListOfComplaintReplyListView.Add(new Views.ComplaintReplyListView(Complaint, null, ComplaintEvent));
            }

            ListOfComplaintReplyListView =
                ListOfComplaintReplyListView.OrderByDescending(c => DateTime.Parse(c.CreatedAt)).ToList();

            foreach (var ComplaintReply in ListOfComplaintReplyListView)
                lytAllResponses.Children.Add(ComplaintReply);

            lblNumberOfResponses.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            lblNumberOfResponses.Text = "+" + Convert.ToString(lytAllResponses.Children.Count);

            if (!lytAllResponses.Children.Any())
            {
                lblNumberOfResponses.IsVisible = false;
                lytAllResponses.IsVisible = false;
            }

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), () =>
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                scrView.IsVisible = true;
                NavigationBar.lblNavigationTitle.Text = "Prigovor.hr";
                NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height; return false;
            });

            NavigationBar.MinimumHeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            NavigationBar.InitBackButtonPressed();
            return true;
        }
    }
}
