using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WeatherService
{
    internal class WeatherInfo
    {
        public class Weather
        {
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
            public int id { get; set; }
        }

        public class Main
        {
            public double temp { get; set; }
            public double temp_feel { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public double pressure { get; set; }
            public double humidity { get; set; }
        }

        public class Root
        {
            public List<Weather> Weather { get; set; }
            public Main main { get; set; }
        }
    }
}
