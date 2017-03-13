using PrigovorHR.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ElementSectionView : ContentView
    {
        public ElementSectionView()
        {
            InitializeComponent();
        
            ViewElement.Text = '\uf196'.ToString();
            ViewElement.TextColor = Color.Gray;

            ViewElement2.Text = '\uf196'.ToString();
            ViewElement2.TextColor = Color.Gray;

            ViewElement3.Text = '\uf196'.ToString();
            ViewElement3.TextColor = Color.Gray;

            ViewElement4.Text = '\uf196'.ToString();
            ViewElement4.TextColor = Color.Gray;



            var ElementSectionInfoPageTap = new TapGestureRecognizer();
            ElementSectionInfoPageTap.Tapped += async (s, e) =>
            {
                await Navigation.PushModalAsync(new ElementSectioInfoPage());

            };
            ViewElement.GestureRecognizers.Add(ElementSectionInfoPageTap);

        }
    
    }
}
