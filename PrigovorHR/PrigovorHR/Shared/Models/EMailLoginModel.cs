using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    class EMailLoginModel
    {
        public string email { get; set; }//: 'bruno@mailinator.com',
        public string password { get; set; } // '123123'

        public static DeviceInfoModel deviceinfo { get; set; }
    }
}
