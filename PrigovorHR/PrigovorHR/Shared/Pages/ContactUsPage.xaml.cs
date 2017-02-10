using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();

            _btnSend.Clicked += _btnSend_Clicked;
        }

        private async void _btnSend_Clicked(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem poruku...");
            var User = Controllers.LoginRegisterController._LoggedUser;
            
            var ContactInfo = new Models.ContactUsModel(User.name_surname, User.telephone, User.email, User.id, _TitleOfMessageEntry.Text, _MessageEntry.Text);

            if (await DataExchangeServices.ContactUs(JsonConvert.SerializeObject(ContactInfo)))
            {
                var MessageSentPage = new MessageSentPage();
                MessageSentPage._PageClosed += (() => { Navigation.PopModalAsync(true); });
                await Navigation.PushModalAsync(MessageSentPage);
            }
            else
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja poruke!" + Environment.NewLine + "Provjerite internet konekciju i pokušajte ponovno", "Greška", "OK");

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }
    }
}
