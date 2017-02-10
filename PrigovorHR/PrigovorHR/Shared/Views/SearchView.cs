using Android.Views.InputMethods;
using PrigovorHR.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public class SearchView : ContentView, IDisposable
    {
        public SearchBar _SearchBarField;
        private SearchController _SearchController;
        public delegate void SearchActivatedHandler(string searchtext);
        public event SearchActivatedHandler _SearchActivated;

        public delegate void SearchDeactivatedHandler();
        public event SearchDeactivatedHandler _SearchDeactivated;


        public delegate void GPSActivationRequestedHandler();
        public event GPSActivationRequestedHandler _GPSActivationRequestedEvent;

        public delegate void QRActivationRequestedHandler();
        public event QRActivationRequestedHandler _QRActivationRequestedEvent;

        public delegate void DirectTagActivationRequestedHandler();
        public event DirectTagActivationRequestedHandler _DirectTagActivationRequestedEvent;

        public bool _GPSActivated = false;
        public bool _isTyping { get { return _SearchController._isTyping; } private set { } }
        public SearchView()
        {
            _SearchBarField = new SearchBar()
            {
                Placeholder = "Pronađite tvrtku/poslovnicu",
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                FontFamily = "Arial",
                TextColor = Color.Black,
             //   WidthRequest = 50,
                PlaceholderColor = Color.FromHex("ffa500"),
               // BackgroundColor = Color.Transparent,
              //  Opacity = 1,
                //   InputTransparent = false,
                //HeightRequest = 50
             //   VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            //var table = new TableView();
            //table.Intent = TableIntent.Form;
            var firstlayout = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand, Spacing=0 };
            var iconslayout = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };
            var searchbarlayout = new StackLayout() { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.FromHex("FF6A00") };
            var SearchBarOptions = new SearchBarOptionsView(true, true, true, true);

         //    iconslayout.Children.Add(SearchBarOptions);
             searchbarlayout.Children.Add(_SearchBarField);
            firstlayout.Children.Add(SearchBarOptions);
            firstlayout.Children.Add(searchbarlayout);
            
            //foreach (var button in SearchBarOptions)
            //    iconslayout.Children.Add(button ?? new Button() { Text = "", IsVisible = false });


            //       table.Root = new TableRoot() { new TableSection("Getting Started") { new ViewCell() { View = layout } } };

            // Content = table;
            //Content = firstlayout;
            _SearchController = new SearchController( _SearchBarField, null);
            _SearchController.SearchActivated += _SearchController_SearchActivated;
            _SearchController.SearchDeactivated += _SearchController_SearchDeactivated;
            SearchBarOptions._OptionClickedEvent += SearchBarOptions__OptionClickedEvent;
            Content = firstlayout;
        }

        private void SearchBarOptions__OptionClickedEvent(SearchBarOptionsView.eOptionButtons optionclicked)
        {
            switch (optionclicked)
            {
                case SearchBarOptionsView.eOptionButtons.gps:
                    _GPSActivated = !_GPSActivated;
                    if (_GPSActivated) _GPSActivationRequestedEvent?.Invoke();
                    break;
                case SearchBarOptionsView.eOptionButtons.logo:
                    break;
                case SearchBarOptionsView.eOptionButtons.qrcode:
                    _QRActivationRequestedEvent?.Invoke();
                    break;
                case SearchBarOptionsView.eOptionButtons.directtag:
                    _DirectTagActivationRequestedEvent?.Invoke();
                    break;
            }
        }

        public new void Unfocus()
        {
            _SearchBarField.Unfocus();
        }

        public new void Focus()
        {
            _SearchBarField.Focus();
        }

        private void _SearchController_SearchDeactivated()
        {
            _SearchDeactivated.Invoke();
        }

        private void _SearchController_SearchActivated(string searchtext)
        {
            _SearchActivated.Invoke(_SearchBarField.Text);
        }

        public void Dispose()
        {
            _SearchBarField = null;
            _SearchController = null;
            _SearchActivated = null;
            _SearchDeactivated = null;
        }
    }
}
