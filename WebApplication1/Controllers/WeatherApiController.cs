using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Weather.DAL.Abstract;
using Weather.Entities;
using Weather.WebAPI.Hubs;
using Weather.WebAPI.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherApiController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWeatherRepository _weatherRepository;
        private readonly INotificationDispatcher _notificationDispatcher;

        public WeatherApiController(IMemoryCache cache, 
            IHttpClientFactory httpClientFactory, 
            IWeatherRepository weatherRepository ,
            INotificationDispatcher notificationDispatcher
            )
        {
            _memoryCache = cache;
            _httpClientFactory = httpClientFactory;
            _weatherRepository = weatherRepository;
            _notificationDispatcher = notificationDispatcher;
        }

        // GET api/WeatherApi
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "1.veri: value1", "2.veri: value2" };
        }

        [HttpGet("GetDailyRecords")]
        public async Task<IEnumerable<WeatherRecord>> GetDailyRecords()
        {
            IEnumerable<WeatherRecord> dbRecord = null ;
            try
            {
                dbRecord = await _weatherRepository.GetDailyRecords();
                return dbRecord; // "OK, " + dbRecord.ToList().Count + " adet sonuc..";
            }
            catch (Exception ex)
            {
                return dbRecord;
            }
        }

     
        [HttpGet("GetWeatherData/{CityName}")]
        public async Task<ActionResult<string>> Get(string cityName)
        {
            //TEST cagrisi..
            //await _notificationDispatcher.BroadCastMessage(cityName + " from server");

            try
            {
                string tempMessage = "";
                WeatherRecord dtoWeather = new WeatherRecord();
                string memCacheKey = cityName.Trim().ToLower();

                var cacheExpOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60), //60dk cachete tut
                    Size = 50
                };

                // 1.KONTROL: Cache e bak
                if (!_memoryCache.TryGetValue(memCacheKey, out WeatherRecord cachedWeatherRecord)) //cachte kayit yok ise
                {                
                    var dbRecord = await _weatherRepository.GetWeatherData(cityName);

                    //step-1: Database e bak.
                    if (dbRecord == null) 
                    {
                        var locationIQClient = _httpClientFactory.CreateClient("LocationIQClient"); //Get latitude longitude data
                        var darkSkyClient = _httpClientFactory.CreateClient("DarkSkyClient"); // Get temperature date
                        var lat = "";
                        var lon = "";
                        List<WeatherObject> resultLocationIQClient = null;
                        DarkSkyObject resultDarkSkyClient = null;

                        //Step-2: Lokasyon bazli lat ve lon bilgisini getir :
                        HttpResponseMessage responseLocationIQ = locationIQClient.GetAsync("/v1/search.php?key=a1779b7817b3b2&q=" + cityName + "&format=json").Result; // + @"&q=ankara&format=json";
                        if (responseLocationIQ.IsSuccessStatusCode)
                        {
                            resultLocationIQClient = await responseLocationIQ.Content.ReadAsAsync<List<WeatherObject>>();

                            lat = resultLocationIQClient[0].lat;
                            lon = resultLocationIQClient[0].lon;
                        }
                        else {
                            return "HATA: "+ cityName + " icin LocationIQ verisi mevcut degil !";
                        }

                        //Step-3:LocationIQ den gelen lat,lon bilgisi ile DarkSky bilgilerini al :
                        HttpResponseMessage responseDarkSky = darkSkyClient.GetAsync("/forecast/f3146e0fc78b4930d41a60703c08e2ae/" + lat + "," + lon).Result; // + @"&q=ankara&format=json";                
                        if (responseDarkSky.IsSuccessStatusCode)
                        {
                            dtoWeather.wrLocation = cityName;
                            resultDarkSkyClient = await responseDarkSky.Content.ReadAsAsync<DarkSkyObject>();

                            //Bugune ait min/max verisi
                            dtoWeather.wrDailyMinValue = resultDarkSkyClient.daily.data[0].apparentTemperatureMin;
                            dtoWeather.wrDailyMaxValue = resultDarkSkyClient.daily.data[0].apparentTemperatureMax;

                            //7 gunluk veri icinden min/max elde et
                            decimal[] tempWeeklyMinValList = new decimal[7];
                            decimal[] tempWeeklyMaxValList = new decimal[7];
                            for (int i = 0; i < 7; i++)
                            {
                                tempWeeklyMinValList[i] = resultDarkSkyClient.daily.data[i].apparentTemperatureMin;
                                tempWeeklyMaxValList[i] = resultDarkSkyClient.daily.data[i].apparentTemperatureMax;
                            }                            
                            dtoWeather.wrMinWeaklyValue = tempWeeklyMinValList.Min();
                            dtoWeather.wrMaxWeaklyValue = tempWeeklyMaxValList.Max();
                        }
                        else {
                            return "HATA: " + cityName + " icin DarySky verisi mevcut degil !";
                        }

                        //DB Insert 
                        var result = await _weatherRepository.AddWeatherData(dtoWeather);
                        if (result == false)
                        {
                            return "ERROR at INSERT !";
                        }

                        tempMessage = "Record From API";
                    } 
                    else // Kayit DB de mevcut ise bunu kullan..
                    {
                        dtoWeather = dbRecord;
                        tempMessage = "Record From Database" ;
                    }

                    //Yeni bir sorgu icin veriyi Cache e ekle ve tum Client lara duyur.
                    _memoryCache.Set(memCacheKey, dtoWeather, cacheExpOptions); // 60 min dogru eklediyor mu ?
                    await _notificationDispatcher.BroadCastMessage(
                        "Yeni Sorgu --------------------\n"
                        + "Lokasyon: " + dtoWeather.wrLocation.ToUpper() +"\n"
                        + "Gunluk ->  Max:" + dtoWeather.wrDailyMaxValue + ", Min:" + dtoWeather.wrDailyMinValue + "\n"
                        + "Haftalık ->  Max:" + dtoWeather.wrMaxWeaklyValue + ", Min:" + dtoWeather.wrMinWeaklyValue + "\n"
                        + "--------------------------------- \n"
                        );
                }
                else // Kayit Cache de mevcut ise bunu kullan..
                {
                    dtoWeather = cachedWeatherRecord;
                    tempMessage = "Record From Cache";
                }

                return tempMessage  + "-> " + cityName + " DailyMinTemp:" + dtoWeather.wrDailyMinValue + " DailyMaxTemp:" + dtoWeather.wrDailyMaxValue;                
            }
            catch (Exception ex)
            {
                return "ERROR at Try(GetWeatherData): " + ex.Message;
            }
        }
    }
}
