using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace TestAPI6.Controllers
{
    

    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Forecast _forecast;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, Forecast forecast, IMemoryCache memoryCache)
        {
            _logger = logger;
            _forecast = forecast;
            _memoryCache = memoryCache;
        }
        /*
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/

        [HttpGet("{cityName}", Name = "GetForecast")]
        public ActionResult<object> GetForecast(string cityName)
        {
            return _forecast.OnGetCacheGetOrCreate(cityName);  
            
        }

        [HttpGet("{cityName}", Name = "GetAverageTemperature")]
        public ActionResult<object> GetAverage(string cityName, DateTime startDate, DateTime endDate)
        {
            return _forecast.ReadAverage(cityName, startDate, endDate);

        }

        [HttpGet("{cityName}", Name = "GetExtremeTemperature")]
        public ActionResult<object> GetExtreme(string cityName, DateTime startDate, DateTime endDate)
        {
            return _forecast.ReadExtreme(cityName, startDate, endDate);

        }

        [HttpGet(Name = "GetCache")]
        public List<string> GetCache()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(_memoryCache) as ICollection;
            var items = new List<string>();
            if (collection != null)
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
            return items;
        }



    }    
}
