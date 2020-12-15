using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Weekday.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ManagerId { get; set; }
        public ApplicationUser Manager { get; set; }

        public virtual ICollection<ApplicationUser> Subordinates { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}