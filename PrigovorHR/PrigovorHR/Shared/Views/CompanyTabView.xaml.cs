using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class CompanyTabView : ContentView
    {
        public CompanyTabView()
        {
            InitializeComponent();

            //lblStore.Text = Views.FontAwesomeLabel.Images.FACube;
            //lblStore.TextColor = Color.FromHex("FF7e65");

            //lblCompany.Text = Views.FontAwesomeLabel.Images.FACubes;
            //lblCompany.TextColor = Color.FromHex("FF7e65");

            //lblOtherStores.Text = Views.FontAwesomeLabel.Images.FAList;
            //lblOtherStores.TextColor = Color.FromHex("FF7e65");



            StoreUnder.IsVisible = true;
            CompanyUnder.IsVisible = false;
            OtherStoresUnder.IsVisible = false;


            //Paljenje taba aktivnih prigovora, gašenje taba zatvorenih prigovora

            var StoreGestureRecognizer = new TapGestureRecognizer();
           StoreGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = true;
                CompanyUnder.IsVisible = false;
                OtherStoresUnder.IsVisible = false;
            };
            lblStore.GestureRecognizers.Add(StoreGestureRecognizer);


            //Paljenje taba zatvorenih prigovora, gašenje taba aktivnih prigovora

            var CompanyGestureRecognizer = new TapGestureRecognizer();
           CompanyGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = false;
                CompanyUnder.IsVisible = true;
                OtherStoresUnder.IsVisible = false;
            };
            lblCompany.GestureRecognizers.Add(CompanyGestureRecognizer);

            var OtherStoresGestureRecognizer = new TapGestureRecognizer();
           OtherStoresGestureRecognizer.Tapped += (s, e) =>
            {
                StoreUnder.IsVisible = false;
                CompanyUnder.IsVisible = false;
                OtherStoresUnder.IsVisible = true;
            };
            lblOtherStores.GestureRecognizers.Add(OtherStoresGestureRecognizer);
        }
    }
}
