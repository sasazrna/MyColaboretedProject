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
using PrigovorHR.Shared.Models;

namespace PrigovorHR.Droid
{
    class AndroidServices
    {
        //[Service]
        //public class GetNewComplaintsBackgroundService : Service
        //{
        //    public override IBinder OnBind(Intent intent)
        //    {
        //        return null;
        //    }

        //    private string ComplaintLastEvent;
        //    public static bool IsRunning = false;
        //    private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
        //    List<Shared.Models.ComplaintModel> complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;
        //    private static List<int> FechedComplaintEvents = new List<int>();
        //    private Dictionary<bool, Dictionary<int, double>> RefreshValues = new Dictionary<bool, Dictionary<int, double>>();
        //    private bool HasNewResults, HasClosedComplaintEvent = false;

        //    //   [return: GeneratedEnum]
        //    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        //    {
        //        RefreshValues.Clear();
        //        RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 120 }, { 1, 120 }, { 2, 5 }, { 3, 5 }, { 4, 15 }, { 5, 30 } });
        //        RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 5 }, { 1, 5 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 2 }, { 5, 5 } });

        //        Task.Run(async () =>
        //        {
        //            try
        //            {
        //                while (true)
        //                {
        //                    await Task.Delay(Convert.ToInt32(RefreshValues[MainActivity.IsUserActive][Convert.ToInt32(DateTime.Now.Hour / 6D)] * 1000 * 60));

        //                    var complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

        //                    if (complaints != null && complaints.Any() && Shared.Controllers.NetworkController.IsInternetAvailable)
        //                    {
        //                        var ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
        //                        var NewComplaintReplys = JsonConvert.DeserializeObject<Shared.Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

        //                        //Check for new closed complaints
        //                        restart:
        //                        foreach (var Complaint in complaints)
        //                        {
        //                            var NewComplaintEvents = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == Complaint.id)?.complaint_events;

        //                            if (NewComplaintEvents != null && NewComplaintEvents.Count > 0 && Complaint.complaint_events?.Count > 0)
        //                                if (NewComplaintEvents.Count != Complaint.complaint_events.Count & !FechedComplaintEvents.Contains(NewComplaintEvents.Last().id))
        //                                {
        //                                    FechedComplaintEvents.Add(NewComplaintEvents.Last().id);
        //                                    var Complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
        //                                    var NewComplaintEvent = NewComplaintEvents.Last();

        //                                    ShowNotification(Complaint.id, Complaint.element.name,
        //                                        !string.IsNullOrEmpty(NewComplaintEvent.message) ? NewComplaintEvent.message :
        //                                        NewComplaintEvent.closed ? "Vaš prigovor je zatvoren" : "Vaš prigovor je otvoren");

        //                                    HasNewResults = true;
        //                                    HasClosedComplaintEvent = NewComplaintEvent.closed;

        //                                    Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
        //                                    Complaints.Add(Complaint);
        //                                    goto restart;
        //                                }
        //                        }

        //                        //Novi neproèitani prigovori.
        //                        foreach (var UnreadComplaint in NewComplaintReplys.user.unread_complaints)
        //                        {
        //                            var Complaint = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == UnreadComplaint.id);
        //                            var LastReply = Complaint?.replies?.Last();

        //                            if (Complaint == null)
        //                                continue;

        //                            if (FechedNewReplys.ContainsKey(Complaint.id) && FechedNewReplys.ContainsValue(LastReply.id))
        //                                continue;

        //                            ShowNotification(Complaint.id, UnreadComplaint.element.name, LastReply.reply);

        //                            if (!FechedNewReplys.ContainsKey(Complaint.id))
        //                                FechedNewReplys.Add(Complaint.id, LastReply.id);
        //                            else
        //                                FechedNewReplys[Complaint.id] = LastReply.id;

        //                            var Complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
        //                            Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
        //                            Complaints.Add(Complaint);

