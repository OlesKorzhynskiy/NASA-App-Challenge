using Microsoft.EntityFrameworkCore;

namespace WeatherStatistic.Core
{
    public class WeatherDataDbContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }

        public WeatherDataDbContext(DbContextOptions options)
            : base(options) { }
    }
}