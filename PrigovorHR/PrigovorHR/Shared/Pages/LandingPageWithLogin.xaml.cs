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
            //When logged in, check if there is complaint that wasnt sent for some reason.
            LoadComplaintAutoSaveData();

            ReferenceToView = this;
            FirstTimeLoginView.SearchIconClickedEvent += () => Navigation.PushPopupAsync(new CompanySearchPage(), true);
            AutomationId = "LandingPageWithLogin";
        }

        private async void LoadComplaintAutoSaveData()
        {
            while (ComplaintModel.RefToAllComplaints == null)
                await Task.Delay(200);

            FirstTimeLoginView.IsVisible = !ComplaintModel.RefToAllComplaints.user.complaints.Any();
            ListOfComplaintsView.IsVisible = !FirstTimeLoginView.IsVisible;

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

                        await Navigation.PushAsync(NewComplaintReplyPage, true);
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
