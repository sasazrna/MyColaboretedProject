using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserAgreementPage : ContentPage
    {
        public UserAgreementPage()
        {
            InitializeComponent();

            webView.Source = new UrlWebViewSource()
            {
                Url = "http://138.68.85.217/hr/uvjeti-koristenja-fizicke-osobe"
            };
        
            //NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
        }

        protected override bool OnBackButtonPressed()
        {
            return OnBackButtonPressed();
        }
    }
}
