using Weekday.Data.Interfaces;
using Weekday.Data.Repositories;

namespace Weekday.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        private INewsRepository _news;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public INewsRepository News
        {
            get
            {
                if (_news == null)
                {
                    _news = new NewsRepository(_context);
                }

                return _news;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
