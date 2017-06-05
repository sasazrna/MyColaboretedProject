using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyElementSectionView : ContentView
    {
        private string ElementSlug;
        public delegate void CompanySectionSelectedHandler();
        private Controllers.TAPController TAPController;

        public CompanyElementSectionView()
        {
            InitializeComponent();
            //lblSectionIcon.Text = FontAwesomeLabel.Images.FAMinusSquareO;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            await Navigation.PushAsync(new Pages.Company_ElementInfoPage(
               JsonConvert.DeserializeObject<Models.CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(ElementSlug)), false), true);
      
        }

        public CompanyElementSectionView(Models.CompanyElementModel CompanyElement)
        {
            InitializeComponent();
            //lblSectionIcon.Text = FontAwesomeLabel.Images.FAMinusSquareO;
            //lblSectionName.Text = CompanyElement.name;
            ElementSlug = CompanyElement.slug;
            TAPController = new Controllers.TAPController(this.Content);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }
    }
}
