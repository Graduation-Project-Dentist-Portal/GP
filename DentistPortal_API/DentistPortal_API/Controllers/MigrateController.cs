using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace DentistPortal_API.Controllers
{
    public class MigrateController : Controller
    {
        [HttpPost]
        [Route("api/migrate-up")]
        public IActionResult Up()
        {
            var serviceProvider = CreateServices();
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabaseUp(scope.ServiceProvider);
                return Ok();
            }
        }

        [HttpPost]
        [Route("api/migrate-down")]
        public IActionResult Down()
        {
            var serviceProvider = CreateServices();
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabaseDown(scope.ServiceProvider);
                return Ok();
            }
        }

        private static IServiceProvider CreateServices()
        {
            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(config.GetConnectionString("DentistPortalDb"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.All())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabaseUp(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private static void UpdateDatabaseDown(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(0);
        }
    }
}
