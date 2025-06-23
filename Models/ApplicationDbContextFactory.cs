using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Eagles_Portal.Models
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            try
            {
                // Build configuration with explicit path
                var basePath = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(basePath, "appsettings.json");

                Console.WriteLine($"Looking for config at: {configPath}");
                Console.WriteLine($"File exists: {File.Exists(configPath)}");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                Console.WriteLine($"Using connection string: {connectionString}");
                optionsBuilder.UseNpgsql(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateDbContext: {ex.Message}");

                // Fallback connection string
                var fallbackConnectionString = "Host=localhost;Port=5432;Database=eagles_portal;Username=postgres;Password=1515";
                Console.WriteLine($"Using fallback connection string: {fallbackConnectionString}");
                optionsBuilder.UseNpgsql(fallbackConnectionString);
            }

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}