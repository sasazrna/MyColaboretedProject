using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Models
{
    public class UserType
    {
        public enum eUserType:int
        {
            Basic=1, ContactPerson=2,AdminPerson=3
        };
    }
}
