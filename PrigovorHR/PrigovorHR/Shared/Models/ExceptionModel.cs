using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    class ExceptionModel
    {
        public string customermessage { get; set; }
        public string developermessage { get; set; }
        public string exceptiontime { get; set; }
        private string internetwasavailable
        {
            get { return Controllers.NetworkController.IsInternetAvailable.ToString(); }
        }
      //  private 
    }
}
