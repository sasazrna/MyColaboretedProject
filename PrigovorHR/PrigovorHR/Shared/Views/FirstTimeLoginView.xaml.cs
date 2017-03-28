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

            
        }
    }
}
