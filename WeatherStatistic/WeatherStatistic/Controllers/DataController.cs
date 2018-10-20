using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherStatistic.Core;
using WeatherStatistic.Enums;

namespace WeatherStatistic.Controllers
{
    public class DataController : Controller
    {
        private string _key = "atQg4cstPkhxNrFyh7ka7XuoIIXRH0CD7ezL8Ftf";
        private const int Period = 30;

        private string GetApiPath => $"https://api.nasa.gov/DONKI/GST?startDate={StartDate}&endDate={EndDate}&api_key={_key}";

        public string StartDate { get; set; } = new DateTime(2010, 1, 1).Date.ToString("yyyy-MM-dd");

        public string EndDate { get; set; } = DateTime.Now.Date.ToString("yyyy-MM-dd");
        
        public WeatherDataDbContext DbContext { get; set; }

        public DataController(WeatherDataDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task<JsonResult> GetData()
        {
            DbContext.WeatherData.RemoveRange(DbContext.WeatherData.Where(weatherData => weatherData.WeatherDataType == WeatherDataTypeEnum.Observed));
            DbContext.SaveChanges();

            HttpClient client = new HttpClient();
            dynamic data = JsonConvert.DeserializeObject(await client.GetStringAsync(GetApiPath));

            foreach (var gst in data)
            {
                foreach (JObject kp in gst.allKpIndex)
                {
                    if (!DateTime.TryParse(kp.Value<string>("observedTime"), out var date))
                        continue;

                    if (!int.TryParse(kp.Value<string>("kpIndex"), out var strength))
                        continue;

                    DbContext.WeatherData.Add(
                        new WeatherData()
                        {
                            Date = date,
                            Strength = strength,
                            WeatherDataType = WeatherDataTypeEnum.Observed
                        }
                    );
                }
            }

            DbContext.SaveChanges();

            return Json(data);
        }

        private string _predictionPath = "https://services.swpc.noaa.gov/products/noaa-planetary-k-index-forecast.json";

        public async Task<JsonResult> GetPredictionData()
        {
            DbContext.WeatherData.RemoveRange(DbContext.WeatherData.Where(weatherData => weatherData.WeatherDataType == WeatherDataTypeEnum.Predicted));
            DbContext.SaveChanges();

            HttpClient client = new HttpClient();
            dynamic data = JsonConvert.DeserializeObject(await client.GetStringAsync(_predictionPath));

            foreach (JArray gst in data)
            {
                if (!DateTime.TryParse(gst[0].ToString(), out var date))
                    continue;

                if (!int.TryParse(gst[1].ToString(), out var strength))
                    continue;

                DbContext.WeatherData.Add(
                    new WeatherData()
                    {
                        Date = date,
                        Strength = strength,
                        WeatherDataType = WeatherDataTypeEnum.Predicted
                    }
                );
            }

            DbContext.SaveChanges();

            return Json(data);
        }
    }
}