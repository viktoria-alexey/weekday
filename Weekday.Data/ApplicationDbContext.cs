using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Weekday.Data.Models;

namespace Weekday.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
    {
        public virtual DbSet<News> CompanyNews { get; set; }
        public async Task DatabaseMigrateAsync()
        {
            await Database.MigrateAsync().ConfigureAwait(false);
        }

        public async Task<bool> AnyUsersAsync()
        {
            return await Users.AnyAsync();
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();
        }

        public async Task<string[]> FilterRoles(IList<string> userRoleIds)
        {
            return await Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return Users
                .Include(u => u.Roles)
                .OrderBy(u => u.UserName).ToList();
        }

        public async Task<IdentityRole[]> GetRoleFiltered(IList<string> userRoleIds)
        {
            return await Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();
            
        }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasOne(u => u.Manager).WithMany(x => x.Subordinates).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<News>().HasOne(n => n.Author).WithMany(u => u.News).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}