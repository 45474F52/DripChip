using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public interface IAnimalRepository
    {
        IEnumerable<Animal> GetAll();
        Animal? Get(long id);
        Task Create(Animal animal);
        Task Update(Animal animal);
        Task<Animal> Delete(long id);
    }
}