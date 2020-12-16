using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weekday.Data.Models;

namespace Weekday.Data.Interfaces
{
    public interface IAccountManager
    {
        Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password);

        Task<IdentityRole> GetRoleByNameAsync(string roleName);

        Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(IdentityRole role);                  
    }
}
