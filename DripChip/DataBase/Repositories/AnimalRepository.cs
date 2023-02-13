using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly DataContext _context;

        public AnimalRepository(DataContext context) => _context = context;

        public async Task Create(Animal animal)
        {
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<Animal> Delete(long id)
        {
            Animal current = Get(id)!;
            _context.Animals.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }

        public Animal? Get(long id) => _context.Animals.Find(id);

        public IEnumerable<Animal> GetAll() => _context.Animals;

        public async Task Update(Animal animal)
        {
            Animal current = Get(animal.Id)!;
            current.UpdateProperties(animal);
            _context.Animals.Update(current);
            await _context.SaveChangesAsync();
        }
    }
}