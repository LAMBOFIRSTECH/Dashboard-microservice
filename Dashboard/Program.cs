using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Dashboard.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
var kestrelSectionCertificate = builder.Configuration.GetSection("Kestrel:EndPoints:Https:Certificate");
var certificateFile = kestrelSectionCertificate["File"];
var certificatePassword = kestrelSectionCertificate["Password"];

builder.Services.Configure<KestrelServerOptions>(options =>
{
    if (string.IsNullOrEmpty(certificateFile) || string.IsNullOrEmpty(certificatePassword))
    {
        throw new InvalidOperationException("Certificate path or password not configured");
    }
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxRequestBodySize = 10 * 1024;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.ConfigureHttpsDefaults(opt =>
    {
        opt.ClientCertificateMode = ClientCertificateMode.NoCertificate; // Required Certificate dans les autres services c'est du allowCertificate
    });
});

var app = builder.Build();
app.UseMiddleware<ContextPathMiddleware>("/lambo-dashboard-manager");
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
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
