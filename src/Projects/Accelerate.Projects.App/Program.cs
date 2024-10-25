using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Websockets.Hubs;
using Azure.Identity;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using static Accelerate.Foundations.Database.Constants.Exceptions;

var builder = WebApplication.CreateBuilder(args);

//var appSecretsId = "5334ac05-3583-4823-9d44-97410596f81b";
builder.Configuration.AddUserSecrets<Program>();
//https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-8.0
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}
// enable CORS
var localFrontendDevServer = "localFrontendDevServer";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: localFrontendDevServer,
        policy =>
        {
            policy.WithOrigins("*", "https://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddRazorPages();
// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
// Add Foundation references to the container
Accelerate.Foundations.Integrations.Elastic.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.MassTransit.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.AzureStorage.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.AzureSecrets.Startup.ConfigureServices(builder.Services, builder.Configuration);

// Force to equal true (isProduct = true) when deploying Schema Updates via EF scaffolding
var isProduction = builder.Environment.IsProduction();
Accelerate.Foundations.Common.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Database.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Communication.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Content.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Account.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Media.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Websockets.Startup.ConfigureServices(builder.Services, builder.Configuration);

// Add Feature references to the container
Accelerate.Features.Content.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Features.Account.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Features.Media.Startup.ConfigureServices(builder.Services, builder.Configuration);



// Add Database Exception filter
// provides helpful error information in the development environment for EF migrations errors.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// enable MVC
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

// enable sessionState
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();

var app = builder.Build();


app.UseSession();

// Add WebAPI based authentication
app.MapGroup($"/{Accelerate.Projects.App.Constants.Routes.WebApiAuthentication}")
    .MapIdentityApi<AccountUser>()
    ;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors(localFrontendDevServer);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
/*
app.MapControllerRoute(name: "default",
               pattern: "{controller=Home}/{action=Index}/{id?}");
*/
app.MapDefaultControllerRoute();


// Map SignalR hubs
#pragma warning disable ASP0014 // Suggest using top level route registrations
Accelerate.Features.Content.Startup.ConfigureApp(app);
Accelerate.Features.Media.Startup.ConfigureApp(app);
#pragma warning restore ASP0014 // Suggest using top level route registrations


app.Run();


