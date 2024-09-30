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
using Accelerate.Foundations.Account.Models.Data;
using Accelerate.Foundations.Common.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Accelerate.Foundations.Account
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            // CONFIGS
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            services.Configure<AccountConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });

            services.Configure<OAuthConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.OAuthConfigurationName).Bind(options);

                options.GoogleAppSecret = configuration[Constants.Config.GoogleAppSecretKey];
                options.GoogleAppId = configuration[Constants.Config.GoogleAppIdKey];
                options.GoogleRedirectUri = configuration[Constants.Config.GoogleRedirectUri];

                options.FacebookAppSecret = configuration[Constants.Config.FacebookAppSecretKey];
                options.FacebookAppId = configuration[Constants.Config.FacebookAppIdKey];
                options.FacebookRedirectUri = configuration[Constants.Config.FacebookRedirectUri];
            });
            // CONTEXT
            services.AddDbContext<Foundations.Database.Services.BaseContext<AccountProfile>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<AccountDbContext>(options => options.UseSqlServer(connString));

            // SERVICES
            services.AddTransient<IEntityService<AccountProfile>, EntityService<AccountProfile>>(); 
            services.AddTransient<IEntityService<AccountUser>, AccountUserService>(); 
            services.AddTransient<IAccountUserService, AccountUserService>();

            services.AddTransient<IAccountUserSearchService, AccountUserSearchService>();

            services.AddTransient<IElasticService<AccountUserDocument>, AccountUserSearchService>();
            // services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            // CONFIGURATION
            services.AddIdentity<AccountUser, AccountRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddDefaultTokenProviders();

            services.AddSingleton<Microsoft.AspNetCore.Identity.IEmailSender<AccountUser>, AccountEmailSender>();
            //services.AddSingleton<IEmailSender, EmailSender>();
           
            
            //services.AddScoped<IUserClaimsPrincipalFactory<AccountUser>, AdditionalUserClaimsPrincipalFactory>();
            //services.AddAuthorization(options => options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));
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
                options.AccessDeniedPath = "/account/AccessDeniedPathInfo";
                options.SaveTokens = true; 
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.ClientId = configuration[Constants.Config.GoogleAppIdKey] ?? string.Empty;//configuration["Google:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration[Constants.Config.GoogleAppSecretKey] ?? string.Empty;
            })
            .AddFacebook(options =>
            {
                options.SaveTokens = true;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                   options.AccessDeniedPath = "/account/AccessDeniedPathInfo";
                options.ClientId = configuration[Constants.Config.FacebookAppIdKey] ?? string.Empty;//configuration["Google:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration[Constants.Config.FacebookAppSecretKey] ?? string.Empty;

                options.Events.OnTicketReceived = (context) =>
                {
                    Console.WriteLine(context.HttpContext.User);
                    return Task.CompletedTask;
                };
                options.Events.OnCreatingTicket = (context) =>
                {
                    Console.WriteLine(context.Identity);
                    return Task.CompletedTask;
                };
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
                context.Database.EnsureCreated();
                // Look for any students.
                 
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static async Task<IdentityResult> InitializeDefaultAdmin(UserManager<AccountUser> userManager)
        {
            try
            {
                var existingAdmin = await userManager.FindByNameAsync("Admin");
                if (existingAdmin != null)
                    return null;

                var user = new AccountUser { UserName = "Admin", Email = "Admin@internal", Domain = Constants.Domains.Internal, EmailConfirmed = true, Status = AccountUserStatus.Active };
                var password = "Password1!!";
                return await userManager.CreateAsync(user, password);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
