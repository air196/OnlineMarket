using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OnlineMarket.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketDbContext>
    {
        public MarketDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().LastIndexOf('\\')) + "\\OnlineMarket";

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var builder = new DbContextOptionsBuilder<MarketDbContext>();

            var connectionString = configuration.GetConnectionString("shop");

            builder.UseNpgsql(connectionString);

            return new MarketDbContext(builder.Options, configuration);
        }
    }
}
