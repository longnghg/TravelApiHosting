using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels
{
    public class Datum
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class GoogleMap
    {
        public List<Datum> data { get; set; }
    }

}
