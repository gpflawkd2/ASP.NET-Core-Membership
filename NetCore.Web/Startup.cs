using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using System;
using System.IO;

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
            // https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/data-protection/configuration/overview.md 참고
            // 암호화키 관리 & 만료 기간 & Application 이름 지정 & 암호화 알고리즘 설정
            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"D:\GitHub\DataProtector\"))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .SetApplicationName("NetCore");

            // 서버에서 관리시
            //.PersistKeysToFileSystem(new DirectoryInfo(@"\\server\share\directory\"))
            // 암호화 알고리즘 적용
            //.UseCryptographicAlgorithms(
            //            new AuthenticatedEncryptorConfiguration()
            //            {
            //                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            //                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            //            })

            // 의존성 주입을 사용하기 위해서 서비스로 등록
            // IUser 인터페이스에 UserService 클래스 인스턴스 주입
            services.AddScoped<IUser, UserService>();

            // DB 접속정보, Migration 프로젝트 지정
            //services.AddDbContext<CodeFirstDbContext>(options =>
            //    options.UseSqlServer(connectionString: Configuration.GetConnectionString(name:"DefaultConnection"),
            //    sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName:"NetCore.Migrations"))
            //);

            //DB 접속정보
            services.AddDbContext<DBFirstDbContext>(options =>
                options.UseSqlServer(connectionString:Configuration.GetConnectionString(name:"DBFirstDBConnection"))
            );

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // MVC 패턴을 사용하기 위해서 서비스로 등록
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
