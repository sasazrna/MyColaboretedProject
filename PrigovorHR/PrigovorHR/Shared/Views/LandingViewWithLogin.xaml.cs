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
        // private CompanysStoreFoundListView CompanysFoundListView;
        //private ListOfComplaintsView_BasicUser ListOfComplaints;
        private bool DisplayClosedComplaints = false;

        public LandingViewWithLogin()
        {
            InitializeComponent();
            _TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");

            //When logged in, check if there is complaint that wasnt sent for some reason.
            object objuser;
            ComplaintModel.WriteNewComplaintModel WriteNewComplaintModel = null;
            if (Application.Current.Properties.TryGetValue("WriteComplaintAutoSave", out objuser))
                WriteNewComplaintModel = JsonConvert.DeserializeObject<ComplaintModel.WriteNewComplaintModel>((string)objuser);

            if (objuser != null && WriteNewComplaintModel.QuickComplaint)
            {
                var QuickComplaintPage = new QuickComplaintPage(null, WriteNewComplaintModel);
                QuickComplaintPage.ComplaintSentEvent += (() => { ListOfComplaintsView.LoadComplaints(); });
                Navigation.PushPopupAsync(QuickComplaintPage);
            }
        }
    }
}
