using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public class LocationPointRepository : ILocationPointRepository
    {
        private readonly DataContext _context;

        public LocationPointRepository(DataContext context) => _context = context;

        public async Task Create(LocationPoint locationPoint)
        {
            _context.LocationPoints.Add(locationPoint);
            await _context.SaveChangesAsync();
        }

        public async Task<LocationPoint> Delete(long id)
        {
            LocationPoint current = Get(id)!;
            _context.LocationPoints.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }

        public LocationPoint? Get(long id) => _context.LocationPoints.Find(id);

        public IEnumerable<LocationPoint> GetAll() => _context.LocationPoints;

        public async Task Update(LocationPoint locationPoint)
        {
            LocationPoint current = Get(locationPoint.Id)!;

            current.Longitude = locationPoint.Longitude;
            current.Latitude = locationPoint.Latitude;

            _context.LocationPoints.Update(current);
            await _context.SaveChangesAsync();
        }
    }
}