using Accelerate.Foundations.Account.Claims;
using Accelerate.Foundations.Account.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Communication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;

namespace Accelerate.Foundations.Account
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            // CONFIGS
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            var socialConfig = configuration
                .GetSection("OAuthConfiguration")
                .Get<OAuthConfiguration>();

            socialConfig.GoogleAppSecret = configuration["GoogleAppSecret"];
            socialConfig.FacebookAppSecret = configuration["FacebookAppSecret"];

            // CONTEXT
            services.AddDbContext<Foundations.Database.Services.BaseContext<AccountProfile>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<AccountDbContext>(options => options.UseSqlServer(connString));

            // SERVICES
            services.AddTransient<IEntityService<AccountProfile>, EntityService<AccountProfile>>();

            services.AddTransient<IElasticService<AccountUserDocument>, AccountElasticService>();
            // services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            // CONFIGURATION
            services.AddIdentity<AccountUser, AccountRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<AccountDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<Microsoft.AspNetCore.Identity.IEmailSender<AccountUser>, AccountEmailSender>();
            //services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IUserClaimsPrincipalFactory<AccountUser>, AdditionalUserClaimsPrincipalFactory>();

            services.AddAuthorization(options => options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options => configuration.Bind("JwtSettings", options))
            .AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
            })
            /*
            .AddTwitter(options =>
            {
                options.ClientId = configuration["Twitter:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["Twitter:ClientSecret"] ?? string.Empty;
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["MicrosoftKeysClientId"];
                microsoftOptions.ClientSecret = configuration["MicrosoftKeysClientSecret"];

                microsoftOptions.Events = new OAuthEvents
                {
                    OnRedirectToAuthorizationEndpoint = context =>
                    {
                        // Force the user to select an account on the Microsoft login page
                        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
                        return Task.CompletedTask;
                    }
                };
            })
            */
            .AddGoogle(options =>
             {
                 options.ClientId = socialConfig?.GoogleAppId ?? string.Empty;//configuration["Google:ClientId"] ?? string.Empty;
                 options.ClientSecret = socialConfig?.GoogleAppSecret ?? string.Empty;
             });

            // Set email timeout to 7 days

            services.ConfigureApplicationCookie(o => {
                o.ExpireTimeSpan = TimeSpan.FromDays(7);
                o.SlidingExpiration = true;
            });
            //Change token lifespan to 3 hours
            services.Configure<DataProtectionTokenProviderOptions>(o =>
                   o.TokenLifespan = TimeSpan.FromHours(3));

        }
       
        public static void InitializeDb(AccountDbContext context)
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
