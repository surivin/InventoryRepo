using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;

namespace InventoryManagement
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
            // var connection = Configuration.GetConnectionString("InventoryDatabase");
            // services.AddDbContext<InventoryContext>(options => 
            // {
            //     var sqlConnection = new SqlConnection(connection);
            //     var credential = new DefaultAzureCredential();
            //     var token = credential
            //         .GetToken(new Azure.Core.TokenRequestContext(
            //             new[] { "https://database.windows.net/.default" }));
            //     sqlConnection.AccessToken = token.Token;
            //     //return sqlConnection;
            // });

            services.AddDbContext<InventoryContext>(options =>
            {
                var aT=new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions{ManagedIdentityClientId="4bf6a940-3706-4367-a834-9f99683d8d34"})
                    .GetToken(new Azure.Core.TokenRequestContext(
                         new[] { "https://database.windows.net/.default" })).Token;
                var dbConnection = new SqlConnection(Configuration.GetConnectionString("InventoryDatabase"))
                {
                    AccessToken = aT
                };
                options.UseSqlServer(dbConnection);
            });


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error");
            // }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Products}/{action=Index}/{id?}");
            });
        }
    }
}
