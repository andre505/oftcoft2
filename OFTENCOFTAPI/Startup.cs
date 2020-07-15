using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OFTENCOFTAPI.Data;
using OFTENCOFTAPI.Installers;
using OFTENCOFTAPI.ApplicationCore.Models.User;

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
            services.InstallServicesInAssembly(Configuration);

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
