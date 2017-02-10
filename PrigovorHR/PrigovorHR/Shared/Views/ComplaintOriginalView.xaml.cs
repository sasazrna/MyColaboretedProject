using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ComplaintOriginalView : ContentView
    {
        private Controllers.TAPController TAPController;

        public ComplaintOriginalView()
        {
            InitializeComponent();
        }
        public ComplaintOriginalView(Models.ComplaintModel Complaint, Models.ComplaintModel.ComplaintReplyModel LastReply)
        {
            InitializeComponent();

            TAPController = new Controllers.TAPController(this, imgOriginalComplaintDetails);
            TAPController.SingleTaped += TAPController_SingleTaped;
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading();
            Task.Run(() => { Device.BeginInvokeOnMainThread(() => { DisplayData(Complaint); }); });
            lblOriginalComplaint_TextLong.IsVisible = true;
            lblOriginalComplaint_TextShort.IsVisible = false;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            lblOriginalComplaint_TextLong.IsVisible = !lblOriginalComplaint_TextLong.IsVisible;
            lblOriginalComplaint_TextShort.IsVisible = !lblOriginalComplaint_TextShort.IsVisible;
            await imgOriginalComplaintDetails.RotateTo(lblOriginalComplaint_TextLong.IsVisible ? 0 : 180, 75);
        }

        private void DisplayData(Models.ComplaintModel Complaint)
        {
            var NameSurname = Controllers.LoginRegisterController._LoggedUser.name_surname;

            lblTypeOfComplaint.Text = "ORIGINALNI PRIGOVOR";

            lblUsername.Text = NameSurname;

            lblOriginalComplaint_TextShort.Text = Complaint.complaint.Length < 111 ? Complaint.complaint : Complaint.complaint.Substring(0, 111) + "...";

            lblOriginalComplaint_TextLong.Text = Complaint.complaint;

            if (lblOriginalComplaint_TextLong.Text.Length < 111)
            {
                imgOriginalComplaintDetails.IsVisible = false;
                TAPController.SingleTaped -= TAPController_SingleTaped;
                TAPController = null;
            }

            lytAttachmentsLayout.Children.Clear();
            foreach (var Attachment in Complaint.attachments)
                lytAttachmentsLayout.Children.Add(new AttachmentView(false, Complaint.id, Attachment.id, Attachment.attachment_url));

            lblProblemDateTime.Text = Complaint.problem_occurred;
            lblComplaintDateTime.Text = Complaint.created_at;
            lytLine.IsVisible = false;// !LastReplyId.HasValue;
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

    }
}
