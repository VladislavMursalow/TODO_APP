using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MsSQL.Repositories;
using System.Data;
using TODO_APP.Infrastructure;
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
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();

            string connectionString = Configuration.GetConnectionString("TODO_DB");
            services.AddTransient<IDbConnection>(provider => new SqlConnection(connectionString));
            services.AddTransient<RepositoryResolver>();
            //MSSQL dependencies 
            services.AddScoped<MsSqlTodoRepository>();
            services.AddScoped<MsSqlCategoriesRepository>();
            //XML dependencies 
            services.AddScoped<XMLTodoReository>();
            services.AddScoped<XMLCategoriesRepository>();

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
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

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
