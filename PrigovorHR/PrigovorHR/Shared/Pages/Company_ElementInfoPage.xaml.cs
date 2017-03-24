using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using PrigovorHR.Shared.Controllers;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Company_ElementInfoPage : ContentPage
    {
        private Controllers.TAPController TAPController;
        private CompanyElementRootModel CompanyElement;
        public static Company_ElementInfoPage ReferenceToView;

        public Company_ElementInfoPage()
        {

        }

        public Company_ElementInfoPage(CompanyElementRootModel companyElement, bool ShowOtherElements)
        {
            try
            {
                InitializeComponent();
                lytCompanyInfoView.Children.Clear();
                lytCompanyElementInfoView.Children.Clear();
                lytCompanyOtherElementsView.Children.Clear();
                lblOtherStores.IsEnabled = ShowOtherElements && Convert.ToBoolean(companyElement.siblings?.Any());
                CompanyElement = companyElement;
                TAPController = new Controllers.TAPController(lblStore, lblCompany, lblOtherStores);
                TAPController.SingleTaped += TAPController_SingleTaped;
                NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
                btnWriteComplaint.Clicked += BtnWriteComplaint_Clicked;
                ReferenceToView = this;
                lytCompanyElementUnderline.IsVisible = true;
                lytCompanyUnderline.IsVisible = false;
                lytCompanyOtherElementsUnderline.IsVisible = false;

               // SetCompanyLogo();

                TAPController_SingleTaped(null, lblStore);

                LogoStack.IsVisible = true;
            }
            catch (Exception ex) { Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString()); }
        }

        private async void SetCompanyLogo()
        {
            try
            {
                var CompanyLogo = await DataExchangeServices.GetCompanyLogo(CompanyElement.element.root_business.id.ToString());
                imgCompanyLogo.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(CompanyLogo)));
            }
            catch(Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "Greška kod private async void SetCompanyLogo()");
            }
        }

        private async void BtnWriteComplaint_Clicked(object sender, EventArgs e)
        {
            var NewComplaintPage = new NewComplaintPage(CompanyElement.element);
            await Navigation.PushModalAsync(NewComplaintPage, true);
            NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopModalAsync(true); Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); };
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await NavigationBar.imgBack.RotateTo(90, 75);
                await Navigation.PopModalAsync(true);
            });
            return true;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if(view == lblStore)
            {
                lytCompanyElementUnderline.IsVisible = true;
                lytCompanyUnderline.IsVisible = false;
                lytCompanyOtherElementsUnderline.IsVisible = false;

                if (!lytCompanyElementInfoView.Children.Any())
                    lytCompanyElementInfoView.Children.Add(new Views.CompanyElementInfoView(CompanyElement.element));

                lytCompanyElementInfoView.IsVisible = true;
                lytCompanyInfoView.IsVisible = false;
                lytCompanyOtherElementsView.IsVisible = false;
                LogoStack.IsVisible = true;
                btnWriteComplaint.IsVisible = true;
            }
            else if(view == lblCompany)
            {
                lytCompanyElementUnderline.IsVisible = false;
                lytCompanyUnderline.IsVisible = true;
                lytCompanyOtherElementsUnderline.IsVisible = false;

                if (!lytCompanyInfoView.Children.Any())
                    lytCompanyInfoView.Children.Add(new Views.CompanyInfoView(CompanyElement.element.root_business));

                lytCompanyInfoView.IsVisible = true;
                lytCompanyElementInfoView.IsVisible = false;
                lytCompanyOtherElementsView.IsVisible = false;
                LogoStack.IsVisible = true;
                btnWriteComplaint.IsVisible = true;
            }
            else if(view == lblOtherStores)
            {
                lytCompanyElementUnderline.IsVisible = false;
                lytCompanyUnderline.IsVisible = false;
                lytCompanyOtherElementsUnderline.IsVisible = true;
                lytCompanyElementInfoView.IsVisible = false;
                lytCompanyInfoView.IsVisible = false;

                if (!lytCompanyOtherElementsView.Children.Any())
                    lytCompanyOtherElementsView.Children.Add(new Views.CompanyOtherElementsView(CompanyElement));

                lytCompanyOtherElementsView.IsVisible = true;
                LogoStack.IsVisible = false;
                btnWriteComplaint.IsVisible = false;
            }
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
        }
    }
}
