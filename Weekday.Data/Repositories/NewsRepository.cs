using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weekday.Data.Interfaces;
using Weekday.Data.Models;

namespace Weekday.Data.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(DbContext context) : base(context)
        {
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;


        public async Task<List<News>> GetNewsAsync(string authorId, int page, int pageSize)
        {
            IQueryable<News> newsQuery = _context.Set<News>()
                .OrderByDescending(x => x.CreateDate);

            if (!string.IsNullOrEmpty(authorId))
            {
                newsQuery = newsQuery.Where(x => x.AuthorId == authorId);
            }

            if (page != -1)
            {
                newsQuery = newsQuery.Skip((page - 1) * pageSize);
            }

            if (pageSize != -1)
            {
                newsQuery = newsQuery.Take(pageSize);
            }

            return await newsQuery.ToListAsync();
        }
    }
}
