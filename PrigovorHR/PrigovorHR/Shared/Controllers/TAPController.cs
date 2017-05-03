using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Controllers
{
    class TAPController
    {
        private int NumberOfTaps;
        private int TapResetTime=250;
        private bool TimerStarted;

        private List<View> _views = new List<View>();
        public delegate void SingleTapHandler(string viewId, View view);
        public event SingleTapHandler SingleTaped;
        public delegate void DoubleTapHandler(string viewId, View view);
        public event DoubleTapHandler DoubleTapped;

        public TAPController(params View[] views)
        {
            var TGR = new TapGestureRecognizer();
            if (views != null)
            {
                foreach (var _view in views?.ToList())
                {
                    TGR = new TapGestureRecognizer();
                    TGR.Tapped += Tgr_Tapped;
                    _view.GestureRecognizers.Add(TGR);
                    _views.Add(_view);
                }
            }
        }

        private void Tgr_Tapped(object sender, EventArgs e)
        {
            var TappedView = ((View)sender);
            NumberOfTaps++;
            if (!TimerStarted)
            {
                TimerStarted = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, TapResetTime), () =>
                {
                    if (NumberOfTaps == 1)
                       SingleTaped?.Invoke(TappedView.Id.ToString(), TappedView);
                    else if (NumberOfTaps > 1)
                        DoubleTapped?.Invoke(TappedView.Id.ToString(), TappedView);

                    NumberOfTaps = 0;

                    Device.StartTimer(new TimeSpan(0, 0, 0, 0, TapResetTime*2), () =>
                    {
                        TimerStarted = false;
                        return false;
                    });

                    return false;//Stop the timer
                });
            }
        }
    }
}
