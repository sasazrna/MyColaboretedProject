
using PrigovorHR.Shared.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class MainNavigationBar : ContentView, IDisposable
    {
        private Dictionary<View, bool> _LastViewed = new Dictionary<View, bool>();
        private bool _TrackLastViewed = true;
        private Controllers.TAPController _TAPController;
        private Controllers.SearchController _SearchControler;
        public static Controllers.QRScannerController _QRScannerController = new Controllers.QRScannerController();
        public  bool _QuickComplaintRequested { get { return _entrySearch.Text.Contains("#"); } }

        private string _Title { get; set; }

        public delegate void SearchActivatedHandler(string searchtext, bool isQRCoded);
        public delegate void SearchDeactivatedHandler();
        public event SearchActivatedHandler SearchActivated;
        public event SearchDeactivatedHandler SearchDeactivated;

        public delegate void GPSActivationRequestedHandler();
        public event GPSActivationRequestedHandler _GPSActivationRequestedEvent;

        //public delegate void QRActivationRequestedHandler();
        //public event QRActivationRequestedHandler _QRActivationRequestedEvent;

        public delegate void DirectTagActivationRequestedHandler();
        public event DirectTagActivationRequestedHandler _DirectTagActivationRequestedEvent;

        public delegate void ShowProfileHandler();
        public event ShowProfileHandler _ShowProfileEvent;

        public delegate void ShowComplaintsHandler();
        public event ShowComplaintsHandler _ShowComplaintsEvent;

        public bool QRActivated { get; set; }
        public bool GPSActivated { get; set; }
        public bool QuickComplaintActivated { get; set; }
        public bool DirectTagActivated { get; set; }
        public bool isTyping { get { return _SearchControler.isTyping; } private set { } }

        private enum eGPSOptionStatus : int { unvailable = 0, available = 1, activated = 2 };
        private eGPSOptionStatus _CurrentGPSStatus = eGPSOptionStatus.unvailable;

        public static MainNavigationBar _RefToView;

        public MainNavigationBar()
        {
            InitializeComponent();

            View[] _ListOfChildViews = new View[] { _imgMenuLayout,
                _imgComplaints, _imgSearch, _imgQRCode, _lblAboutUs, _lblContactUs, _lblLogOut, _imgClose, _lblMyProfile, _lblMyComplaints };

            _TAPController = new Controllers.TAPController( _ListOfChildViews);
            _TAPController.SingleTaped += _TAPController_SingleTaped;
            _SearchControler = new Controllers.SearchController(null, _entrySearch);
            _SearchControler.SearchActivated += _SearchControler_SearchActivated;
            _SearchControler.SearchDeactivated += _SearchControler_SearchDeactivated;
            _entrySearch.Unfocused += EntrySearch_Unfocused;
            Controllers.QRScannerController.ScanCompletedEvent += QRScannerController__ScanCompletedEvent;
            _RefToView = this;

            //_CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));

            //if (_CurrentGPSStatus != eGPSOptionStatus.unvailable)
            //{
            //    if (_CurrentGPSStatus == eGPSOptionStatus.activated)
            //        _CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));
            //    else
            //        _CurrentGPSStatus = eGPSOptionStatus.activated;

            //    btn.Image = Enum.GetName(typeof(eGPSOptionStatus), (int)_CurrentGPSStatus) + ".png";
            //}
            //else
            //    Acr.UserDialogs.UserDialogs.Instance.Confirm(new Acr.UserDialogs.ConfirmConfig()
            //    {
            //        OkText = "DA",
            //        CancelText = "NE",
            //        //  IsCancellable = true,
            //        Message = "GPS nije uključen ili dostupan!" + Environment.NewLine + "Želite li otvoriti postavke i aktivirati GPS?",
            //        Title = "PrigovorHR"
            //    }.SetAction((re) => { if (re) { Controllers.GPSController.OpenGPSOptions(); } }));

            // Device.StartTimer(new TimeSpan(0, 0, 1), (() => { UpdateChildrenLayout(); return true; }));

            _SearchOptionsLayout.SizeChanged += _SearchOptionsLayout_SizeChanged;
        }

        private void _SearchOptionsLayout_SizeChanged(object sender, EventArgs e)
        {
            _SearchOptionsLayout.SizeChanged -= _SearchOptionsLayout_SizeChanged;
            if (_SearchOptionsLayout.MinimumHeightRequest < 0 & _SearchOptionsLayout.IsVisible)
                _SearchOptionsLayout.MinimumHeightRequest = _SearchOptionsLayout.Height;
            _SearchOptionsLayout.SizeChanged += _SearchOptionsLayout_SizeChanged;
        }

        private void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await ((Page)Parent.Parent.Parent).Navigation.PopAsync(true);

                //if (isQRFormat)
                //{
                    _entrySearch.Text = result;
                    _SearchControler.isQRTextActive = true;
                //}
                //else
                //{
                //    Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani kod nije QR formata" + Environment.NewLine + "Skenirajte ispravan kod", "Prigovor.hr", "OK");
                //}
            });
        }

        private void _TAPController_SingleTaped(string viewId, View view)
        {
            //await view.ScaleTo(view.Scale + 0.3, 70);
            //await view.ScaleTo(view.Scale - 0.3, 70);

            if (view == _imgMenuLayout)
            {
                _MenuFrame.IsVisible = !_MenuFrame.IsVisible;
                DeactivateSearchField();
                if (!_LastViewed.ContainsKey(_imgMenuLayout) & _TrackLastViewed)
                    _LastViewed.Add(_imgMenuLayout, false);
            }
            else if (view == _imgSearch)
            {
                ActivateSearchField();
                _MenuFrame.IsVisible = false;
                if (!_LastViewed.ContainsKey(_imgSearch) & _TrackLastViewed)
                    _LastViewed.Add(_imgSearch, false);
            }
            else if (view == _imgClose)
            {
                if (string.IsNullOrEmpty(_entrySearch.Text))
                {
                    DeactivateSearchField();
                }
                else
                {
                    _SearchControler.stopTextChangedEvent = true;
                    _entrySearch.Text = string.Empty;
                    _SearchControler.stopTextChangedEvent = false;
                    ActivateSearchField();
                }
            }
            else if (view == _imgQRCode)
            {
                 ((Page)Parent.Parent.Parent).Navigation.PushAsync(_QRScannerController, true);
                _QRScannerController.StartScan();
            }
            if (view == _lblAboutUs)
            {

            }
            else if (view == _lblContactUs)
            {
                 Navigation.PushModalAsync(new ContactUsPage());
            }
            else if (view == _lblMyProfile)
            {
                _ShowProfileEvent?.Invoke();
                _MenuFrame.IsVisible = false;
            }
            else if (view == _lblMyComplaints | view == _imgComplaints)
            {
                _ShowComplaintsEvent?.Invoke();
                _MenuFrame.IsVisible = false;
            }
            else if (view == _lblLogOut)
            {
                Acr.UserDialogs.UserDialogs.Instance.Confirm(
                    new Acr.UserDialogs.ConfirmConfig()
                    {
                        Title = "Odjava",
                        CancelText = "Odustani",
                        OkText = "Odjavi me",
                        Message = "Jeste li sigurni u odjavu iz aplikacije?",
                        OnAction = (Confirm) =>
                        {
                            if (Confirm)
                                Controllers.LoginRegisterController.UserLogOut();
                        }
                    });
            }
        }

        public async void animate()
        {
            while (true)
            {
                await _imgComplaints.RotateTo(20, 400);
                await _imgComplaints.RotateTo(-100, 400);
            }
        }

        private void EntrySearch_Unfocused(object sender, FocusEventArgs e)
        {
            if(_TrackLastViewed)
              DeactivateSearchField();
        }

        private void _SearchControler_SearchDeactivated()
        {
            DeactivateSearchField();
        }

        private void _SearchControler_SearchActivated(string searchtext, bool isQRCoded)
        {
            SearchActivated?.Invoke(searchtext, isQRCoded);
         //   DeactivateSearchField();
        }

        public void ChangeNavigationTitle(string newtitle)
        {
            _lblNavigationTitle.Text = newtitle;
            if (newtitle == "Prigovor.hr")
                _imgLogo.IsVisible = true;
            else _imgLogo.IsVisible = false;

            // _entrySearch.Text = "Neštoo";
        }

        public void ActivateSearchField()
        {
            _imgLogo.IsVisible = false;
            _imgSearch.IsVisible = false;
            _lblNavigationTitle.IsVisible = false;
            _imgComplaints.IsVisible = false;
            _imgQRCode.IsVisible = false;
            _lblNumOfUnreadResponses.IsVisible = false;
            _imgClose.IsVisible = true;
            _entrySearch.IsVisible = true;
            _SearchOptionsLayout.IsVisible = true;
            _entrySearch.Focus();
            if(!string.IsNullOrEmpty(_entrySearch.Text ))
            SearchActivated?.Invoke(_entrySearch.Text, false);

        }

        public void DeactivateSearchField()
        {
            if (_lblNavigationTitle.Text == "Prigovor.hr")
                _imgLogo.IsVisible = true;

            _entrySearch.Unfocus();
            _entrySearch.IsVisible = false;
            _imgClose.IsVisible = false;
            _SearchOptionsLayout.IsVisible = false;
            _imgSearch.IsVisible = true;
            _lblNavigationTitle.IsVisible = true;
            _imgComplaints.IsVisible = true;
            _imgQRCode.IsVisible = true;
            _lblNumOfUnreadResponses.IsVisible = true;
            _imgMenu.Focus();
            SearchDeactivated?.Invoke();
        }

        public  bool BackButtonPressedEvent()
        {
            if (_LastViewed.Count > 0)
            {
                _TrackLastViewed = false;
               _TAPController_SingleTaped(string.Empty, _LastViewed.Last().Key);

                if (_LastViewed.Count > 1)
                   _TAPController_SingleTaped(string.Empty, _LastViewed.First().Key);

                _LastViewed.Remove(_LastViewed.Last().Key);
                _TrackLastViewed = true;
                return true;
            }
            else
            {
                _TrackLastViewed = true;
                return false;
            }
        }

        private int _numOfUnreadedComplaints;

        public int NumOfUnreadedComplaints
        {
            get { return _numOfUnreadedComplaints; }
            set
            {
                _numOfUnreadedComplaints = value;
                _lblNumOfUnreadResponses.Text = value.ToString();
            }
        }

        public void Dispose()
        {
        }
    }
}
