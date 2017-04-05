using FAB.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class FABView : ContentView
    {
        public FABView()
        {
            InitializeComponent();
        }

        public void SetFab()
        {
            //FAB.Forms.FloatingActionButton FAB1 = new FAB.Forms.FloatingActionButton() { NormalColor = Color.FromHex("#FF7e65"),
            //    RippleColor = Color.FromHex("#FF7e65"), Size = FAB.Forms.FabSize.Normal, Source = "clear.png"
            //};

            //Content =  FAB1 ;

            var layout = new RelativeLayout();

            var normalFab = new FAB.Forms.FloatingActionButton();
            normalFab.Source = "clear.png";
            normalFab.Size = FabSize.Normal;

            layout.Children.Add(
                normalFab,
                xConstraint: Constraint.RelativeToParent((parent) => { return (parent.Width - normalFab.Width) - 16; }),
                yConstraint: Constraint.RelativeToParent((parent) => { return (parent.Height - normalFab.Height) - 16; })
            );

            Content = layout;
        }
    }
}
