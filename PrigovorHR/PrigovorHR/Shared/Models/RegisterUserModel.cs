using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    class RegisterUserModel
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public const string business_business = "";
        public const string business_oib = "";
        public const string business_address = "";
        public const bool businessRegistration = false;
        public static DeviceInfoModel deviceinfo { get; set; }
    }


//     {
//"kind": "plus#person",
//"etag": "\"xw0en60W6-NurXn4VBU-CMjSPEw/CIcPtylqJ0YaBo53fzBPOPlQJ04\"",
//"gender": "male",
//"emails": [
// {
//  "value": "vedran@samorazvoj.hr",
//  "type": "account"
// }
//],
//"objectType": "person",
//"id": "107334323185165523824",
//"displayName": "Vedran Mikuličić",
//"name": {
// "familyName": "Mikuličić",
// "givenName": "Vedran"
//},
//"url": "https://plus.google.com/107334323185165523824",
//"image": {
// "url": "https://lh3.googleusercontent.com/-3aEgkP4svXE/AAAAAAAAAAI/AAAAAAAAAAA/uPT8-d1ezKE/photo.jpg?sz=50",
// "isDefault": true
//},
//"isPlusUser": true,
//"circledByCount": 1,
//"verified": false,
//"domain": "samorazvoj.hr"
//}
}
