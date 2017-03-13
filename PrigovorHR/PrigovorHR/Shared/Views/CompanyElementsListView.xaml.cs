using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CompanyElementsListView : ContentView
    {

        #region Definitions and variables
        public delegate void ElementSelectedHandler(CompanyElementModel CompanyElement);
        public event ElementSelectedHandler ElementSelectedEvent;
        public delegate void ChangeLabelDataHandler(string Text, bool IsCounty);
        public event ChangeLabelDataHandler ChangeLabelDataEvent;
        private List<CountyCityLabel> ListOfCountyCityLabels = new List<CountyCityLabel>();
        private Dictionary<ScrollDirection, CountyCityLabel> PrevAndNextLabel = new Dictionary<ScrollDirection, CountyCityLabel>();
        private double _CompanyElementLayoutHeight;
        public enum ScrollDirection { Up = 1, Down = 2 }

        private double CompanyElementLayoutHeight
        {
            get
            {
                if (_CompanyElementLayoutHeight > 0)
                    return _CompanyElementLayoutHeight;
                else
                {
                    _CompanyElementLayoutHeight = lytCompanyElement.Children.OfType<CompanyStoreFoundView>().First().Height;
                    return _CompanyElementLayoutHeight;
                }
            }
        }

        private class CountyCityLabel : Label
        {
            public bool IsCounty { get; set; }
            public bool IsCity { get; set; }

            public new bool IsVisible { get; set; }
            public int Index { get; set; }
            public static int LastIndex = 0;
            public StackLayout RefToLayout;
        }
        #endregion

        public CompanyElementsListView()
        {
            InitializeComponent();
        }

        public void DisplayData(List<Models.CompanyElementModel> _data, bool CreateCityLabel)
        {
            lytCompanyElement.Children.Clear();
            string LastCity = string.Empty;
            string LastCounty = string.Empty;
            CountyCityLabel.LastIndex = 0;

            foreach (var data in _data)
            {
                if (string.IsNullOrEmpty(LastCounty))
                    LastCounty = data.county?.name;

                if (string.IsNullOrEmpty(LastCity))
                    LastCity = data.city?.name;

                if (CreateCityLabel & !string.IsNullOrEmpty(LastCounty) && data.county != null && LastCounty != data.county.name)
                {
                    var lyt = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.FromHex("#f3f3f3"), Spacing = 5 };
                    lytCompanyElement.Children.Add(lyt);

                    var Label = new CountyCityLabel
                    {
                        Text = data.county.name,
                        TextColor = Color.Gray,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        IsCity = false,
                        IsCounty = true,
                        IsVisible = true,
                        Index = ++CountyCityLabel.LastIndex,
                        RefToLayout = lyt
                    };
                    lyt.Children.Add(Label);

                    if (!PrevAndNextLabel.Any())
                        PrevAndNextLabel.Add(ScrollDirection.Up, Label);

                    ListOfCountyCityLabels.Add(Label);
                    LastCounty = data.county.name;
                }

                if (CreateCityLabel & !string.IsNullOrEmpty(LastCity) && data.city != null && LastCity != data.city.name)
                {
                    var lyt = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.FromHex("#f3f3f3"), Spacing = 5 };
                    lytCompanyElement.Children.Add(lyt);

                    var Label = new CountyCityLabel
                    {
                        Text = data.city.name,
                        TextColor = Color.Gray,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        IsCity = true,
                        IsCounty = false,
                        IsVisible = true,
                        Index = ++CountyCityLabel.LastIndex,
                        RefToLayout = lyt
                    };
                    lyt.Children.Add(Label);

                    if (!PrevAndNextLabel.Any())
                        PrevAndNextLabel.Add(ScrollDirection.Up, Label);

                    ListOfCountyCityLabels.Add(Label);
                    LastCity = data.city.name;
                }

                var CompanyElementView = new CompanyStoreFoundView(data);
                CompanyElementView.SingleClicked += CompanyElementView_SingleClicked;
                lytCompanyElement.Children.Add(CompanyElementView);
            }
        }

        private void CompanyElementView_SingleClicked(CompanyElementModel CompanyElement)
        {
            ElementSelectedEvent?.Invoke(CompanyElement);
        }

        public void Scrolling(double ScrollYValue, ScrollDirection ScrollDirection)
        {
            CountyCityLabel Label;
            PrevAndNextLabel.TryGetValue(ScrollDirection, out Label);
            if (Label == null) return;

            bool LabelChanged = false;

            if (ScrollYValue < CompanyElementLayoutHeight)
            {
                ChangeLabelDataEvent?.Invoke("", false);
            }
            else if (ScrollYValue > Label.RefToLayout.Y + Label.Height)
            {
                if (Label.IsVisible)
                {
                    Label.IsVisible = false;
                    ChangeLabelDataEvent?.Invoke(Label.Text, Label.IsCounty);
                    LabelChanged = true;
                }
            }
            else if (ScrollYValue < Label.RefToLayout.Y)
            {
                if (!Label.IsVisible)
                {
                    Label.IsVisible = true;
                    var PrevLabel = Label.Index > 1 ? ListOfCountyCityLabels.First(l => l.Index == Label.Index - 1) : Label;
                    ChangeLabelDataEvent?.Invoke(PrevLabel.Text, PrevLabel.IsCounty);
                    LabelChanged = true;
                }
            }
   
            if (LabelChanged)
            {
                PrevAndNextLabel.Clear();
                if (ScrollDirection == ScrollDirection.Up)
                {
                    PrevAndNextLabel.Add(ScrollDirection.Up, ListOfCountyCityLabels.First(l => l.Index == Label.Index + 1));
                    PrevAndNextLabel.Add(ScrollDirection.Down, Label);
                }
                else
                {
                    PrevAndNextLabel.Add(ScrollDirection.Up, Label);
                    PrevAndNextLabel.Add(ScrollDirection.Down, Label.Index > 1 ? ListOfCountyCityLabels.First(l => l.Index == Label.Index - 1) : Label);
                }
            }
        }
    }
}
