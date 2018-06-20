using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P1611706.Model;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Routing.Template;

namespace P1611706
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddOptions();

            services.AddMvc();

            var config = Configuration.GetSection("connectionSetting").Get<connectionSetting>();
            var connection = config.connectiongString;
			var connectionB = config.connectiongStringB;

			//services.AddDbContext<P1611706DB>(options => options.UseSqlServer(connection,b => b.UseRowNumberForPaging()));

			services.AddDbContext<P1611706DB>(options => options.UseSqlServer(connection,b => b.UseRowNumberForPaging()));
			services.AddDbContext<LeaderDemoDB>(options => options.UseSqlServer(connectionB, b => b.UseRowNumberForPaging()));


			services.Configure<LdapString>(Configuration.GetSection("LdapString"));

            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

            services.AddSession(options =>
            {
                options.CookieName = ".NetEscapades.Session";
                //options.IdleTimeout = System.TimeSpan.FromMinutes(1);

                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.CookieHttpOnly = true;
            });

        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			//app.UseApplicationInsightsRequestTelemetry();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Main/Error");
			}

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseStaticFiles();


			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Login}/{action=Index}/{id?}");

			

			}); 


			

		}
    }
}
