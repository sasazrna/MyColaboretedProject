using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Views;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class NewComplaintResponse : ContentPage
    {
        public NewComplaintResponse()
        {
            InitializeComponent();

            attach_pdf_font.Text = '\uf1c1'.ToString();
            attach_pdf_font.TextColor = Color.Gray;

            attach_photo_font.Text = '\uf030'.ToString();
            attach_photo_font.TextColor = Color.Gray;

            attach_voice_font.Text = '\uf041'.ToString();
            attach_voice_font.TextColor = Color.Gray;

            dodani_pdf.Text = '\uf1c1'.ToString();
            dodani_pdf.TextColor = Color.Gray;

            send_font.Text = '\uf2c6'.ToString();
            send_font.TextColor = Color.FromHex("#FF7e65");
            send_font.FontSize = 55;


        }
    }
}
