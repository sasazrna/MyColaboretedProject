using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Controllers
{
    public class ExceptionController 
    {
        public ExceptionController(Exception ex, string customermessage, string developermessage)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString());
        }
    }
}
