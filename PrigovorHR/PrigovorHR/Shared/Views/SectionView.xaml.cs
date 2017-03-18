using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class SectionView : ContentView
    {
        public SectionView()
        {
            InitializeComponent();
            SectionDetails1.Text = Views.FontAwesomeLabel.Images.FAMinusSquareO;
            SectionDetails2.Text = Views.FontAwesomeLabel.Images.FAMinusSquareO;
            SectionDetails3.Text = Views.FontAwesomeLabel.Images.FAMinusSquareO;
        }
    }
}
