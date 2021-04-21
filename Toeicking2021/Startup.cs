using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Toeicking2021.Data;
using Toeicking2021.Models;
using Toeicking2021.Services.MailService;
using Toeicking2021.Services.MembersDBService;
using Toeicking2021.Services.SentenceDBService;
using Toeicking2021.Utilities;

namespace Toeicking2021
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddSessionStateTempDataProvider();
            services.AddSession();
            // 註冊cookie驗證
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        // 以下這兩個設定可有可無
                        options.Cookie.HttpOnly = true;
                        options.AccessDeniedPath = "/Mmeber/Login";   // 登入後存取權限不足會跳到這一頁。
                        options.LoginPath = "/Member/Login"; // 登入頁。
                    });
            // 註冊DbContext
            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // 註冊AUTOMAPPER
            services.AddAutoMapper(typeof(Startup));
            // 註冊IHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // 註冊HttpClient
            services.AddHttpClient();
            // 獲得appsettings.json內某區段("MailSettings")的值，並裝進一個model物件(MailSettings)，屬性名稱要對應好
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<Encryption>(Configuration.GetSection("Encryption"));
            // 註冊Repository Pattern
            services.AddScoped<IMembersDBService, MembersDBService>();
            services.AddScoped<ISentenceDBService, SentenceDBService>();
            services.AddTransient<IMailService, MailService>();
            // 註冊有使用靜態欄位並DI的Utilities類別(沒有介面)，不然無法呼叫
            // "沒有"使用靜態欄位並DI的Utilities類別不用在這註冊，可直接呼叫
            services.AddScoped<HttpClientHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});
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
            // 使用靜態檔案，外界才可存取
            app.UseStaticFiles();
            app.UseRouting();
            // cookie驗證，位置必須在app.UseAuthorization();前面
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
