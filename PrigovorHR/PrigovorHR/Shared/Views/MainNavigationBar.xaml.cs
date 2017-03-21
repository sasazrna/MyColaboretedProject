
using Newtonsoft.Json;
using PrigovorHR.Shared.Models;
using PrigovorHR.Shared.Pages;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavigationBar : ContentView
    {
        private bool TrackLastViewed = true;
        private Controllers.TAPController TAPController;

        public static Controllers.QRScannerController QRScannerController = new Controllers.QRScannerController();

        private string _Title { get; set; }

        public static MainNavigationBar ReferenceToView;

        public bool HasUnreadedReplys { set { imgComplaints.Text = !value ? Views.FontAwesomeLabel.Images.FAPEnvelopeOpen : Views.FontAwesomeLabel.Images.FAPEnvelopeClosed; 
                                              imgComplaints.TextColor = !value ? Color.Gray : Color.FromHex("#FF6A00");}}

        public void HideMenuFrame() => _MenuFrame.IsVisible = false;

        public MainNavigationBar()
        {
            InitializeComponent();

            View[] _ListOfChildViews = new View[] { imgMenuLayout,
                imgComplaints, imgSearch, imgQRCode, lblContactUs, lblLogOut, lblMyProfile };

            imgComplaints.Text = Views.FontAwesomeLabel.Images.FAEnvelope;
            imgComplaints.TextColor = Color.Gray;

            imgQRCode.Text = Views.FontAwesomeLabel.Images.FAQrcode;
            imgQRCode.TextColor = Color.Gray;

            imgMenu.Text = Views.FontAwesomeLabel.Images.FABars;
            imgMenu.TextColor = Color.Gray;

            imgSearch.Text = Views.FontAwesomeLabel.Images.FASearch;
            imgSearch.TextColor = Color.Gray;

            TAPController = new Controllers.TAPController( _ListOfChildViews);
            TAPController.SingleTaped += _TAPController_SingleTaped;
            Controllers.QRScannerController.ScanCompletedEvent += QRScannerController__ScanCompletedEvent;
            ReferenceToView = this;

            //_CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));

            //if (_CurrentGPSStatus != eGPSOptionStatus.unvailable)
            //{
            //    if (_CurrentGPSStatus == eGPSOptionStatus.activated)
            //        _CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));
            //    else
            //        _CurrentGPSStatus = eGPSOptionStatus.activated;

            //    btn.Image = Enum.GetName(typeof(eGPSOptionStatus), (int)_CurrentGPSStatus) + ".png";
            //}
            //else
            //    Acr.UserDialogs.UserDialogs.Instance.Confirm(new Acr.UserDialogs.ConfirmConfig()
            //    {
            //        OkText = "DA",
            //        CancelText = "NE",
            //        //  IsCancellable = true,
            //        Message = "GPS nije uključen ili dostupan!" + Environment.NewLine + "Želite li otvoriti postavke i aktivirati GPS?",
            //        Title = "PrigovorHR"
            //    }.SetAction((re) => { if (re) { Controllers.GPSController.OpenGPSOptions(); } }));

            // Device.StartTimer(new TimeSpan(0, 0, 1), (() => { UpdateChildrenLayout(); return true; }));

            //_SearchOptionsLayout.SizeChanged += _SearchOptionsLayout_SizeChanged;
        }

        private async void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)
        {
            try
            {
                await Navigation.PopModalAsync(true);
                result = result.Substring(0, result.LastIndexOf("/"));
                result = result.Remove(0, result.LastIndexOf("/") + 1);
                var Result = await DataExchangeServices.GetCompanyElementData(result);

                if (Result.Contains("Error:"))
                {
                    Controllers.VibrationController.Vibrate();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                    return;
                }

                var CompanyElement = JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(Result);
                if (CompanyElement != null)
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushModalAsync(new Pages.Company_ElementInfoPage(CompanyElement, true), true);
                    });
                else Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString());
            }
        }

        private  async void _TAPController_SingleTaped(string viewId, View view)
        {
            if (view == imgMenuLayout)
            {
             //   LandingViewWithLogin.ReferenceToView.ShowMenu();
                _MenuFrame.IsVisible = !_MenuFrame.IsVisible;
            }
            else if (view == imgSearch)
            {
                _MenuFrame.IsVisible = false;
                await Navigation.PushPopupAsync(new CompanySearchPage());
            }
            else if (view == imgQRCode)
            {
                _MenuFrame.IsVisible = false;
                await Navigation.PushModalAsync(QRScannerController, true);
                QRScannerController.StartScan();
            }
            //if (view == lblAboutUs)
            //{

            //}
            else if (view == lblContactUs)
            {
                _MenuFrame.IsVisible = false;
                await Navigation.PushModalAsync(new ContactUsPage());
            }
            else if (view == lblMyProfile)
            {
                _MenuFrame.IsVisible = false;
                await Navigation.PushModalAsync(new ProfilePage(), true);
            }
            else if (view == lblLogOut)
            {

                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                    new Acr.UserDialogs.ConfirmConfig()
                    {
                        Title = "Odjava",
                        CancelText = "Odustani",
                        OkText = "Odjavi me",
                        Message = "Jeste li sigurni u odjavu iz aplikacije?",
                        OnAction = (Confirm) =>
                        {
                            if (Confirm)
                                Controllers.LoginRegisterController.UserLogOut();
                        }
                    });
            }
        }

        public void ChangeNavigationTitle(string newtitle)
        {
            _lblNavigationTitle.Text = newtitle;
            if (newtitle == "Prigovor.hr")
                _imgLogo.IsVisible = true;
            else _imgLogo.IsVisible = false;
        }
    }
}