        //                            var UnreadComplaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
        //                            UnreadComplaints.Add(UnreadComplaint);
        //                            HasNewResults = true;
        //                        }

        //                        if (HasNewResults)
        //                        {
        //                            Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
        //                            Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
        //                            await Xamarin.Forms.Application.Current.SavePropertiesAsync();
        //                        }

        //                        if (MainActivity.IsUserActive & HasNewResults)
        //                        {
        //                            Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
        //                            Device.BeginInvokeOnMainThread(() =>
        //                            {
        //                                if (HasClosedComplaintEvent)
        //                                    Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(2, false);
        //                                else
        //                                    Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(1, false);
        //                            });
        //                            HasNewResults = false;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex) { Shared.Controllers.ExceptionController.HandleException(ex, "public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)"); }
        //        });

        //        return StartCommandResult.Sticky;
        //    }

        //    private void ShowNotification(int ComplaintId, string Title, string Text)
        //    {
        //        Intent resultIntent = new Intent(this, typeof(MainActivity)).AddFlags(ActivityFlags.BroughtToFront);
        //        resultIntent.PutExtra("ComplaintId", ComplaintId);
        //        PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, ComplaintId, resultIntent, PendingIntentFlags.UpdateCurrent);

        //        Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

        //        string longTextMessage = Text;
        //        textStyle.BigText(longTextMessage);

        //        var Notification = new Notification();
        //        if (MainActivity.SDKVersion >= 21)
        //        {
        //            Notification = new Notification.Builder(this)
        //             .SetContentTitle(Title)
        //             .SetContentText(longTextMessage)
        //             .SetVisibility(NotificationVisibility.Public)//if <5 remove set visibility
        //             .SetSmallIcon(Resource.Drawable.LOGO)
        //             .SetDefaults(NotificationDefaults.All)
        //             .SetStyle(textStyle)
        //             .SetPriority(7)
        //             .SetAutoCancel(true)
        //             .Build();
        //        }
        //        else
        //        {
        //            Notification = new Notification.Builder(this)
        //            .SetContentTitle(Title)
        //            .SetContentText(longTextMessage)
        //            .SetSmallIcon(Resource.Drawable.LOGO)
        //            .SetDefaults(NotificationDefaults.All)
        //            .SetStyle(textStyle)
        //            .SetPriority(7)
        //            .SetAutoCancel(true)
        //            .Build();
        //        }

        //        Notification.ContentIntent = resultPendingIntent;
        //        var NotificationMgr = NotificationManager.FromContext(this);
        //        NotificationMgr.Notify("Prigovor", ComplaintId, Notification);
        //    }
        //}



        [BroadcastReceiver]
        public class AlarmReceiver : BroadcastReceiver
        {
            private string ComplaintLastEvent;
            public static bool IsRunning = false;
            private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
            List<Shared.Models.ComplaintModel> complaints = new List<Shared.Models.ComplaintModel>();
            private static List<int> FechedComplaintEvents = new List<int>();
            private Dictionary<bool, Dictionary<int, double>> RefreshValues = new Dictionary<bool, Dictionary<int, double>>();
            private bool HasNewResults, HasClosedComplaintEvent = false;

            public class Models
            {
                public class ElementReviewModel
                {
                    public int id { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public int user_id { get; set; }
                    public int complaint_id { get; set; }
                    public int? satisfaction { get; set; }
                    public int? speed { get; set; }
                    public int? communication_level_user { get; set; }
                }

                public class ElementTypeModel
                {
                    public int id { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public string name { get; set; }
                    public string description { get; set; }
                }

                public class CountyModel
                {
                    public int id { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public string name { get; set; }
                }

                public class CityModel
                {
                    public int id { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public int county_id { get; set; }
                    public string name { get; set; }
                    public string rank { get; set; }
                }

                public class CompanyModel
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public string address { get; set; }
                    public string oib { get; set; }
                    public string description { get; set; }
                    public string complaint_received_message { get; set; }
                    public string slug { get; set; }
                    public int county_id { get; set; }
                    public CountyModel county { get; set; }
                    public CityModel city { get; set; }
                    public int city_id { get; set; }
                }

