using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complio.Shared.Models;
using Complio.Shared.Views;
using Xamarin.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.Xaml;
using Complio.Shared.Controllers;
using System.IO;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPageWithLogin : ContentPage
    {
        public static LandingPageWithLogin ReferenceToView;
        private TAPController TAPController;
        Dictionary<string, string[]> FabImages = new Dictionary<string, string[]>{
            { "FabWriteMessage", new string[]{"FaFabButtonPen.png" } },
            { "FabQR", new string[]{"awsomeQR.png" }  },
            { "FabOpenOptions", new string[] { "FaFabButtonAdd.png", "FaFabButtonClose.png" } }};

        private Dictionary<string, View> Fabs = new Dictionary<string, View>();

        public FirstTimeLoginView firstTimeLoginView { get { return FirstTimeLoginView; } set { FirstTimeLoginView = value; } }
        public ListOfComplaintsView_BasicUser listOfComplaintsView { get { return ListOfComplaintsView; } set { ListOfComplaintsView = value; } }
        public static Controllers.QRScannerController QRScannerController;
        private bool AnimatingFABOpacity = false;

        public LandingPageWithLogin()
        {
            InitializeComponent();
            //When logged in, check if there is complaint that wasnt sent for some reason.
            LoadComplaintAutoSaveData();
            Device.StartTimer(new TimeSpan(0, 0, 1), () => { SetFABS(); return false; });
            
            ReferenceToView = this;
            FirstTimeLoginView.SearchIconClickedEvent += () => Navigation.PushPopupAsync(new CompanySearchPage(), true);
            ListOfComplaintsView.ListScrolledEvent += ListOfComplaintsView_ListScrolledEvent;
            AutomationId = "LandingPageWithLogin";

            if (!GPSController.IsGPSEnabled)
                Acr.UserDialogs.UserDialogs.Instance.Alert("Vaš GPS je isključen ili je ograničen pristup aplikacije vašem GPS-u." + 
                    Environment.NewLine + "Za punu funkcionalnost aplikacije je potrebno uključiti GPS", "Napomena", "OK");
        }

        private void ListOfComplaintsView_ListScrolledEvent()
        {
            if (Fabs.ContainsKey("FabOpenOptions"))
            {
                Fabs["FabOpenOptions"].Opacity = 1;

                if (ListOfComplaintsView.NumOfDisplayedComplaints > 4)
                {
                    AnimatingFABOpacity = false;
                    AnimateFABOpacity();
                }

                if (FabOpened)
                    OpenCloseFabs();
            }
        }

        private async void AnimateFABOpacity()
        {
            await Task.Delay(2000);
            AnimatingFABOpacity = true;
            while (AnimatingFABOpacity)
            {
                await Task.Delay(20);

                if (Fabs["FabOpenOptions"].Opacity <= 0.5)
                    AnimatingFABOpacity = false;
                else
                    Fabs["FabOpenOptions"].Opacity -= 0.001;
            }
        }

        private async void LoadComplaintAutoSaveData()
        {
            while (ComplaintModel.RefToAllComplaints == null)
                await Task.Delay(200);

            FirstTimeLoginView.IsVisible = !ComplaintModel.RefToAllComplaints.user.complaints.Any();
            ListOfComplaintsView.IsVisible = !FirstTimeLoginView.IsVisible;

            object objuser;
            ComplaintModel.DraftComplaintModel WriteNewComplaintModel = null;
            if (Application.Current.Properties.TryGetValue("WriteComplaintAutoSave", out objuser))
                WriteNewComplaintModel = JsonConvert.DeserializeObject<ComplaintModel.DraftComplaintModel>((string)objuser);

            if (objuser != null)
            {
                if (WriteNewComplaintModel.QuickComplaint)
                {
                    var QuickComplaintPage = new QuickComplaintPage(null, WriteNewComplaintModel);
                    QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView.LoadComplaints(); });
                    await Navigation.PushPopupAsync(QuickComplaintPage);
                }
                else
                {   if (WriteNewComplaintModel.complaint_id > 0)
                    {
                        var NewComplaintReplyPage =
                            new NewComplaintReplyPage(ComplaintModel.RefToAllComplaints.user.complaints.Single(c => c.id == WriteNewComplaintModel.complaint_id),
                                                      WriteNewComplaintModel);

                        await Navigation.PushAsync(NewComplaintReplyPage, true);
                        NewComplaintReplyPage.ReplaySentEvent += (int id) => { Navigation.PopAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                    else
                    {
                        var NewComplaintPage = new NewComplaintPage(null, WriteNewComplaintModel.MessageType, WriteNewComplaintModel);
                        NewComplaintPage.ToolbarItems.Add(new ToolbarItem("tbiSendComplaint", "awsomeSend2.png", (() => { NewComplaintPage.SendComplaint(); }), ToolbarItemOrder.Primary, 10));
                        await Navigation.PushAsync(NewComplaintPage);
                        NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                }
            }
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            FabButton_ClickAsync(view, null);
        }

        private void SetFABS()
        {
            var StartWithFab = AppGlobal.AppIsComplio ? 2 : 0;
            for (int i = StartWithFab; i < FabImages.Count; i++)
                SetFAB(i);

            foreach (var FAB in Fabs.Values)
            {
                lytRelative.Children.Add(
                    FAB,
                    xConstraint: Constraint.RelativeToParent((parent) => { return (parent.Width - FAB.Width) - 16; }),
                    yConstraint: Constraint.RelativeToParent((parent) => { return (parent.Height - FAB.Height) - 16; }));
            }

            TAPController = new TAPController(Fabs.Values.ToArray());
            TAPController.SingleTaped += TAPController_SingleTaped;
            ListOfComplaintsView_ListScrolledEvent();
        }

        private void SetFAB(int i)
        {
            var FAB = new Image();

            FAB.AutomationId = FabImages.Keys.ToList()[i];

            FAB.Source = FabImages.Values.ToList()[i][0];

            FAB.HeightRequest = this.Width / 10+16;
            FAB.WidthRequest = this.Width / 10+16;

            if (FAB.AutomationId != "FabOpenOptions")
                FAB.Opacity = 0;

            Fabs.Add(FabImages.Keys.ToList()[i], FAB);
        }

        private async void FabButton_ClickAsync(object sender, EventArgs e)
        {
            Fabs["FabOpenOptions"].Opacity = 1;
            var ClickedButton = ((View)sender);

            switch (ClickedButton.AutomationId)
            {
                case "FabOpenOptions":
                    if (AppGlobal.AppIsComplio)
                       OpenCloseFabs();
                    else
                        await Navigation.PushPopupAsync(new CompanySearchPage());
                    break;

                case "FabWriteMessage":
                    await Navigation.PushPopupAsync(new CompanySearchPage());
                    break;

                case "FabQR":
                    await Navigation.PushAsync(QRScannerController = new Controllers.QRScannerController(), true);
                    QRScannerController.ScanCompletedEvent += QRScannerController__ScanCompletedEvent;
                    await Task.Delay(300);
                    QRScannerController.StartScan();
                    break;
            }
        }

        bool FabOpened = false;
        private async void OpenCloseFabs()
        {
            var FabsToAnimate = Fabs.Where(fab => fab.Value.IsVisible & fab.Key != "FabOpenOptions").ToList();

            if (!FabOpened)
            {
                Fabs["FabOpenOptions"].RotateTo(-45);
                Fabs["FabOpenOptions"].Opacity = 1 ;
                for (int i = 0; i < FabsToAnimate.Count; i++)
                {
                    FabsToAnimate[i].Value.Opacity = 1;
                    await FabsToAnimate[i].Value.TranslateTo(0, -60 * (i + 1), 70);
                }
            }
            else
            {
                Fabs["FabOpenOptions"].RotateTo(0);
              //  Fabs["FabOpenOptions"].Opacity = 0.5;

                for (int i = 0; i < FabsToAnimate.Count; i++)
                {
                    await FabsToAnimate[i].Value.TranslateTo(0, 0, 70);
                        FabsToAnimate[i].Value.Opacity = 0;
                }
            }

            FabOpened = !FabOpened;
        }

        protected override void OnAppearing()
        {
            ((NavigationPage)App.Current.MainPage).BackgroundColor = Color.White;
            base.OnAppearing();
        }

        private void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync(true);
                    if (result.Contains("prigovor.hr") | result.Contains("complio.me"))
                    {
                        //    result = result.Substring(0, result.LastIndexOf("/"));
                        result = result.Remove(0, result.LastIndexOf("/") + 1);
                        var Result = await DataExchangeServices.GetDirectTagResult(result);

                        if (Result.Contains("Error:"))
                        {
                            Controllers.VibrationController.Vibrate();
                            Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                            return;
                        }

                        var CompanyElement = JsonConvert.DeserializeObject<CompanyElementRootModel>(Result);

                        if (CompanyElement != null)
                            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new Company_ElementInfoPage(CompanyElement, true), true));
                        else Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");
                    }
                    else Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");
                });
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "private async void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)");
            }
        }
    }
}
