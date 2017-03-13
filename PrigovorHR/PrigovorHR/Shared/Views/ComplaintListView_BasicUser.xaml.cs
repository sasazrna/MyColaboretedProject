using PrigovorHR.Shared.Models;
using Refractored.XamForms.PullToRefresh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ComplaintListView_BasicUser : ContentView
    {
        private Controllers.TAPController TAPController;
        private ComplaintModel Complaint;
        public delegate void ComplaintClickedHandler(ComplaintModel Complaint);
        public bool IsUnreaded = false;
        public ComplaintListView_BasicUser()
        {
            InitializeComponent();
        }

        public ComplaintListView_BasicUser(ComplaintModel _Complaint)
        {
            InitializeComponent();
            this.BackgroundColor = Color.White.WithLuminosity(1);
             var Reply = _Complaint.replies.LastOrDefault();
            Complaint = _Complaint;

            lblShortComplaint.Text = Reply == null ? _Complaint.complaint :  Reply.reply;

            var LastResponse = Reply == null ? DateTime.Parse(_Complaint.updated_at) : DateTime.Parse(Reply.updated_at);

            if (LastResponse.Date == DateTime.Now.Date)
                lblComplaintResponseDate.Text = LastResponse.ToString().Substring(0, LastResponse.ToString().LastIndexOf(":"));
            else
                lblComplaintResponseDate.Text = LastResponse.ToString("dd.MMM");

            IsUnreaded = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any(uc => uc.id == _Complaint.id);
            lblShortComplaint.FontAttributes = IsUnreaded ? FontAttributes.Bold | FontAttributes.Italic : FontAttributes.None;

            lblNameOfContactPerson.Text =
                Complaint.replies.Any() ?
                Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? 
                "nepoznato" : "nepoznato";

            lblStoreName.Text = _Complaint.element.name;
            TAPController = new Controllers.TAPController(this.Content);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
         //   await AnimateColor(view);
            await Navigation.PushModalAsync(new Pages.ComplaintPage(Complaint), true);
            await DataExchangeServices.ComplaintReaded(JsonConvert.SerializeObject(new { complaint_id = Complaint.id }));

            if (MainNavigationBar._RefToView.NumOfUnreadedComplaints > 0)
                MainNavigationBar._RefToView.NumOfUnreadedComplaints--;

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
