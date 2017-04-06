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
        private Views.FontAwesomeLabel FAB = new Views.FontAwesomeLabel();

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
                imgOtherStores.IsEnabled = ShowOtherElements && Convert.ToBoolean(companyElement.siblings?.Any());
                CompanyElement = companyElement;
                NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
                
                ReferenceToView = this;
                lytCompanyElementUnderline.IsVisible = true;
                lytCompanyUnderline.IsVisible = false;
                lytCompanyOtherElementsUnderline.IsVisible = false;

                SetCompanyLogo();

                if(CompanyElement.element.type == null)
                {
                    lytImages.IsVisible = false;
                    lytUnderlines.IsVisible = false;
                    lytCompanyElementInfoView.IsVisible = false;
                    lytCompanyOtherElementsView.IsVisible = false;
                    TAPController_SingleTaped(null, imgCompany);
                }
                else TAPController_SingleTaped(null, imgStore);

                LogoStack.IsVisible = true;
            }
            catch (Exception ex)
            {
                ExceptionController.HandleException(ex, "public Company_ElementInfoPage(CompanyElementRootModel companyElement, bool ShowOtherElements)");
            }

           // TAPController = new Controllers.TAPController(btnWriteComplaint);
            //FAB.Text = Views.FontAwesomeLabel.Images.FABan;
            //FAB.TextColor = Color.FromHex("#FF7e65");
            //FAB.BackgroundColor = Color.Gray;
            //FAB.FontSize = 50;
            TAPController = new TAPController(imgStore, imgCompany, imgOtherStores, btnWriteComplaint);
            TAPController.SingleTaped += TAPController_SingleTaped;
            //FAB.AutomationId = "FAB";
            //lytRelative.Children.Add(
            //    FAB,
            //    xConstraint: Constraint.RelativeToParent((parent) => { return (parent.Width - FAB.Width) - 16; }),
            //    yConstraint: Constraint.RelativeToParent((parent) => { return (parent.Height - FAB.Height) - 16; }));
            //  Fabs.Add(FabImages.Keys.ToList()[i], FAB);
            //
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
                ExceptionController.HandleException(ex, "Greška kod private async void SetCompanyLogo()");
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
            if (view == btnWriteComplaint)
                BtnWriteComplaint_Clicked(null, null);

            if(view == imgStore)
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
            else if(view == imgCompany)
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
            else if(view == imgOtherStores)
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
