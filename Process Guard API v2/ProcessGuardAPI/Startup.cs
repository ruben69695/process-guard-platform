using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProcessGuardAPI.Models;

namespace ProcessGuardAPI
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
            // We add the connection string to the MySQL Database from the appsettings.json file
            services.AddDbContext<Models.processguarddbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("PERSONAL_RDS_MYSQL_DATABASE")));
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling
                  = ReferenceLoopHandling.Ignore;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseMvc();
        }
    }
}
