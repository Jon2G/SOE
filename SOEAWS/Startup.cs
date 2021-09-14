using Kit.Razor.Formatters;
using Kit.Sql.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Radzen;
using SOEAWS.Data;
using SOEAWS.Formaters;
using SOEAWS.Services;
using SOEWeb.Shared;
using SOEWeb.Shared.Secrets;
using System.Net.Http;

namespace SOEAWS
{
    public class Startup
    {
        static Startup()
        {
            DotNetEnviroment.Load();
            WebData.ConnectionString = Environment.GetEnvironmentVariable("AWSCONNECTIONSTRING");
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            //services.AddMudServices();
            //radzen
            services.AddSingleton<ICommentsService, CommentsService>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddSingleton<WeatherForecastService>();
            services.AddScoped<HttpClient>(s =>
            {
                var uriHelper = s.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
                return new HttpClient
                {
                    BaseAddress = new Uri(uriHelper.BaseUri)
                };
            });
            services.AddControllers(options =>
                options.InputFormatters.Add(new ByteArrayInputFormatter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SOEWeb", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
            });
            //services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                //app.UseWebAssemblyDebugging();
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SOEWeb v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                //endpoints.MapFallbackToFile("index.html");
                endpoints.MapFallbackToPage("/_Host");
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                //});
            });
        }
    }
}
