using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Extensions;
using Newtonsoft.Json;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewComplaintReplyPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private double Latitude, Longitude;
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
            }
            else
            {
                foreach (var Attachment in WriteNewComplaintModel.attachments ?? new List<Models.ComplaintModel.ComplaintAttachmentModel>())
                {
                    var AttachmentView = new AttachmentView(true, 0, 0, Attachment.attachment_url, true, Convert.FromBase64String(Attachment.attachment_data));
                    lytAttachments.Children.Add(AttachmentView);
                    AttachmentView.AutomationId = Attachment.attachment_mime;

                    AttachmentView.AttachmentDeletedEvent += (View v) =>
                    {
                        lytAttachments.Children.Remove(v);
                        WriteNewComplaintModel.attachments.Remove(WriteNewComplaintModel.attachments.Single(a => a.attachment_mime == v.AutomationId.ToString()));
                    };
                }

                Complaint.complaint = WriteNewComplaintModel.complaint;
                editReplyText.Text = complaint.complaint;
            }

            ComplaintCoversationHeaderView.SetHeaderInfo(Complaint.replies.Any() ?
                       Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? Complaint.element.name :
                       Complaint.element.name, Complaint.element.name);

            imgAttachDocs.Text = '\uf1c1'.ToString();
            imgAttachDocs.TextColor = Color.Gray;

            imgTakePhoto.Text = '\uf030'.ToString();
            imgTakePhoto.TextColor = Color.Gray;

            imgTakeGPSLocation.Text = '\uf041'.ToString();
            imgTakeGPSLocation.TextColor = Color.Gray;

            btnSendReply.Text = Views.FontAwesomeLabel.Images.FASend_msg;
            btnSendReply.TextColor = Color.FromHex("#FF7e65");

            TAPController = new Controllers.TAPController(imgAttachDocs, imgTakeGPSLocation, imgTakePhoto, btnSendReply);

            TAPController.SingleTaped += TAPController_SingleTaped;
            //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            editReplyText.TextChanged += EditReplyText_TextChanged;
          //  NavigationPage.SetHasNavigationBar(this, false);

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

            foreach (var Attachment in lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>())
                attachment_ids.Add(await DataExchangeServices.SendReplyAttachment(Attachment.Data, Attachment.AttachmentFileName));

            if (lytAttachments.Children.Any() && (attachment_ids == null | attachment_ids.Any(aid => aid == 0)))
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
                SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType.Unsent);
            }
        }

        private void SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType DraftType)
        {
            var DraftReply = new Models.ComplaintModel.ComplaintReplyModel()
            {
                attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>(),
                reply = editReplyText.Text,
                complaint_id = Complaint.id,
                user_id = Controllers.LoginRegisterController.LoggedUser.id.Value
            };

            foreach (var Attachment in lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>())
                DraftReply.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
                {
                    attachment_data = Convert.ToBase64String(Attachment.Data),
                    attachment_url = Attachment.AttachmentFileName,
                    complaint_reply_id = Complaint.id,
                    user_id = Controllers.LoginRegisterController.LoggedUser.id.Value
                });

            ComplaintDraftGuid = Controllers.ComplaintDraftController.SaveDraft(null, DraftReply, ComplaintDraftGuid, Complaint.element.slug, DraftType);
        }

        private void BtnSaveReply_Clicked(object sender, EventArgs e)
        {
            SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType.Draft);
            Acr.UserDialogs.UserDialogs.Instance.Toast("Vaš odgovor je spremljen u skice", new TimeSpan(0, 0, 3));
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

            if (view == imgAttachDocs)
            {
                var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                if (!string.IsNullOrEmpty(Picker?.FileName))
                {
                    var AttachmentView = new AttachmentView(false, 0, 0, Picker.FileName, true, Picker.DataArray);
                    lytAttachments.Children.Add(AttachmentView);

                    AttachmentView.AttachmentDeletedEvent += (View v) =>
                    {
                        lytAttachments.Children.Remove(v);
                        WriteNewComplaintModel.attachments.Remove(WriteNewComplaintModel.attachments.Single(a => a.attachment_mime == v.Id.ToString()));
                    };

                    WriteNewComplaintModel.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
                    {
                        attachment_data = Convert.ToBase64String(Picker.DataArray),
                        attachment_extension = Picker.FileName.Substring(Picker.FileName.LastIndexOf(".")),
                        attachment_url = Picker.FileName,
                        attachment_mime = AttachmentView.Id.ToString()
                    });
                }
                Picker = null;
            }
            else if (view == imgTakePhoto)
            {
                var photo = await Controllers.CameraController.TakePhoto();
                if (photo != null)
                {
                    var MS = new System.IO.MemoryStream();
                    photo.GetStream().CopyTo(MS);
                    var PhotoName = photo.Path.Substring(photo.Path.LastIndexOf("/") + 1);
                    var AttachmentView = new AttachmentView(false, 0, 0, PhotoName, true, MS.ToArray());
                    lytAttachments.Children.Add(AttachmentView);

                    AttachmentView.AttachmentDeletedEvent += (View v) =>
                    {
                        lytAttachments.Children.Remove(v);
                        WriteNewComplaintModel.attachments.Remove(WriteNewComplaintModel.attachments.Single(a => a.attachment_mime == v.Id.ToString()));
                    };

                    WriteNewComplaintModel.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
                    {
                        attachment_data = Convert.ToBase64String(MS.ToArray()),
                        attachment_extension = PhotoName.Substring(PhotoName.LastIndexOf(".")),
                        attachment_url = PhotoName,
                        attachment_mime = AttachmentView.Id.ToString()
                    });
                }
            }
            else if (view == imgTakeGPSLocation)
            {
                if (imgTakeGPSLocation.TextColor != Color.FromHex("#FF6A00"))
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Tražim vašu lokaciju", Acr.UserDialogs.MaskType.Clear);

                    var MyLocation = await Controllers.GPSController.GetPosition();
                    if (MyLocation != null)
                    {
                        Latitude = MyLocation.Latitude;
                        Longitude = MyLocation.Longitude;
                        imgTakeGPSLocation.TextColor = Color.FromHex("#FF6A00");
                        Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Vaša lokacija je pronađena");
                    }
                    else
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom dobivanja vaše lokacije!" + Environment.NewLine + "Provjerite jeli vam GPS uključen te da aplikaciji dozvolite pristup GPS-u", "Greška", "OK");

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                {
                    Latitude = 0;
                    Longitude = 0;
                    imgTakeGPSLocation.TextColor = Color.Gray;
                }
            }
            else if (view == btnSendReply)
                BtnSendReply_Clicked(null,null );
            SaveToDevice();
        }

        private void editReplyText_Focused(object sender, FocusEventArgs e)
        {
            ComplaintCoversationHeaderView.IsVisible = false;
            MainsStack.Padding = new Thickness(25, 15, 25, 30);
        }

        private void editReplyText_Unfocused(object sender, FocusEventArgs e)
        {
            ComplaintCoversationHeaderView.IsVisible = true;
            MainsStack.Padding = new Thickness(25, 35, 25, 30);
        }

        protected override bool OnBackButtonPressed()
        {
            if (!string.IsNullOrEmpty(editReplyText.Text) | lytAttachments.Children.Any())
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                       new Acr.UserDialogs.ConfirmConfig()
                       {
                           Title = "Prekid",
                           CancelText = "NE",
                           OkText = "DA",
                           Message = "Jeste li sigurni u prekid?",
                           OnAction = (Confirm)=>{
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
