using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;
using System;

namespace NetCore.Web
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

            Common.SetDataProtection(services, @"D:\GitHub\_DataProtector\", "NetCore", Enums.CryptoType.CngCbc);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 의존성 주입을 사용하기 위해서 서비스로 등록
            // IUser 인터페이스에 UserService 클래스 인스턴스 주입
            services.AddScoped<DBFirstDbInitializer>();
            services.AddScoped<IUser, UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // DB 접속정보(Code-First Migration)
            //services.AddDbContext<CodeFirstDbContext>(options =>
            //    options.UseSqlServer(connectionString: Configuration.GetConnectionString(name:"DefaultConnection"),
            //    sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName:"NetCore.Migrations"))
            //);

            //DB 접속정보(Database-First Migration)
            services.AddDbContext<DBFirstDbContext>(options =>
                options.UseSqlServer(connectionString:Configuration.GetConnectionString(name:"DBFirstDBConnection"))
            );

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection(key: "Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            // MVC 패턴을 사용하기 위해서 서비스로 등록
            services.AddMvc();

            // 신원보증
            services.AddAuthentication(defaultScheme:CookieAuthenticationDefaults.AuthenticationScheme) 
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Membership/Forbidden";
                    options.LoginPath = "/Membership/Login";
                });

            // 승인권한
            services.AddAuthorization();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".NetCore.Session";
                //세션 제한시간
                options.IdleTimeout = TimeSpan.FromMinutes(30); //기본값은 20분
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DBFirstDbInitializer seeder)
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

            app.UseAuthentication();
            app.UseCookiePolicy();

            // app.UseMvc() 위에 있어야함
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            seeder.PlantSeedData();
        }
    }
}
