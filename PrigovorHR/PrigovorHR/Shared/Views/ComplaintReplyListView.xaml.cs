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
        private Models.ComplaintModel Complaint;
        private Models.ComplaintModel.ComplaintReplyModel ComplaintReply;

        public ComplaintReplyListView()
        {
            InitializeComponent();
        }

        public ComplaintReplyListView(Models.ComplaintModel _Complaint, Models.ComplaintModel.ComplaintReplyModel _ComplaintReply)
        {
            InitializeComponent();

            Complaint = _Complaint;
            ComplaintReply = _ComplaintReply;
            lblDateTimeOfResponse.Text = DateTime.Parse(ComplaintReply.created_at).ToString();
            lblDateTimeOfResponse.Text = lblDateTimeOfResponse.Text.Substring(0, lblDateTimeOfResponse.Text.LastIndexOf(":") + 1);
            lblReplyTextLong.Text = ComplaintReply.reply;
            lblUsername.Text = _ComplaintReply.user.name_surname;
            lblNameInitials.Text = lblUsername.Text.Substring(0, 1) + "." + lblUsername.Text.Substring(lblUsername.Text.LastIndexOf(" ")+1, 1);

            lytAttachmentsLayout.Children.Clear();
            foreach (var Attachment in _ComplaintReply.attachments)
                lytAttachmentsLayout.Children.Add(new AttachmentView(true, ComplaintReply.id, Attachment.id, Attachment.attachment_url));
        }
    }
}
