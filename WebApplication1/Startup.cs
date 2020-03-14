using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Weather.DAL;
using Weather.DAL.Concrete.EntityFramework;
using Weather.DAL.Abstract;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Internal;
using Weather.WebAPI.Hubs;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //In-Memory cache kullanildi.
            services.AddMemoryCache();
            
            //EF Context 
            services.AddDbContext<WeatherContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("BigDataTeknolojiTESTDatabase")));

            //Weather Service 
            services.AddTransient<IWeatherRepository, WeatherRepository>();


            //API-1: LocationIQ
            var LocationIQAPIUrl = Configuration.GetValue<string>("LocationIQAPIUrl");
            Uri endPointLocationIQAPI = new Uri(LocationIQAPIUrl);
            services.AddHttpClient("LocationIQClient", client => {
                client.BaseAddress = endPointLocationIQAPI;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            
            //API-2: DarkSky
            var DarkSkyAPIUrl = Configuration.GetValue<string>("DarkSkyAPIUrl");
            Uri endPointDarkSkyAPI = new Uri(DarkSkyAPIUrl);            
            services.AddHttpClient("DarkSkyClient", client => { 
                client.BaseAddress = endPointDarkSkyAPI;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            });


            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {   //builder.WithOrigins("http://localhost:4200")
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.AllowCredentials()
                .Build();
            }));

            services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors("ApiCorsPolicy"); //call UseCors before the UseAuthorization, UseEndpoints

            app.UseSignalR(route =>
            {
                route.MapHub<NotificationHub>("/notificationHub");
            });

            app.UseMvc();
            //app.UseMvc(  routes => {routes.MapRoute(name: "default",template: "{controller=Home}/{action=Index}");});
        }
    }
}
