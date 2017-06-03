﻿using Newtonsoft.Json;
using PrigovorHR.Shared.Controllers;
using PrigovorHR.Shared.Models;
using PrigovorHR.Shared.Views;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class APPMasterDetailPage : MasterDetailPage
    {
        public static Controllers.QRScannerController QRScannerController;
        public static APPMasterDetailPage ReferenceToView;
  //      public static Dictionary<string, List<ToolbarItem>> ToolBarForPage;

        public APPMasterDetailPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            NavigationPage.SetHasNavigationBar(this, false);
            ReferenceToView = this;
            MasterBehavior = MasterBehavior.Popover;

            //ToolBarForPage = new Dictionary<string, List<ToolbarItem>>()
            //     { {LandingPageWithLogin.AutomationId.ToString() ,
            //        new List<ToolbarItem>() { new ToolbarItem("tbiLogo", "LOGO.png", (()=> { }), ToolbarItemOrder.Primary, 0)  ,
            //        new ToolbarItem("tbiPrigovorText", "", (()=> { }), ToolbarItemOrder.Primary, 0) {Text="Prigovor.HR"},
            //        new ToolbarItem("tbiQRScanner", "QRIcon.png",  (()=> { tbiSearch_Clicked(null, null); }) , ToolbarItemOrder.Primary, 1),
            //        new ToolbarItem("tbiSearch", "SearchIcon.png", (()=> { tbiQRScanner_Clicked(null, null); }), ToolbarItemOrder.Primary, 2)} } };

            IsPresentedChanged += APPMasterDetailPage_IsPresentedChanged;
        }

        private void APPMasterDetailPage_IsPresentedChanged(object sender, EventArgs e)
        {
            if (IsPresented)
                ((APPMasterDetailPageMaster)Master).SetProfileImage();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as APPMasterDetailPageMenuItem;
            if (item == null)
                return;

            if (item.TargetType != null)
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                page.Title = item.Title;
                Detail = new NavigationPage(page) { BackgroundColor = Color.White };
                MasterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
            else
            {
                if (item.Id > 0 & item.Id < 5)
                {
                    ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(item.Id, true);
                    Detail = DetailNavigationPage;
                }

                if (item.Id == 6)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Confirm(
                           new Acr.UserDialogs.ConfirmConfig()
                           {
                               Title = "Odjava",
                               CancelText = "Odustani",
                               OkText = "Odjavi me",
                               Message = "Jeste li sigurni u odjavu iz aplikacije?",
                               OnAction = (Confirm) => { if (Confirm) LoginRegisterController.UserLogOut(); }
                           });

                    MasterPage.ListView.SelectedItem = null;
                }
                IsPresented = false;
            }
        }

        public async Task PushPage(Page Page)
        {
            await Navigation.PushAsync(Page, true);
        }

        public async void PopPage(bool PopPage)
        {
            await Navigation.PopAsync();
        }

        private async void tbiQRScanner_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(QRScannerController = new Controllers.QRScannerController(), true);
            QRScannerController.ScanCompletedEvent += QRScannerController__ScanCompletedEvent;
            await Task.Delay(300);
            QRScannerController.StartScan();
        }

        private async  void tbiSearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushPopupAsync(new CompanySearchPage());
        }

        private void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync(true);
                    if (result.Contains("prigovor.hr") | result.Contains("complio.me"))
                    {
                        //    result = result.Substring(0, result.LastIndexOf("/"));
                        result = result.Remove(0, result.LastIndexOf("/") + 1);
                        var Result = await DataExchangeServices.GetDirectTagResult(result);

                        if (Result.Contains("Error:"))
                        {
                            Controllers.VibrationController.Vibrate();
                            Acr.UserDialogs.UserDialogs.Instance.Alert("Došlo je do greške prilikom pretraživanja!" + Environment.NewLine + "Provjerite internet konekciju vašeg uređaja", "Prigovor.HR", "OK");
                            return;
                        }

                        var CompanyElement = JsonConvert.DeserializeObject<CompanyElementRootModel>(Result);

                        if (CompanyElement != null)
                            Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new Company_ElementInfoPage(CompanyElement, true), true));
                        else Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");
                    }
                    else Acr.UserDialogs.UserDialogs.Instance.Alert("Skenirani QR kod nije pronađen u bazi podataka!", "Nepostojeći QR kod", "OK");
                });
            }
            catch (Exception ex)
            {
                Controllers.ExceptionController.HandleException(ex, "private async void QRScannerController__ScanCompletedEvent(string result, bool isQRFormat)");
            }
        }
    }

}
