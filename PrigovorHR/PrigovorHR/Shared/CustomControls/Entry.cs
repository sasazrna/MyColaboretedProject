using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.CustomControls
{
    public class Entry : Xamarin.Forms.Entry
    {
        private Xamarin.Forms.Label lblLabel = new Label() {
            FontSize = Device.GetNamedSize(NamedSize.Micro, 
                typeof(Label)), FontAttributes = FontAttributes.Bold};
        private int EntryPositionInStack = 0;

        public Entry()
        {
            lblLabel.TextColor = Color.FromHex("#ff7e65");
            lblLabel.FadeTo(0, 0);
            lblLabel.TranslateTo(0, 40, 0);
            //Focused += Entry_Focused;
            TextChanged += Entry_TextChanged;
            Unfocused += Entry_Unfocused;
        }

        bool LabelVisible = false;
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsFocused) return;

            if (Text.Length > 0 & !LabelVisible)
            {
                LabelVisible = true;
                var StackChildren = ((StackLayout)Parent).Children;

                if (EntryPositionInStack == 0)
                {
                    for (int i = 0; i < StackChildren.Count; i++)
                        if (StackChildren[i] == this)
                        {
                            EntryPositionInStack = i;
                            break;
                        }

                    ((StackLayout)Parent).Children.Insert(EntryPositionInStack, lblLabel);
                }

                lblLabel.Text = Placeholder;
                lblLabel.FadeTo(1, 200);
                lblLabel.TranslateTo(0, 20, 200);
            }
            else if(Text.Length<1 )
            {
                LabelVisible = false;
                lblLabel.FadeTo(0, 200);
                lblLabel.TranslateTo(0, 40, 200);
            }
        }

        private  void Entry_Unfocused(object sender, FocusEventArgs e)
        {
             lblLabel.FadeTo(0, 200);
             lblLabel.TranslateTo(0, 40, 200);
        }

        private  void Entry_Focused(object sender, FocusEventArgs e)
        {
            var StackChildren = ((StackLayout)Parent).Children;

            if (EntryPositionInStack == 0)
            {
                for (int i = 0; i < StackChildren.Count; i++)
                    if (StackChildren[i] == this)
                    {
                        EntryPositionInStack = i;
                        break;
                    }

                ((StackLayout)Parent).Children.Insert(EntryPositionInStack, lblLabel);
            }

            lblLabel.Text = Placeholder;
             lblLabel.FadeTo(1, 200);
             lblLabel.TranslateTo(0, 20, 200);
        }
    }
}
