using TechnoMarkt.Data;
using TechnoMarkt.Models.Identity;
using Microsoft.AspNetCore.Identity;
using TechnoMarkt.Extensions;

namespace TechnoMarkt
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices();
            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/{1}/Views/{0}.cshtml");
                    options.AreaViewLocationFormats.Insert(1, "/Areas/{2}/_Shared/Views/Shared/{0}.cshtml");
                    options.AreaViewLocationFormats.Insert(2, "/Areas/{2}/_Shared/Views/{0}.cshtml");
                });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Home/Index");
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseSession();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "account",
                pattern: "Account/{action=Login}/{id?}",
                defaults: new { controller = "Account" });

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "cart",
                pattern: "Cart/{action=Index}/{id?}",
                defaults: new { controller = "Cart" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Home" });

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    IServiceProvider serviceProvider = scope.ServiceProvider;


                    Console.WriteLine("Trying to bring up Chromium...");
                    var browserFetcher = new PuppeteerSharp.BrowserFetcher();
                    await browserFetcher.DownloadAsync();
                    Console.WriteLine("Chromium was successfully downloaded.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR]: {ex.Message}");
                }
            }

            app.Run();
        }
    }
}

