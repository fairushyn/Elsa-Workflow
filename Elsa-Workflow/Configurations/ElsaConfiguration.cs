using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Extensions;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa_Workflow.Extensions;
using Elsa_Workflow.Handlers;
using Elsa_Workflow.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa_Workflow.Configurations
{
    public class ElsaConfiguration
    {
        public static void ConfigureElsa(IServiceCollection services, IConfiguration configuration)
        {
            services.AddServerSideBlazor();

            services
                // Add Elsa services
                .AddElsa(
                    elsa =>
                    {
                        // Configure Elsa to use MongoDB provider
                        elsa.AddMongoDbStores(configuration, databaseName: "UserRegistration", connectionStringName: "MongoDB");
                    })
                
                // Add Elsa dashboard services
                .AddElsaDashboard()
                
                // Add the activities that will be used
                .AddEmailActivities(options => options.Bind(configuration.GetSection("Elsa:Smtp")))
                .AddHttpActivities(options => options.Bind(configuration.GetSection("Elsa:Http")))
                .AddTimerActivities(options => options.Bind(configuration.GetSection("Elsa:Timers")))
                .AddUserActivities()
                // Add a MongoDB collection
                .AddMongoDbCollection<User>("Users")
                
                // Add liquid handler
                .AddNotificationHandlers(typeof(LiquidConfigurationHandler));
        }
    }
}
