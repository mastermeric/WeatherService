using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.WebAPI.Models
{
    public class DTOWeather
    {
        public double DailyMaxTemperature { get; set; }
        public double DailyMinTemperature { get; set; }
        public double WeeklyMaxTemperature { get; set; }
        public double WeeklyMinTemperature { get; set; }
    }
}
