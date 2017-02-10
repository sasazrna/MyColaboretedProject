using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;


namespace PrigovorHR.Shared.Models
{
    public class User
    {
        public User()
        {

        }
                        
        public string token { get; set; }

        public bool isLogged { get; set; }

        public bool isNotificationEnabled { get; set; }

        public int? id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        public string name_surname { get { return name + " " + surname; } }
        public string email { get; set; }
        public string telephone { get; set; }
        public string profileimage { get; set; }
        public string password { get; set; }
        public UserType.eUserType usertype { get; set; }
        public LoginTypeModel.eLoginType LoginType { get; set; }
        public List<ComplaintModel> complaints { get; set; }
        public IList<ElementReviewModel> element_reviews { get; set; }
        public IList<ComplaintModel> unread_complaints { get; set; }
    }

    public class UserToken
    {
        public static string token
        {
            get { return Controllers.LoginRegisterController._LoggedUser?.token.Trim('"'); }
            set{
                Controllers.LoginRegisterController._LoggedUser.token = value.Replace("Bearer ", string.Empty).Trim('"');}
        }
    }
}
