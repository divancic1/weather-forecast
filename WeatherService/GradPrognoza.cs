using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService
{
    public class GradPrognoza
    {
        public string cityName { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public int id { get; set; }
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int dt { get; set; }
    }
}
