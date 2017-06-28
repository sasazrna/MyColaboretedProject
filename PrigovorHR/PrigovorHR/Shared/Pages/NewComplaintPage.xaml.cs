using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complio.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;

namespace Complio.Shared.Pages
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
        public static NewComplaintPage ReferenceToPage;
        private int MessageType;
        private Dictionary<int, string> InstructionForEditor = 
            new Dictionary<int, string>() { { 0, "Napišite prigovor..." },
                { 2, "Napišite pohvalu..." }, { 3, "Napišite prijedlog..." }, { 4, "Napišite upit..." } };

        public NewComplaintPage()
        {
            InitializeComponent();
        }

        public NewComplaintPage(Models.CompanyElementModel companyElement,
                                int messageType,
                                Models.ComplaintModel.DraftComplaintModel _WriteNewComplaintModel = null)
        {
            InitializeComponent();

            FaNow.Text = Views.FontAwesomeLabel.Images.FAClockO;
            FaPast.Text = Views.FontAwesomeLabel.Images.FACalendarO;
            MessageType = messageType;

            WriteNewComplaintModel = _WriteNewComplaintModel;
            CompanyElement = companyElement;
            if (WriteNewComplaintModel == null)
            {
                WriteNewComplaintModel = new Models.ComplaintModel.DraftComplaintModel();
                WriteNewComplaintModel.QuickComplaint = false;
                WriteNewComplaintModel.element_id = companyElement.id;
                WriteNewComplaintModel.element_slug = companyElement.slug;
                ComplaintCoversationHeaderView.SetHeaderInfo(CompanyElement.name, CompanyElement.name);
                AttachmentListView = new AttachmentListView(ref WriteNewComplaintModel, false);
            }
            else
            {
                AttachmentListView = new AttachmentListView(ref WriteNewComplaintModel, false);

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
            lytAttachmentsAndEditors.Children.RemoveAt(0);
            lytAttachmentsAndEditors.Children.Insert(0, AttachmentListView);

            arrivalTimePicke.PropertyChanged += ArrivalTimePicke_PropertyChanged;

            arrivalDatePicker.IsVisible = false;
            arrivalTimePicke.IsVisible = false;

            Sada_stack.IsVisible = false;
            Ranije_stack.IsVisible = false;

            if (MessageType>0)
            {
                lytTimePicker.IsVisible = false;
                editSuggestionText.IsVisible = false;
            }

            editComplaintText.Text = InstructionForEditor[Convert.ToInt32(MessageType)];

            TAPController = new Controllers.TAPController(SadaStackButton, RanijeStackButton);

            TAPController.SingleTaped += TAPController_SingleTaped;
            editComplaintText.TextChanged += EditComplaintText_TextChanged;
            editSuggestionText.TextChanged += EditComplaintText_TextChanged;
            arrivalDatePicker.DateSelected += ArrivalDatePicker_DateSelected;
            AutomationId = "NewComplaintPage";
            ReferenceToPage = this;         
        }

        private void Editor_FocusedUnfocused(object sender, FocusEventArgs e)
        {
            if (MessageType == 0)
            {
                lytTimePicker.IsVisible = !e.IsFocused;
                ComplaintCoversationHeaderView.IsVisible = !e.IsFocused;
            }
            var SelectedEditor = ((Editor)e.VisualElement);

            AttachmentListView.HideUnhideAttachments(e.IsFocused);

            if (e.IsFocused)
            {
                editSuggestionText.IsVisible = SelectedEditor.AutomationId == editSuggestionText.AutomationId;
                editComplaintText.IsVisible = SelectedEditor.AutomationId == editComplaintText.AutomationId;

                if (SelectedEditor.AutomationId == editSuggestionText.AutomationId)
                    editComplaintTextUderStack.IsVisible = false;

                foreach (var IFE in InstructionForEditor)
                    if (SelectedEditor.Text == IFE.Value)
                        Device.StartTimer(new TimeSpan(0, 0, 0, 0, 10), () => { SelectedEditor.Text = string.Empty; return false; });
            }
            else
            {
                editSuggestionText.IsVisible = MessageType == 0;
                editComplaintText.IsVisible = true;

                if (SelectedEditor.AutomationId == editSuggestionText.AutomationId)
                    editComplaintTextUderStack.IsVisible = true;

                if (string.IsNullOrEmpty(editComplaintText.Text))
                    editComplaintText.Text = InstructionForEditor[Convert.ToInt32(MessageType)];
                if (string.IsNullOrEmpty(editSuggestionText.Text))
                    editSuggestionText.Text = "Napišite prijedlog...";
            }
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (WriteNewComplaintModel.attachments == null)
                WriteNewComplaintModel.attachments = new List<Models.ComplaintModel.ComplaintAttachmentModel>();

             if (view == SadaStackButton)
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
           
            SaveToDevice();
        }

        public async void SendComplaint()
        {
            if (editComplaintText.Text == null || editComplaintText.Text?.Length < 20)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaš prigovor treba biti duži od 20 znakova!", null, "OK");
                return;
            }

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem vaš prigovor");
            await Task.Delay(19);
            List<int> attachment_ids = new List<int>();

            foreach (var Attachment in AttachmentListView.GetAttachmentsData())
                attachment_ids.Add(await DataExchangeServices.SendComplaintAttachment(Attachment.Data, Attachment.AttachmentFileName));

            if (attachment_ids == null | attachment_ids.Any(aid => aid == 0))
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
                 longitude = Longitude,
                 messageType = MessageType
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

        protected override void OnDisappearing()
        {
            Application.Current.Properties.Remove("WriteComplaintAutoSave");
            Application.Current.SavePropertiesAsync();
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (!string.IsNullOrEmpty(editComplaintText.Text) | AttachmentListView.GetAttachmentsData().Any())
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
