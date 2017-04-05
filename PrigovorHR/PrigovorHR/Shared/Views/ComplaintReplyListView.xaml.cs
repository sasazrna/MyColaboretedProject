using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ComplaintReplyListView : ContentView
    {
        private Models.ComplaintModel Complaint;
        private Models.ComplaintModel.ComplaintReplyModel ComplaintReply;
        private Models.ComplaintModel.ComplaintEvent ComplaintEvent;
        public string CreatedAt;
        public ComplaintReplyListView()
        {
            InitializeComponent();
        }

        public ComplaintReplyListView(Models.ComplaintModel complaint, Models.ComplaintModel.ComplaintReplyModel complaintReply, Models.ComplaintModel.ComplaintEvent complaintEvent)
        {
            InitializeComponent();

            Complaint = complaint;
            ComplaintReply = complaintReply;
            ComplaintEvent = complaintEvent;

            string created_at = ComplaintReply != null ? ComplaintReply.created_at : ComplaintEvent.created_at;
            string replytext = ComplaintReply != null ? ComplaintReply.reply : ComplaintEvent.message;
            string username = ComplaintReply != null ? ComplaintReply.user.name_surname : ComplaintEvent.user.name_surname;
            CreatedAt = created_at;

            lblDateTimeOfResponse.Text = DateTime.Parse(created_at).ToString();
            lblDateTimeOfResponse.Text = lblDateTimeOfResponse.Text.Substring(0, lblDateTimeOfResponse.Text.LastIndexOf(":"));
            lblReplyTextLong.Text = replytext;
            lblUsername.Text = username;
            lblNameInitials.Text = lblUsername.Text.Substring(0, 1) + "." + lblUsername.Text.Substring(lblUsername.Text.LastIndexOf(" ") + 1, 1);

            lytAttachmentsLayout.Children.Clear();

            if (complaintReply != null)
                foreach (var Attachment in complaintReply.attachments)
                    lytAttachmentsLayout.Children.Add(new AttachmentView(true, ComplaintReply.id, Attachment.id, Attachment.attachment_url, false, null));

            lytBottomLine.BackgroundColor = ComplaintEvent != null ? Color.Green : Color.Silver;
        }
    }
}
