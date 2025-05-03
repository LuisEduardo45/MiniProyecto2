
using Infrastructure.Repositories;

namespace Infrastructure.Persistence.Repositories
{
    public class BaseRepository
    {
        public readonly ApplicationDbContext context;


        public BaseRepository(ApplicationDbContext _context)
        {
            context = _context;

        }

        public void SaveAsync()
        {
            context.SaveChanges();
        }
        public void Begin()
        {
            context.Database.BeginTransaction();
        }
        public void Commit()
        {
            context.Database.CommitTransaction();
        }
        public void Rollback()
        {
            context.Database.RollbackTransaction();
        }

    }
}
