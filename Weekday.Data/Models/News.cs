using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Weekday.Data.Models
{
    public class News
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Content { get; set; }

        public DateTime CreateDate { get; set; }

        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
    }
}
