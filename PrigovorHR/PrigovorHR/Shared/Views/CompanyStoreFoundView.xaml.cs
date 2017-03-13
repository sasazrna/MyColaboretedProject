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

    public partial class CompanyStoreFoundView : ContentView
    {
        private Controllers.TAPController TAPController;
        public delegate void SingleTapHandler(CompanyElementModel CompanyElement);
        public event SingleTapHandler SingleClicked;
        public delegate void DoubleTapHandler();
        public event DoubleTapHandler DoubleClicked;
        private  CompanyElementModel CompanyElement;

        public CompanyStoreFoundView(CompanyElementModel companyElement)
        {
            InitializeComponent();
            TAPController = new Controllers.TAPController(this);
            CompanyElement = companyElement;
            _lblStoreName.Text = CompanyElement.name;
            _lblAddress.Text = CompanyElement.address;
            _lblCompanyName.Text = CompanyElement.root_business.name;
            //_lblFirstLetter.Text = storeName.Substring(0, 1);
            TAPController.SingleTaped += TAPController_SingleTaped;
            TAPController.DoubleTapped += TAPController_DoubleTapped;
        }

        public CompanyStoreFoundView()
        {
            InitializeComponent();
        }

        private void TAPController_DoubleTapped(string viewId, View view)
        {
            DoubleClicked?.Invoke();
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            SingleClicked?.Invoke(CompanyElement);
        }

        public void Dispose()
        {
            TAPController.SingleTaped -= TAPController_SingleTaped;
            TAPController.DoubleTapped -= TAPController_DoubleTapped;
            TAPController = null;
        }
    }
}

