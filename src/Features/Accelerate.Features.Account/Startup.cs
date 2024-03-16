using Accelerate.Features.Account.Claims;
using Accelerate.Features.Account.Data;
using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Models.Entities;
using Accelerate.Features.Account.Services;
using Accelerate.Foundations.Communication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Accelerate.Features.Account
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            var socialConfig = configuration
                .GetSection("OAuthConfiguration")
                .Get<OAuthConfiguration>();

            // CONTEXT
            services.AddDbContext<Foundations.Database.Services.BaseContext<AccountProfile>>(options => options.UseSqlServer(configuration.GetConnectionString(Constants.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<AccountDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(Constants.Settings.ConnectionStringName)));

            // SERVICES
            // services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            // CONFIGURATION
            services.AddIdentity<AccountUser, AccountRole>(options =>
                options.SignIn.RequireConfirmedAccount = false)
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
