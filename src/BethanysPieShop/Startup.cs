using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Net;
using System.Text;
using BethanysPieShop.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop
{
    public class Startup
    {
        private IConfigurationRoot _configurationRoot;
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();
                
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how toefcore configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IPieRepository, PieRepository>();

            var connection = _configurationRoot.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                                        options.UseSqlServer(_configurationRoot.GetConnectionString("DefaultConnection")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            DbInitializer.Seed(app);
        }

    }
}
