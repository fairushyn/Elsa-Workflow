using Elsa.Activities.Http.Extensions;
using Elsa_Workflow.Business;
using Elsa_Workflow.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elsa_Workflow
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();

            // Configuring Business
            services.AddScoped<IUserBusiness, UserBusiness>();

            // Configuring Elsa
            ElsaConfiguration.ConfigureElsa(services, Configuration);
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                // options.InstanceName = "Elsa_Workflow_";
            });

            // Configuring Swagger
            SwaggerConfiguration.ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add this configuration to enable Elsa's CSS an JS dashboard loading on startup
            app.UseStaticFiles();

            // Add Elsa's middleware to handle HTTP requests to workflows 
            app.UseHttpActivities();

            // Enabling Swagger
            app.UseSwagger();

            // Serving swagger to an endpoint
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa Workflow");
                config.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
