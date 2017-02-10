using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ComplaintReplyListView : ContentView
    {
        private Controllers.TAPController TAPController;
        private Models.ComplaintModel Complaint;
        private Models.ComplaintModel.ComplaintReplyModel ComplaintReply;
        public ComplaintReplyListView()
        {
            InitializeComponent();
            lytAttachmentsLayout.IsVisible = false;
        }

        public ComplaintReplyListView(Models.ComplaintModel _Complaint, Models.ComplaintModel.ComplaintReplyModel _ComplaintReply)
        {
            InitializeComponent();

            Complaint = _Complaint;
            ComplaintReply = _ComplaintReply;
            lblElementName.Text = _Complaint.element.name;
            lblDateTimeOfResponse.Text = DateTime.Parse(ComplaintReply.created_at).ToString();
            lblReplyTextShort.Text = ComplaintReply.reply.Length < 111 ? ComplaintReply.reply : ComplaintReply.reply.Substring(0, 111) + "...";
            lblReplyTextLong.Text = ComplaintReply.reply;
            lblUsername.Text = _ComplaintReply.user.name_surname; //_ComplaintReply.by_contact != 0 ? _ComplaintReply.by_contact.ToString() : Controllers.LoginRegisterController._LoggedUser.name_surname;
            imgAttachmentImage.IsVisible = ComplaintReply.attachments.Any();

            lytAttachmentsLayout.Children.Clear();
            foreach (var Attachment in _ComplaintReply.attachments)
                lytAttachmentsLayout.Children.Add(new AttachmentView(true, ComplaintReply.id, Attachment.id, Attachment.attachment_url));

            TAPController = new Controllers.TAPController(this);
            lblReplyTextLong.IsVisible = false;
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (lblReplyTextShort.Text.Length >= 111)
            {
                lblReplyTextLong.IsVisible = !lblReplyTextLong.IsVisible;
                lblReplyTextShort.IsVisible = !lblReplyTextShort.IsVisible;
            }
            lytAttachmentsLayout.IsVisible = ComplaintReply.attachments.Any() && !lytAttachmentsLayout.IsVisible;
        }
    }
}
