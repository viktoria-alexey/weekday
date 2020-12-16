using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Weekday.Data.Models;

namespace Weekday.Data
{
    public interface IApplicationDbContext
    {
        DbSet<News> CompanyNews { get; set; }
        Task DatabaseMigrateAsync();
        Task<bool> AnyUsersAsync();
        Task<ApplicationUser> GetUserById(string userId);
        Task<string[]> FilterRoles(IList<string> userRoleIds);
        IEnumerable<ApplicationUser> GetUsers();
        Task<IdentityRole[]> GetRoleFiltered(IList<string> userRoleIds);
    }
}