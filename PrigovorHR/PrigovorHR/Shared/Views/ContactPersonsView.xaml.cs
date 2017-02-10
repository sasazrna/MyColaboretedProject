using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ContactPersonsView : ContentView
    {
        public static List<ContactPersonsModel> ListOfContactPersons = new List<ContactPersonsModel>();
        public ContactPersonsView(string _Name, string _Surname, string _EMail, string _Telephone)
        {
            InitializeComponent();
            lblName.Text = _Name + _Surname;
            lblEMail.Text = _EMail;
            lblTelephone.Text = _Telephone;

            ListOfContactPersons.Add(new ContactPersonsModel { EMail = _EMail, Name = _Name, Surname = _Surname, Telephone = _Telephone });           
        }

        public static void EmptyListOfContactPersons()
        {
            ListOfContactPersons.Clear();
        }

        public class ContactPersonsModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string EMail { get; set; }
            public string Telephone { get; set; }
        }
    }
}
