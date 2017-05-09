using System;
using Android.App;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.OS;
using Android.Util;
using PrigovorHR.Shared.Views;
namespace PrigovorHR.Droid
{
    public class KeyBoardOverlayFix
    {
        private readonly View mChildOfContent;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;

        public static void assistActivity(Activity activity, IWindowManager windowManager)
        {
            new KeyBoardOverlayFix(activity, windowManager);
        }

        private KeyBoardOverlayFix(Activity activity, IWindowManager windowManager)
        {

            var softButtonsHeight = getSoftbuttonsbarHeight(windowManager);

            var content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);
            mChildOfContent = content.GetChildAt(0);
            var vto = mChildOfContent.ViewTreeObserver;
            vto.GlobalLayout += (sender, e) => possiblyResizeChildOfContent(softButtonsHeight);
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void possiblyResizeChildOfContent(int softButtonsHeight)
        {
            var usableHeightNow = computeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                var usableHeightSansKeyboard = mChildOfContent.RootView.Height - softButtonsHeight;
                var heightDifference = usableHeightSansKeyboard - usableHeightNow;
                if (heightDifference > (usableHeightSansKeyboard / 4))
                {
                    // keyboard probably just became visible
                    frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference+50;// + (softButtonsHeight / 2);
                }
                else
                {
                    // keyboard probably just became hidden
                    frameLayoutParams.Height = usableHeightSansKeyboard;
                }
                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            var r = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(r);
            return (r.Bottom - r.Top);
        }

        private int getSoftbuttonsbarHeight(IWindowManager windowManager)
        {
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                var metrics = new DisplayMetrics();
                windowManager.DefaultDisplay.GetMetrics(metrics);
                int usableHeight = metrics.HeightPixels;
                windowManager.DefaultDisplay.GetRealMetrics(metrics);
                int realHeight = metrics.HeightPixels;
                if (realHeight > usableHeight)
                    return realHeight - usableHeight;
                else
                    return 0;
            }
            return 0;
        }
    }
}