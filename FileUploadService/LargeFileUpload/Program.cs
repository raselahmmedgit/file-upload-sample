
using FileUploadService.Utilities;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace LargeFileUpload
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddHttpContextAccessor();

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            var physicalProvider = new PhysicalFileProvider(builder.Configuration.GetValue<string>("StoredFilesPath").ToString());
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            builder.Services.AddSingleton<IFileProvider>(physicalProvider);
            
            builder.Services.AddScoped<IUploadLargeFileHelper, UploadLargeFileHelper>();

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                 options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
