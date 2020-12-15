using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Weekday.Data.Models;

namespace Weekday.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<News> CompanyNews { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasOne(u => u.Manager).WithMany(x => x.Subordinates).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<News>().HasOne(n => n.Author).WithMany(u => u.News).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}