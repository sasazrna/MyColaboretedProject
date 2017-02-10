using Plugin.Media.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Controllers
{
    class CameraController
    {
        public static async Task<MediaFile> TakePhoto()
        {
            return await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });
        }
    }
}
