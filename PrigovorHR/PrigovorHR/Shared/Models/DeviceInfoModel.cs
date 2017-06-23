
using Plugin.DeviceInfo;
namespace Complio.Shared.Models
{
   public class DeviceInfoModel
    {
        public static string Id { get { return CrossDeviceInfo.Current.Id; } }
        public static string Model { get { return CrossDeviceInfo.Current.Model; } }
        public static string Platform { get { return CrossDeviceInfo.Current.Platform.ToString(); } }
        public static string Version { get { return CrossDeviceInfo.Current.Version; } }
    }
}
