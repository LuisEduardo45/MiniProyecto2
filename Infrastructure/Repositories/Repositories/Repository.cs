using Domain;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Repositories
{
    public class Repository : BaseRepository, IRepository
    {
        public Repository(ApplicationDbContext _context) : base(_context)
        {
        }

        public async Task AddMilk(Milk milk)
        {
            try
            {
                Begin();
                context.Milks.Add(milk);
                await context.SaveChangesAsync();
                Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding milk", ex);
            }
        }

        public Task<List<Milk>> GetAllMilks()
        {

            return context.Milks.ToListAsync();
        }
    }
}
