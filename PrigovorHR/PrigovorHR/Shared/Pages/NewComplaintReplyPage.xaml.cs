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
        private Models.ComplaintModel.WriteNewComplaintModel WriteNewComplaintModel;

        public NewComplaintReplyPage(Models.ComplaintModel complaint, Models.ComplaintModel.WriteNewComplaintModel _WriteNewComplaintModel = null)
        {
            InitializeComponent();
            Complaint = complaint;

                WriteNewComplaintModel = _WriteNewComplaintModel;

                if (WriteNewComplaintModel == null)
                {
                    WriteNewComplaintModel = new Models.ComplaintModel.WriteNewComplaintModel();
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
                           Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? "nepoznato" :
                           "nepoznato", Complaint.element.name);

                btnSendReply.TranslateTo(0, -60, 0);

                imgAttachDocs.Text = '\uf1c1'.ToString();
                imgAttachDocs.TextColor = Color.Gray;

                imgTakePhoto.Text = '\uf030'.ToString();
                imgTakePhoto.TextColor = Color.Gray;

                imgTakeGPSLocation.Text = '\uf041'.ToString();
                imgTakeGPSLocation.TextColor = Color.Gray;

                TAPController = new Controllers.TAPController(imgAttachDocs, imgTakeGPSLocation, imgTakePhoto);
                btnSaveReply.Clicked += BtnSaveReply_Clicked;
                btnSendReply.Clicked += BtnSendReply_Clicked;

                TAPController.SingleTaped += TAPController_SingleTaped;
                NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
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
            if (editReplyText.Text.Length < 20)
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
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vašeg prigovora!" + System.Environment.NewLine + "Provjerite vašu internet konekciju te kliknite ponovno za slanje", "Greška u slanju prigovora", "OK");
            }
        }

        private void BtnSaveReply_Clicked(object sender, EventArgs e)
        {

        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

            if (view == imgAttachDocs)
            {
                var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                if (!string.IsNullOrEmpty(Picker.FileName))
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
                        attachment_url = Picker.FileName, attachment_mime = AttachmentView.Id.ToString()
                    });
                }
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
                        attachment_data =  Convert.ToBase64String(MS.ToArray()),
                        attachment_extension = PhotoName.Substring(PhotoName.LastIndexOf(".")),
                        attachment_url = PhotoName,
                        attachment_mime = AttachmentView.Id.ToString()
                    });
                }
            }
            else if (view == imgTakeGPSLocation)
            {
                if (imgTakeGPSLocation.BackgroundColor != Color.FromHex("#FF6A00"))
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Tražim vašu lokaciju", Acr.UserDialogs.MaskType.Clear);

                    var MyLocation = await Controllers.GPSController.GetPosition();
                    if (MyLocation != null)
                    {
                        Latitude = MyLocation.Latitude;
                        Longitude = MyLocation.Longitude;
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom dobivanja vaše lokacije!" + Environment.NewLine + "Provjerite jeli vam GPS uključen te da aplikaciji dozvolite pristup GPS-u", "Greška", "OK");
                    }
                }
                else
                {
                    Latitude = 0;
                    Longitude = 0;
                    imgTakeGPSLocation.BackgroundColor = Color.Gray;
                }
            }
            SaveToDevice();
        }

        protected override bool OnBackButtonPressed()
        {
            NavigationBar.InitBackButtonPressed();
            return true;
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            if (!string.IsNullOrEmpty(editReplyText.Text) | lytAttachments.Children.Any())
            {
                Acr.UserDialogs.UserDialogs.Instance.ActionSheet(
                    new Acr.UserDialogs.ActionSheetConfig()
                    {
                        Title = "Izlazak",
                        Message = "Odlučili ste prekinuti slanje prigovora, želite li ga spremiti za poslije?",
                        UseBottomSheet = true,
                        Options = new List<Acr.UserDialogs.ActionSheetOption>()
                    { new Acr.UserDialogs.ActionSheetOption("DA", (()=> {  })),
                      new Acr.UserDialogs.ActionSheetOption("NE", (async()=> {await Navigation.PopModalAsync(true); })),
                      new Acr.UserDialogs.ActionSheetOption("Nemoj prekinuti", ()=>{ })}
                    });
            }
            else await Navigation.PopModalAsync(true);

            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            await Application.Current.SavePropertiesAsync();
        }
    }
}
