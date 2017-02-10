using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    public class LoginTypeModel
    {
       public enum eLoginType : int { email = 1, facebook = 2, google = 3, linkedin = 4 }
    }
}
