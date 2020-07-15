using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OFTENCOFTAPI.Controllers;
using OFTENCOFTAPI.Data;
using OFTENCOFTAPI.Models;
using OFTENCOFTAPI.Services;
using OFTENCOFTAPI.Models.User;
using OFTENCOFTAPI.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace OFTENCOFTAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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

            //db context

            services.AddDbContext<OFTENCOFTDBContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("OFTCON")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 7;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
           .AddEntityFrameworkStores<OFTENCOFTDBContext>().AddDefaultTokenProviders();


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            //});

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            //
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            //app.UseAuthorization();

            UsersandRolesInitializer.SeedData(userManager, roleManager);
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
                {
                    // fallback when no content is provided in an api response
                    if (!context.HttpContext.Response.ContentLength.HasValue ||
                        context.HttpContext.Response.ContentLength == 0)
                    {
                        context.HttpContext.Response.ContentType = "text/plain";
                        await context.HttpContext.Response.WriteAsync(
                              $"Status Code: {context.HttpContext.Response.StatusCode}");
                    }
                }
                else
                {
                    context.HttpContext.Response.Redirect($"/Error?code={context.HttpContext.Response.StatusCode}");
                }
            });

            // Added swagger middleware to App pipeline
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "National Uptake V1");
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });           
        }
    }
}
