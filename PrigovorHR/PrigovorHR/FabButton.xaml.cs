using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR
{
    public partial class FabButton : ContentPage
    {
        public FabButton()
        {
            InitializeComponent();


            //fab2Btn.TranslateTo(0, 0, 100);
            //fab2Btn.FadeTo(0, 100);
            //fab2Btn.RotateTo(360, 0, Easing.SinOut);


            //fab1Btn.TranslateTo(0, 0, 100);
            //fab1Btn.FadeTo(0, 100);
            //fab1Btn.RotateTo(360, 100, Easing.SinOut);


            fab2Btn.TranslateTo(0, 0, 100);
            fab2Btn.FadeTo(0, 100);
            fab2Btn.RotateTo(360, 0, Easing.SinOut);


            fab1Btn.TranslateTo(0, 0, 100);
            fab1Btn.FadeTo(0, 100);
            fab1Btn.RotateTo(360, 100, Easing.SinOut);


        }


        private int clickedTotal = 1;

        void Handle_FabClicked3(object sender, System.EventArgs e)
        {
            clickedTotal += 1;
            if (clickedTotal % 2 == 0)
            {




                fab2Btn.TranslateTo(0, -160, 100);
                fab2Btn.FadeTo(1, 100);
                fab2Btn.RotateTo(360, 100);


                fab1Btn.TranslateTo(0, -80, 100);
                fab1Btn.FadeTo(1, 100);
                fab1Btn.RotateTo(360, 100);
            }
            else
            {



                fab2Btn.TranslateTo(0, 0, 100);
                fab2Btn.FadeTo(0, 100);
                fab2Btn.RotateTo(360, 0);


                fab1Btn.TranslateTo(0, 0, 100);
                fab1Btn.FadeTo(0, 100);
                fab1Btn.RotateTo(360, 100);

            }
        }


    }
}