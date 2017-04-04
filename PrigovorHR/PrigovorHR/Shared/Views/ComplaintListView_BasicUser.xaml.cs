﻿using PrigovorHR.Shared.Models;
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
        }

        public ComplaintListView_BasicUser(ComplaintModel complaint)
        {
            InitializeComponent();
            var Reply = complaint.replies?.LastOrDefault();
            Complaint = complaint;
            var ClosedComplaintMessage = Complaint.complaint_events?.LastOrDefault(ce=>ce.closed)?.message;

            lblShortComplaint.Text = Complaint.closed & !string.IsNullOrEmpty(ClosedComplaintMessage) ? ClosedComplaintMessage :
                                      Reply == null ? Complaint.complaint : Reply.reply;

            var LastResponse = Reply == null ? DateTime.Parse(Complaint.updated_at) : DateTime.Parse(Reply.updated_at);//updateat ne postoji kod skica.

            if (LastResponse.Date == DateTime.Now.Date)
                lblComplaintResponseDate.Text = LastResponse.ToString().Substring(0, LastResponse.ToString().LastIndexOf(":"));
            else
                lblComplaintResponseDate.Text = LastResponse.ToString("dd.MMM");

            IsUnreaded = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any(uc => uc.id == complaint.id);
            lblShortComplaint.FontAttributes = IsUnreaded ? FontAttributes.Bold | FontAttributes.Italic : FontAttributes.None;

            //Ako je complaint zatvoren treba dohvatit osobu koja je to odgovorila
            lblNameOfContactPerson.Text =
                Complaint.replies.Any() ?
                Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? 
                "nepoznato" : "nepoznato";

            lblStoreName.Text = complaint.element.name; // treba mi i parent u slučaju da je dubina u pitanju.

            if (Complaint.closed)
            {
                var Evaluation = ComplaintModel.RefToAllComplaints.user.element_reviews?.SingleOrDefault(er => er.complaint_id == Complaint?.id);
                if (Evaluation != null)
                {
                    var AverageGrade = new List<double?>() { Evaluation.communication_level_user, Evaluation.satisfaction, Evaluation.speed }.Average();

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
