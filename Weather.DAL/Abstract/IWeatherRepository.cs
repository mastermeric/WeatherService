using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Weather.Entities;

namespace Weather.DAL.Abstract
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<WeatherRecord>> GetDailyRecords();
        Task<WeatherRecord> GetWeatherData(string location);
        Task<bool> AddWeatherData(WeatherRecord record);
    }
}
