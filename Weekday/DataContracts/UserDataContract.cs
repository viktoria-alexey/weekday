using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Weekday.DataContracts
{
    public class UserDataContract
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Username is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 200 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public ICollection<string> RoleIds { get; set; }

        public string NewPassword { get; set; }

        public string CurrentPassword { get; set; }
    }
}