                public class CompanyElementModel
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public string latitude { get; set; }
                    public string longitude { get; set; }
                    public string phone { get; set; }
                    public int? parent_id { get; set; }
                    public string address { get; set; }
                    public string slug { get; set; }
                    public string working_hours { get; set; }
                    public string description { get; set; }
                    public string location_tag { get; set; }
                    public CompanyModel root_business { get; set; }
                    public List<CompanyElementModel> children { get; set; }
                    public int business_id { get; set; }
                    public string postcode { get; set; }
                    public CountyModel county { get; set; }
                    public int city_id { get; set; }
                    public CityModel city { get; set; }
                    public int? location_id { get; set; }
                    public int type_id { get; set; }
                    public ElementTypeModel type { get; set; }
                    public int root_business_id { get; set; }
                }

                public class User
                {
                    public User()
                    {

                    }

                    public string token { get; set; }

                    public bool isLogged { get; set; }

                    public bool isNotificationEnabled { get; set; }

                    public int? id { get; set; }
                    public string name { get; set; }
                    public string surname { get; set; }

                    public string name_surname { get { return name + " " + surname; } }
                    public string email { get; set; }
                    public string telephone { get; set; }
                    public string profileimage { get; set; }
                    public string password { get; set; }
                    public List<ComplaintModel> complaints { get; set; }
                    public List<ComplaintModel> unread_complaints { get; set; }
                    public List<ElementReviewModel> element_reviews { get; set; }
                }

                public class ComplaintModel
                {
                    public interface ComplaintModelInterface
                    {
                        int id { get; set; }
                        string created_at { get; set; }
                        string updated_at { get; set; }
                    }

                    public static RootComplaintModel RefToAllComplaints { get; set; } = null;
                    public enum TypeOfComplaint { Active = 1, Closed = 2, Draft = 3, Unsent = 4 }
                    public TypeOfComplaint typeOfComplaint { get; set; }

                    public int id { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public int user_id { get; set; }
                    public int element_id { get; set; }
                    public string complaint { get; set; }
                    public bool closed { get; set; }
                    public string problem_occurred { get; set; }//??
                    public string suggestion { get; set; }
                    public string last_event { get; set; }
                    public List<ComplaintEvent> complaint_events { get; set; }

                    public string latitude { get; set; }

                    public List<ComplaintAttachmentModel> attachments { get; set; }
                    public CompanyElementModel element { get; set; }
                    public List<ComplaintReplyModel> replies { get; set; }

                    public class ComplaintReplyModel
                    {
                        public int id { get; set; }
                        public string created_at { get; set; }
                        public string updated_at { get; set; }
                        public int complaint_id { get; set; }
                        public int user_id { get; set; }
                        public string reply { get; set; }
                        public int by_contact { get; set; }
                        public List<ComplaintAttachmentModel> attachments { get; set; }
                        public User user { get; set; }
                    }

                    public class ComplaintAttachmentModel
                    {
                        public int id { get; set; }
                        public string created_at { get; set; }
                        public string updated_at { get; set; }
                        public int complaint_reply_id { get; set; }
                        public int? user_id { get; set; }
                        public string attachment_url { get; set; }
                        public string attachment_extension { get; set; }
                        public string attachment_mime { get; set; }
                        public string attachment_data { get; set; }
                    }

                    public class ComplaintEvent
                    {
                        public int id { get; set; }
                        public string created_at { get; set; }
                        public string updated_at { get; set; }
                        public bool opened { get; set; }
                        public bool closed { get; set; }
                        public int user_id { get; set; }
                        public int complaint_id { get; set; }
                        public string message { get; set; }
                        public int? reply_id { get; set; }
                        public User user { get; set; }
                    }

                    public class DraftComplaintModel
                    {
                        public enum DraftType { AutoSave = 1, Draft = 2, Unsent = 3 };
                        public DraftType draftType { get; set; }

                        private Guid draftguid;
                        public Guid DraftGuid { get { return draftguid; } set { value = new Guid(); draftguid = value; } }
                        public bool QuickComplaint { get; set; } = false;
                        public int user_id { get; set; }
                        public int element_id { get; set; }
                        public int complaint_id { get; set; }
                        public string element_slug { get; set; }
                        public string ElementName { get; set; }
                        public string complaint { get; set; }
                        public string problem_occurred { get; set; }
                        public string suggestion { get; set; }
                        public string complaint_received_message { get; set; }
                        public List<ComplaintAttachmentModel> attachments { get; set; }
                    }

                    public static explicit operator ComplaintModel(Shared.Models.ComplaintModel v)
                    {
                        return (ComplaintModel)v;
                    }
                }

