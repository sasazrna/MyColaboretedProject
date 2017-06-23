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
using Complio.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Media;
using Android.Graphics;
using Newtonsoft.Json;
using Complio.Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;

namespace Complio.Droid
{
    class AndroidServices
    {
        [BroadcastReceiver]
        public class AlarmReceiver : BroadcastReceiver
        {
            public static AlarmReceiver ReferenceToReciver;
            private string ComplaintLastEvent;
            public static bool IsRunning = false;
            private static Dictionary<int, int> FechedNewReplys = new Dictionary<int, int>();
            private static List<int> FechedComplaintEvents = new List<int>();
            private Dictionary<bool, Dictionary<int, double>> RefreshValues = new Dictionary<bool, Dictionary<int, double>>();
            private bool HasNewResults, HasClosedComplaintEvent = false;
            private string[] ServiceAddresses = new string[] { "https://prigovor.hr/api/", "http://138.68.85.217/api/" };
            private string ServiceAddress;
            public static RootComplaintModel RootComplaint = new RootComplaintModel();

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

            public static void UpdateComplaintsListFromPortable(string JSON, string UserToken)
            {
                RootComplaint = JsonConvert.DeserializeObject<RootComplaintModel>(JSON);
                RootComplaint.user.token = UserToken;
                WriteComplaintsDataToStorage(JSON, UserToken);
            }

            private static void WriteComplaintsDataToStorage(string JSON, string UserToken)
            {
                if (!Directory.Exists(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/"))
                    Directory.CreateDirectory(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/");

                StreamWriter SW = new StreamWriter(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/tempcomplaintsdata.co", false);
                SW.WriteLine(JSON);
                SW.Write(UserToken);
                SW.Close();
            }
        
            private static void ReadComplaintsDataFromStorage()
            {
                try
                {
                    StreamReader SR = new StreamReader(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/tempcomplaintsdata.co");
                    RootComplaint = JsonConvert.DeserializeObject<RootComplaintModel>(SR.ReadLine());
                    RootComplaint.user.token = SR.ReadLine();
                    SR.Close();
                    File.Delete(Android.OS.Environment.ExternalStorageDirectory.Path + "/PrigovorHR/tempcomplaintsdata.co");
                }
                catch { }
            }

            public override void OnReceive(Context context, Intent intent)
            {
                //ShowNotification(0, "IsRunning", IsRunning.ToString(), context);
                if (IsRunning) return;
                RefreshValues.Clear();
                RefreshValues.Add(false, new Dictionary<int, double>() { { 0, 120 }, { 1, 120 }, { 2, 5 }, { 3, 5 }, { 4, 15 }, { 5, 30 } });
                RefreshValues.Add(true, new Dictionary<int, double>() { { 0, 5 }, { 1, 5 }, { 2, 0.25 }, { 3, 0.25 }, { 4, 2 }, { 5, 5 } });

                Task.Run(async () =>
                {
                    try
                    {
                        IsRunning = true;

                        if (RootComplaint.user == null)
                            ReadComplaintsDataFromStorage();

                        var complaints = RootComplaint?.user?.complaints;

                        if (complaints != null && complaints.Any()) //&/*& Shared.Controllers.NetworkController.IsInternetAvailable*/)
                        {
                            var ComplaintLastEvent = complaints.Select(c => DateTime.Parse(c.updated_at)).Max().ToString("dd.MM.yyyy. H:mm");
                            var GetDataResult = await GetData(ComplaintLastEvent);
                           // ShowNotification(0, "Data", GetDataResult, context);

                            var NewComplaintReplys = JsonConvert.DeserializeObject<RootComplaintModel>(GetDataResult);

                            //Check for new closed complaints
                            restart:
                            foreach (var Complaint in complaints)
                            {
                                var NewComplaintEvents = NewComplaintReplys.user.complaints.FirstOrDefault(c => c.id == Complaint.id)?.complaint_events;

                                if (NewComplaintEvents != null && NewComplaintEvents.Count() > 0 && Complaint.complaint_events?.Count > 0)
                                    if (NewComplaintEvents.Count != Complaint.complaint_events.Count & !FechedComplaintEvents.Contains(NewComplaintEvents.Last().id))
                                    {
                                        FechedComplaintEvents.Add(NewComplaintEvents.Last().id);
                                        // var Complaints = ComplaintModel.RefToAllComplaints?.user.complaints;
                                        var NewComplaintEvent = NewComplaintEvents.Last();

                                        if (NewComplaintEvent.user_id != RootComplaint.user.id)
                                        {
                                            ShowNotification(Complaint.id, Complaint.element.name,
                                                !string.IsNullOrEmpty(NewComplaintEvent.message) ? NewComplaintEvent.message :
                                                NewComplaintEvent.closed ? "Vaš prigovor je zatvoren" : "Vaš prigovor je otvoren", context);
                                        }
                                        HasNewResults = true;
                                        HasClosedComplaintEvent = NewComplaintEvent.closed;

                                        complaints.Remove(complaints.Single(c => c.id == Complaint.id));
                                        complaints.Add(Complaint);
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

                                var Complaints = RootComplaint.user.complaints;
                                Complaints.Remove(Complaints.Single(c => c.id == Complaint.id));
                                Complaints.Add(Complaint);

                                var UnreadComplaints = RootComplaint.user.unread_complaints;
                                UnreadComplaints.Add(UnreadComplaint);
                                HasNewResults = true;
                            }

                            if (MainActivity.IsUserActive & HasNewResults)
                            {
                                Xamarin.Forms.Application.Current.Properties.Remove("AllComplaints");
                                Xamarin.Forms.Application.Current.Properties.Add("AllComplaints", JsonConvert.SerializeObject(RootComplaint));
                                await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                                WriteComplaintsDataToStorage(JsonConvert.SerializeObject(RootComplaint), RootComplaint.user.token);
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

                        await Task.Delay(Convert.ToInt32(RefreshValues[MainActivity.IsUserActive][Convert.ToInt32(DateTime.Now.Hour / 6D)] * 1000 * 60));
                        IsRunning = false;
                    }
                    catch (Exception ex)
                    {
                        IsRunning = false;
                        Shared.Controllers.ExceptionController.HandleException(ex, "public override void OnReceive(Context context, Intent intent)");
                    }
                });
            }

            internal async Task<string> GetData(string value = null)
            {
#if DEBUG
                var serviceAddress = ServiceAddresses[1];

#else
                 ServiceAddress = ServiceAddresses[0];

#endif

                var byteArray = System.Text.Encoding.UTF8.GetBytes("forge:123123p");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    using (var client = new HttpClient())
                    {
                        HttpContent content = new StringContent(value, System.Text.Encoding.UTF8, "application/json");
                        var response = new HttpResponseMessage();
                        client.DefaultRequestHeaders.Authorization = header;
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        header = new AuthenticationHeaderValue("Bearer", RootComplaint.user.token);
                        client.DefaultRequestHeaders.Authorization = header;
                        var fullAddress = serviceAddress + "svi-moji-prigovori-nakon-datuma/" + value;

                        response = await client.GetAsync(fullAddress);

                        if (response.IsSuccessStatusCode)
                        {
                            try { RootComplaint.user.token = response.Headers.GetValues("Authorization").Last(); } catch { }
                            return await response.Content.ReadAsStringAsync();

                        }
                        else return "";
                    }
                }
                catch (Exception ex)
                {
                    return "Error:";
                }
            }
        }
    }
}
