using BusifyAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BusifyAPI.Configurations
{
    public static class DbConfig
    {
        public static void ConfigureDb(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<BusifyDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly("BusifyAPI.Data")
                ));

            builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        }
    }

    public interface IDatabaseInitializer
    {
        void Initialize();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly BusifyDbContext _context;

        public DatabaseInitializer(BusifyDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();
        }
    }
}