using Accelerate.Foundations.Users.Claims;
using Accelerate.Foundations.Users.Data;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Users.Models.Entities;
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
using Accelerate.Foundations.Users.Models.Data;
using Accelerate.Foundations.Common.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Foundations.Integrations.Twilio.Services;
using Accelerate.Foundations.Users.Handlers;

namespace Accelerate.Foundations.Users
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            // CONFIGS
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            var enableOAuth = false;
            services.Configure<UsersConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);

                enableOAuth = configuration.GetValue<bool>(Constants.Config.EnableOAuth);
            });

            try
            {
                if (enableOAuth) { 
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
                }
            }
            catch(Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
            // CONTEXT
            services.AddDbContext<Foundations.Database.Services.BaseContext<UsersProfile>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(connString));

            // SERVICES
            services.AddTransient<IEntityService<UsersProfile>, EntityService<UsersProfile>>(); 
            services.AddTransient<IEntityService<UsersUser>, UsersUserService>(); 
            services.AddTransient<IUsersUserService, UsersUserService>();
            
            services.AddTransient<IUsersUserSearchService, UsersUserSearchService>();
             
            services.AddTransient<IElasticService<UsersUserDocument>, UsersUserSearchService>();
            // services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            // CONFIGURATION
            services.AddIdentity<UsersUser, UsersRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<UsersUser>, AdditionalUserClaimsPrincipalFactory>();

            services.AddTransient<ITwilioSmsSender, TwilioSmsSender>();
            services.AddTransient<IEmailSender<UsersUser>, UsersEmailSender>();

            //services.AddSingleton<IEmailSender, EmailSender>();

            //services.AddScoped<IUserClaimsPrincipalFactory<UsersUser>, AdditionalUserClaimsPrincipalFactory>();
            //services.AddAuthorization(options => options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));
            if(enableOAuth) {
                SetAuthWithOAuthConfig(services, configuration);
            } 
            else {
                SetAuthConfig(services, configuration);
            }
            // 2FA

            // MFA

            services.AddAuthorization(options =>
                options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));
            // Force ASP.NET Core OpenID Connect client to require MFA
            // [Authorize(Policy="RequireMfa")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireMfa", policyIsAdminRequirement =>
                {
                    policyIsAdminRequirement.Requirements.Add(new RequireMfa());
                });
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

        private static void SetAuthWithOAuthConfig(IServiceCollection services, IConfiguration configuration)
        {
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
           })
           /*
           .AddOpenIdConnect(options =>
            {
                options.SignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "<OpenID Connect server URL>";
                options.RequireHttpsMetadata = true;
                options.ClientId = "<OpenID Connect client ID>";
                options.ClientSecret = "<>";
                options.ResponseType = "code";
                options.UsePkce = true;
                options.Scope.Add("profile");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("acr_values", "mfa");
                        return Task.FromResult(0);
                    }
                };
            })*/
           ;
        }
        private static void SetAuthConfig(IServiceCollection services, IConfiguration configuration)
        {
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
           });
        }

        public static void InitializeDb(UsersDbContext context)
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
    }
}
