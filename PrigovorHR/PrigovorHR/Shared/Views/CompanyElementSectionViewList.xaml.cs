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
    public partial class CompanyElementSectionViewList : ContentView
    {
        public CompanyElementSectionViewList()
        {
            InitializeComponent();
        }

        public CompanyElementSectionViewList(Models.CompanyElementModel CompanyElement)
        {
            InitializeComponent();
            lytCompanyElementSectionView.Children.Clear();

            foreach (var Child in CompanyElement.children)
                lytCompanyElementSectionView.Children.Add(new CompanyElementSectionView(Child));

            lblNoSections.IsVisible = !lytCompanyElementSectionView.Children.Any();    
        }
    }
}
