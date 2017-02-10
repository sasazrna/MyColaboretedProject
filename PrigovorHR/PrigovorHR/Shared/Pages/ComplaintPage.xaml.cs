using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class ComplaintPage : ContentPage
    {
      private Controllers.TAPController TAPController;
       public ComplaintPage()
        {
            InitializeComponent();
        }

        public ComplaintPage(Models.ComplaintModel Complaint)
        {
            InitializeComponent();

            //btnAddResponse.Clicked += BtnAddResponse_Clicked;
            TAPController = new Controllers.TAPController(lytNumberOfResponses, NavigationBar.imgBack);
            TAPController.SingleTaped += TAPController_SingleTaped;

            lytAllResponses.Children.Clear();
            lytOriginalAndLastComplaintReply.Children.Clear();
            scrView.IsVisible = false;
            NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height;
            NavigationBar.lblNavigationTitle.Text = "Otvaram prigovor...";
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam vaš prigovor");

            Device.StartTimer(new TimeSpan(0, 0, 0, 1), () =>
               {
                   Display(Complaint);
                   return false;
               });

            fab2Btn.TranslateTo(0, 0, 100);
            fab2Btn.FadeTo(0, 100);
            fab2Btn.RotateTo(360, 0, Easing.SinOut);

            btnAddResponse.TranslateTo(0, 0, 100);
            btnAddResponse.FadeTo(0, 100);
            btnAddResponse.RotateTo(360, 100, Easing.SinOut);
            fab3Btn.Clicked += Fab3Btn_Clicked;
        }

        int clickedTotal = 1;
        private async void Fab3Btn_Clicked(object sender, EventArgs e)
        {
            clickedTotal++;
            if (clickedTotal % 2 == 0)
            {
              await  fab2Btn.TranslateTo(0, -200, 100);
                //fab2Btn.FadeTo(1, 100);
                //fab2Btn.RotateTo(360, 100, Easing.SinOut);
            await    btnAddResponse.TranslateTo(0, -100, 100);
                //btnAddResponse.FadeTo(1, 100);
                //btnAddResponse.RotateTo(360, 100, Easing.SinOut);
            }
            else
            {
               await fab2Btn.TranslateTo(0, 0, 100);
                //fab2Btn.FadeTo(0, 100);
               //fab2Btn.RotateTo(360, 0, Easing.SinOut);
          await      btnAddResponse.TranslateTo(0, 0, 100);
                //btnAddResponse.FadeTo(0, 100);
                //btnAddResponse.RotateTo(360, 100, Easing.SinOut);
            }
        }

        private async void Display(Models.ComplaintModel Complaint)
        {
            lytOriginalAndLastComplaintReply.Children.Add(new Views.ComplaintOriginalView(Complaint, null));

            var Result = await DataExchangeServices.GetCompanyElementData(Complaint.element.slug);

            if (Complaint.replies.Any())
            {
                foreach (var Reply in Complaint.replies.OrderByDescending(r => r.updated_at))
                    lytAllResponses.Children.Add(new Views.ComplaintReplyListView(Complaint, Reply));
            }
            else
            {
                lytNumberOfResponses.IsVisible = false;
                lytAllResponses.IsVisible = false;
            }

            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), () =>
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                scrView.IsVisible = true;
                NavigationBar.lblNavigationTitle.Text = "Prigovor.hr";
                NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height; return false;
            });

            NavigationBar.MinimumHeightRequest = Views.MainNavigationBar._RefToView.Height;
        }

        private async void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == lytNumberOfResponses)
            {
                lytAllResponses.IsVisible = !lytAllResponses.IsVisible;
                await imgArrow.RotateTo(lytAllResponses.IsVisible ? 0 : 180, 75);
            }
            else if (view == NavigationBar.imgBack)
            {
               await view.RotateTo(90, 100);
               await Navigation.PopModalAsync(true);
            }

            NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height;
        }

        protected override bool OnBackButtonPressed()
        {
            TAPController_SingleTaped(null, NavigationBar.imgBack);
            return true;
        }
        private void BtnAddResponse_Clicked(object sender, EventArgs e)
        {
            //Open frame to write response
        }
    }
}