                public class RootComplaintModel
                {
                    public User user { get; set; }
                }
            }

            private void ShowNotification(int ComplaintId, string Title, string Text, Context context)
            {
                Intent resultIntent = new Intent(context, typeof(MainActivity)).AddFlags(ActivityFlags.BroughtToFront);
                resultIntent.PutExtra("ComplaintId", ComplaintId);
                PendingIntent resultPendingIntent = PendingIntent.GetActivity(context, ComplaintId, resultIntent, PendingIntentFlags.UpdateCurrent);

                Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

                string longTextMessage = Text;
                textStyle.BigText(longTextMessage);

                var Notification = new Notification();
                if (MainActivity.SDKVersion >= 21)
                {
                    Notification = new Notification.Builder(context)
                     .SetContentTitle(Title)
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
                    Notification = new Notification.Builder(context)
                    .SetContentTitle(Title)
                    .SetContentText(longTextMessage)
                    .SetSmallIcon(Resource.Drawable.LOGO)
                    .SetDefaults(NotificationDefaults.All)
                    .SetStyle(textStyle)
                    .SetPriority(7)
                    .SetAutoCancel(true)
                    .Build();
                }

                Notification.ContentIntent = resultPendingIntent;
                var NotificationMgr = NotificationManager.FromContext(context);
                NotificationMgr.Notify("Prigovor", ComplaintId, Notification);
            }

