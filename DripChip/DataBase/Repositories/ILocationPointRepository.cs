using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public interface ILocationPointRepository
    {
        IEnumerable<LocationPoint> GetAll();
        LocationPoint? Get(long id);
        Task Create(LocationPoint locationPoint);
        Task Update(LocationPoint locationPoint);
        Task<LocationPoint> Delete(long id);
    }
}