using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Models
{
    class ContactUsModel
    {

        public ContactUsModel(string _name, string _phone, string _email, int? _user, string _title, string _message)
        {
            name = _name; phone = _phone; email = _email; user = _user; title = _title; message = _message; type = 0;
        } 

        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int? user { get; set; }
        public string title { get; set; }
        public string message { get; set; }

        public int type { get; set; }
    }
}
