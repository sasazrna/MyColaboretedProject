using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewComplaintPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private string Latitude = string.Empty, Longitude = string.Empty;
        public delegate void ComplaintSentHandler(int id);
        public event ComplaintSentHandler ComplaintSentEvent;
        private Models.ComplaintModel.DraftComplaintModel WriteNewComplaintModel;
        private Guid ComplaintDraftGuid;
        private Models.CompanyElementModel CompanyElement;
        private DateTime ProblemOccurred;
        private DateTime DateProblem;
        private DateTime TimeProblem;

        public NewComplaintPage()
        {
            InitializeComponent();
        }

        public NewComplaintPage(Models.CompanyElementModel companyElement, Models.ComplaintModel.DraftComplaintModel _WriteNewComplaintModel = null)
        {
            InitializeComponent();

            FaNow.Text = Views.FontAwesomeLabel.Images.FAClockO;
            FaPast.Text = Views.FontAwesomeLabel.Images.FACalendarO;

            WriteNewComplaintModel = _WriteNewComplaintModel;
            CompanyElement = companyElement;
            if (WriteNewComplaintModel == null)
            {
                WriteNewComplaintModel = new Models.ComplaintModel.DraftComplaintModel();
                WriteNewComplaintModel.QuickComplaint = false;
                WriteNewComplaintModel.element_id = companyElement.id;
                WriteNewComplaintModel.element_slug = companyElement.slug;
                ComplaintCoversationHeaderView.SetHeaderInfo(CompanyElement.name, CompanyElement.name);
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

                editComplaintText.Text = WriteNewComplaintModel.complaint ?? string.Empty;
                Task.Run(async () =>
                {
                    var CompanyElementRoot =
                    JsonConvert.DeserializeObject<Models.CompanyElementRootModel>
                    (await DataExchangeServices.GetCompanyElementData(WriteNewComplaintModel.element_slug));
                    CompanyElement = CompanyElementRoot.element;
                    ComplaintCoversationHeaderView.SetHeaderInfo(CompanyElement.name, CompanyElement.name);
                });

                editSuggestionText.Text = WriteNewComplaintModel.suggestion ?? string.Empty;
                ProblemOccurred = DateTime.Parse(WriteNewComplaintModel.problem_occurred ?? DateTime.Now.ToString());
                labela_vremena_sad.Text = ProblemOccurred.ToString();
                labela_vremena_sad.IsVisible = true;
            }
        
            imgAttachDocs.Text = '\uf1c1'.ToString();
            imgAttachDocs.TextColor = Color.Gray;

            imgTakePhoto.Text = '\uf030'.ToString();
            imgTakePhoto.TextColor = Color.Gray;

            imgTakeGPSLocation.Text = '\uf041'.ToString();
            imgTakeGPSLocation.TextColor = Color.Gray;

            btnSendComplaint.Text = Views.FontAwesomeLabel.Images.FASend_msg;
            btnSendComplaint.TextColor = Color.FromHex("#FF7e65");

            arrivalTimePicke.PropertyChanged += ArrivalTimePicke_PropertyChanged;
          
            arrivalDatePicker.IsVisible = false;
            arrivalTimePicke.IsVisible = false;

            Sada_stack.IsVisible = false;
            Ranije_stack.IsVisible = false;

            TAPController = new Controllers.TAPController(imgAttachDocs, imgTakeGPSLocation, imgTakePhoto, btnSendComplaint, SadaStackButton, RanijeStackButton);

            TAPController.SingleTaped += TAPController_SingleTaped;
            //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            editComplaintText.TextChanged += EditComplaintText_TextChanged;
            editSuggestionText.TextChanged += EditComplaintText_TextChanged;
            arrivalDatePicker.DateSelected += ArrivalDatePicker_DateSelected;           
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

            if (view == imgAttachDocs)
                InitAttachDocs();
            else if (view == imgTakePhoto)
                InitTakePhoto();
            else if (view == imgTakeGPSLocation)
                InitTakeGPSLocation();
            else if (view == SadaStackButton)
            {
                ZaPopunit_stack.IsVisible = false;
                Ranije_stack.IsVisible = false;
                Sada_stack.IsVisible = true;
                labela_vremena_sad.Text = DateTime.Now.ToString();
                ProblemOccurred = DateTime.Now;
                WriteNewComplaintModel.problem_occurred = ProblemOccurred.ToString();
                SaveToDevice();
            }
            else if (view == RanijeStackButton)
            {
                ZaPopunit_stack.IsVisible = false;
                Sada_stack.IsVisible = false;
                Ranije_stack.IsVisible = true;
                arrivalDatePicker.Focus();
                ProblemOccurred = DateTime.Now;
            }
            else if (view == btnSendComplaint)
                SendComplaint();
            SaveToDevice();
        }

        private async void InitAttachDocs()
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
                SaveToDevice();
            }
        }

        private async void InitTakePhoto()
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
                SaveToDevice();
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

                    imgTakeGPSLocation.TextColor = Color.FromHex("#FF6A00");

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Vaša lokacija je pronađena");
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
                imgTakeGPSLocation.TextColor = Color.Gray;
            }
            SaveToDevice();
        }

        private async void SendComplaint()
        {
            if (editComplaintText.Text == null || editComplaintText.Text?.Length < 20)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaš prigovor treba biti duži od 20 znakova!", null, "OK");
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem vaš prigovor");
            await Task.Delay(19);
            List<int> attachment_ids = new List<int>();

            foreach (var Attachment in lytAttachments.Children.OfType<AttachmentView>().Cast<AttachmentView>())
                attachment_ids.Add(await DataExchangeServices.SendComplaintAttachment(Attachment.Data, Attachment.AttachmentFileName));

            if (lytAttachments.Children.Any() && (attachment_ids == null | attachment_ids.Any(aid => aid == 0)))
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vaših privitaka!" + System.Environment.NewLine + "Pokušajte ponovno poslati", "Greška", "OK");
                return;
            }

            var result = await DataExchangeServices.SendComplaint(
             JsonConvert.SerializeObject(new
             {
                 complaint = editComplaintText.Text,
                 element_id = CompanyElement.id,
                 attachment_ids = attachment_ids,
                 suggestion = editSuggestionText.Text,
                 problemOccurred = ProblemOccurred.ToString("dd.MMMMM yyyy"),
                 problemOccurred_submit = ProblemOccurred.ToString("dd.M.yyyy"),
                 problemOccurredTime = ProblemOccurred.ToString("HH:mm"),
                 latitude = Latitude,
                 longitude = Longitude
             }));
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            if (!result.Contains("Error"))
            {
                Application.Current.Properties.Remove("WriteComplaintAutoSave");
                await Application.Current.SavePropertiesAsync();
                var ComplaintSentPage = new ComplaintSentPage(CompanyElement.root_business.complaint_received_message, false);
                ComplaintSentPage._PageClosed += (() => { Navigation.PopModalAsync(); ComplaintSentEvent?.Invoke(0); });
                await Navigation.PushModalAsync(ComplaintSentPage);
                return;
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja vašeg prigovora, moguće zbog internet konekcije" + Environment.NewLine +
                 /*   "Vaš prigovor je spremljen na vašem mobitelu te će biti automatski poslan prvom prilikom", */"Greška u slanju prigovora", "OK");
                //    SaveReply(Models.ComplaintModel.DraftComplaintModel.DraftType.Unsent);
            }
        }

        private void EditComplaintText_TextChanged(object sender, TextChangedEventArgs e)
        {
            WriteNewComplaintModel.complaint = editComplaintText.Text;
            WriteNewComplaintModel.suggestion = editSuggestionText.Text;
            SaveToDevice();
        }

        private void SaveToDevice()
        {
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.Properties.Add("WriteComplaintAutoSave", JsonConvert.SerializeObject(WriteNewComplaintModel));
            Application.Current.SavePropertiesAsync();
        }

        private void ArrivalTimePicke_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            {
                labelasati.Text = arrivalTimePicke.Time.ToString();
                labelasati.TextColor = Color.Silver;
                ProblemOccurred = new DateTime(ProblemOccurred.Year, ProblemOccurred.Month, ProblemOccurred.Day, arrivalTimePicke.Time.Hours, arrivalTimePicke.Time.Minutes, 0);
                TimeProblem = ProblemOccurred;
                WriteNewComplaintModel.problem_occurred = ProblemOccurred.ToString();
                SaveToDevice();
            }
        }

        private void ArrivalDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            labelavremena.Text = e.NewDate.ToString("dd.MM.yyyy ");
            labelavremena.TextColor = Color.Silver;
            labelasati.Text = arrivalTimePicke.Time.ToString();
            labelasati.TextColor = Color.Silver;
            ProblemOccurred = e.NewDate;
            ProblemOccurred = new DateTime(ProblemOccurred.Year, ProblemOccurred.Month, ProblemOccurred.Day, TimeProblem.Hour, TimeProblem.Minute, 0);
            WriteNewComplaintModel.problem_occurred = ProblemOccurred.ToString();
            SaveToDevice();
            arrivalTimePicke.Focus();
        }

        protected override bool OnBackButtonPressed()
        {
            if (!string.IsNullOrEmpty(editComplaintText.Text) | lytAttachments.Children.Any())
            {
                Acr.UserDialogs.UserDialogs.Instance.ActionSheet(
                    new Acr.UserDialogs.ActionSheetConfig()
                    {
                        Title = "Jeste li sigurni u prekid?",
                        // Message = "Odlučili ste prekinuti slanje prigovora, želite li ga spremiti za poslije?",
                        UseBottomSheet = false,
                        Options = new List<Acr.UserDialogs.ActionSheetOption>()
                    { new Acr.UserDialogs.ActionSheetOption("DA", (async()=> {await Navigation.PopAsync(true); })),
                      new Acr.UserDialogs.ActionSheetOption("NE", ()=> { return; } ) }
                    });

            }
            else Navigation.PopAsync(true);

            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.SavePropertiesAsync();

            return false;
        }
    }
}
