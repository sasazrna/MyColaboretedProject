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
    public partial class ComplaintSentPage : ContentPage
    {
        public delegate void PageClosedHandler();
        public event PageClosedHandler _PageClosed;
        public ComplaintSentPage(string Message, bool IsResponse)
        {
            InitializeComponent();
            lblMessage.Text = "";
            lblComplaintSent.Text = IsResponse ? "Vaš odgovor je poslan" : "Vaš prigovor je poslan" + System.Environment.NewLine + System.Environment.NewLine + Message;
            ClosePage();
        }

        private async void ClosePage()
        {
            await Task.Delay(3500);
            await Navigation.PopModalAsync(true);
            _PageClosed?.Invoke();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
