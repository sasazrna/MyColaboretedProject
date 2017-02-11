using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
    public class ComplaintModel
    {
        public static RootComplaintModel RefToAllComplaints;

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
        public List<ComplaintAttachmentModel> attachments { get; set; }
        public CompanyElementModel element { get; set; }
        public IList<ComplaintReplyModel> replies { get; set; }

        public class ComplaintReplyModel
        {
            public int id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public int complaint_id { get; set; }
            public int user_id { get; set; }
            public string reply { get; set; }
            public int by_contact { get; set; }
            public IList<ComplaintAttachmentModel> attachments { get; set; }
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

        public class WriteNewComplaintModel
        {
            public bool QuickComplaint { get; set; } = false;
            public int element_id { get; set; }
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
