using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Weekday.Data.Interfaces;
using Weekday.Data.Models;

namespace Weekday.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            if (!await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Generating default data");

                await CreateRoleAsync(UserRoles.Administrator);
                await CreateRoleAsync(UserRoles.Employee);
                await CreateUserAsync("admin", "Admin1_", "Inbuilt Administrator", "admin@company.com", "+375 (29) 000-0000", new string[] { UserRoles.Administrator });
                await CreateUserAsync("employee", "Employee1_", "Inbuilt Employee", "employee@company.com", "+375 (29) 000-0000", new string[] { UserRoles.Employee });

                _logger.LogInformation("Default data generated");
            }
        }

        private async Task CreateRoleAsync(string roleName)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                var result = await _accountManager.CreateRoleAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Seeding \"{roleName}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
                }
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true
            };

            var result = await _accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Succeeded)
            {
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }

            return applicationUser;
        }
    }
}
