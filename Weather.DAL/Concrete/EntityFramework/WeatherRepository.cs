using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Weather.DAL.Abstract;
using Weather.Entities;

namespace Weather.DAL.Concrete.EntityFramework
{
    public class WeatherRepository:IWeatherRepository
    {
        private readonly WeatherContext _weatherContext;
        private readonly HttpClient _httpClient;        

        public WeatherRepository(WeatherContext weatherContext)
        {
            _weatherContext = weatherContext;
        }

        public async Task<IEnumerable<WeatherRecord>> GetDailyRecords()
        {
            IEnumerable<WeatherRecord> result = null;

            try
            {
                string sql = "SELECT wrLocation, wrDailyMinValue, wrDailyMaxValue, wrMinWeaklyValue, wrMaxWeaklyValue, wrInsertDate FROM weatherRecords ";
                sql += "WHERE wrInsertDate = cast(GETDATE() AS DATE)"; 

                //result = await Task<IEnumerable<WeatherRecord>>.Run(() => _weatherContext.WeatherRecords.FromSql(sql.ToString()).ToList());
                result = await Task<IEnumerable<WeatherRecord>>.Run(() => _weatherContext.WeatherRecords.FromSql(sql));
            }
            catch (Exception ex)
            {
                //loglama ifadeleri burada. Ayrica bir Business layer icinde Loglama
            }

            return result;
        }

        public async Task<WeatherRecord> GetWeatherData(string location)
        {
            WeatherRecord result = null;
            //StringBuilder sql = new StringBuilder();
            //sql.Append("SELECT wrLocation, wrDailyMinValue, wrDailyMaxValue, wrMinWeaklyValue, wrMaxWeaklyValue, wrInsertDate FROM weatherRecords");
            //sql.Append("WHERE cast(wrInsertDate AS DATE) = cast(GETDATE() AS DATE)"); // get todays recordsonly           
            try
            {
                //result = await _weatherContext.WeatherRecords.FirstOrDefaultAsync(rec => rec.wrLocation.Trim() == location);
                result = await _weatherContext.WeatherRecords.FirstOrDefaultAsync(rec => rec.wrLocation == location & rec.wrInsertDate == DateTime.Now.Date);
                //result = await _weatherContext.WeatherRecords.FindAsync(location);
            }
            catch (Exception ex)
            {
                //loglama ifadeleri burada. Ayrica bir Business layer icinde Loglama
            }

            return result;
        }

        public async Task<bool> AddWeatherData(WeatherRecord record)
        {
            bool result = true;
            try
            {
                record.wrInsertDate = DateTime.Now;
                await _weatherContext.AddAsync<WeatherRecord>(record);
                await _weatherContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result = false;
                //loglama ifadeleri burada. Ayrica bir Business layer icinde Loglama
            }

            return result;
        }

    }
}
