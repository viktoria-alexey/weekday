using System;

namespace Weekday.DataContracts
{
    public class NewsDataContract
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreateDate { get; set; }

        public string AuthorId { get; set; }

        public UserDataContract Author { get; set; }
    }
}
