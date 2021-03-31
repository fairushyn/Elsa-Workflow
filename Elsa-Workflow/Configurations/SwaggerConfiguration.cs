using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Elsa_Workflow.Configurations
{
    public class SwaggerConfiguration
    {
        public static void ConfigureSwagger(IServiceCollection services)
        {
            // TODO services.AddSwaggerExamplesFromAssemblyOf<ModelExample>();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "Elsa Workflow", Version = "v1" });
                // TODO config.ExampleFilters();
            });
        }
    }
}
