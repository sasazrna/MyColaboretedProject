using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complio.Shared.Models
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
}
