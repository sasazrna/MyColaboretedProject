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
using PrigovorHR.Shared.Controllers;

namespace PrigovorHR.Shared.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingViewWithLogin : ContentView
    {
        public static LandingViewWithLogin ReferenceToView;
        private Controllers.TAPController TAPController;
        public LandingViewWithLogin()
        {
            InitializeComponent();
            _TopNavigationBar.ChangeNavigationTitle("Prigovor.hr");

            //When logged in, check if there is complaint that wasnt sent for some reason.
            LoadComplaintAutoSaveData();

            Device.StartTimer(new TimeSpan(0, 0, 30), () =>
            {
                //  Acr.Notifications.CrossNotifications.Current.Send("Prigovor.hr - Nova poruka od: " + "Vivas caffe bar Vrbani", "Poštovani, zahvaljujemo na vašoj prijavi te se nadamo.....", )
                return true;
            });
            //MenuBack.Text = '\uf060'.ToString();
            //MenuBack.TextColor = Color.Gray;
            //MenuBack.FontSize = 35;
            //MenuStack.HeightRequest = ((Page)Parent).HeightRequest;
            //TAPController = new Controllers.TAPController(MenuBack);
            //  TAPController.SingleTaped += (string id, View view) => { HideMenu(); };
            ReferenceToView = this;
        }

        //public async void ShowMenu()=> await MenuStack.TranslateTo(0, 0, 250);
        //public async void HideMenu() => await MenuStack.TranslateTo(-450, 0, 250);

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

                    if (WriteNewComplaintModel.complaint_id > 0)
                    {
                        var NewComplaintReplyPage =
                            new NewComplaintReplyPage(ComplaintModel.RefToAllComplaints.user.complaints.Single(c => c.id == WriteNewComplaintModel.complaint_id),
                                                      WriteNewComplaintModel);

                        await Navigation.PushModalAsync(NewComplaintReplyPage);
                        NewComplaintReplyPage.ReplaySentEvent += (int id) => { Navigation.PopModalAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                    else
                    {
                        var NewComplaintPage = new NewComplaintPage(null, WriteNewComplaintModel);

                        await Navigation.PushModalAsync(NewComplaintPage);
                        NewComplaintPage.ComplaintSentEvent += (int id) => { Navigation.PopModalAsync(true); ListOfComplaintsView.LoadComplaints(); };
                    }
                }
            }
        }
    }
}
