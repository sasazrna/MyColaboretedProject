using FAB.Forms;
using Newtonsoft.Json;
using PrigovorHR.Shared.Views;
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
        private int _clickedTotal=1;

        public ComplaintPage()
        {
            InitializeComponent();
        }

        public ComplaintPage(Models.ComplaintModel Complaint)
        {
            InitializeComponent();
            labela.Text = FontAwesomeLabel.Images.FAAdjust;
            labela.TextColor = Color.Teal;

            labela2.Text = FontAwesomeLabel.Images.FAArrowCircleOLeft;
            labela2.TextColor = Color.Teal;

            //  lytNumberOfResponses.Text = "+3";

            //btnAddResponse.Clicked += BtnAddResponse_Clicked;
            TAPController = new Controllers.TAPController(NavigationBar.imgBack);
            TAPController.SingleTaped += TAPController_SingleTaped;

            lytAllResponses.Children.Clear();
            lytOriginalAndLastComplaintReply.Children.Clear();
            scrView.IsVisible = false;
            NavigationBar.HeightRequest = Views.MainNavigationBar._RefToView.Height;
            NavigationBar.lblNavigationTitle.Text = "Otvaram prigovor...";

            Btn3.TranslateTo(0, 0, 100);
            Btn3.RotateTo(360, 0);

            Btn2.TranslateTo(0, 0, 100);
            Btn2.RotateTo(360, 100);
            Btn1.Clicked += FabButton_Click;
            Btn2.Clicked += FabButton_Click;
            Btn3.Clicked += FabButton_Click;
            scrView.Scrolled += ScrView_Scrolled;
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam vaš prigovor");

            Device.StartTimer(new TimeSpan(0, 0, 0, 1), () =>
               {
                   DisplayData(Complaint);
                   return false;
               });
        }


        private void ScrView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (_clickedTotal % 2 == 0)
                FabButton_Click(Btn1, new EventArgs());
        }

        private async void FabButton_Click(object sender, EventArgs e)
        {
            var ClickedButton = ((FloatingActionButton)sender);

            if (ClickedButton == Btn1)
            {
                _clickedTotal += 1;
                if (_clickedTotal % 2 == 0)
                {
                    await Btn2.TranslateTo(0, -60, 70);
                    await Btn2.RotateTo(360, 70);

                    await Btn3.TranslateTo(0, -120, 70);
                    await Btn3.RotateTo(360, 70);
                }
                else
                {
                    await Btn3.TranslateTo(0, 0, 70);
                    await Btn3.RotateTo(360, 70);

                    await Btn2.TranslateTo(0, 0, 70);
                    await Btn2.RotateTo(360, 70);
                }
            }
            else if (ClickedButton == Btn2)
                await Navigation.PushModalAsync(new NewComplaintResponse());
            else
                await Navigation.PushModalAsync(new CloseComplaintPage());
        }

        private void DisplayData(Models.ComplaintModel Complaint)
        {
            lytOriginalAndLastComplaintReply.Children.Add(new Views.ComplaintOriginalView(Complaint, null));

         //   var Result = await DataExchangeServices.GetCompanyElementData(Complaint.element.slug);

            if (Complaint.replies.Any())
            {
                foreach (var Reply in Complaint.replies.OrderByDescending(r => DateTime.Parse( r.updated_at)))
                    lytAllResponses.Children.Add(new Views.ComplaintReplyListView(Complaint, Reply));

                lblNumberOfResponses.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                lblNumberOfResponses.Text = "+" + Convert.ToString(lytAllResponses.Children.Count + 1);
            }
            else
            {
                lblNumberOfResponses.IsVisible = false;
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
            if (view == lblNumberOfResponses)
            {
                lytAllResponses.IsVisible = !lytAllResponses.IsVisible;

                //if (lytAllResponses.IsVisible)
                //{
                //    lblNumberOfResponses.Text = "-";
                //    lblNumberOfResponses.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                //}
                //else
                //{
                //    lblNumberOfResponses.Text = Convert.ToString(lytAllResponses.Children.Count + 1);
                //    lblNumberOfResponses.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                //}
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
