using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrashCollector.Data;
using TrashCollector.Models;
using TrashCollector.Services;

namespace TrashCollector
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Comment out this line if re-doing migrations, then uncomment
            CreateUserRoles(services).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;
            
            var roleCheckAdmin = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheckAdmin)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var roleCheckManager = await RoleManager.RoleExistsAsync("Manager");
            if (!roleCheckManager)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Manager"));
            }
            var roleCheckEmployee = await RoleManager.RoleExistsAsync("Employee");
            if (!roleCheckEmployee)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Employee"));
            }
            var roleCheckCustomer = await RoleManager.RoleExistsAsync("Customer");
            if (!roleCheckCustomer)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Customer"));
            }

            //Assign Admin role to the main User here we have given our newly loregistered login id for Admin management
            ApplicationUser user = await UserManager.FindByEmailAsync("joelcorey@fastmail.com");
            var User = new ApplicationUser();
            await UserManager.AddToRoleAsync(user, "Admin");

        }
    }
}
