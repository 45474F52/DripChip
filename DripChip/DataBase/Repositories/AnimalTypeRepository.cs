using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public class AnimalTypeRepository : IAnimalTypeRepository
    {
        private readonly DataContext _context;

        public AnimalTypeRepository(DataContext context) => _context = context;

        public async Task Create(AnimalType animalType)
        {
            _context.AnimalsTypes.Add(animalType);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalType> Delete(long id)
        {
            AnimalType current = Get(id)!;
            _context.AnimalsTypes.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }

        public AnimalType? Get(long id) => _context.AnimalsTypes.Find(id);

        public IEnumerable<AnimalType> GetAll() => _context.AnimalsTypes;

        public async Task Update(AnimalType animalType)
        {
            AnimalType current = Get(animalType.Id)!;

            current.Type = animalType.Type;

            _context.AnimalsTypes.Update(current);
            await _context.SaveChangesAsync();
        }
    }
}