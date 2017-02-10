using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Controllers
{
    public interface IAndroidCallers
    {
         void OpenGPSSettings();

        void RecordSound(string nameofelement);
        void StopRecordingSound();
        void PlayRecordedSound();
        void OpenNetworkSettings();
        void CloseApp();
        void SaveFile(string FileName, byte[] FileData);
        void OpenFile(string FileName);
        ImageSource ConvertUrlToImage(string url);
    }

    public interface IAndroidFacebookCallers
    {
        ContentPage Login();
    }
}
