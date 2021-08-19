using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddFile(options =>
                {
                    options.LogDirectory = "Logs";  //로그저장폴더
                    options.FileName = "log-";      //로그파일 접두어 ex)log-20200101.txt
                    options.FileSizeLimit = null;   //로그파일 사이즈 제한(기본: 10MB)
                    options.RetainedFileCountLimit = null;  //로그파일 보유겟수(기본: 2개)
                }))
                .ConfigureWebHostDefaults(webBuider =>
                {
                    webBuider.UseStartup<Startup>();
                });
    }
}
