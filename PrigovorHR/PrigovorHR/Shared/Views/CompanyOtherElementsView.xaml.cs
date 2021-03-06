﻿using Newtonsoft.Json;
using Complio.Shared.Models;
using Complio.Shared.Pages;
using System;
using System.Collections.Generic;

using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyOtherElementsView
    {
        private CompanyElementRootModel CompanyElement;
        private double LastScrollValue = 0;
        private List<CompanyElementModel> OrderedElements = new List<CompanyElementModel>();

        public CompanyOtherElementsView()
        { }
        public CompanyOtherElementsView(CompanyElementRootModel companyElement)
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
                 //   OnBackButtonPressed();
                }
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "public OtherCompanyStoresPage(CompanyElementRootModel companyElement)");
            }
     //       NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
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

            await Navigation.PushAsync(new Company_ElementInfoPage(companyElement, false));
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

        private void ScrvScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            var ScrollDirection = e.ScrollY > LastScrollValue ? Views.CompanyElementsListView.ScrollDirection.Up : Views.CompanyElementsListView.ScrollDirection.Down;
            CompanyElementsListView.Scrolling(LastScrollValue = e.ScrollY, ScrollDirection);
        }
    }
}
