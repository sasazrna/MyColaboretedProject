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

    public partial class SideMenuPage : ContentPage
    {
        public SideMenuPage()
        {
            InitializeComponent();

            Menu.Text = '\uf0c9'.ToString();
            Menu.TextColor = Color.Gray;
            Menu.FontSize = 35;

            MenuBack.Text = '\uf060'.ToString();
            MenuBack.TextColor = Color.Gray;
            MenuBack.FontSize = 35;

            var ShowMenu = new TapGestureRecognizer();
            ShowMenu.Tapped += async (s, e) =>
            {
                await MenuStack.TranslateTo(0, 0, 250);
            };
            Menu.GestureRecognizers.Add(ShowMenu);

            var HideMenu = new TapGestureRecognizer();
            HideMenu.Tapped += async (s, e) =>
            {
                await MenuStack.TranslateTo(-450, 0, 250);
            };
            MenuBack.GestureRecognizers.Add(HideMenu);
        }
    }
}
