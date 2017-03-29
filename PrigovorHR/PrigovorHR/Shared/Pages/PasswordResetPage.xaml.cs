using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class PasswordResetPage : ContentPage
    {
        public PasswordResetPage()
        {
            InitializeComponent();

            btnSendResetRequest.Clicked += BtnSendResetRequest_Clicked;
            NavigationBar.BackButtonPressedEvent += (async () => { await Navigation.PopModalAsync(); });
        }

        private async void BtnSendResetRequest_Clicked(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Šaljem zahtjev");
            await Task.Delay(20);
            var Result = await DataExchangeServices.ResetPassword(JsonConvert.SerializeObject(new { email = entryEMail.Text }));

            try
            {


                if (!Result.Contains("Error"))
                {
                    JObject Jobj = JObject.Parse(Result);
                    if (((string)Jobj["status"]) == "success")
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Zahtjev za resetiranjem lozinke uspješno poslan!", "Uspjeh", "OK");
                        OnBackButtonPressed();
                    }
                    else
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Zahtjev neuspješan, provjerite jeste li upisali točan e-mail!", "Greška", "OK");
                }
                else
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja zahtjeva!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Greška", "OK");
            }
            catch(Exception ex )
            {
                Controllers.ExceptionController.HandleException(ex, "private async void BtnSendResetRequest_Clicked(object sender, EventArgs e)");
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom slanja zahtjeva!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Greška", "OK");
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => 
            {
                await NavigationBar.imgBack.RotateTo(90, 75);
                await Navigation.PopModalAsync();
            });
            return true;
        }
    }
}
