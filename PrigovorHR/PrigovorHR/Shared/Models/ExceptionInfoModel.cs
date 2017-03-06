using PrigovorHR.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    public static class ExceptionInfoModel
    {
        public class ExceptionInfo
        {
            public string user_id { get { return LoginRegisterController.LoggedUser?.id?.ToString(); } }

            //exception info
            public string exception_date { get { return DateTime.Now.ToString(); } }
            public string exception { get; set; }
            public string developer_message { get; set; }
            public bool is_network_available { get { return NetworkController.IsInternetAvailable; } }

            //device info
            public string x_id { get { return  DeviceInfoModel.Id; } }
            public string model { get { return DeviceInfoModel.Model; } }
            public string platform { get { return DeviceInfoModel.Platform; } }
            public string version { get { return DeviceInfoModel.Id; } }

        }

    }
}
