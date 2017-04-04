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
        [Service]
        public class GetNewComplaintsBackgroundService : Service
        {
            public override IBinder OnBind(Intent intent)
            {
                return null;
            }

            private string ComplaintLastEvent;
            public static bool IsRunning = false;
            private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
            List<Shared.Models.ComplaintModel> complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

            [return: GeneratedEnum]
            public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
            {
                Task.Run(async () =>
                {
                    var complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

                    if (complaints != null && complaints.Any(c => !c.closed) && Shared.Controllers.NetworkController.IsInternetAvailable)
                    {
                        var ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
                        var NewComplaintReplys = JsonConvert.DeserializeObject<Shared.Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                        //treba dodati zatvorene prigovore 
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
                            PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, Complaint.id, resultIntent, PendingIntentFlags.UpdateCurrent);

                            Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

                            string longTextMessage = LastReply.reply;
                            textStyle.BigText(longTextMessage);

                            var Notification = new Notification();
                            if (MainActivity.SDKVersion >= 21)
                            {
                                Notification = new Notification.Builder(this)
                                 .SetContentTitle(UnreadComplaint.element.name)
                                 .SetContentText(longTextMessage)
                                 .SetVisibility(NotificationVisibility.Public)//if <5 remove set visibility
                                 .SetSmallIcon(Resource.Drawable.LOGO)
                                 .SetDefaults(NotificationDefaults.All)
                                 .SetStyle(textStyle)
                                 .SetPriority(7)
                                 .SetAutoCancel(true)
                                 .Build();
                            }
                            else
                            {
                                Notification = new Notification.Builder(this)
                                   .SetContentTitle(UnreadComplaint.element.name)
                                   .SetContentText(longTextMessage)
                                   .SetSmallIcon(Resource.Drawable.LOGO)
                                   .SetDefaults(NotificationDefaults.All)
                                   .SetStyle(textStyle)
                                   .SetPriority(7)
                                   .SetAutoCancel(true)
                                   .Build();
                            }

                            Notification.ContentIntent = resultPendingIntent;
                            var NotificationMgr = NotificationManager.FromContext(this);
                            NotificationMgr.Notify("Prigovor", Complaint.id, Notification);

                            if (!FechedNewReplys.ContainsKey(Complaint.id))
                                FechedNewReplys.Add(Complaint.id, LastReply.id);
                            else
                                FechedNewReplys[Complaint.id] = LastReply.id;

                            var Complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
                            Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                            Complaints.Add(Complaint);

                            var UnreadComplaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
                            UnreadComplaints.Add(UnreadComplaint);
                        }

                        Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
                        Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
                        await Xamarin.Forms.Application.Current.SavePropertiesAsync();

                        if (MainActivity.IsUserActive)
                            Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                    }
                });
                return StartCommandResult.Sticky;
            }
        }



        //[BroadcastReceiver]
        //public class AlarmReceiver : BroadcastReceiver
        //{
        //    private string ComplaintLastEvent;
        //    public static bool IsRunning = false;
        //    private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
        //    List<Shared.Models.ComplaintModel> complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

        //    public override void OnReceive(Context context, Intent intent)
        //    {

        //        //RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });
        //        //RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });
        //        Notification Notification = new Notification.Builder(context)
        //                             .SetContentTitle("UnreadComplaint.element.name")
        //                             .SetContentText("longTextMessage")
        //                             //.SetVisibility(NotificationVisibility.Public)
        //                             .SetSmallIcon(Resource.Drawable.LOGO)
        //                             .SetDefaults(NotificationDefaults.All)
        //                             .SetPriority(7)
        //                             .SetAutoCancel(true)
        //                             .Build();
        //     //   Notification.ContentIntent = resultPendingIntent;
        //        var NotificationMgr = NotificationManager.FromContext(context);
        //        NotificationMgr.Notify("Prigovor", 0, Notification);
               
        //        IsRunning = true;
        //        Task.Run(async () =>
        //        {
        //            //while (true)
        //            //{
        //            //try
        //            //{

        //           var BackgroundService = new Intent(context, typeof(AndroidServices.GetNewComplaintsBackgroundService));
        //           context.StartService(BackgroundService);

        //           //complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

        //           // Notification = new Notification.Builder(context)
        //           //        .SetContentTitle(complaints.Count.ToString())
        //           //        .SetContentText("longTextMessage")
        //           //        .SetVisibility(NotificationVisibility.Public)
        //           //        .SetSmallIcon(Resource.Drawable.LOGO)
        //           //        .SetDefaults(NotificationDefaults.All)
        //           //        .SetPriority(7)
        //           //        .SetAutoCancel(true)
        //           //        .Build();
        //           // //   Notification.ContentIntent = resultPendingIntent;
        //           //  NotificationMgr = NotificationManager.FromContext(context);
        //           // NotificationMgr.Notify("Prigovor", 0, Notification);

        //           // if (complaints != null && complaints.Any(c => !c.closed) && Shared.Controllers.NetworkController.IsInternetAvailable)
        //           //     {
        //           //         ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
        //           //         var NewComplaintReplys = JsonConvert.DeserializeObject<Shared.Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

        //           //         //treba dodati zatvorene prigovore 
        //           //         foreach (var UnreadComplaint in NewComplaintReplys.user.unread_complaints)
        //           //         {
        //           //             var Complaint = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == UnreadComplaint.id);
        //           //             var LastReply = Complaint?.replies?.Last();

        //           //             if (Complaint == null)
        //           //                 continue;

        //           //             if (FechedNewReplys.ContainsKey(Complaint.id) && FechedNewReplys.ContainsValue(LastReply.id))
        //           //                 continue;

        //           //             Intent resultIntent = new Intent(context, typeof(MainActivity)).AddFlags(ActivityFlags.BroughtToFront);
        //           //             resultIntent.PutExtra("ComplaintId", Complaint.id);
        //           //             PendingIntent resultPendingIntent = PendingIntent.GetActivity(context, Complaint.id, resultIntent, PendingIntentFlags.UpdateCurrent);

        //           //             Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

        //           //             string longTextMessage = LastReply.reply;
        //           //             textStyle.BigText(longTextMessage);

        //           //              Notification = new Notification.Builder(context)
        //           //                   .SetContentTitle(UnreadComplaint.element.name)
        //           //                   .SetContentText(longTextMessage)
        //           //                   .SetVisibility(NotificationVisibility.Public)
        //           //                   .SetSmallIcon(Resource.Drawable.LOGO)
        //           //                   .SetDefaults(NotificationDefaults.All)
        //           //                   .SetStyle(textStyle)
        //           //                   .SetPriority(7)
        //           //                   .SetAutoCancel(true)
        //           //                   .Build();
        //           //             Notification.ContentIntent = resultPendingIntent;
        //           //              NotificationMgr = NotificationManager.FromContext(context);
        //           //             NotificationMgr.Notify("Prigovor", Complaint.id, Notification);

        //           //             if (!FechedNewReplys.ContainsKey(Complaint.id))
        //           //                 FechedNewReplys.Add(Complaint.id, LastReply.id);
        //           //             else
        //           //                 FechedNewReplys[Complaint.id] = LastReply.id;

        //           //             var Complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
        //           //             Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
        //           //             Complaints.Add(Complaint);

        //           //             var UnreadComplaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
        //           //             UnreadComplaints.Add(UnreadComplaint);
        //           //         }

        //           //         Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
        //           //         Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
        //           //         await Xamarin.Forms.Application.Current.SavePropertiesAsync();

        //           //         if (MainActivity.IsUserActive)
        //           //             Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
        //           //     }
        //            //}
        //            //catch (Exception ex)
        //            //{
        //            //    Shared.Controllers.ExceptionController.HandleException(ex, "public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)");
        //            //}
        //            // }
        //        });
        //    }
        //}
    }
}
