using System;

using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Util;
using Complio.Shared.Views;
using static PrigovorHR.AndroidRenderers.AndroidRenderers;
using Android.Runtime;

[assembly: ExportRenderer(typeof(CurvedCornersLabel), typeof(CurvedCornersLabelRenderer))]
[assembly: ExportRenderer(typeof(FontAwesomeLabel), typeof(FontAwesomeLabelRenderer))]
[assembly: ExportRenderer(typeof(EntryEditText), typeof(EntryEditTextRenderer))]

//[assembly: ExportRenderer(typeof(MultiLineLabel), typeof(CustomMultiLineLabelRenderer))]

namespace PrigovorHR.AndroidRenderers
{
    class AndroidRenderers
    {
        public class CurvedCornersLabelRenderer : LabelRenderer
        {
            private GradientDrawable _gradientBackground;

            protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
            {
                base.OnElementChanged(e);

                var view = (CurvedCornersLabel)Element;
                if (view == null) return;

                // creating gradient drawable for the curved background
                _gradientBackground = new GradientDrawable();
                _gradientBackground.SetShape(ShapeType.Rectangle);
                _gradientBackground.SetColor(view.CurvedBackgroundColor.ToAndroid());

                // Thickness of the stroke line
                _gradientBackground.SetStroke(4, view.CurvedBackgroundColor.ToAndroid());

                // Radius for the curves
                _gradientBackground.SetCornerRadius(
                    DpToPixels(this.Context,
                    Convert.ToSingle(view.CurvedCornerRadius)));

                // set the background of the label
                Control.SetBackground(_gradientBackground);
            }

            /// <summary>
            /// Device Independent Pixels to Actual Pixles conversion
            /// </summary>
            /// <param name="context"></param>
            /// <param name="valueInDp"></param>
            /// <returns></returns>
            public static float DpToPixels(Context context, float valueInDp)
            {
                DisplayMetrics metrics = context.Resources.DisplayMetrics;
                return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
            }
        }

        public class FontAwesomeLabelRenderer : LabelRenderer
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

        public class EntryEditTextRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
        {
            public EntryEditTextRenderer()
            { }

            public EntryEditTextRenderer(IntPtr javaReference, JniHandleOwnership transfer)
            { }
        }


        //public class CustomMultiLineLabelRenderer : LabelRenderer
        //{
        //    protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        //    {
        //        base.OnElementChanged(e);

        //        MultiLineLabel multiLineLabel = (MultiLineLabel)Element;

        //        if (multiLineLabel != null && multiLineLabel.Lines != -1)
        //        {
        //            Control.SetSingleLine(false);
        //            Control.SetLines(multiLineLabel.Lines);
        //        }
        //    }
        //}

    }
}
