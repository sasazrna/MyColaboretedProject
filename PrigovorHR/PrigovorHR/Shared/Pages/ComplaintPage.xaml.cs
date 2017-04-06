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

        Dictionary<string, string[]> FabImages = new Dictionary<string, string[]>{ { "fabReply", new string[] { "FaFabButtonMsg.png", "FaFabButtonUnlock.png" } },
               { "fabCloseComplaint", new string[] { "FaFabButtonLock.png" } },
               {"fabRateComplaint", new string[] { "FaFabButtonRate.png" } } , {"fabOpenOptions", new string[] { "FaFabButtonAdd.png" } }, };

        Dictionary<string, string[]> FabIcons = new Dictionary<string, string[]>{ { "fabReply", new string[] { Views.FontAwesomeLabel.Images.FAReply, Views.FontAwesomeLabel.Images.FAUnlock } },
               { "fabCloseComplaint", new string[] {Views.FontAwesomeLabel.Images.FALock } },
               {"fabRateComplaint", new string[] {Views.FontAwesomeLabel.Images.FAStar } } , {"fabOpenOptions", new string[] {  Views.FontAwesomeLabel.Images.FAPlusCircle } } };

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
            ComplaintEvaluated = ComplaintModel.RefToAllComplaints.user.element_reviews?.SingleOrDefault(er => er.complaint_id == Complaint.id)?.satisfaction != null;

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

            if (AppGlobal.GetAndroidSDKVersion() < 21)
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
            dynamic FAB;

            if (AppGlobal.GetAndroidSDKVersion() >= 21)
                FAB = new FloatingActionButton();
            else
                FAB = new Image();

            FAB.AutomationId = FabImages.Keys.ToList()[i];

            FAB.Source = FabImages.Values.ToList()[i][Convert.ToInt32(FAB.AutomationId == "fabReply" & Complaint.closed)];
            if (FAB.GetType() == typeof(FloatingActionButton))
            {
                FAB.NormalColor = Color.FromHex("#FF7e65");
                FAB.RippleColor = Color.Blue;
                ((FloatingActionButton)FAB).Clicked += FabButton_Click;
            }
            else
            {
                var fab = ((Image)FAB);
                fab.HeightRequest = 55;
                fab.WidthRequest = 55;
               // fab.TextColor = Color.FromHex("#FF7e65");
             //   fab.Text = FabIcons.Values.ToList()[i][Convert.ToInt32(FAB.AutomationId == "fabReply" & Complaint.closed)];

                //if (fab.AutomationId != "fabOpenOptions")
                //{
                //    fab.Opacity = 0;
                //    fab.FontSize = 50;
                //}
                //else fab.FontSize = 60;
            }

            Fabs.Add(FabImages.Keys.ToList()[i], FAB);
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
                {
                    if (FabsToAnimate[i].Value.GetType() == typeof(Views.FontAwesomeLabel))
                        FabsToAnimate[i].Value.Opacity = 1;
                    await FabsToAnimate[i].Value.TranslateTo(0, -60 * (i + 1), 70);
                }
            }
            else
            {
                for (int i = 0; i < FabsToAnimate.Count; i++)
                {
                    await FabsToAnimate[i].Value.TranslateTo(0, 0, 70);
                    if (FabsToAnimate[i].Value.GetType() == typeof(Views.FontAwesomeLabel))
                        FabsToAnimate[i].Value.Opacity = 0;
                }
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
