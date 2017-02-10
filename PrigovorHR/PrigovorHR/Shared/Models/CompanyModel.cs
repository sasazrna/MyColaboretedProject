using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Models
{
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
}
