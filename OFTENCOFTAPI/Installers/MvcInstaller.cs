using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OFTENCOFTAPI.Controllers;
using OFTENCOFTAPI.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace OFTENCOFTAPI.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddCors();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //configure jwt
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();


            //
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;


            });

            services.ConfigureApplicationCookie(options =>
            {
                ////   Cookie settings
                //options.Cookie.HttpOnly = true;
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                //options.LoginPath = "/Account/Login";
                //options.AccessDeniedPath = "/Home/Privacy";
                //options.SlidingExpiration = true;

                options.EventsType = typeof(CustomCookieAuthenticationEvents);

            });

            services.AddTransient<TicketsController>();
            services.AddTransient<DrawsController>();
            services.AddTransient<CustomCookieAuthenticationEvents>();
            services.AddTransient<IEmailService, EmailSender>();

            // Swagger Docs Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "National Uptake API", Version = "v1" });
            });

        }
    }
}
