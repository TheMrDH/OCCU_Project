using Microsoft.EntityFrameworkCore;
using OCCU_Project.Models;

namespace OCCU_Project.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite("Data Source=./Data/OCCU_DB.sqlite");
        }

        public DbSet<StatusObject> StatusObjects { get; set; }
        public DbSet<DataContentObject> DataContentObjects { get; set; }
    }
}
