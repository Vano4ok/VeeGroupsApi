using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VeeGroupsApi
{
    public class DesignTimeDbRepositoryContext : IDesignTimeDbContextFactory<DataBaseContext>
    {
        public DataBaseContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();

            var builder = new DbContextOptionsBuilder<DataBaseContext>();

            var connectionString = configuration.GetConnectionString("sqlConnection");

            builder.UseSqlServer(connectionString);

            return new DataBaseContext(builder.Options);
        }
    }
}
