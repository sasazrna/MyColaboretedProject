using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Controllers
{
    class CameraController
    {
        public static async Task<MediaFile> TakePhoto()
        {
            try
            {
              await  CrossMedia.Current.Initialize();
               return await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });
            }catch(Exception ex) { Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString()); }
            return null;
        }
    }
}
