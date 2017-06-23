using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    class SoundRecorder
    {
        public static void RecordSound(string nameofelement)
        {
            DependencyService.Get<IAndroidCallers>().RecordSound(nameofelement);//file will be called by element name, but we need some kind of procedure for giving and storing file names by some rules.
        }

        public static void StopRecordingSound()
        {
            DependencyService.Get<IAndroidCallers>().StopRecordingSound();
        }

        public static void PlayRecordedSound()
        {
            DependencyService.Get<IAndroidCallers>().PlayRecordedSound();
        }
    }
}
