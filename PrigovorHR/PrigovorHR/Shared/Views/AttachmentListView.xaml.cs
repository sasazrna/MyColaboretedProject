using Newtonsoft.Json;
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
    public partial class AttachmentListView : ContentView
    {
        private Controllers.TAPController TAPController;
        public Models.ComplaintModel.DraftComplaintModel WriteNewComplaintModel;
        private string Latitude = string.Empty, Longitude = string.Empty;
        private bool IsReply = false;

        public AttachmentListView()
        {
            InitializeComponent();
        }

        public AttachmentListView(ref Models.ComplaintModel.DraftComplaintModel writeNewComplaintModel, bool isReply)
        {
            InitializeComponent();

            WriteNewComplaintModel = writeNewComplaintModel;
            IsReply = isReply;

            imgAttachDocs.Text = '\uf1c1'.ToString();
            imgAttachDocs.TextColor = Color.Gray;

            imgTakePhoto.Text = '\uf030'.ToString();
            imgTakePhoto.TextColor = Color.Gray;

            imgTakeGPSLocation.Text = '\uf041'.ToString();
            imgTakeGPSLocation.TextColor = Color.Gray;

            if (IsReply)
                imgTakeGPSLocation.IsVisible = false;

            ListDropDownOption.Text = Views.FontAwesomeLabel.Images.FACaretDown;
            ListDropDownOption.TextColor = Color.Gray;

            ClearListOption.Text = Views.FontAwesomeLabel.Images.FAClose;
            ClearListOption.TextColor = Color.Gray;
            TAPController = new Controllers.TAPController(imgAttachDocs, imgTakeGPSLocation, imgTakePhoto, ClearListOption, ListDropDownOption);
            TAPController.SingleTaped += TAPController_SingleTaped;

            if (WriteNewComplaintModel?.attachments != null)
                SetAttachments(WriteNewComplaintModel.attachments);
        }

        private void SetAttachments(List<Models.ComplaintModel.ComplaintAttachmentModel> attachments)
        {
            foreach (var Attachment in attachments ?? new List<Models.ComplaintModel.ComplaintAttachmentModel>())
            {
                var AttachmentView = new AttachmentView(true, 0, 0, Attachment.attachment_url, true, Convert.FromBase64String(Attachment.attachment_data));
                lytAttachments.Children.Add(AttachmentView);
                AttachmentView.AutomationId = Attachment.attachment_mime;

                if(Attachment.attachment_url.Contains("loc:"))
                    imgTakeGPSLocation.TextColor = Color.FromHex("#FF6A00");

                AttachmentView.AttachmentDeletedEvent += (View v) =>
                {
                    lytAttachments.Children.Remove(v);
                    attachments.Remove(attachments.Single(a => a.attachment_mime == v.AutomationId.ToString()));

                    lytAttachmentContainer.IsVisible = lytAttachments.Children.Any();
                };
            }

            if (attachments.Any())
            {
                lytAttachmentContainer.IsVisible = true;
                lytAttachments.IsVisible = true;
            }
        }

        public void SetAttachment(string AttachmentName, byte[] Data)
        {
            var AttachmentView = new AttachmentView(false, 0, 0, AttachmentName, true, Data);
            lytAttachments.Children.Add(AttachmentView);

            AttachmentView.AttachmentDeletedEvent += (View v) =>
            {
                if (((AttachmentView)v).AttachmentFileName.Contains("loc:"))
                {
                    InitTakeGPSLocation();
                }
                else
                {
                    WriteNewComplaintModel.attachments.Remove(WriteNewComplaintModel.attachments.Single(a => a.attachment_mime == v.Id.ToString()));
                    lytAttachments.Children.Remove(v);

                    lytAttachmentContainer.IsVisible = lytAttachments.Children.Any();
                }
            };

            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

                WriteNewComplaintModel.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
                {
                    attachment_data = Data != null ? Convert.ToBase64String(Data) : string.Empty,
                    attachment_extension =
                    !AttachmentName.Contains("loc:") ? AttachmentName.Substring(AttachmentName.LastIndexOf(".")) : string.Empty,
                    attachment_url = AttachmentName,
                    attachment_mime = AttachmentView.Id.ToString()
                });

            SaveToDevice();
            lytAttachmentContainer.IsVisible = true;
            lytAttachments.IsVisible = true;
            ListDropDownOption.RotateTo(0, 100);
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == imgAttachDocs)
                InitAttachDocs();
            else if (view == imgTakePhoto)
                InitTakePhoto();
            else if (view == imgTakeGPSLocation)
                InitTakeGPSLocation();
            else if (view == ListDropDownOption)
            {
                lytAttachments.IsVisible = !lytAttachments.IsVisible;
                ListDropDownOption.RotateTo(!lytAttachments.IsVisible ? 180 : 0, 100);
            }
            else if (view == ClearListOption)
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(new Acr.UserDialogs.ConfirmConfig()
                {
                    Message = "Jeste li sigurni u brisanje svih privitaka?",
                    CancelText = "NE",
                    OkText = "DA",
                    Title = "Brisanje privitaka",
                    OnAction = (bool b) =>
                    {
                        if (b)
                        {
                            lytAttachmentContainer.IsVisible = false;
                            lytAttachments.Children.Clear();
                            WriteNewComplaintModel = new Models.ComplaintModel.DraftComplaintModel();
                            imgTakeGPSLocation.TextColor = Color.Gray;
                        }
                    }
                });
            }
        }

        private async void InitAttachDocs()
        {
            var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
            if (!string.IsNullOrEmpty(Picker?.FileName))
                SetAttachment(Picker.FileName, Picker.DataArray);
        }

        private async void InitTakePhoto()
        {
            var photo = await Controllers.CameraController.TakePhoto();
            if (photo != null)
            {
                var MS = new System.IO.MemoryStream();
                photo.GetStream().CopyTo(MS);
                var PhotoName = photo.Path.Substring(photo.Path.LastIndexOf("/") + 1);
                SetAttachment(PhotoName, MS.ToArray());

                if (!AppGlobal.AppIsComplio & string.IsNullOrEmpty(Latitude) & !IsReply)
                    InitTakeGPSLocation();
            }
        }

        private async void InitTakeGPSLocation()
        {
            if (imgTakeGPSLocation.TextColor != Color.FromHex("#FF6A00"))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Tražim vašu lokaciju", Acr.UserDialogs.MaskType.Clear);

                var MyLocation = await Controllers.GPSController.GetPosition();
                if (MyLocation != null)
                {
                    Latitude = MyLocation.Latitude.ToString().Replace(".", ",");
                    Longitude = MyLocation.Longitude.ToString().Replace(".", ",");
                    WriteNewComplaintModel.latitude = Latitude;
                    WriteNewComplaintModel.longitude = Longitude;
                    imgTakeGPSLocation.TextColor = Color.FromHex("#FF6A00");

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Vaša lokacija je pronađena");
                    SetAttachment("loc:" + Latitude + "," + Longitude, null);
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom dobivanja vaše lokacije!" + Environment.NewLine + "Provjerite jeli vam GPS uključen te da aplikaciji dozvolite pristup GPS-u", "Greška", "OK");
                }
            }
            else
            {
                Latitude = string.Empty;
                Longitude = string.Empty;
                WriteNewComplaintModel.latitude = Latitude;
                WriteNewComplaintModel.longitude = Longitude;
                imgTakeGPSLocation.TextColor = Color.Gray;
                WriteNewComplaintModel.attachments.Remove(WriteNewComplaintModel.attachments.Single(a => a.attachment_url.Contains("loc:")));
                lytAttachments.Children.Remove(GetAttachmentsData().Single(a => a.AttachmentFileName.Contains("loc:")));
                lytAttachmentContainer.IsVisible = lytAttachments.Children.Any();
            }
            SaveToDevice();
        }

        public void HideUnhideAttachments(bool Hide)
        {
            lytAttachments.IsVisible = !Hide;
        }

        public List<AttachmentView> GetAttachmentsData()
        {
            return lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>().ToList();
        }
        private void SaveToDevice()
        {
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.Properties.Add("WriteComplaintAutoSave", JsonConvert.SerializeObject(WriteNewComplaintModel));
            Application.Current.SavePropertiesAsync();
        }
    }
}