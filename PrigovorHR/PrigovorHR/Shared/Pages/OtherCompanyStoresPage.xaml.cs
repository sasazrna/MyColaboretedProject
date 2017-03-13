using Newtonsoft.Json;
using PrigovorHR.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherCompanyStoresPage : ContentPage
    {
        private CompanyElementRootModel CompanyElement;
        private double LastScrollValue = 0;
        private List<CompanyElementModel> OrderedElements = new List<CompanyElementModel>();
        public OtherCompanyStoresPage(CompanyElementRootModel companyElement)
        {
            InitializeComponent();
            CompanyElement = companyElement;

            try
            {
                OrderedElements = SetAndOrderElementsToDisplay(companyElement);
                SetCountyCityLabels();

                if (OrderedElements != null)
                {
                    CompanyElementsListView.DisplayData(OrderedElements, true);
                    CompanyElementsListView.ChangeLabelDataEvent += CompanyElementsListView_ChangeLabelDataEvent;
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do problema pri učitavanju poslovnica!" + System.Environment.NewLine + "Provjerite internet konekciju", "Greška", "OK");
                    OnBackButtonPressed();
                }
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "public OtherCompanyStoresPage(CompanyElementRootModel companyElement)");
            }
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
            CompanyElementsListView.ElementSelectedEvent += CompanyElementsListView_ElementSelectedEvent;
            scrvScroll.Scrolled += ScrvScroll_Scrolled;
        }

        private void CompanyElementsListView_ChangeLabelDataEvent(string Text, bool IsCounty)
        {
            if (string.IsNullOrEmpty(Text))
                SetCountyCityLabels();
            else
            if (IsCounty)
                lblElementCounty.Text = Text;
            else
                lblElementCity.Text = Text;
        }

        private void SetCountyCityLabels()
        {
            lblElementCounty.Text = OrderedElements[0].county?.name;
            lblElementCity.Text = OrderedElements[0].city?.name;
        }

        private List<CompanyElementModel> SetAndOrderElementsToDisplay(CompanyElementRootModel companyElement)
        {
            try
            {
                var elements = companyElement.siblings;

                var FirstElementsToShow = elements.Where(sib => sib.city?.id == CompanyElement.element.city?.id && sib.id != companyElement.element.id).ToList();
                FirstElementsToShow = FirstElementsToShow.Concat(elements.Where(sib => sib.county?.id == CompanyElement.element.county?.id &&
                                                                                       !FirstElementsToShow.Select(fe => fe.id).Contains(sib.id) &&
                                                                                       sib.id != companyElement.element.id)
                                                          .OrderBy(sib => sib.county?.name)
                                                          .ThenBy(sib => sib.city?.name))
                                                          .ToList();



                var OtherElementsToShow = elements.Where(sib => !FirstElementsToShow.Select(fe => fe.id).Contains(sib.id))
                                                  .OrderBy(sib => sib.county?.name)
                                                  .ThenBy(sib => sib.city?.name)
                                                  .ToList();

                FirstElementsToShow.InsertRange(FirstElementsToShow.Count, OtherElementsToShow);
                return FirstElementsToShow;
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "private List<CompanyElementModel> SetAndOrderElementsToDisplay(CompanyElementRootModel companyElement)");
                return null;
            }
        }

        private async void CompanyElementsListView_ElementSelectedEvent(CompanyElementModel CompanyElement)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavanje", Acr.UserDialogs.MaskType.Clear);
            var companyElement =
                JsonConvert.DeserializeObject<CompanyElementRootModel>(await DataExchangeServices.GetCompanyElementData(CompanyElement.slug));

            await Navigation.PushModalAsync(new Company_ElementInfoPage(companyElement));
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private void ScrvScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            var ScrollDirection = e.ScrollY > LastScrollValue ? Views.CompanyElementsListView.ScrollDirection.Up : Views.CompanyElementsListView.ScrollDirection.Down;
            CompanyElementsListView.Scrolling(LastScrollValue = e.ScrollY, ScrollDirection);
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

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }
    }
}
