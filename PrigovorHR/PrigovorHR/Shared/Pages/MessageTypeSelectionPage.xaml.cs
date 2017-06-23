using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageTypeSelectionPage : ContentPage
    {
        private Models.CompanyElementModel CompanyElement;
        public MessageTypeSelectionPage()
        {
            InitializeComponent();
        }

        public MessageTypeSelectionPage(Models.CompanyElementModel element)
        {
            InitializeComponent();
            CompanyElement = element;
        }

        private async void btn_Clicked(object sender, EventArgs e)
        {
            var NewComplaintPage = new NewComplaintPage(CompanyElement, Convert.ToInt32(((Button)sender).AutomationId));
            await Navigation.PushAsync(NewComplaintPage, true);
            NewComplaintPage.ToolbarItems.Add(new ToolbarItem("tbiSendComplaint", "awsomeSend2.png", (() => { NewComplaintPage.SendComplaint(); }), ToolbarItemOrder.Primary, 10));
            NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopAsync(true); Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints(); };
        }
    }
}