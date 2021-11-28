using AmcrestMQTT.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmcrestMQTT

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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Amcrest to MQTT", Version = "v1" });
                c.AddServer(new OpenApiServer() {Description="My server", Url="" });
            });
            services.AddHttpClient();



            //appSettings.Bind(options);
            var dataFolder = Configuration.GetValue<string>("DataFolder");
            var optionsText = System.IO.File.ReadAllText(System.IO.Path.Combine(dataFolder, "options.json"));
            var options = System.Text.Json.JsonSerializer.Deserialize<Settings>(optionsText, new System.Text.Json.JsonSerializerOptions() {  PropertyNameCaseInsensitive=true});

            services.AddSingleton(options);
            services.AddTransient<AmcrestToMqttWorker>();

            services.AddHostedService<MQTTWorker>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Amcrest to MQTT v1");
                c.RoutePrefix = "";
            });
            app.UseSwagger();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

       

    }
}
