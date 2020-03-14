using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Weather.Entities;

namespace Weather.DAL.Concrete.EntityFramework
{
    public class WeatherContext:DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        { 

        }

        public DbSet<WeatherRecord> WeatherRecords { get; set; }
    }
}
