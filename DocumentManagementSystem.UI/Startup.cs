using AutoMapper;
using DocumentManagementSystem.Business.DependencyResolvers.Microsoft;
using DocumentManagementSystem.Business.Helpers;
using DocumentManagementSystem.Business.Interfaces;
using DocumentManagementSystem.Business.Services;
using DocumentManagementSystem.Business.ValidationRules;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Dtos.DocumentDtos;
using DocumentManagementSystem.UI.Mappings.AutoMapper;
using DocumentManagementSystem.UI.Models;
using DocumentManagementSystem.UI.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // Add this using directive
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DocumentManagementSystem.UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // If AddDependencies method registers some services, ensure it doesn't conflict with the following registrations
            services.AddDependencies(Configuration);

            // Register FluentValidation validators
            services.AddTransient<IValidator<UserCreateModel>, UserCreateModelValidator>();
            services.AddTransient<IValidator<DocumentCreateDto>, DocumentCreateDtoValidator>(); // Added this line

            // Register Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                opt.Cookie.Name = "DocumentManagementCookie";
                opt.Cookie.HttpOnly = true;
                opt.Cookie.SameSite = SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.ExpireTimeSpan = TimeSpan.FromDays(20);
                opt.LoginPath = new PathString("/Account/SignIn");
                opt.LogoutPath = new PathString("/Account/LogOut");
                opt.AccessDeniedPath = new PathString("/Account/AccessDenied");
            });

            // Register Services
            services.AddScoped<IDocumentService, DocumentService>(); // Added this line
            services.AddScoped<IAnnouncementService, AnnouncementService>(); // Already present

            // Register DbContext
            services.AddDbContext<DocumentContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); // Added this line

            // Register AutoMapper profiles
            var profiles = ProfileHelper.GetProfiles();
            profiles.Add(new UserCreateModelProfile());
            profiles.Add(new UserUpdateModelProfile());

            var configuration = new MapperConfiguration(opt =>
            {
                opt.AddProfiles(profiles);
            });

            var mapper = configuration.CreateMapper();
            services.AddSingleton(mapper);

            // Alternatively, you can use services.AddAutoMapper() if preferred
            // services.AddAutoMapper(typeof(Startup)); // Uncomment and adjust if you prefer this method

            // Register MVC services
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        }

    }
}
