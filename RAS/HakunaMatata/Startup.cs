using System;
using Application.Common.Options;
using Application.Interfaces;
using Commom;
using HakunaMatata.Data;
using HakunaMatata.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Westwind.AspNetCore.LiveReload;

namespace HakunaMatata
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
            services.AddDbContext<HakunaMatataContext>
                (opts => opts.UseSqlServer(Configuration["ConnectionString:ConnectStr"]));

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // Add framework services.
            services.AddMvc();

            services.Configure<VnPayOption>(Configuration.GetSection("PaymentConfig:VnPay"));
            services.Configure<MoMoOption>(Configuration.GetSection("PaymentConfig:MoMo"));
            
            //enable CORS
            services.AddCors(option => option.AddPolicy("MyBlogPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            
            //register services here
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IFileServices, FileServices>();
            services.AddScoped<IRealEstateServices, RealEstateServices>();
            services.AddScoped<IAboutUsServices, AboutUsServices>();
            services.AddScoped<IFAQServices, FAQServices>();
            services.AddScoped<ILevelServices, LevelServices>();
            services.AddScoped<IPolicyServices, PolicyServices>();
            services.AddScoped<IRealEstateTypeServices, RealEstateTypeServices>();
            services.AddScoped<IAgentServices, AgentServices>();
            services.AddScoped<ICommonServices, CommonServices>();

            services.AddScoped<IVerification, Verification>();
            //services.AddScoped<VerifyFilter>();

            services.AddScoped<IVNPayService, VNPayService>();
            services.AddScoped<IMomoService, MomoService>();
            
            //services.AddSingleton<IVerification>(new Verification(
            //    Configuration.GetSection("Twilio").Get<Configuration.Twilio>()));
            services.AddLiveReload();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddMvc().AddRazorRuntimeCompilation();

            //authentication cookie
            services.AddAuthentication("MyCookieScheme")
               .AddCookie("MyCookieScheme", options =>
               {
                   options.AccessDeniedPath = new PathString("/AdminArea/Account/Denied");
                   options.Cookie = new CookieBuilder
                   {
                       HttpOnly = true,
                       Name = "HakunaMata.Cookie",
                       SameSite = SameSiteMode.Lax,
                       SecurePolicy = CookieSecurePolicy.SameAsRequest
                   };
                   options.LoginPath = new PathString("/dang-nhap");
               });
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
            
            app.UseSession();
            
            app.UseLiveReload();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyBlogPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "AdminArea",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

/*                endpoints.MapControllerRoute(
                    name: "Account",
                    pattern: "{controller=Account}/{action=Index}/{id?}");*/

                endpoints.MapControllerRoute(
                    name: "RealEstate",
                    pattern: "{controller=RealEstate}/{action=Index}/{id?}");
            });
        }
    }
}