            public override void OnReceive(Context context, Intent intent)
            {
                //RefreshValues.Clear();
                //RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 120 }, { 1, 120 }, { 2, 5 }, { 3, 5 }, { 4, 15 }, { 5, 30 } });
                //RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 5 }, { 1, 5 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 2 }, { 5, 5 } });

                Task.Run(async () =>
                {
                    try
                    {
                        //while (true)
                        //{
                        // await Task.Delay(Convert.ToInt32(RefreshValues[MainActivity.IsUserActive][Convert.ToInt32(DateTime.Now.Hour / 6D)] * 1000 * 60));

                        List<Models.ComplaintModel> complaints = new List<Models.ComplaintModel>();
                        if (!MainActivity.IsUserActive)
                            complaints = Models.ComplaintModel.RefToAllComplaints?.user?.complaints;
                        else
                            foreach (var complaint in Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints)
                                complaints.Add((Models.ComplaintModel)complaint);
                     
                        if (complaints != null && complaints.Any()) //&/*& Shared.Controllers.NetworkController.IsInternetAvailable*/)
                        {
                            var ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
                            var NewComplaintReplys = JsonConvert.DeserializeObject<Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                            //Check for new closed complaints
                            restart:
                            foreach (var Complaint in complaints)
                            {
                                var NewComplaintEvents = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == Complaint.id)?.complaint_events;

                                if (NewComplaintEvents != null && NewComplaintEvents.Count > 0 && Complaint.complaint_events?.Count > 0)
                                    if (NewComplaintEvents.Count != Complaint.complaint_events.Count & !FechedComplaintEvents.Contains(NewComplaintEvents.Last().id))
                                    {
                                        FechedComplaintEvents.Add(NewComplaintEvents.Last().id);
                                        var Complaints = Models.ComplaintModel.RefToAllComplaints?.user.complaints;
                                        var NewComplaintEvent = NewComplaintEvents.Last();

                                      //  if(NewComplaintEvent.user_id != razlièit od logiranog
                                        ShowNotification(Complaint.id, Complaint.element.name,
                                            !string.IsNullOrEmpty(NewComplaintEvent.message) ? NewComplaintEvent.message :
                                            NewComplaintEvent.closed ? "Vaš prigovor je zatvoren" : "Vaš prigovor je otvoren", context);

                                        HasNewResults = true;
                                        HasClosedComplaintEvent = NewComplaintEvent.closed;

                                        Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                                        Complaints.Add(Complaint);
                                        goto restart;
                                    }
                            }

                            //Novi neproèitani prigovori.
                            foreach (var UnreadComplaint in NewComplaintReplys.user.unread_complaints)
                            {
                                var Complaint = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == UnreadComplaint.id);
                                var LastReply = Complaint?.replies?.Last();

                                if (Complaint == null)
                                    continue;

                                if (FechedNewReplys.ContainsKey(Complaint.id) && FechedNewReplys.ContainsValue(LastReply.id))
                                    continue;

                                ShowNotification(Complaint.id, UnreadComplaint.element.name, LastReply.reply, context);

                                if (!FechedNewReplys.ContainsKey(Complaint.id))
                                    FechedNewReplys.Add(Complaint.id, LastReply.id);
                                else
                                    FechedNewReplys[Complaint.id] = LastReply.id;

                                var Complaints = Models.ComplaintModel.RefToAllComplaints?.user.complaints;
                                Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                                Complaints.Add(Complaint);

                                var UnreadComplaints = Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
                                UnreadComplaints.Add(UnreadComplaint);
                                HasNewResults = true;
                            }

                            if (MainActivity.IsUserActive & HasNewResults)
                            {
                                Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
                                Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
                                await Xamarin.Forms.Application.Current.SavePropertiesAsync();

                                Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    if (HasClosedComplaintEvent)
                                        Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(2, false);
                                    else
                                        Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(1, false);
                                });
                                HasNewResults = false;
                            }
                        }
                    }
                    catch (Exception ex) { Shared.Controllers.ExceptionController.HandleException(ex, "public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)"); }
                    //});


















                    ////RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });
                    ////RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 0.25 }, { 1, 0.25 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 0.25 }, { 5, 0.25 } });
                    //Notification Notification = new Notification.Builder(context)
                    //                     .SetContentTitle("UnreadComplaint.element.name")
                    //                     .SetContentText("longTextMessage")
                    //                     //.SetVisibility(NotificationVisibility.Public)
                    //                     .SetSmallIcon(Resource.Drawable.LOGO)
                    //                     .SetDefaults(NotificationDefaults.All)
                    //                     .SetPriority(7)
                    //                     .SetAutoCancel(true)
                    //                     .Build();
                    ////   Notification.ContentIntent = resultPendingIntent;
                    //var NotificationMgr = NotificationManager.FromContext(context);
                    //NotificationMgr.Notify("Prigovor", 0, Notification);

                    //IsRunning = true;
                    //Task.Run(async () =>
                    //{
                    //    //while (true)
                    //    //{
                    //    //try
                    //    //{

                    //    var BackgroundService = new Intent(context, typeof(AndroidServices.GetNewComplaintsBackgroundService));
                    //    context.StartService(BackgroundService);

                    //    //complaints = Shared.Models.ComplaintModel.RefToAllComplaints?.user?.complaints;

