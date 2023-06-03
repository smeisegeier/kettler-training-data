using AutoWrapper;
using FileUploader.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploader
{
    /* excludes
     * 02.03.14
     * 12.04.14
     * 04.03.16
     */

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string db_con = "";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // //* azure option
            // services.AddAzureClients(azureClientFactoryBuilder =>
            // {
            // 	azureClientFactoryBuilder.AddSecretClient(Configuration.GetSection("KeyVault"));
            // });
            // services.AddSingleton<IKeyVaultManager, KeyVaultManager>();

            // db_con = ConfigurationExtensions.GetConnectionString(Configuration, "kettler-db-con");
            db_con = "Server=tcp:demosqlserverxd.database.windows.net,1433;Database=DemoSqlDb;Authentication=Active Directory Managed Identity;Trusted_Connection=False;Encrypt=True;PersistSecurityInfo=True;";

            //* get constring from localsettings
            // db_con = System.Configuration.ConfigurationManager.AppSettings.Get("db_con");

            // $env:ASPNETCORE_ENVIRONMENT='Staging'
            services.AddControllersWithViews();

            services.AddDbContext<MyDbContext>(options => options
                .UseSqlServer(db_con)
                );

            //* inmemory option
            // services.AddDbContext<MyDbContext>(options => options.UseInMemoryDatabase("Test"));
        }

        //* This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions() { IsApiOnly = false, IsDebug = true }); // use before routing
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
