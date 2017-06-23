using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    public static class TranslateController
    {
        public static void Translate(params dynamic[] Views)
        {
            foreach (var Label in Views)
                Label.Text = "RESETIRAM";
        }
    }
}
