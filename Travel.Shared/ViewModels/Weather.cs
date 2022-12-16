using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Alert
    {
        public string @event { get; set; }
        public string event_vi { get; set; }
        public string description { get; set; }
        public string description_vi { get; set; }
    }

    public class Current
    {
        public int dt { get; set; }
        public double temp { get; set; }
    }

    public class Daily
    {
        public int dt { get; set; }
        public Temp temp { get; set; }
        public int humidity { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double? rain { get; set; }
    }



    public class WeatherResponse
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public Current current { get; set; }
        public List<Daily> daily { get; set; }
        public List<Alert> alerts { get; set; }
    }

    public class Temp
    {
        public double day { get; set; }
        public double night { get; set; }
    }

}
