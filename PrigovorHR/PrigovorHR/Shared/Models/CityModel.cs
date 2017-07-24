using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Models
{
    public class CityModel
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int county_id { get; set; }
        public string name { get; set; }
        public string rank { get; set; }

        public static List<string> AllCities
        {
            get
            {
                {
                    var assembly = typeof(Pages.LandingPageWithLogin).GetTypeInfo().Assembly;
                    Stream stream = assembly.GetManifestResourceStream("Complio.Shared.Cities.txt");
                    var list = new List<string>();

                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                            list.Add(reader.ReadLine());
                    }
                    return list;
                }
            }
        }
    }
}