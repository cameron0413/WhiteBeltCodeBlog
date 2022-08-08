using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WhiteBeltCodeBlog.Models;

namespace WhiteBeltCodeBlog.Data
{
    public static class DataUtility
    {
        private const string _adminEmail = "cameron0413@gmail.com";
        private const string _modEmail = "";
        private const string _adminRole = "Administrator";
        private const string _modRole = "Moderator";



        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);

        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(":");
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task SeedDataAsync(IServiceProvider svcProvider)
        {
            // Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            // Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            // Service: An instance of Configuration Service
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();


            // Seed Roles
            await SeedRolesAsync(roleManagerSvc);

            // Seed Users
            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);

        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            if (!await roleManager.RoleExistsAsync(_modRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_modRole));
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            if(!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Cameron",
                    LastName = "Wilson",
                    PhoneNumber = "443-895-2408",
                    EmailConfirmed = true
                };


                await userManager.CreateAsync(adminUser, "O Meu Brasil1");
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }

            if (!context.Users.Any(u => u.Email == _modEmail))
            {
                BlogUser modUser = new()
                {
                    Email = _modEmail,
                    UserName = _modEmail,
                    FirstName = "WBCB",
                    LastName = "Moderator",
                    PhoneNumber = "999-555-1000",
                    EmailConfirmed = true
                };


                await userManager.CreateAsync(modUser, "O Meu Brasil1");
                await userManager.AddToRoleAsync(modUser, _modRole);
            }

        }

    }
}
