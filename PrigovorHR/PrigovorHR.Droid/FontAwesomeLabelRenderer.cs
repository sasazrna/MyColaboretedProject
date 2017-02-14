using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

/* 'Icon' is the namespace for my solution. So instead you should include your namespaces, of course. */
using PrigovorHR;
using PrigovorHR.Droid;

[assembly: ExportRenderer(typeof(FontAwesomeLabel), typeof(FontAwesomeLabelRenderer))]
namespace PrigovorHR.Droid
{
    class FontAwesomeLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var label = Control;
            Typeface font;
            try
            {
                font = Typeface.CreateFromAsset(Forms.Context.Assets, "Fonts/fontawesome-webfont.ttf");
                label.Typeface = font;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TTF file not found. Make sure the Android project contains it at '/Assets/Fonts/fontawesome-webfont.ttf'.");
            }

        }
    }
}