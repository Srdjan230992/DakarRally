using DakarRally.Models;
using DakarRally.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DakarRally
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration properties.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Method gets called by the runtime. This method is used to add services to the container.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<VehicleDbContext>(opt =>
                                               opt.UseInMemoryDatabase("Vehicles"));

            services.AddControllers();

            services.AddScoped<IRaceInformationsService, RaceInformationsServiceImpl>();
            services.AddScoped<IRaceService, RaceServiceImpl>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DakarRally", Version = "v1" });
            });
        }

        /// <summary>
        /// Method gets called by the runtime. This method is used to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Mechanism to configure an application's request.</param>
        /// <param name="env">Information about the web hosting environment an application is running.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/error");

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DakarRally v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();      

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStatusCodePages();
        }
    }
}