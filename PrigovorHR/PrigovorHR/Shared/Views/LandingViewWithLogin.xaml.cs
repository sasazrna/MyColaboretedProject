using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrigovorHR.Shared.Models;
using PrigovorHR.Shared.Pages;
using Xamarin.Forms;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingViewWithLogin : ContentView
    {
        public LandingViewWithLogin()
        {
            InitializeComponent();
            _TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");

            //When logged in, check if there is complaint that wasnt sent for some reason.
            LoadComplaintAutoSaveData();
        }

        private async void LoadComplaintAutoSaveData()
        {
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
                {
                    while (ComplaintModel.RefToAllComplaints == null)
                        await Task.Delay(200);

                    var NewComplaintReplyPage =
                        new NewComplaintReplyPage(ComplaintModel.RefToAllComplaints.user.complaints.Single(c => c.id == WriteNewComplaintModel.complaint_id),
                                                  WriteNewComplaintModel);

                   await Navigation.PushModalAsync(NewComplaintReplyPage);
                    NewComplaintReplyPage.ReplaySentEvent += (int id) => { Navigation.PopModalAsync(true); ListOfComplaintsView.LoadComplaints(); };
                }
            }

        }
    }
}
