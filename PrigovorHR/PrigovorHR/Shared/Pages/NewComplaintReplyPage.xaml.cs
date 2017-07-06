using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complio.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;
using Newtonsoft.Json;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewComplaintReplyPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private Models.ComplaintModel Complaint;
        public delegate void ReplySentHandler(int id);
        public event ReplySentHandler ReplaySentEvent;
        private Models.ComplaintModel.DraftComplaintModel WriteNewComplaintModel;
        private Guid ComplaintDraftGuid;

        public NewComplaintReplyPage(Models.ComplaintModel complaint, Models.ComplaintModel.DraftComplaintModel _WriteNewComplaintModel = null)
        {
            InitializeComponent();

            Complaint = complaint;

            WriteNewComplaintModel = _WriteNewComplaintModel;

            if (WriteNewComplaintModel == null)
            {
                WriteNewComplaintModel = new Models.ComplaintModel.DraftComplaintModel();
                WriteNewComplaintModel.QuickComplaint = false;
                WriteNewComplaintModel.element_id = complaint.element_id;
                WriteNewComplaintModel.complaint_id = complaint.id;
                WriteNewComplaintModel.element_slug = complaint.element.slug;

                AttachmentListView = new AttachmentListView(ref WriteNewComplaintModel, true);
            }
            else
            {
                AttachmentListView = new AttachmentListView(ref WriteNewComplaintModel, true);

                Complaint.complaint = WriteNewComplaintModel.complaint;
                editReplyText.Text = complaint.complaint;
            }
            lytAttachmentsAndEditors.Children.RemoveAt(0);
            lytAttachmentsAndEditors.Children.Insert(0, AttachmentListView);

            ComplaintCoversationHeaderView.SetHeaderInfo(Complaint.replies.Any() ?
                       Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? Complaint.element.name :
                       Complaint.element.name, Complaint.element.name);


            TAPController = new Controllers.TAPController(btnSendReply);

            TAPController.SingleTaped += TAPController_SingleTaped;
            editReplyText.TextChanged += EditReplyText_TextChanged;
        }
       
        private void EditReplyText_TextChanged(object sender, TextChangedEventArgs e)
        {
            WriteNewComplaintModel.complaint = editReplyText.Text;
            SaveToDevice();
        }

        private void SaveToDevice()
        {
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.Properties.Add("WriteComplaintAutoSave", JsonConvert.SerializeObject(WriteNewComplaintModel));
            Application.Current.SavePropertiesAsync();
        }

        private void BtnSendReply_Clicked(object sender, EventArgs e)
        {
            if (Complaint.closed)
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(new Acr.UserDialogs.ConfirmConfig()
                {
                    Message = "Ovaj prigovor je zatvoren, slanjem vašeg odgovora će biti ponovno aktivan!" + Environment.NewLine +
                              "Jeste li sigurni u aktiviranje ovog prigovora?",
                    CancelText = "NE",
                    OkText = "DA",
                    Title = "Aktivacija prigovora",
                    OnAction = (bool b) => { if (b) SendReply(); else return; }
                });
            }
            else SendReply();
        }

        private async void SendReply()
        {
            if (editReplyText.Text == null || editReplyText.Text?.Length < 20)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaš odgovor treba biti duži od 20 znakova!", null, "OK");
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem vaš prigovor");
            await Task.Delay(19);
            List<int> attachment_ids = new List<int>();

            foreach (var Attachment in AttachmentListView.GetAttachmentsData())
                attachment_ids.Add(await DataExchangeServices.SendReplyAttachment(Attachment.Data, Attachment.AttachmentFileName));

            if (attachment_ids == null | attachment_ids.Any(aid => aid == 0))
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vaših privitaka!" + System.Environment.NewLine + "Pokušajte ponovno poslati", "Greška", "OK");
                return;
            }

            var result = await DataExchangeServices.SendReply(
             JsonConvert.SerializeObject(new
             {
                 reply = editReplyText.Text,
                 complaint_id = Complaint.id,
                 attachment_ids = attachment_ids,
                 close = false
             }));
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            if (!result.Contains("Error"))
            {
                Application.Current.Properties.Remove("WriteComplaintAutoSave");
                await Application.Current.SavePropertiesAsync();
                var ComplaintSentPage = new ComplaintSentPage(Complaint.element.root_business.complaint_received_message, false);
                ComplaintSentPage._PageClosed += (() => { Navigation.PopModalAsync(); ReplaySentEvent?.Invoke(0); });
                await Navigation.PushModalAsync(ComplaintSentPage);
                return;
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vašeg prigovora, moguće zbog internet konekcije" + Environment.NewLine +
              /*      "Vaš prigovor je spremljen na vašem mobitelu te će biti automatski poslan prvom prilikom"*/ "Greška u slanju prigovora", "OK");
                //SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType.Unsent);
            }
        }

        //private void SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType DraftType)
        //{
        //    var DraftReply = new Models.ComplaintModel.ComplaintReplyModel()
        //    {
        //        attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>(),
        //        reply = editReplyText.Text,
        //        complaint_id = Complaint.id,
        //        user_id = Controllers.LoginRegisterController.LoggedUser.id.Value
        //    };

        //    foreach (var Attachment in lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>())
        //        DraftReply.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
        //        {
        //            attachment_data = Convert.ToBase64String(Attachment.Data),
        //            attachment_url = Attachment.AttachmentFileName,
        //            complaint_reply_id = Complaint.id,
        //            user_id = Controllers.LoginRegisterController.LoggedUser.id.Value
        //        });

        //    ComplaintDraftGuid = Controllers.ComplaintDraftController.SaveDraft(null, DraftReply, ComplaintDraftGuid, Complaint.element.slug, DraftType);
        //}

        private void BtnSaveReply_Clicked(object sender, EventArgs e)
        {
            //SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType.Draft);
            //Acr.UserDialogs.UserDialogs.Instance.Toast("Vaš odgovor je spremljen u skice", new TimeSpan(0, 0, 3));
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

            if (view == btnSendReply)
                BtnSendReply_Clicked(null, null);
        }

        private void editReplyText_Focused(object sender, FocusEventArgs e)
        {
            AttachmentListView.HideUnhideAttachments(true);
            ComplaintCoversationHeaderView.IsVisible = false;
            MainsStack.Padding = new Thickness(25, 15, 25, 30);
            editReplyText.Text = string.Empty; 
        }

        private void editReplyText_Unfocused(object sender, FocusEventArgs e)
        {
            AttachmentListView.HideUnhideAttachments(false);
            ComplaintCoversationHeaderView.IsVisible = true;
            MainsStack.Padding = new Thickness(25, 35, 25, 30);
            editReplyText.Text = "Vaš odgovor...";
        }

        protected override bool OnBackButtonPressed()
        {
            if (!string.IsNullOrEmpty(editReplyText.Text) | AttachmentListView.GetAttachmentsData().Any())
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                       new Acr.UserDialogs.ConfirmConfig()
                       {
                           Title = "Prekid",
                           CancelText = "NE",
                           OkText = "DA",
                           Message = "Jeste li sigurni u prekid?",
                           OnAction = (Confirm) =>
                           {
                               if (!Confirm) { return; }
                               else
                               {
                                   Application.Current.Properties.Remove("WriteComplaintAutoSave");
                                   Application.Current.SavePropertiesAsync();
                                   Navigation.PopAsync(true);
                               }
                           }
                       });
            }
            else Navigation.PopAsync(true);

            return true;
        }
    }
}
