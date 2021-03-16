using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Repository;
using Repository.Configuration;
using Services;
using Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebService
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
            //authentication configure
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/UnauthorizedPage/";
                    options.Cookie.Name = "UserLoginCookie";
                });
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            /*services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy =>
                    policy.RequireClaim("Role", "Admin"));
                option.AddPolicy("Manager", policy =>
                    policy.RequireClaim("Role", "Manager"));
                option.AddPolicy("Seller", policy =>
                    policy.RequireClaim("Role", "Seller"));
            });*/

            services.Configure<DatabaseSettings>(
                Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<IMongoRepository, MongoRepository>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IPostService, PostService>();


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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

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
