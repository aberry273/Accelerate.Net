using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Websockets.Hubs;
using Azure.Identity;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using MassTransit.JobService;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Microsoft.AspNetCore.RateLimiting;
using Accelerate.Projects.Api.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Superstable API",
        Description = "Superstables APIs to transfer funds between accounts almost instantenously.",
        TermsOfService = new Uri("https://superstable.xyz.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Contact us",
            Url = new Uri("https://superstable.xyz.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Superstable License",
            Url = new Uri("https://superstable.xyz.com/license")
        }
    });
    // XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Foundation references to the container
Accelerate.Foundations.Integrations.Elastic.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.MassTransit.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.AzureStorage.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.AzureSecrets.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.Quartz.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.Twilio.Startup.ConfigureServices(builder.Services, builder.Configuration);

// Force to equal true (isProduct = true) when deploying Schema Updates via EF scaffolding
var isProduction = builder.Environment.IsProduction();
Accelerate.Foundations.Mediator.Startup.ConfigureServices(builder.Services, builder.Configuration);

Accelerate.Foundations.Common.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Database.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Communication.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Users.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Websockets.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Funding.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);

Accelerate.Foundations.Accounts.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Transactions.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Rates.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);
Accelerate.Foundations.Orders.Startup.ConfigureServices(builder.Services, builder.Configuration, isProduction);


// Add Feature references to the container
Accelerate.Features.Accounts.Startup.ConfigureServices(builder.Services, builder.Configuration);


// Add Database Exception filter
// provides helpful error information in the development environment for EF migrations errors.

// enable MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Rate limiting
var tokenPolicy = "token";
var myOptions = new RateLimitOptions();
builder.Configuration.GetSection("ApiRateLimits").Bind(myOptions);

builder.Services.AddRateLimiter(_ => _
    .AddTokenBucketLimiter(policyName: tokenPolicy, options =>
    {
        options.TokenLimit = myOptions.TokenLimit;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = myOptions.QueueLimit;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod);
        options.TokensPerPeriod = myOptions.TokensPerPeriod;
        options.AutoReplenishment = myOptions.AutoReplenishment;
    }));

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

// Setup local admin
//var sp = builder.Services.BuildServiceProvider();
//var userService = sp.GetService<IUsersUserService>();
//userService.InitializeAdmin();


app.UseSession();

// Add WebAPI based authentication
/*
app.MapGroup($"/{Accelerate.Projects.Api.Constants.Routes.WebApiAuthentication}")
    .MapIdentityApi<UsersUser>()
    ;
*/
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


app.Run();


