using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.CustomControls
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
            Focused += Entry_Focused;
            TextChanged += Entry_TextChanged;
            Unfocused += Entry_Unfocused;
        }

        bool LabelVisible = false;
        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsFocused) return;

            try
            {
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
                    await lblLabel.TranslateTo(0, 20, 200);
                }
                else if (Text.Length < 1)
                {
                    LabelVisible = false;
                    lblLabel.FadeTo(0, 200);
                    await lblLabel.TranslateTo(0, 40, 200);
                    ((StackLayout)Parent).Children.RemoveAt(EntryPositionInStack);
                    EntryPositionInStack = 0;
                }
            }
            catch { }
        }

        private  void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            if (EntryPositionInStack > 0)
            {
                lblLabel.FadeTo(0, 200);
                lblLabel.TranslateTo(0, 40, 200);
                ((StackLayout)Parent).Children.RemoveAt(EntryPositionInStack);
                EntryPositionInStack = 0;
            }
        }

        private  void Entry_Focused(object sender, FocusEventArgs e)
        {
            var StackChildren = ((StackLayout)Parent).Children;
            try
            {
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
            catch { }
        }
    }
}
