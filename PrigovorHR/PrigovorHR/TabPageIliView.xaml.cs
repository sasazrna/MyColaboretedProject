using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR
{
    public partial class TabPageIliView : ContentPage
    {
        public TabPageIliView()
        {
            InitializeComponent();
            ZatPrigUnder.IsVisible = false;
            ZatPrigStack.IsVisible = false;


            //Paljenje taba aktivnih prigovora, gašenje taba zatvorenih prigovora

            var AktPrigGestureRecognizer = new TapGestureRecognizer();
            AktPrigGestureRecognizer.Tapped += (s, e) =>
            {
                AktPrigovorStack.IsVisible = true;
                AktPrigUnder.IsVisible = true;
                ZatPrigUnder.IsVisible = false;
                ZatPrigStack.IsVisible = false;
            };
            AktPrigLabel.GestureRecognizers.Add(AktPrigGestureRecognizer);


            //Paljenje taba zatvorenih prigovora, gašenje taba aktivnih prigovora

            var ZatPrigGestureRecognizer = new TapGestureRecognizer();
            ZatPrigGestureRecognizer.Tapped += (s, e) =>
            {
                AktPrigovorStack.IsVisible = false;
                AktPrigUnder.IsVisible = false;
                ZatPrigStack.IsVisible = true;
                ZatPrigUnder.IsVisible = true;
            };
            ZatPrigLabel.GestureRecognizers.Add(ZatPrigGestureRecognizer);
        }
    }
}