using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace WeatherService
{
    
    public partial class WeatherService : ServiceBase
    {

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Prognoza"].ConnectionString;
        string apikey = ConfigurationManager.AppSettings["APIkey"];
        Timer timer = new Timer();
        private int eventId = 1;
        public WeatherService(string[] args)
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
        }

        public WeatherService()
        {
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("\nWeatherService started: "+DateTime.Now+"\n");
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Prognoza"].ConnectionString;
            string apikey = ConfigurationManager.AppSettings["APIkey"];
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Prognoza"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("CityListGet", connection);
                command.CommandType = CommandType.StoredProcedure;               
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                City city = new City();
                                city.Name = reader.GetString(0);
                                city.Id = reader.GetInt32(1);
                                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}&units=metric", city.Name, apikey);
                                var json = webClient.DownloadString(url);
                                WeatherInfo.Root Info = JsonConvert.DeserializeObject<WeatherInfo.Root>(json);                                
                                GradPrognoza gradPrognoza = new GradPrognoza();
                                var weather = Info.Weather[0];
                                gradPrognoza.main = weather.main;
                                gradPrognoza.icon = weather.icon;
                                gradPrognoza.temp = Info.main.temp;
                                gradPrognoza.temp_max = Info.main.temp_max;
                                gradPrognoza.temp_min = Info.main.temp_min;
                                PrognozaFill(gradPrognoza, city);
                                Console.WriteLine(city.Name+" forcast entered into DB.");
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    Console.WriteLine("Error fetching data");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }
        protected override void OnStop()
        {
            eventLog1.WriteEntry("WeatherService stopped.");
        }

        public void PrognozaFill(GradPrognoza gradPrognoza, City city)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                bool result = false;
                SqlCommand scCommand = new SqlCommand("PrognozaFill", connection);
                scCommand.CommandType = CommandType.StoredProcedure;
                scCommand.Parameters.Add("@Main", SqlDbType.VarChar, 50).Value = gradPrognoza.main;
                scCommand.Parameters.Add("@Temp", SqlDbType.Decimal).Value = gradPrognoza.temp;
                scCommand.Parameters.Add("@Temp_max", SqlDbType.Decimal).Value = gradPrognoza.temp_max;
                scCommand.Parameters.Add("@Temp_min", SqlDbType.Decimal).Value = gradPrognoza.temp_min;
                scCommand.Parameters.Add("@Icon", SqlDbType.NVarChar, 50).Value = gradPrognoza.icon;
                scCommand.Parameters.Add("@CityId ", SqlDbType.Int).Value = city.Id;
                try
                {
                    if (scCommand.Connection.State == ConnectionState.Closed)
                    {
                        scCommand.Connection.Open();
                    }
                    scCommand.ExecuteNonQuery();
                    eventLog1.WriteEntry("WeatherService: added entry for "+city.Name+" at "+DateTime.Now+"\n");
                }
                catch (Exception)
                {

                }
                finally
                {
                    scCommand.Connection.Close();
                }
            }
        }
    }
}
