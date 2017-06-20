using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
            btnSend.Clicked += _btnSend_Clicked;
            //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;

           
        }

        private async void _btnSend_Clicked(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem poruku...");
            var User = Controllers.LoginRegisterController.LoggedUser;
            
            var ContactInfo = new Models.ContactUsModel(User.name_surname, User.telephone, User.email, User.id, MessageTitleEntry.Text, MessageEntry.Text);

            if (await DataExchangeServices.ContactUs(JsonConvert.SerializeObject(ContactInfo)))
            {
                var MessageSentPage = new MessageSentPage();
                MessageSentPage.PageClosed += (() => { Navigation.PopModalAsync(true); });
                await Navigation.PushModalAsync(MessageSentPage);
            }
            else
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja poruke!" + Environment.NewLine + "Provjerite internet konekciju i pokušajte ponovno", "Greška", "OK");

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            return OnBackButtonPressed();
        }

        private void MessageEntry_Focused(object sender, FocusEventArgs e)
        {
            FakeStack.IsVisible = false;
            LogoStack.IsVisible = false;
        }

        private void MessageEntry_Unfocused(object sender, FocusEventArgs e)
        {
            FakeStack.IsVisible = true;
            FakeStack.IsVisible = true;
        }

     


    }
}