                    //    // Notification = new Notification.Builder(context)
                    //    //        .SetContentTitle(complaints.Count.ToString())
                    //    //        .SetContentText("longTextMessage")
                    //    //        .SetVisibility(NotificationVisibility.Public)
                    //    //        .SetSmallIcon(Resource.Drawable.LOGO)
                    //    //        .SetDefaults(NotificationDefaults.All)
                    //    //        .SetPriority(7)
                    //    //        .SetAutoCancel(true)
                    //    //        .Build();
                    //    // //   Notification.ContentIntent = resultPendingIntent;
                    //    //  NotificationMgr = NotificationManager.FromContext(context);
                    //    // NotificationMgr.Notify("Prigovor", 0, Notification);

                    //    // if (complaints != null && complaints.Any(c => !c.closed) && Shared.Controllers.NetworkController.IsInternetAvailable)
                    //    //     {
                    //    //         ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
                    //    //         var NewComplaintReplys = JsonConvert.DeserializeObject<Shared.Models.RootComplaintModel>(await DataExchangeServices.CheckForNewReplys(ComplaintLastEvent));

                    //    //         //treba dodati zatvorene prigovore 
                    //    //         foreach (var UnreadComplaint in NewComplaintReplys.user.unread_complaints)
                    //    //         {
                    //    //             var Complaint = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == UnreadComplaint.id);
                    //    //             var LastReply = Complaint?.replies?.Last();

                    //    //             if (Complaint == null)
                    //    //                 continue;

                    //    //             if (FechedNewReplys.ContainsKey(Complaint.id) && FechedNewReplys.ContainsValue(LastReply.id))
                    //    //                 continue;

                    //    //             Intent resultIntent = new Intent(context, typeof(MainActivity)).AddFlags(ActivityFlags.BroughtToFront);
                    //    //             resultIntent.PutExtra("ComplaintId", Complaint.id);
                    //    //             PendingIntent resultPendingIntent = PendingIntent.GetActivity(context, Complaint.id, resultIntent, PendingIntentFlags.UpdateCurrent);

                    //    //             Notification.BigTextStyle textStyle = new Notification.BigTextStyle();

                    //    //             string longTextMessage = LastReply.reply;
                    //    //             textStyle.BigText(longTextMessage);

                    //    //              Notification = new Notification.Builder(context)
                    //    //                   .SetContentTitle(UnreadComplaint.element.name)
                    //    //                   .SetContentText(longTextMessage)
                    //    //                   .SetVisibility(NotificationVisibility.Public)
                    //    //                   .SetSmallIcon(Resource.Drawable.LOGO)
                    //    //                   .SetDefaults(NotificationDefaults.All)
                    //    //                   .SetStyle(textStyle)
                    //    //                   .SetPriority(7)
                    //    //                   .SetAutoCancel(true)
                    //    //                   .Build();
                    //    //             Notification.ContentIntent = resultPendingIntent;
                    //    //              NotificationMgr = NotificationManager.FromContext(context);
                    //    //             NotificationMgr.Notify("Prigovor", Complaint.id, Notification);

                    //    //             if (!FechedNewReplys.ContainsKey(Complaint.id))
                    //    //                 FechedNewReplys.Add(Complaint.id, LastReply.id);
                    //    //             else
                    //    //                 FechedNewReplys[Complaint.id] = LastReply.id;

                    //    //             var Complaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.complaints;
                    //    //             Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                    //    //             Complaints.Add(Complaint);

                    //    //             var UnreadComplaints = PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints?.user.unread_complaints;
                    //    //             UnreadComplaints.Add(UnreadComplaint);
                    //    //         }

                    //    //         Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
                    //    //         Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(PrigovorHR.Shared.Models.ComplaintModel.RefToAllComplaints));
                    //    //         await Xamarin.Forms.Application.Current.SavePropertiesAsync();

                    //    //         if (MainActivity.IsUserActive)
                    //    //             Shared.Views.ListOfComplaintsView_BasicUser.ReferenceToView.LoadComplaints();
                    //    //     }
                    //    //}
                    //    //catch (Exception ex)
                    //    //{
                    //    //    Shared.Controllers.ExceptionController.HandleException(ex, "public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)");
                    //    //}
                    //    // }
                    //}/*);*/
                });
            }
        }
    }
}
