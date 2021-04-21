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
            // ���Ucookie����
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        // �H�U�o��ӳ]�w�i���i�L
                        options.Cookie.HttpOnly = true;
                        options.AccessDeniedPath = "/Mmeber/Login";   // �n�J��s���v�������|����o�@���C
                        options.LoginPath = "/Member/Login"; // �n�J���C
                    });
            // ���UDbContext
            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // ���UAUTOMAPPER
            services.AddAutoMapper(typeof(Startup));
            // ���UIHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // ���UHttpClient
            services.AddHttpClient();
            // ��oappsettings.json���Y�Ϭq("MailSettings")���ȡA�ø˶i�@��model����(MailSettings)�A�ݩʦW�٭n�����n
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<Encryption>(Configuration.GetSection("Encryption"));
            // ���URepository Pattern
            services.AddScoped<IMembersDBService, MembersDBService>();
            services.AddScoped<ISentenceDBService, SentenceDBService>();
            services.AddTransient<IMailService, MailService>();
            // ���U���ϥ��R�A����DI��Utilities���O(�S������)�A���M�L�k�I�s
            // "�S��"�ϥ��R�A����DI��Utilities���O���Φb�o���U�A�i�����I�s
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
            // �ϥ��R�A�ɮסA�~�ɤ~�i�s��
            app.UseStaticFiles();
            app.UseRouting();
            // cookie���ҡA��m�����bapp.UseAuthorization();�e��
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
