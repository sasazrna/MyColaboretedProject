using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class CompanyElementsListView : ContentView
    {
        public delegate void ElementSelectedHandler(int ElementId);
        public event ElementSelectedHandler ElementSelectedEvent;
         
        public CompanyElementsListView()
        {
            InitializeComponent();
        }

        public void DisplayData(List<Models.CompanyElementModel> _data)
        {
            lytCompanyElement.Children.Clear();
            foreach (var data in _data)
            {
                var CompanyElementView = new CompanyStoreFoundView(data);
                CompanyElementView.SingleClicked += CompanyElementView__SingleTaped;
                lytCompanyElement.Children.Add(CompanyElementView);
            }

            // _StackLayout.Children.LastOrDefault()?.Focus();
        }

        private void CompanyElementView__SingleTaped(int ElementId)
        {
            ElementSelectedEvent?.Invoke(ElementId);
        }
    }
}
