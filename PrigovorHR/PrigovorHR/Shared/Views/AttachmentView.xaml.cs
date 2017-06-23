
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class AttachmentView : ContentView
    {
        private Controllers.TAPController TAPController;
        private int AttachmentId = 0;
        public string AttachmentFileName = string.Empty;
        private int ComplaintReplyId = 0;
        private bool IsReply = false;
        public byte[] Data = null;
        public delegate void AttachmentClickedHandler(Models.ComplaintModel.ComplaintAttachmentModel Attachment);
        public event AttachmentClickedHandler AttachmentClickedEvent;
        public delegate void AttachmentDeletedHandler(View view);
        public event AttachmentDeletedHandler AttachmentDeletedEvent;

        public AttachmentView() { }
        public AttachmentView(bool isReply, int complaintreplyId, int attachmentId, string attachmentFileName, bool disposable, byte[] data)
        {
            InitializeComponent();

            AttachmentId = attachmentId;
            AttachmentFileName = attachmentFileName;
            ComplaintReplyId = complaintreplyId;
            lblAttachmentName.Text = attachmentFileName;
            imgClose.IsVisible = disposable;
            imgClose.Text = Views.FontAwesomeLabel.Images.FATimes;
            Data = data;
            IsReply = isReply;
            TAPController = new Controllers.TAPController(lblAttachmentName, imgClose);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == lblAttachmentName)
            {
                if (Data == null)
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
                else
                {
                    DependencyService.Get<Controllers.IAndroidCallers>().SaveFile(AttachmentFileName, Data);
                    DependencyService.Get<Controllers.IAndroidCallers>().OpenFile(AttachmentFileName);
                    DependencyService.Get<Controllers.IAndroidCallers>().DeleteFile(AttachmentFileName);
                }
            }
            else
            {
                Data = null;
                AttachmentDeletedEvent?.Invoke(this);
            }
        }
    }
}
