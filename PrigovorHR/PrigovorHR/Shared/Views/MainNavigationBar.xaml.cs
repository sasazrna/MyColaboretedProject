
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
        private Controllers.TAPController TAPController;
        public static Controllers.QRScannerController QRScannerController = new Controllers.QRScannerController();
        public delegate void OpenCloseMenuDelegate(bool Open);
        public event OpenCloseMenuDelegate OpenCloseMenuEvent;
        private bool IsMenuOpen = false;

        private string _Title { get; set; }

        public static MainNavigationBar ReferenceToView;

        public bool HasUnreadedReplys { set { imgComplaints.Text = !value ? Views.FontAwesomeLabel.Images.FAPEnvelopeOpen : Views.FontAwesomeLabel.Images.FAPEnvelopeClosed; 
                                              imgComplaints.TextColor = !value ? Color.Gray : Color.FromHex("#FF6A00");}}

        public MainNavigationBar()
        {
            InitializeComponent();

            View[] _ListOfChildViews = new View[] { imgMenuLayout,
                imgComplaints, imgSearch, imgQRCode};

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
                OpenCloseMenuEvent?.Invoke(IsMenuOpen  = !IsMenuOpen);
            else if (view == imgSearch)
                await Navigation.PushPopupAsync(new CompanySearchPage());
            else if (view == imgQRCode)
            {
                await Navigation.PushModalAsync(QRScannerController, true);
                QRScannerController.StartScan();
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
