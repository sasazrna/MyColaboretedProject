using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Models
{
    public class CompanyElementModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string phone { get; set;}
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
        public string root_business_full_name { get; set; }
        public int root_business_id { get; set; }
    }

    public class CompanyElementRootModel
    {
        public CompanyElementModel element { get; set; }
        public List<CompanyElementModel> siblings { get; set; }
    }
}
