using Newtonsoft.Json;
using Complio.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    //    public static List<>
    //imaj listu svih , prati koji su u kojem i ne smiju biti isti u dva različita slučaja, skica i unsent, unsent mora imati network connection check
    //
    public class ComplaintDraftController: ComplaintModel.DraftComplaintModel
    {
        private static List<Models.ComplaintModel.DraftComplaintModel> DraftComplaints = new List<Models.ComplaintModel.DraftComplaintModel>();

        public static List<Models.ComplaintModel.DraftComplaintModel> LoadDrafts()
        {
            object objDraftComplaints;
            List<ComplaintModel.DraftComplaintModel> DraftComplaintModel = new List<ComplaintModel.DraftComplaintModel>();
            if (Application.Current.Properties.TryGetValue("DraftComplaints", out objDraftComplaints))
                 DraftComplaintModel = JsonConvert.DeserializeObject<List<ComplaintModel.DraftComplaintModel>>((string)objDraftComplaints);

            StartUnsentComplaintControl();
            return DraftComplaintModel.Where(dc => dc.user_id == LoginRegisterController.LoggedUser.id.Value).ToList();
        }

        public static Guid SaveDraft(ComplaintModel Complaint, ComplaintModel.ComplaintReplyModel Reply, Guid DraftGuid, string ElementSlug, DraftType DraftType)
        {
            if (Reply != null)
            {
                if (Guid.Empty == DraftGuid)
                {
                    DraftComplaints.Add(new Models.ComplaintModel.DraftComplaintModel()
                    {
                        attachments = Reply.attachments,
                        complaint = Reply.reply,
                        complaint_id = Reply.complaint_id,
                        draftType = DraftType,
                        element_slug = ElementSlug,
                        user_id = Reply.user_id
                    });

                    SaveToDevice();
                    return DraftComplaints.Last().DraftGuid;
                }
                else
                {
                    var DraftComplaint = DraftComplaints.First(dc => dc.DraftGuid == DraftGuid);
                    DraftComplaint.attachments = Reply.attachments;
                    DraftComplaint.complaint = Reply.reply;
                    DraftComplaint.complaint_id = Reply.complaint_id;

                    SaveToDevice();
                    return DraftComplaint.DraftGuid;
                }
            }
            return new Guid();
        }

        private static void SaveToDevice()
        {
            Application.Current.Properties.Remove("DraftComplaints");
            Application.Current.Properties.Add("DraftComplaints", JsonConvert.SerializeObject(DraftComplaints));
            Application.Current.SavePropertiesAsync();
        }

        private static async void StartUnsentComplaintControl()
        {
            while(true)
            {
                var UnsentComplaints = DraftComplaints.Where(dc => dc.draftType == ComplaintModel.DraftComplaintModel.DraftType.Unsent);
                if (NetworkController.IsInternetAvailable & UnsentComplaints.Any())
                {
                    foreach(var UnsentComplaint in UnsentComplaints)
                    {
                        if(UnsentComplaint.complaint_id>0)
                        {
                            var attachment_ids = new List<int>();
                            foreach (var Attachment in UnsentComplaint.attachments)
                                attachment_ids.Add(await DataExchangeServices.SendReplyAttachment(Convert.FromBase64String(Attachment.attachment_data), Attachment.attachment_url));

                            if (attachment_ids.Any(aid => aid == 0))
                                return;

                            var result = await DataExchangeServices.SendReply(
                             JsonConvert.SerializeObject(new
                             {
                                 reply = UnsentComplaint.complaint,
                                 complaint_id = UnsentComplaint.complaint_id,
                                 attachment_ids = attachment_ids,
                                 close = false
                             }));
                        }
                    }
                }

                await Task.Delay(60000);
            }
        }
    }
}
