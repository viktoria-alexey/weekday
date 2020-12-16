using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weekday.Data.Models;

namespace Weekday.Data.Interfaces
{
    public interface IAccountManager
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId);

        Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);

        Task<IReadOnlyCollection<IdentityRole>> GetAllRolesAsync();

        Task<IdentityRole> GetRoleByIdAsync(string roleId);

        Task<IdentityRole> GetRoleByNameAsync(string roleName);

        Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(IdentityRole role);

        Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password);

        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user);

        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string newPassword);

        Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId);

        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user);
    }
}
