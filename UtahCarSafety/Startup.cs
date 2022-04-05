using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UtahCarSafety.Models;

namespace UtahCarSafety
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
            services.AddControllersWithViews();

            //GIVE TO TYLER
            services.AddDbContext<CrashesDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:CrashesDbConnection"]);
            });

            services.AddScoped<ICrashesRepository, EFCrashesRepository>();
            //UNTIL THIS POINT -- Add Pomelo (search MySQL)


            services.AddDbContext<AppIdentityDBContext>(options =>
            {
                options.UseSqlite(Configuration["ConnectionStrings:IdentityConnection"]);

            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDBContext>();

            services.AddRazorPages();

            services.AddDistributedMemoryCache();

            services.AddSession();

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("teamName",
                    "{teamName}/Page{pageNum}",
                    new { Controller = "Home", action = "Index" }
                    );

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 }
                    );

                endpoints.MapControllerRoute("teamName",
                    "{teamName}",
                    new { Controller = "Home", action = "Index", pageNum = 1 }
                    );

                endpoints.MapDefaultControllerRoute();

            });

            IdentitySeedData.EnsurePopulated(app);
        }
    }
}
