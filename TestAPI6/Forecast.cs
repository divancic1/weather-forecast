using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;



namespace TestAPI6
{
    public class Forecast 
    {
        private readonly IMemoryCache _memoryCache;

        public Forecast(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public object OnGetCacheGetOrCreate(string cityName)
        {
            string? forecast = null;
            string cacheKey = cityName;
            if (_memoryCache.TryGetValue(cacheKey, out forecast))
            {
                var test = _memoryCache.Get(cacheKey);
                return test;
            }            
            else
            {
                forecast = ReadFromDb(cityName);
                var cacheEntryOptions =
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                _memoryCache.Set(cityName, forecast, cacheEntryOptions);
                var test = _memoryCache.Get(cacheKey);
                return test;
            }

            /*var cachedValue = await _memoryCache.GetOrCreateAsync(
                CacheKeys.Entry,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                    return Task.FromResult(DateTime.Now);
                });
            return cachedValue;*/
            /*forecast = ReadFromDb(cityName).Result;

            _memoryCache.Set(cacheKey, forecast,
                new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));*/
        }

        public string ReadFromDb(string cityName)
        {
            string connectionString = "Server=divancic;Database=Prognoza;Trusted_Connection=True;MultipleActiveResultSets=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("LatestCityForecast", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CityName", SqlDbType.VarChar, 24).Value = cityName;
                //SqlDataAdapter da = new SqlDataAdapter(command);
                //DataTable dt = new DataTable();
                string commandstring = "EXEC LatestCityForecast " + cityName;

                try
                {
                        if (command.Connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        var reader = command.ExecuteReader();
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        string JSONString = JsonConvert.SerializeObject(dataTable);
                        return JSONString;

                }
                catch (Exception ex)
                {
                   return Convert.ToString(ex);
                }
                finally { connection.Close(); }
            }
        }

        public string ReadAverage(string cityName, DateTime startDate, DateTime endDate)
        {
            string connectionString = "Server=divancic;Database=Prognoza;Trusted_Connection=True;MultipleActiveResultSets=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("GetTemperatureAverage", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CityName", SqlDbType.VarChar, 24).Value = cityName;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                string commandstring = "EXEC GetExtreme " + cityName + ", " + startDate + ", " + endDate;

                try
                {
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    var reader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    string JSONString = JsonConvert.SerializeObject(dataTable);
                    return JSONString;

                }
                catch (Exception ex)
                {
                    return Convert.ToString(ex);
                }
                finally { connection.Close(); }
            }
        }
        public string ReadExtreme(string cityName, DateTime startDate, DateTime endDate)
        {
            string connectionString = "Server=divancic;Database=Prognoza;Trusted_Connection=True;MultipleActiveResultSets=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("GetTemperatureExtreme", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@CityName", SqlDbType.VarChar, 24).Value = cityName;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
                string commandstring = "EXEC GetExtreme " + cityName + ", " + startDate + ", " + endDate;

                try
                {
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    var reader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    string JSONString = JsonConvert.SerializeObject(dataTable);
                    return JSONString;

                }
                catch (Exception ex)
                {
                    return Convert.ToString(ex);
                }
                finally { connection.Close(); }
            }
        }
    }
}
