using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class FirstTimeLoginView : ContentView
    {
        private Controllers.TAPController TAPController;
        public delegate void SearchIconClickedHandler();
        public event SearchIconClickedHandler SearchIconClickedEvent;

        public FirstTimeLoginView()
        {
            InitializeComponent();
            lblSearch.Text = Views.FontAwesomeLabel.Images.FASearch;
            lblSearch.TextColor = Color.Gray;
            lblSearch.FontSize = 25;

            lblPen.Text = Views.FontAwesomeLabel.Images.FAPencil;
            lblPen.TextColor = Color.Gray;
            lblPen.FontSize = 25;

            lblInfo.Text = Views.FontAwesomeLabel.Images.FAInfo;
            lblInfo.TextColor = Color.Gray;
            lblInfo.FontSize = 25;

            lblHandShake.Text = Views.FontAwesomeLabel.Images.FAHandShake;
            lblHandShake.TextColor = Color.Gray;
            lblHandShake.FontSize = 25;

            TAPController = new Controllers.TAPController(lblSearch);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private  void TAPController_SingleTaped(string viewId, View view)
        {
            SearchIconClickedEvent?.Invoke();
          //  await ((Page)Parent).Navigation.PushPopupAsync(new Pages.CompanySearchPage());
        }
    }
}
