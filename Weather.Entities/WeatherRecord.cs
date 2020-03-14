using System;
using System.ComponentModel.DataAnnotations;

namespace Weather.Entities
{
    public class WeatherRecord
    {
        [Key]
        public string wrLocation { get; set; }
        public decimal wrDailyMinValue { get; set; }
        public decimal wrDailyMaxValue { get; set; }
        public decimal wrMinWeaklyValue { get; set; }
        public decimal wrMaxWeaklyValue { get; set; }
        public DateTime wrInsertDate { get; set; }

    }
}
