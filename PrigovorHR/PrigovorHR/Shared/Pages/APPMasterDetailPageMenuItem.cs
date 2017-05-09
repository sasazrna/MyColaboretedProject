using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public class APPMasterDetailPageMenuItem
    {
        public APPMasterDetailPageMenuItem()
        {
          //  TargetType = typeof(APPMasterDetailPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }

       public FileImageSource Icon { get; set; }

    }

    
}
