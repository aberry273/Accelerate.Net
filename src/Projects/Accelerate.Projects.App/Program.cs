using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Projects.App.Data;
using Accelerate.Projects.App.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using static Accelerate.Foundations.Database.Constants.Exceptions;

var builder = WebApplication.CreateBuilder(args);

//var appSecretsId = "5334ac05-3583-4823-9d44-97410596f81b";
builder.Configuration.AddUserSecrets<Program>();

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
Accelerate.Foundations.Common.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Database.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Communication.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Content.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Account.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Websockets.Startup.ConfigureServices(builder.Services, builder.Configuration);

Accelerate.Foundations.Integrations.Elastic.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Foundations.Integrations.MassTransit.Startup.ConfigureServices(builder.Services, builder.Configuration);

// Add Feature references to the container
Accelerate.Features.Content.Startup.ConfigureServices(builder.Services, builder.Configuration);
Accelerate.Features.Account.Startup.ConfigureServices(builder.Services, builder.Configuration);
  


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

app.UseCors(localFrontendDevServer);

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
/*
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MessageHub>("/messageHub");
});
*/
//IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<ContentPostEntity>> _messageHub;
app.MapHub<BaseHub<ContentPostEntity>>($"/{Accelerate.Features.Content.Constants.Settings.WebSocketHubName}");
#pragma warning restore ASP0014 // Suggest using top level route registrations


app.Run();


