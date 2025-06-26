using FSI.MealTracker.Infrastructure.Context;
using System.Data;

namespace FSI.MealTracker.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        private readonly IDbContext _context;

        protected BaseRepository(IDbContext context)
        {
            _context = context;
        }

        protected IDbConnection CreateConnection()
        {
            return _context.CreateConnection();
        }
    }
}