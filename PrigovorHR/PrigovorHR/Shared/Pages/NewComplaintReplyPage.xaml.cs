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
        //  internal event Controllers.EventHandlers.ComplaintSentHandler ComplaintSentEvent;
        public delegate void ReplySentHandler();
        public event ReplySentHandler ReplaySentEvent;

        public NewComplaintReplyPage(Models.ComplaintModel complaint)
        {
            InitializeComponent();
            Complaint = complaint;

            imgAttachDocs.Text = '\uf1c1'.ToString();
            imgAttachDocs.TextColor = Color.Gray;

            imgTakePhoto.Text = '\uf030'.ToString();
            imgTakePhoto.TextColor = Color.Gray;

            imgTakeGPSLocation.Text = '\uf041'.ToString();
            imgTakeGPSLocation.TextColor = Color.Gray;

            ComplaintCoversationHeaderView.SetHeaderInfo(Complaint.replies.Any() ?
                          Complaint.replies.LastOrDefault(r => r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? "nepoznato" :
                          "nepoznato", Complaint.element.name, true);

            TAPController = new Controllers.TAPController(imgAttachDocs, imgTakeGPSLocation, imgTakePhoto);
            TAPController.SingleTaped += TAPController_SingleTaped;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            ComplaintCoversationHeaderView.SendComplaintEvent += ComplaintCoversationHeaderView_SendComplaintEvent;
        }

        private async void ComplaintCoversationHeaderView_SendComplaintEvent()
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem vaš prigovor");
            await Task.Delay(19);
            List<int> attachment_ids = new List<int>();

            foreach (var Attachment in lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>())
                attachment_ids.Add((int)await DataExchangeServices.SendReplyAttachment(Attachment.Data, Attachment.AttachmentFileName));

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
                 attachment_ids = attachment_ids
             }));
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            if (!result.Contains("Error"))
            {
                //await Rg.Plugins.Popup.Services.PopupNavigation.PopAsync(true);
                //Application.Current.Properties.Remove("WriteComplaintAutoSave");
                //await Application.Current.SavePropertiesAsync();

                var ComplaintSentPage = new ComplaintSentPage(Complaint.element.root_business.complaint_received_message, false);
                ComplaintSentPage._PageClosed += (() => { Navigation.PopModalAsync(); ReplaySentEvent?.Invoke(); });
                await Navigation.PushModalAsync(ComplaintSentPage);
                return;
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vašeg prigovora!" + System.Environment.NewLine + "Provjerite vašu internet konekciju te kliknite ponovno za slanje", "Greška u slanju prigovora", "OK");
            }
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == imgAttachDocs)
            {
                var Picker = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                if (!string.IsNullOrEmpty(Picker.FileName))
                {
                    var AttachmentView = new AttachmentView(false, 0, 0, Picker.FileName, true, Picker.DataArray);
                    lytAttachments.Children.Add(AttachmentView);
                    AttachmentView.AttachmentDeletedEvent += (View v) => { lytAttachments.Children.Remove(v); };
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
                    AttachmentView.AttachmentDeletedEvent += (View v) => { lytAttachments.Children.Remove(v); };
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
        }
    }
}
