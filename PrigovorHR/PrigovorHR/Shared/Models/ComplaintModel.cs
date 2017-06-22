using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace PrigovorHR.Shared.Models
{
    public class ComplaintModel
    {
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
        public int? messageType { get; set; }

        public static Dictionary<int, Tuple<string, Color>> messageTypeNamesDefinitions =
            new Dictionary<int, Tuple<string, Color>>() { { 0, new Tuple<string, Color>("Prigovor", Color.Red) } ,
                                                          { 2, new Tuple<string, Color>("Pohvala", Color.Green) },
                                                          { 3, new Tuple<string, Color>("Prijedlog", Color.Yellow) },
                                                          { 4, new Tuple<string, Color>("Upit", Color.Blue) } };

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
            public enum DraftType { AutoSave=1, Draft=2, Unsent=3 };
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
    }

    public class RootComplaintModel
    {
        public  User user { get; set; }
    }
}
