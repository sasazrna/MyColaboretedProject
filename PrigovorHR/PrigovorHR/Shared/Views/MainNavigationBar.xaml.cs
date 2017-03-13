
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
        private Dictionary<View, bool> LastViewed = new Dictionary<View, bool>();
        private bool TrackLastViewed = true;
        private Controllers.TAPController TAPController;

        public static Controllers.QRScannerController QRScannerController = new Controllers.QRScannerController();

        private string _Title { get; set; }

        public delegate void GPSActivationRequestedHandler();
        public event GPSActivationRequestedHandler _GPSActivationRequestedEvent;

        public delegate void ShowComplaintsHandler();
        public event ShowComplaintsHandler _ShowComplaintsEvent;

        private enum eGPSOptionStatus : int { unvailable = 0, available = 1, activated = 2 };
        private eGPSOptionStatus _CurrentGPSStatus = eGPSOptionStatus.unvailable;

        public static MainNavigationBar ReferenceToView;

        public bool HasUnreadedReplys { set { imgComplaints.TextColor = !value ? Color.Gray : Color.FromHex("#FF6A00"); } }

        public MainNavigationBar()
        {
            InitializeComponent();

            View[] _ListOfChildViews = new View[] { imgMenuLayout,
                imgComplaints, imgSearch, imgQRCode, lblAboutUs, lblContactUs, lblLogOut, lblMyProfile, lblMyComplaints };

            imgComplaints.Text = Views.FontAwesomeLabel.Images.FAEnvelope;
            imgComplaints.TextColor = Color.Gray;


            imgQRCode.Text = Views.FontAwesomeLabel.Images.FAQrcode;
            imgQRCode.TextColor = Color.Black;

            imgMenu.Text = Views.FontAwesomeLabel.Images.FABars;
            imgMenu.TextColor = Color.Black;

            imgSearch.Text = Views.FontAwesomeLabel.Images.FASearch;
            imgSearch.TextColor = Color.Black;

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

                var Result = await DataExchangeServices.GetSearchResults(result);

                if (Result == "Error:")
                {
                    Controllers.VibrationController.Vibrate();
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                    return;
                }
     
            var CompanyElement = JsonConvert.DeserializeObject<List<Models.CompanyElementModel>>(Result);
            if (CompanyElement.Count == 1)
                await Navigation.PushAsync(new NewComplaint(), true);
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
                _MenuFrame.IsVisible = !_MenuFrame.IsVisible;
                if (!LastViewed.ContainsKey(imgMenuLayout) & TrackLastViewed)
                    LastViewed.Add(imgMenuLayout, false);
            }
            else if (view == imgSearch)
            {
                await Navigation.PushPopupAsync(new CompanySearchPage());
            }
            else if (view == imgQRCode)
            {
               await Navigation.PushModalAsync(QRScannerController, true);
                QRScannerController.StartScan();
            }
            if (view == lblAboutUs)
            {

            }
            else if (view == lblContactUs)
            {
                await Navigation.PushModalAsync(new ContactUsPage());
            }
            else if (view == lblMyProfile)
            {
                await Navigation.PushModalAsync(new ProfilePage(), true);
            }
            else if (view == lblMyComplaints | view == imgComplaints)
            {
                _ShowComplaintsEvent?.Invoke();
                _MenuFrame.IsVisible = false;
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

        public  bool BackButtonPressedEvent()
        {
            if (LastViewed.Count > 0)
            {
                TrackLastViewed = false;
               _TAPController_SingleTaped(string.Empty, LastViewed.Last().Key);

                if (LastViewed.Count > 1)
                   _TAPController_SingleTaped(string.Empty, LastViewed.First().Key);

                LastViewed.Remove(LastViewed.Last().Key);
                TrackLastViewed = true;
                return true;
            }
            else
            {
                TrackLastViewed = true;
                return false;
            }
        }
    }
}
