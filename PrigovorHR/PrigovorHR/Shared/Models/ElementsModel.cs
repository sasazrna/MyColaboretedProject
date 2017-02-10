using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    class ElementsModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string searchtags { get; set; }
        public string directtag { get; set; }
        public bool isLocation { get; set; }
        public string location { get; set; }//ne može biti string, provjeri što jest.
    }
}
