using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using Xamarin.Forms;
namespace PrigovorHR.Shared.Controllers
{
    public class QRScannerController : ZXingScannerPage
    {
        //   private ZXingScannerPage ZSP;
        public delegate void ScanCompleteHandler(string result, bool isQRFormat);
        public static event ScanCompleteHandler _ScanCompletedEvent;
        public QRScannerController()
        {
      
            //new ZXingScannerPage(new MobileBarcodeScanningOptions()
            //{
            //    AutoRotate = true,
            //    PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE },
            //    TryHarder = false,
            //    UseFrontCameraIfAvailable = false,
            //    UseNativeScanning = false
            //});
            this.DefaultOverlayShowFlashButton = false;
            this.DefaultOverlayTopText = "Skeniraj QR kod";

            OnScanResult += QRScannerController_OnScanResult;
        }

        private void QRScannerController_OnScanResult(ZXing.Result result)
        {
            //await Navigation.PopModalAsync(true);
            IsScanning = false;
            IsAnalyzing = false;
            PauseAnalysis();
            _ScanCompletedEvent?.Invoke(result.Text, (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE | result.BarcodeFormat == ZXing.BarcodeFormat.DATA_MATRIX));
        }

        public void StartScan()
        {
            IsScanning = true;
            IsTorchOn = true;
            IsAnalyzing = true;
            IsVisible = true;
        }

        public async void StopScan()
        {
            IsScanning = false;
            IsAnalyzing = false;
            await Navigation.PopModalAsync(true);
            PauseAnalysis();
        }
    }
}
