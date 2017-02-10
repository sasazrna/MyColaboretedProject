using PrigovorHR.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    class SearchBarOptionsView:StackLayout
    {
        public enum eOptionButtons :int { logo=0, directtag=1, gps=2, qrcode=3};

        public delegate void OptionClickedHandler(eOptionButtons optionclicked);
        public event OptionClickedHandler _OptionClickedEvent;

     //   private delegate void GPSEnabledChangedHandler(
        private enum eGPSOptionStatus : int { unvailable=0, available=1, activated=2 };
        private eGPSOptionStatus _CurrentGPSStatus = eGPSOptionStatus.unvailable;
        private List<Button> _Buttons = new List<Button>();
        private List<StackLayout> _ButtonLayouts = new List<StackLayout>();
        public  SearchBarOptionsView(bool logo, bool quickfind, bool gps, bool qrcode)
        {
            Orientation = StackOrientation.Horizontal;
            BackgroundColor = Color.FromHex("FF6A00");
            if (logo)
            {
                //set logo as button, a little bigger than the rest of crew.
                _Buttons.Add(new Button() { Text = "LOGO", BorderRadius=-1, BorderWidth=-1, TextColor = Color.White, FontSize = 16, FontAttributes = FontAttributes.Bold, CommandParameter = "logo", BackgroundColor = Color.FromHex("FF6A00") });
                _ButtonLayouts.Add(new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start });
                _ButtonLayouts.Last().Children.Add(_Buttons.Last());
                _Buttons.Last().Clicked += SearchBarOptionsViewButton_Clicked;

            }
            else
            {
                //Add(null);
            }

            if (quickfind)
            {
                _Buttons.Add(new Button() { Text = "@", TextColor = Color.White, FontSize = 16, FontAttributes = FontAttributes.Bold, CommandParameter = "directtag", BackgroundColor = Color.FromHex("FF6A00") });
                _ButtonLayouts.Add(new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.CenterAndExpand });
                _ButtonLayouts.Last().Children.Add(_Buttons.Last());
                _Buttons.Last().Clicked += SearchBarOptionsViewButton_Clicked;

                //find @symbol and use it. If clicked and using change colour
            }

            if (gps)
            {
                _CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));
             //    Acr.UserDialogs.UserDialogs.Instance.AlertAsync(Enum.GetName(typeof(eGPSOptionStatus), Convert.ToInt32(Controllers.GPSController._GPSEnabled)) + ".png");

                _Buttons.Add(new Button()
                {
                    Image = Enum.GetName(typeof(eGPSOptionStatus), Convert.ToInt32(Controllers.GPSController._GPSEnabled)) + ".png", Scale = 0.5,
                    FontAttributes = FontAttributes.Bold,
                    CommandParameter = "gps",
                    BackgroundColor = Color.FromHex("FF6A00")
                });

                _ButtonLayouts.Add(new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.CenterAndExpand });
                _ButtonLayouts.Last().Children.Add(_Buttons.Last());

                _Buttons.Last().Clicked += SearchBarOptionsViewButton_Clicked;
                Controllers.GPSController._GPSEnabledChangedEvent += GPSController__GPSEnabledChangedEvent;
            }

            if (qrcode)
            {
                _Buttons.Add(new Button() { FontSize = 16, FontAttributes = FontAttributes.Bold, CommandParameter = "qrcode", BackgroundColor = Color.FromHex("FF6A00") });
                _ButtonLayouts.Add(new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.CenterAndExpand });
                _ButtonLayouts.Last().Children.Add(_Buttons.Last());

                _Buttons.Last().Clicked += SearchBarOptionsViewButton_Clicked;
            }

            foreach (var bl in _ButtonLayouts)
                Children.Add(bl);
        }

        private void SearchBarOptionsViewButton_Clicked(object sender, EventArgs e)
        {
            var ButtonClicked = ((Button)sender);
            switch (ButtonClicked.CommandParameter.ToString())
            {
                case "gps":
                    GPSOptionClicked(ref ButtonClicked);
                    break;
            }

            _OptionClickedEvent?.Invoke((eOptionButtons)Enum.Parse(typeof(eOptionButtons), ButtonClicked.CommandParameter.ToString()));
        }

        private void GPSOptionClicked(ref Button btn)
        {
            if (_CurrentGPSStatus != eGPSOptionStatus.unvailable)
            {
                if (_CurrentGPSStatus == eGPSOptionStatus.activated)
                    _CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(Controllers.GPSController._GPSEnabled)));
                else
                    _CurrentGPSStatus = eGPSOptionStatus.activated;

                btn.Image = Enum.GetName(typeof(eGPSOptionStatus), (int)_CurrentGPSStatus) + ".png";
            }
            else
                Acr.UserDialogs.UserDialogs.Instance.Confirm(new Acr.UserDialogs.ConfirmConfig()
                {
                    OkText = "DA",
                    CancelText = "NE",
                    //  IsCancellable = true,
                    Message = "GPS nije uključen ili dostupan!" + Environment.NewLine + "Želite li otvoriti postavke i aktivirati GPS?",
                    Title = "PrigovorHR"
                }.SetAction((re) => { if (re) { Controllers.GPSController.OpenGPSOptions(); } }));
        }


        private void GPSController__GPSEnabledChangedEvent(bool state)
        {
            _Buttons[((int)eOptionButtons.gps)].Image =  Enum.GetName(typeof(eGPSOptionStatus), Convert.ToInt32(state)) + ".png";
            _CurrentGPSStatus = ((eGPSOptionStatus)(Convert.ToInt32(state)));
        }
    }
}
