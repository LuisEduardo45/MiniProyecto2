using Domain;

namespace Infrastructure.Repositories.IRepositories
{
    public interface IRepository
    {
        Task AddMilk(Milk milk);
        Task<List<Milk>> GetAllMilks();
    }
}
