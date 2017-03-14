using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ComplaintCoversationHeaderView : ContentView
    {
        private Controllers.TAPController TAPController;
        public delegate void SendComplaintHandler();
        public event SendComplaintHandler SendComplaintEvent;
        public ComplaintCoversationHeaderView()
        {
            InitializeComponent();
            TAPController = new Controllers.TAPController(imgSave, imgSend);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            if (view == imgSave)
            {

            }
            else
            {
                SendComplaintEvent?.Invoke();
            }
        }
   
    public void SetHeaderInfo(string ContactPerson, string StoreName, bool Replying)
        {
            lblNameOfContactPerson.Text = ContactPerson;
            lblStoreName.Text = StoreName;
            imgSend.Text =  "\uf2c6";
            imgSave.Text = FontAwesomeLabel.Images.FADownload;

            lytSendResponse.IsVisible = Replying;
        }
    }
}
