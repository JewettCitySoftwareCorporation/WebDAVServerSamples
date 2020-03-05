using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebDAVServer.FileSystemStorage.AspNetCore
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.webdav.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            HostingEnvironment = env;
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddWebDav(Configuration, HostingEnvironment);
            services.AddGSuite(Configuration);
            services.AddSingleton<WebSocketsService>();
      
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            // Basic auth requires SSL connection. To enable non - SSL connection for testing purposes read the following articles:
            // - In case of Windows & MS Office: http://support.microsoft.com/kb/2123563
            // - In case of Mac OS X & MS Office: https://support.microsoft.com/en-us/kb/2498069
            //app.UseBasicAuth();
            //app.UseDigestAuth();
            app.UseWebSockets();
            app.UseWebSocketsMiddleware();
            app.UseGSuite();
            app.UseWebDav(HostingEnvironment);     
        }
    }
}
