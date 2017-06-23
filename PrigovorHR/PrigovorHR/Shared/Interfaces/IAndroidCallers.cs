using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
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

        void DeleteFile(string FileName);

        int GetSDKVersion();

        void UpdateComplaintsListFromPortableToNative(string JSON, string UserToken);
    }

    public interface IAndroidFacebookCallers
    {
        ContentPage Login();
    }
}
