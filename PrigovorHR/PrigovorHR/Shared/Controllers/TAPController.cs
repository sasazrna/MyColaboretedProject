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
        private int _NumberOfTaps;
        private int _TapResetTime=250;
        private bool _TimerStarted;

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
            _NumberOfTaps++;
            if (!_TimerStarted)
            {
                _TimerStarted = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, _TapResetTime), () =>
                {
                    if (_NumberOfTaps == 1)
                       SingleTaped?.Invoke(TappedView.Id.ToString(), TappedView);
                    else if (_NumberOfTaps > 1)
                        DoubleTapped?.Invoke(TappedView.Id.ToString(), TappedView);

                    _NumberOfTaps = 0;
                    _TimerStarted = false;
                    return false;//Stop the timer
                });
            }
        }
    }
}
