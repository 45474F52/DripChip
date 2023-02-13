using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public interface IAnimalTypeRepository
    {
        IEnumerable<AnimalType> GetAll();
        AnimalType? Get(long id);
        Task Create(AnimalType animalType);
        Task Update(AnimalType animalType);
        Task<AnimalType> Delete(long id);
    }
}