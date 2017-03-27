using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PrigovorHR.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Media;
using Android.Graphics;
using Newtonsoft.Json;

namespace PrigovorHR.Droid
{
    class AndroidServices
    {
        [Service(IsolatedProcess = false)]
        public class GetNewComplaintsBackgroundService : Service
        {
            private string ComplaintLastEvent;
            public static bool IsRunning = false;
            private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
            private Dictionary<bool, Dictionary<int, double>> RefreshValues = new Dictionary<bool, Dictionary<int, double>>();
            private static List<Intent> ListOfIntent=new List<Intent>();

            public override IBinder OnBind(Intent intent)
            {

                return null;
            }

            public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
            {
                RefreshValues.Clear();
                RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });
                RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });

                IsRunning = true;
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            var TimeZone = Convert.ToInt32(Convert.ToDouble(DateTime.Now.Hour) / 6D);
                            var RefreshTime = (int)(RefreshValues[MainActivity.IsUserActive][TimeZone] * 1000 * 60);

                            await Task.Delay(RefreshTime);
                            var complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;
                           
                            if (complaints != null && complaints.Any(c => !c.closed))
                            {
                                ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.last_event)).Max().ToString("dd.MM.yyyy. H:mm");
                                var NewComplaintReplys = JsonConvert.DeserializeObject<Shared.Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                                foreach (var UnreadComplaint in NewComplaintReplys.user.unread_complaints)
                                {
                                    var Complaint = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == UnreadComplaint.id);
                                    var LastReply = Complaint?.replies?.Last();

                                    if (Complaint == null)
                                        continue;

                                    if (FechedNewReplys.ContainsKey(Complaint.id) && FechedNewReplys.ContainsValue(LastReply.id))
                                        continue;

                                    Intent resultIntent = new Intent(this, typeof(MainActivity)).AddFlags(ActivityFlags.BroughtToFront);
                                    resultIntent.PutExtra("ComplaintId", Complaint.id);
                                    ListOfIntent.Add(resultIntent);
                                    PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, Complaint.id, ListOfIntent.Last(), PendingIntentFlags.UpdateCurrent);

                                    Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

                                    string longTextMessage = LastReply.reply;
                                    textStyle.BigText(longTextMessage);

                                    Notification Notification = new Notification.Builder(this)
                                          .SetContentTitle(UnreadComplaint.element.name)
                                          .SetContentText(longTextMessage)
                                          .SetVisibility(NotificationVisibility.Public)
                                          .SetSmallIcon(Resource.Drawable.LOGO)
                                          .SetDefaults(NotificationDefaults.All)
                                          .SetStyle(textStyle)
                                          .SetPriority(7)
                                          .Build();
                                    Notification.ContentIntent = resultPendingIntent;
                                    var NotificationManager = (NotificationManager)GetSystemService(NotificationService);
                                    NotificationManager.Notify("Prigovor", Complaint.id, Notification);

                                    if (!FechedNewReplys.ContainsKey(Complaint.id))
                                        FechedNewReplys.Add(Complaint.id, LastReply.id);
                                    else
                                        FechedNewReplys[Complaint.id] = LastReply.id;

                                    var Complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
                                    Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                                    Complaints.Add(Complaint);

                                    var UnreadComplaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
                                    UnreadComplaints.Add(UnreadComplaint);

                                    Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
                                    Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
                                    await Xamarin.Forms.Application.Current.SavePropertiesAsync();

                                    if (MainActivity.IsUserActive)
                                        Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Shared.Controllers.ExceptionController.HandleException(ex, "public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)"); }
                        }
                });

                return StartCommandResult.Sticky;
            }
        }
    }
}