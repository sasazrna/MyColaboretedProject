using FAB.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using PrigovorHR.Shared.Controllers;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComplaintPage : ContentPage
    {
        private int _clickedTotal=1;
        private Models.ComplaintModel Complaint;

        public ComplaintPage()
        {
            InitializeComponent();
        }

        public ComplaintPage(Models.ComplaintModel complaint)
        {
            InitializeComponent();
            Complaint = complaint;
            lytAllResponses.Children.Clear();
            lytOriginalComplaint.Children.Clear();
            scrView.IsVisible = false;
            btnCloseComplaint.IsVisible = !complaint.closed;

            ComplaintCoversationHeaderView.SetHeaderInfo(Complaint.replies.Any() ? 
                Complaint.replies.LastOrDefault(r=>r.user_id != Controllers.LoginRegisterController.LoggedUser.id)?.user?.name_surname ?? "nepoznato" : 
                "nepoznato", Complaint.element.name);

            NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
            NavigationBar.lblNavigationTitle.Text = "Otvaram prigovor...";

            btnCloseComplaint.TranslateTo(0, 0, 100);

            btnReplay.TranslateTo(0, 0, 100);
            Btn1.Clicked += FabButton_Click;
            btnReplay.Clicked += FabButton_Click;
            btnCloseComplaint.Clicked += FabButton_Click;
            scrView.Scrolled += ScrView_Scrolled;
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Učitavam vaš prigovor");

            Device.StartTimer(new TimeSpan(0, 0, 0, 0,500), () =>
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
                    await btnReplay.TranslateTo(0, -60, 70);
                    await btnCloseComplaint.TranslateTo(0, -120, 70);
                }
                else
                {
                    await btnCloseComplaint.TranslateTo(0, 0, 70);
                    await btnReplay.TranslateTo(0, 0, 70);
                }
            }
            else if (ClickedButton == btnReplay)
            {
                var NewComplaintReplyPage = new NewComplaintReplyPage(Complaint);
                await Navigation.PushModalAsync(NewComplaintReplyPage);
                NewComplaintReplyPage.ReplaySentEvent += (int id) => {  Navigation.PopModalAsync(true); Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); };
            }
            else
            {
                var CloseComplaintPage = new CloseComplaintPage(Complaint);
                await Navigation.PushModalAsync(CloseComplaintPage);
                CloseComplaintPage.ComplaintClosed += (int id) => {  Navigation.PopModalAsync(true); Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); };
            }
        }

        private void DisplayData(Models.ComplaintModel Complaint)
        {
            lytOriginalComplaint.Children.Add(new Views.ComplaintOriginalView(Complaint, null));

            if (Complaint.replies.Any())
            {
                foreach (var Reply in Complaint.replies.OrderByDescending(r => DateTime.Parse( r.updated_at)))
                    lytAllResponses.Children.Add(new Views.ComplaintReplyListView(Complaint, Reply));

                //prikaži zadnju poruku u listi replyova
                //if(Complaint.closed)
                //var LastMessage = Complaint.complaint_events?.LastOrDefault(ce => ce.closed).message
                //    lytAllResponses.Children.Add(new Views.ComplaintReplyListView(Complaint, Reply));

                lblNumberOfResponses.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                lblNumberOfResponses.Text = "+" + Convert.ToString(lytAllResponses.Children.Count);
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
                NavigationBar.HeightRequest = Views.MainNavigationBar.ReferenceToView.Height; return false;
            });

            NavigationBar.MinimumHeightRequest = Views.MainNavigationBar.ReferenceToView.Height;
            NavigationBar.BackButtonPressedEvent += NavigationBar_BackButtonPressedEvent;
        }

        private async void NavigationBar_BackButtonPressedEvent()
        {
            await Navigation.PopModalAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            NavigationBar.InitBackButtonPressed();
            return true;
        }
        private void BtnAddResponse_Clicked(object sender, EventArgs e)
        {
            //Open frame to write response
        }
    }
}
