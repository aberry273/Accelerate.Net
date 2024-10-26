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

namespace Accelerate.Foundations.Users
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            // CONFIGS
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            
            try
            {
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
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

            services.AddSingleton<Microsoft.AspNetCore.Identity.IEmailSender<UsersUser>, UsersEmailSender>();
            //services.AddSingleton<IEmailSender, EmailSender>();

            //services.AddScoped<IUserClaimsPrincipalFactory<UsersUser>, AdditionalUserClaimsPrincipalFactory>();
            //services.AddAuthorization(options => options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")));
            services.Configure<UsersConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            
                if (!bool.Parse(configuration[Constants.Config.EnableOAuth]))
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
                else
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
                        options.AccessDeniedPath = "/Users/AccessDeniedPathInfo";
                        options.SaveTokens = true; 
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                        options.ClientId = configuration[Constants.Config.GoogleAppIdKey] ?? string.Empty;//configuration["Google:ClientId"] ?? string.Empty;
                        options.ClientSecret = configuration[Constants.Config.GoogleAppSecretKey] ?? string.Empty;
                    })
                    .AddFacebook(options =>
                    {
                        options.SaveTokens = true;
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                           options.AccessDeniedPath = "/Users/AccessDeniedPathInfo";
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
                    /*
                    .AddTwitter(options =>
                    {
                        options.ClientId = configuration["Twitter:ClientId"] ?? string.Empty;
                        options.ClientSecret = configuration["Twitter:ClientSecret"] ?? string.Empty;
                    })
                    .AddMicrosoftUsers(microsoftOptions =>
                    {
                        microsoftOptions.ClientId = configuration["MicrosoftKeysClientId"];
                        microsoftOptions.ClientSecret = configuration["MicrosoftKeysClientSecret"];

                        microsoftOptions.Events = new OAuthEvents
                        {
                            OnRedirectToAuthorizationEndpoint = context =>
                            {
                                // Force the user to select an Users on the Microsoft login page
                                context.Response.Redirect(context.RedirectUri + "&prompt=select_Users");
                                return Task.CompletedTask;
                            }
                        };
                    })
                    */

                }
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
