using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using PrigovorHR.Shared.Models;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{

    public partial class QuickComplaintPage : PopupPage
    {
        private Controllers.TAPController TapController;
        private Models.ComplaintModel.DraftComplaintModel WriteNewComplaintModel = new Models.ComplaintModel.DraftComplaintModel();
        private byte[] PhotoData;
        private string PhotoName = string.Empty;
        internal event Controllers.EventHandlers.ComplaintSentHandler ComplaintSentEvent;

        public QuickComplaintPage(Models.CompanyElementModel _CompanyElement = null, Models.ComplaintModel.DraftComplaintModel _WriteNewComplaintModel = null)
        {
            InitializeComponent();
            TapController = new Controllers.TAPController(complaintLabel, suggestionLabel, imgTakePhoto, btnSendComplaint);

            WriteNewComplaintModel.QuickComplaint = true;

            if (_CompanyElement != null)
            {
                WriteNewComplaintModel.element_id = _CompanyElement.id;
                WriteNewComplaintModel.complaint_received_message = _CompanyElement.root_business.complaint_received_message;
                lblElementName.Text = _CompanyElement.name;
            }
            //a gdje je povratak slike natrag u bytearray?? Napravi to

            if (_WriteNewComplaintModel != null)
            {
                WriteNewComplaintModel = _WriteNewComplaintModel;
                suggestionEditor.Text = WriteNewComplaintModel.suggestion;
                complaintEditor.Text = WriteNewComplaintModel.complaint;

                if (WriteNewComplaintModel.attachments != null)
                {
                    imgTakePhoto.TextColor = Color.Orange;
                    PhotoData = Convert.FromBase64String(WriteNewComplaintModel.attachments[0].attachment_data);
                }
                lblElementName.Text = _WriteNewComplaintModel.ElementName;

                PrigEditor_TextChanged(null, new TextChangedEventArgs(suggestionEditor.Text, suggestionEditor.Text));
            }

            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

            imgTakePhoto.Text = Views.FontAwesomeLabel.Images.FACamera;
            imgTakePhoto.TextColor = imgTakePhoto.TextColor != Color.Orange ? Color.Gray : Color.Orange;

            btnSendComplaint.Text = Views.FontAwesomeLabel.Images.FASend_msg;
            btnSendComplaint.TextColor = Color.FromHex("#ff7e65");

            TapController.SingleTaped += TapController_SingleTaped;
            complaintEditor.TextChanged += PrigEditor_TextChanged;
            suggestionEditor.TextChanged += PrigEditor_TextChanged;
        }


        private void PrigEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            suggestionLabel.TextColor = complaintEditor.Text?.Length > 9 ? Color.FromHex("#FF6A00") : Color.Gray;
            if (e.NewTextValue?.Length > 200)
                complaintEditor.Text = e.OldTextValue;
            else
            {
                WriteNewComplaintModel.complaint = complaintEditor.Text;
                WriteNewComplaintModel.suggestion = suggestionEditor.Text;
                SaveToDevice();
            }
        }

        private async void TakePhoto()
        {
            if (imgTakePhoto.TextColor != Color.Orange)
            {
                var photo = await Controllers.CameraController.TakePhoto();
                if (photo != null)
                {
                    imgTakePhoto.TextColor = Color.Orange;
                    var MS = new System.IO.MemoryStream();
                    photo.GetStream().CopyTo(MS);
                    PhotoData = MS.ToArray();
                    WriteNewComplaintModel.attachments.Clear();
                    WriteNewComplaintModel.attachments.Add(new Models.ComplaintModel.ComplaintAttachmentModel()
                    {
                        attachment_url = photo.Path,
                        attachment_extension = photo.Path.Substring(photo.Path.LastIndexOf(".")),
                        attachment_data = Convert.ToBase64String(PhotoData)
                    });
                    SaveToDevice();
                }
            }
            else
            {
                if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Želite li poništiti fotografiju?", "Poništavanje fotografije", "DA", "NE"))
                {
                    imgTakePhoto.TextColor = Color.Gray;
                    WriteNewComplaintModel.attachments.Clear();
                    PhotoData = null;
                    SaveToDevice();
                }
            }
        }

        private void SaveToDevice()
        {
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.Properties.Add("WriteComplaintAutoSave", JsonConvert.SerializeObject(WriteNewComplaintModel));
            Application.Current.SavePropertiesAsync();
        }

        private void TapController_SingleTaped(string viewId, View view)
        {
            if (view == complaintLabel)
                _complaintLayout.IsVisible = true;
            else if (view == suggestionLabel & suggestionLabel.TextColor != Color.Gray)
                _complaintLayout.IsVisible = false;
            else if (view == btnSendComplaint)
                SendComplaint();
            else if(view== imgTakePhoto)
                TakePhoto();

            complaintUnderlineLayout.IsVisible = _complaintLayout.IsVisible;
            suggestionLayout.IsVisible = !_complaintLayout.IsVisible;
            suggestionUnderlineLayout.IsVisible = !_complaintLayout.IsVisible;
        }

        protected override bool OnBackButtonPressed()
        {
            Close(null, null);
            return true;
        }

        private void Close(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(complaintEditor.Text) |
                !string.IsNullOrEmpty(suggestionEditor.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                    new Acr.UserDialogs.ConfirmConfig()
                    {
                        CancelText = "NE",
                        OkText = "DA",
                        Message = "Jeste li sigurni u prekid rada na prigovoru?",
                        Title = "Prekid prigovora",
                        OnAction = ((result) =>
                        {
                            if (result)
                            {
                                Rg.Plugins.Popup.Services.PopupNavigation.PopAsync(true);
                                Application.Current.Properties.Remove("WriteComplaintAutoSave");
                                Application.Current.SavePropertiesAsync();
                            }
                        })
                    });
            }
            else Rg.Plugins.Popup.Services.PopupNavigation.PopAsync(true);
        }

        private async void SendComplaint()
        {
            if ((string.IsNullOrEmpty(WriteNewComplaintModel.complaint)) ||
                (!string.IsNullOrEmpty(WriteNewComplaintModel.complaint) & WriteNewComplaintModel.complaint.Length < 20))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaš prigovor treba biti duži od 20 znakova!", null, "OK");
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem vaš prigovor");
            await Task.Delay(19);
            int[] attachment_ids = new int[1];

            if (PhotoData != null)
                attachment_ids = new int[1] { await DataExchangeServices.SendComplaintAttachment(PhotoData,
                                             WriteNewComplaintModel.attachments[0].attachment_url.Substring(
                                             WriteNewComplaintModel.attachments[0].attachment_url.LastIndexOf("/")+1)) };
            else
                attachment_ids = null;

            if ((attachment_ids != null && attachment_ids.First() > 0) || attachment_ids == null)
            {
                var result = await DataExchangeServices.SendComplaint(
                 JsonConvert.SerializeObject(new
                 {
                     complaint = WriteNewComplaintModel.complaint,
                     element_id = WriteNewComplaintModel.element_id,
                     attachment_ids = attachment_ids,
                     suggestion = WriteNewComplaintModel.suggestion,
                     problem_occurred = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", "/")
                 }));
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                if (!result.Contains("Error"))
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.PopAsync(true);
                    Application.Current.Properties.Remove("WriteComplaintAutoSave");
                    await Application.Current.SavePropertiesAsync();

                    var ComplaintSentPage = new ComplaintSentPage(WriteNewComplaintModel.complaint_received_message, false);
                    ComplaintSentPage._PageClosed += (() => { Navigation.PopModalAsync(); ComplaintSentEvent?.Invoke(); });
                    await Navigation.PushModalAsync(ComplaintSentPage);
                    return;
                }
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vašeg prigovora!" + System.Environment.NewLine + "Provjerite vašu internet konekciju te kliknite ponovno za slanje", "Greška u slanju prigovora", "OK");
        }
    }
}

