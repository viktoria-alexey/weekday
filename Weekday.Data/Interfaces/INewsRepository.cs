using System.Collections.Generic;
using System.Threading.Tasks;
using Weekday.Data.Models;

namespace Weekday.Data.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        Task<List<News>> GetNewsAsync(string authorId, int page, int pageSize);
    }
}
