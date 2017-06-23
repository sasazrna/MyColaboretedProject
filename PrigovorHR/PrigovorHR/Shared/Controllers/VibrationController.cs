using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Controllers
{
    class VibrationController
    {
        public static void Vibrate()
        {
            Plugin.Vibrate.CrossVibrate.Current.Vibration(300);
        }
    }
}
