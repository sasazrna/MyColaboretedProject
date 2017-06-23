using System.Collections.Generic;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Complio.Shared.Controllers
{
    public class QRScannerController : ZXingScannerPage
    {
        //   private ZXingScannerPage ZSP;
        public delegate void ScanCompleteHandler(string result, bool isQRFormat);
        public event ScanCompleteHandler ScanCompletedEvent;
        public QRScannerController()
        {

            new ZXingScannerPage(new MobileBarcodeScanningOptions()
            {
                AutoRotate = true,
                PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.DATA_MATRIX },
                TryHarder = true,
                UseFrontCameraIfAvailable = false,
                UseNativeScanning = true,
            }, new StackLayout()).AutoFocus();

            OnScanResult += QRScannerController_OnScanResult;
        }

        private void QRScannerController_OnScanResult(ZXing.Result result)
        {
           // Navigation.PopAsync(true);
            IsScanning = false;
            IsAnalyzing = false;
            PauseAnalysis();
            ScanCompletedEvent?.Invoke(result.Text, (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE | result.BarcodeFormat == ZXing.BarcodeFormat.DATA_MATRIX));
        }

        public void StartScan()
        {
            IsScanning = true;
            IsTorchOn = true;
            IsAnalyzing = true;
            IsVisible = true;
        }

    
    }
}
