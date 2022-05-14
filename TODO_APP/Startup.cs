using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsSQL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TODO_APP.Repositories;
using TODO_APP.Repositories.Infrastructure;
using TODO_APP.Repositories.XML;

namespace TODO_APP
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
            string connectionString = Configuration.GetConnectionString("TODO_DB");
            services.AddSession();

            services.AddTransient<IDbConnection>(provider => new SqlConnection(connectionString));
            //MSSQL dependencies 
            services.AddScoped<MsSqlTodoRepository>();
            services.AddScoped<MsSqlCategoriesRepository>();
            //XML dependencies 
            services.AddScoped<XMLTodoReository>();
            services.AddScoped<XMLCategoriesRepository>();

            services.AddTransient<CategoryReslover>(provider => serviceTypeName => 
            {
                switch (serviceTypeName) 
                {
                    case "MsSql":
                        return provider.GetService<MsSqlCategoriesRepository>();
                    case "XML":
                        return provider.GetService<XMLCategoriesRepository>();
                    default:
                        return null;
                }
            });

            services.AddTransient<TodoReslover>(provider => serviceTypeName =>
            {
                switch (serviceTypeName)
                {
                    case "MsSql":
                        return provider.GetService<MsSqlTodoRepository>(); break;
                    case "XML":
                        return provider.GetService<XMLTodoReository>(); break;
                    default:
                        return null;
                }
            });
            services.AddControllersWithViews();

           
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Todo/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Todo}/{action=Index}/{id?}");
            });
        }
    }
}
