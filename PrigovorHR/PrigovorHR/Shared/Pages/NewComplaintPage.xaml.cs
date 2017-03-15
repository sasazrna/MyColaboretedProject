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
    public partial class NewComplaintPage : ContentPage
    {
        public NewComplaintPage()
        {
            InitializeComponent();

            arrivalDatePicker.IsVisible = false;
            arrivalTimePicke.IsVisible = false;

            attach_pdf_font.Text = Views.FontAwesomeLabel.Images.FAFile_pdf;
            attach_pdf_font.TextColor = Color.Gray;

            attach_photo_font.Text = Views.FontAwesomeLabel.Images.FACamera;
            attach_photo_font.TextColor = Color.Gray;

            attach_voice_font.Text = Views.FontAwesomeLabel.Images.FAMapMarker;
            attach_voice_font.TextColor = Color.Gray;


            dodani_pdf.Text = Views.FontAwesomeLabel.Images.FAFile_pdf;
            dodani_pdf.TextColor = Color.Gray;

            Sada_stack.IsVisible = false;
            Ranije_stack.IsVisible = false;


            //Prikaz vremena sada

            var SadStackTab = new TapGestureRecognizer();
            SadStackTab.Tapped += (s, e) =>
            {
                ZaPopunit_stack.IsVisible = false;
                Ranije_stack.IsVisible = false;
                Sada_stack.IsVisible = true;
                labela_vremena_sad.Text = DateTime.Now.ToString();
            };
            SadaStackButton.GestureRecognizers.Add(SadStackTab);


            //Paljenje dijaloga za namještanje vremena ranije

            var PrijeStackTab = new TapGestureRecognizer();
            PrijeStackTab.Tapped += (s, e) =>
            {
                ZaPopunit_stack.IsVisible = false;
                Sada_stack.IsVisible = false;
                Ranije_stack.IsVisible = true;
            };
            RanijeStackButton.GestureRecognizers.Add(PrijeStackTab);


            // DatePicker selected Event

            arrivalDatePicker.DateSelected += ArrivalDatePicker_DateSelected;


            // Fokusiranje DatePickera

            var DateGestureRecognizer = new TapGestureRecognizer();
            DateGestureRecognizer.Tapped += (s, e) =>
            {
                arrivalDatePicker.Focus();
            };
            RanijeStackButton.GestureRecognizers.Add(DateGestureRecognizer);


            //Ispis TimePickera

            arrivalTimePicke.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
                {
                    labelasati.Text = arrivalTimePicke.Time.ToString();
                    labelasati.TextColor = Color.Silver;
                }
            };
        }


        // Ispis DatePickera

        private void ArrivalDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            labelavremena.Text = e.NewDate.ToString("dd.MM.yyyy.");
            labelavremena.TextColor = Color.Silver;
            arrivalTimePicke.Focus();
        }
    }
    
}
