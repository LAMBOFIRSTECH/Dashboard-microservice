using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Dashboard.Middlewares;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
        opt.SwaggerDoc("1", new OpenApiInfo
        {
                Title = "Dashboard des microservices | Api",
                Description = "An ASP.NET Core Web API for presnting microservices state and deploy version on Environment",
                Version = "1",
                Contact = new OpenApiContact
                {
                        Name = "Artur Lambo",
                        Email = "lamboartur94@gmail.com"
                }
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false);
builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
// builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(con =>
    {
         con.SwaggerEndpoint("/lambo-dashboard-manager/swagger/v1.0/swagger.yml", "Tableau des microservices par environnement");

         con.RoutePrefix = string.Empty;
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
    endpoints.MapGet("/version", async context => await context.Response.WriteAsync("Version de l'API : v1.0"));
});
app.Run();
