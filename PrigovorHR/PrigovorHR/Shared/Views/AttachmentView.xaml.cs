using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class AttachmentView : ContentView
    {
        private Controllers.TAPController TAPController;
        private int AttachmentId = 0;
        private string AttachmentFileName = string.Empty;
        private int ComplaintReplyId = 0;
        private bool IsReply = false;
        public delegate void AttachmentClickedHandler(Models.ComplaintModel.ComplaintAttachmentModel Attachment);
        public event AttachmentClickedHandler AttachmentClickedEvent;

        public AttachmentView() { }
        public AttachmentView(bool isReply, int complaintreplyId, int attachmentId, string attachmentFileName)
        {
            InitializeComponent();

            AttachmentId = attachmentId;
            AttachmentFileName = attachmentFileName;
            ComplaintReplyId = complaintreplyId;
            lblAttachmentName.Text = attachmentFileName;
            IsReply = isReply;
            TAPController = new Controllers.TAPController(lblAttachmentName);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Otvaram " + AttachmentFileName);
            AttachmentClickedEvent?.Invoke(new Models.ComplaintModel.ComplaintAttachmentModel() { attachment_url = AttachmentFileName, id = AttachmentId });

            var result = !IsReply ?
                 await DataExchangeServices.GetComplaintAttachmentData(ComplaintReplyId, AttachmentFileName) :
                 await DataExchangeServices.GetReplyAttachmentData(ComplaintReplyId, AttachmentFileName);

            if (!result.Contains("Error:"))
            {
                DependencyService.Get<Controllers.IAndroidCallers>().SaveFile(AttachmentFileName, Convert.FromBase64String(result));
                DependencyService.Get<Controllers.IAndroidCallers>().OpenFile(AttachmentFileName);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert(result);
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }
    }
}
