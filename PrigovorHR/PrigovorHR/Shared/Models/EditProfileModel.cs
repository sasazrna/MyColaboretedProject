using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Models
{
    class EditProfileModel
    {
        private string NewPassword;
        private User NewUserData;

        public EditProfileModel(User newUserData)
        {
            NewUserData = newUserData;
            NewPassword = NewUserData.password != Controllers.LoginRegisterController.LoggedUser.password ? NewUserData.password : string.Empty;
        }

        public string name { get { return NewUserData.name; } }
        public string surname { get { return NewUserData.surname; } }
        public string email { get { return Controllers.LoginRegisterController.LoggedUser.email; } }
        public string oldPassword { get { return Controllers.LoginRegisterController.LoggedUser.password; } }
        public string phone { get { return NewUserData.telephone; } }
        //public string profile_image_input { get { return NewUserData.profileimage; } }
        public string new_password { get { return NewPassword; } }
    }
}
