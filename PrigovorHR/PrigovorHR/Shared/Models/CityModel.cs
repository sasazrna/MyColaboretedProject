﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
