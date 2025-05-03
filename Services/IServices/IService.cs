using Services.Dtos;

namespace Services.IServices
{
    public interface IService
    {
        Task AddMilk(int v, DateTime now);
        Task<List<MilkModel>> GetAllMilks();
    }
}
