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
            lblTimeOfComplaint.Text = _Complaint.created_at;
            lblTimeOfProblem.Text = _Complaint.problem_occurred;
            var Reply = _Complaint.replies.LastOrDefault();

            lblShortComplaint.Text = !_Complaint.replies.Any() ?
                                     _Complaint.complaint.Length < 100 ? _Complaint.complaint : _Complaint.complaint.Substring(0, 100) :
                                     Reply.reply.Length < 100 ? Reply.reply : Reply.reply.Substring(0, 100);

            IsUnreaded = ComplaintModel.RefToAllComplaints.user.unread_complaints.Any(uc => uc.id == _Complaint.id);
            lblShortComplaint.FontAttributes = IsUnreaded ? FontAttributes.Bold | FontAttributes.Italic : FontAttributes.None;

            lblStoreName.Text = _Complaint.element.name;
            Complaint = _Complaint;
            TAPController = new Controllers.TAPController(this.Content);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private async  void TAPController_SingleTaped(string viewId, View view)
        {
            await Navigation.PushModalAsync(new Pages.ComplaintPage(Complaint), true);
            await DataExchangeServices.ComplaintReaded(JsonConvert.SerializeObject(new { complaint_id = Complaint.id }));

            if (MainNavigationBar._RefToView.NumOfUnreadedComplaints > 0)
                MainNavigationBar._RefToView.NumOfUnreadedComplaints--;

            lblShortComplaint.FontAttributes = FontAttributes.None;
        }
    }
}
