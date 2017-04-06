using PrigovorHR.Shared.Models;
using Refractored.XamForms.PullToRefresh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ComplaintListView_BasicUser : ContentView
    {
        private Controllers.TAPController TAPController;
        public ComplaintModel Complaint;
        public delegate void ComplaintClickedHandler(ComplaintModel Complaint);
        public bool IsUnreaded = false;


       
        public ComplaintListView_BasicUser()
        {
            InitializeComponent();
            //lblChecked.Text = Views.FontAwesomeLabel.Images.FACheckSquareO;
            //lblChecked.TextColor = Color.Green;

        }

        public ComplaintListView_BasicUser(ComplaintModel complaint)
        {
            InitializeComponent();

            try
            {
                BackgroundColor = Color.White.WithLuminosity(1);
                var Reply = complaint.replies?.LastOrDefault();
                Complaint = complaint;
                var LastClosedComplaintEvent = Complaint.complaint_events?.LastOrDefault(ce => ce.closed);

                lblShortComplaint.Text = Complaint.closed & !string.IsNullOrEmpty(LastClosedComplaintEvent?.message) ? LastClosedComplaintEvent?.message :
                                          Reply == null ? Complaint.complaint : Reply.reply;

                var LastResponse = complaint.closed ? DateTime.Parse(LastClosedComplaintEvent.created_at) :
                    Reply == null ? DateTime.Parse(Complaint.updated_at) : DateTime.Parse(Reply.updated_at);//updateat ne postoji kod skica.

                if (LastResponse.Date == DateTime.Now.Date)
                    lblComplaintResponseDate.Text = LastResponse.ToString().Substring(0, LastResponse.ToString().LastIndexOf(":"));
                else
                    lblComplaintResponseDate.Text = LastResponse.ToString("dd.MMM");

                IsUnreaded = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any(uc => uc.id == complaint.id);

                if(IsUnreaded)
                {
                    lblShortComplaint.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;
                    lblRead.Text = FontAwesomeLabel.Images.FAPEnvelopeClosed;
                    lblRead.TextColor = Color.FromHex("#FF7e65");
                }
                else
                {
                    lblRead.Text = FontAwesomeLabel.Images.FAPEnvelopeOpen;
                    lblRead.TextColor = Color.Gray;
                }

                lblLock.Text = Complaint.closed ? FontAwesomeLabel.Images.FALock : FontAwesomeLabel.Images.FAUnlock;
                lblLock.TextColor = Color.Black;

                lblNameOfContactPerson.Text = 
                    Complaint.closed && LastClosedComplaintEvent != null && LastClosedComplaintEvent.user?.id != Controllers.LoginRegisterController.LoggedUser.id ? LastClosedComplaintEvent.user?.name_surname :
                    Complaint.replies.Any() ?
                    Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ??
                    "nepoznato" : "nepoznato";

                lblStoreName.Text = complaint.element.name; // treba mi i parent u slučaju da je dubina u pitanju.

                lblNumOfResponses.Text = "(+" + complaint.replies.Count + ")";
                lblNumOfResponses.IsVisible = complaint.replies.Any();

                if (Complaint.closed)
                {
                    var Evaluation = ComplaintModel.RefToAllComplaints.user.element_reviews?.SingleOrDefault(er => er.complaint_id == Complaint?.id);
                    if (Evaluation != null)
                    {
                        var AverageGrade = new List<double?>() { Evaluation.communication_level_user, Evaluation.satisfaction, Evaluation.speed }.Average();
                        if (AverageGrade != null)
                        {
                            int starId = 0;
                            bool IsDecimal = AverageGrade != Convert.ToInt32(AverageGrade);
                            bool First = false;
                            foreach (var star in lytEvaluationLayout.Children.Cast<FontAwesomeLabel>())
                            {
                                bool IsGradeBiggerThanStar = ++starId <= AverageGrade;
                                star.TextColor = IsGradeBiggerThanStar ? Color.Orange : Color.Gray;

                                if (!First & !IsGradeBiggerThanStar & IsDecimal)
                                {
                                    First = true;
                                    star.Text = FontAwesomeLabel.Images.FAStarHalfO;
                                    star.TextColor = Color.Orange;
                                }
                                else star.Text = FontAwesomeLabel.Images.FAStar;
                            }
                            lytEvaluationLayout.IsVisible = true;
                        }
                    }
                }
            }catch(Exception ex) { Controllers.ExceptionController.HandleException(ex, "public ComplaintListView_BasicUser(ComplaintModel complaint)"); }

            TAPController = new Controllers.TAPController(Content);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        public void OpenComplaint()
        {
            TAPController_SingleTaped(null, null);
        }

        public void MarkAsReaded()
        {
            lblShortComplaint.FontAttributes = FontAttributes.None;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            await view.FadeTo(0.3, 45);
            await view.FadeTo(1, 115);

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
            lblShortComplaint.FontAttributes = FontAttributes.None;
        }

        private  async Task AnimateColor(View view)
        {
            var RGB = new Color(view.BackgroundColor.R, view.BackgroundColor.G, view.BackgroundColor.B);
            for (int i = 0; i < 100; i++)
            {
                view.BackgroundColor = new Color(RGB.R, RGB.G, RGB.B - i);
                await Task.Delay(10);
            }
        }
    }
}
