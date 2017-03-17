using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Controllers
{
//    public static List<>
//imaj listu svih , prati koji su u kojem i ne smiju biti isti u dva različita slučaja, skica i unsent, unsent mora imati network connection check
//
    public static class ComplaintDraftController
    {
        public enum Type { Draft=1,Unsent=2};

        public class Complaint:Models.ComplaintModel
        {

        }

        public class Reply:Models.ComplaintModel.ComplaintReplyModel
        {
            public  void Save()
            {
            //    this.
            }
        }

        //public static void Save(
    }
}
