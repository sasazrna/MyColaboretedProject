using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CloseComplaintPage
    {
        private Models.ComplaintModel Complaint;
        public delegate void ComplaintClosedHandler(int id);
        public event ComplaintClosedHandler ComplaintClosed;

        public CloseComplaintPage(Models.ComplaintModel complaint)
        {
            InitializeComponent();
            Complaint = complaint;
            btnZatvoriPrigovor.Clicked += BtnZatvoriPrigovor_Clicked;
            lytLastMessage.IsVisible = !complaint.closed;
            btnZatvoriPrigovor.Text = !complaint.closed ? "Zatvori prigovor" : "Ocijeni";
        }

        private void BtnZatvoriPrigovor_Clicked(object sender, EventArgs e)
        {
            if (ComplaintEvaluationView.StoredEvaluationGrades.Count == 3)
                if (!Complaint.closed)
                    CloseAndEvaluateComplaint();
                else EvaluateComplaint();
            else
                Acr.UserDialogs.UserDialogs.Instance.Alert("Molimo vas, prije zatvaranja prigovora ocijenite rješenje vašeg prigovora!", "Prigovor.hr", "OK");
        }

        private async void CloseAndEvaluateComplaint()
        {
            var CloseComplaintData = new
            {
                satisfaction = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[0],
                speed = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[1],
                communication_level_user = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[2],
                message = editorLastMessage.Text,
                rate_element = 1,
                complaint_id = Complaint.id
            };

            if (await DataExchangeServices.CloseComplaint(JsonConvert.SerializeObject(CloseComplaintData)))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowSuccess( "Vaš prigovor je uspješno zatvoren!", 3500);
                await Task.Delay(3500);
                await Navigation.PopAsync(true);
                ComplaintClosed?.Invoke(Complaint.id);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom zatvaranja prigovora!" + System.Environment.NewLine + "Pokušajte ponovno", "Greška", "OK");
            }
        }

        private async void EvaluateComplaint()
        {
            var RateComplaintData = new
            {
                satisfaction = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[0],
                speed = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[1],
                communication_level_user = ComplaintEvaluationView.StoredEvaluationGrades.Values.ToList()[2],
                rate_element = 1,
                complaint_id = Complaint.id
            };

            if (await DataExchangeServices.EvaluateComplaint(JsonConvert.SerializeObject(RateComplaintData)))
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowSuccess("Uspješno ste ocijenili rješenje prigovora", 3500);
                await Task.Delay(3500);
                await Navigation.PopAsync(true);
                ComplaintClosed?.Invoke(Complaint.id);
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom ocjenjivanja prigovora!" + System.Environment.NewLine + "Pokušajte ponovno", "Greška", "OK");
            }
        }
    }
}
