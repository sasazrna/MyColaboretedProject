using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class MessageSentPage : ContentPage
    {
        public delegate void PageClosedHandler();
        public event PageClosedHandler _PageClosed;
        public MessageSentPage()
        {
            InitializeComponent();

            ClosePage();
        }

        private async void ClosePage()
        {
            await Task.Delay(3500);
            await Navigation.PopModalAsync(true);
            _PageClosed?.Invoke();
        }
    }
}
