using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Models;
using PrigovorHR.Shared.Views;
using Xamarin.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.Xaml;
using PrigovorHR.Shared.Controllers;
using System.IO;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPageWithLogin : ContentPage
    {
        public static LandingPageWithLogin ReferenceToView;
        private TAPController TAPController;

        public FirstTimeLoginView firstTimeLoginView { get { return FirstTimeLoginView; } set { FirstTimeLoginView = value; } }
        public ListOfComplaintsView_BasicUser listOfComplaintsView { get { return ListOfComplaintsView; } set { ListOfComplaintsView = value; } }
       // public ComplaintListTabView complaintListTabView { get { return ComplaintListTabView; } set { ComplaintListTabView = value; } }


        public LandingPageWithLogin()
        {
            InitializeComponent();
            //TopNavigationBar_OpenCloseMenuEvent(false);
           //NavigationPage.SetHasNavigationBar(this, false);

            //TopNavigationBar.OpenCloseMenuEvent += TopNavigationBar_OpenCloseMenuEvent;

            //imgBack.Text = FontAwesomeLabel.Images.FAArrowLeft;
            //imgBack.TextColor = Color.Gray;

          //  TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");

            //When logged in, check if there is complaint that wasnt sent for some reason.
            LoadComplaintAutoSaveData();

            ReferenceToView = this;
            //TAPController = new TAPController(lblContact, lblLogOut, lblProfile, imgBack, imgProfilePicture);
            //TAPController.SingleTaped += TAPController_SingleTaped;
            FirstTimeLoginView.SearchIconClickedEvent += () => Navigation.PushPopupAsync(new CompanySearchPage(), true);
            AutomationId = "LandingPageWithLogin";
            //ProfilePage.ProfileUpdatedEvent += () => imgProfilePicture.Source =
            // ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(LoginRegisterController.LoggedUser.profileimage)));
        }

        //private async void TAPController_SingleTaped(string viewId, View view)
        //{
        //    if (view == lblContact)
        //        await Navigation.PushModalAsync(new ContactUsPage(), true);
        //    else if (view == lblProfile | view == imgProfilePicture)
        //        await Navigation.PushModalAsync(new ProfilePage(), true);
        //    else if (view == imgBack)
        //        TopNavigationBar_OpenCloseMenuEvent(lytContent.IsVisible);
        //    else if (view == lblLogOut)
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance.Confirm(
        //            new Acr.UserDialogs.ConfirmConfig()
        //            {
        //                Title = "Odjava",
        //                CancelText = "Odustani",
        //                OkText = "Odjavi me",
        //                Message = "Jeste li sigurni u odjavu iz aplikacije?",
        //                OnAction = (Confirm) => { if (Confirm) LoginRegisterController.UserLogOut(); }
        //            });
        //    }
        //}

        //private async void TopNavigationBar_OpenCloseMenuEvent(bool IsMenuOpen)
        //{
        //    lytContent.IsVisible = !IsMenuOpen;

        //    if (!string.IsNullOrEmpty(LoginRegisterController.LoggedUser.profileimage))
        //    {
        //        var ProfileImageByte = Convert.FromBase64String(LoginRegisterController.LoggedUser.profileimage);
        //        imgProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(ProfileImageByte));
        //    }
        //    else imgProfilePicture.Source = "person.png";

        //    imgProfilePicture.TranslateTo(0, IsMenuOpen ? 30 : 0, 100);
        //    imgProfilePicture.FadeTo(IsMenuOpen ? 1 : 0, 100);
            
        //    lblProfile.TranslateTo(0, IsMenuOpen ? 110 : 0, 100);
        //    lblProfile.FadeTo(IsMenuOpen ? 1 : 0, 100);

        //    lblContact.TranslateTo(0, IsMenuOpen ? 150 : 0, 100);
        //    lblContact.FadeTo(IsMenuOpen ? 1 : 0, 100);

        //    lblLogOut.TranslateTo(0, IsMenuOpen ? 190 : 0, 100);
        //    lblLogOut.FadeTo(IsMenuOpen ? 1 : 0, 100);

        //    imgBack.TranslateTo(0, 0, 100);
        //    imgBack.FadeTo(IsMenuOpen ? 1 : 0, 100);

        //    await Task.Delay(100);
        //    imgProfilePicture.IsVisible = lblProfile.IsVisible = lblContact.IsVisible = lblLogOut.IsVisible = lblLogOut.IsVisible = imgBack.IsVisible = IsMenuOpen;
        //}

        private async void LoadComplaintAutoSaveData()
        {
            while (ComplaintModel.RefToAllComplaints == null)
                await Task.Delay(200);

            FirstTimeLoginView.IsVisible = !ComplaintModel.RefToAllComplaints.user.complaints.Any();
            ListOfComplaintsView.IsVisible = !FirstTimeLoginView.IsVisible;
        //    ComplaintListTabView.IsVisible = !FirstTimeLoginView.IsVisible;

            object objuser;
            ComplaintModel.DraftComplaintModel WriteNewComplaintModel = null;
            if (Application.Current.Properties.TryGetValue("WriteComplaintAutoSave", out objuser))
                WriteNewComplaintModel = JsonConvert.DeserializeObject<ComplaintModel.DraftComplaintModel>((string)objuser);

            if (objuser != null)
            {
                if (WriteNewComplaintModel.QuickComplaint)
                {
                    var QuickComplaintPage = new QuickComplaintPage(null, WriteNewComplaintModel);
                    QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView.LoadComplaints(); });
                    await Navigation.PushPopupAsync(QuickComplaintPage);
                }
                else
                {   if (WriteNewComplaintModel.complaint_id > 0)
                    {
                        var NewComplaintReplyPage =
                            new NewComplaintReplyPage(ComplaintModel.RefToAllComplaints.user.complaints.Single(c => c.id == WriteNewComplaintModel.complaint_id),
                                                      WriteNewComplaintModel);

                        await Navigation.PushAsync(new NavigationPage(NewComplaintReplyPage) { BackgroundColor = Color.White });
                        NewComplaintReplyPage.ReplaySentEvent += (int id) => { Navigation.PopAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                    else
                    {
                        var NewComplaintPage = new NewComplaintPage(null, WriteNewComplaintModel);

                        await Navigation.PushAsync(NewComplaintPage);
                        NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                }
            }
        }
    }
}
