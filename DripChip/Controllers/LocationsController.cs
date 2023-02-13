using DripChip.Core.Helper;
using DripChip.DataBase.Repositories;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Locations")]
    public sealed class LocationsController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly ILocationPointRepository _locationPointRepository;
        private readonly List<LocationPoint> _locationPoints;

        public LocationsController(IAnimalRepository animalRepository, ILocationPointRepository locationPointRepository)
        {
            _animalRepository = animalRepository;
            _locationPointRepository = locationPointRepository;
            _locationPoints = _locationPointRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<LocationPoint> GetLocationPoint(int? id)
        {
            if (id != null && id > 0)
            {
                LocationPoint? locationPoint = _locationPoints.FirstOrDefault(a => a.Id == id);
                return locationPoint != null ? Ok(locationPoint) : NotFound();
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<LocationPoint>> AddLocationPoint([FromBody] JsonObject data)
        {
            double? latitude = JsonObjectParser<double>.Parse(data, nameof(latitude));
            double? longitude = JsonObjectParser<double>.Parse(data, nameof(longitude));

            if (ValidateRequestDatas(latitude, longitude))
            {
                if (_locationPoints.FirstOrDefault(p => p.Latitude == latitude && p.Longitude == longitude) == null)
                {
                    LocationPoint locationPoint = new()
                    {
                        Latitude = (double)latitude!,
                        Longitude = (double)longitude!
                    };

                    _locationPoints.Add(locationPoint);
                    await _locationPointRepository.Create(locationPoint);

                    return CreatedAtAction(nameof(AddLocationPoint), locationPoint);
                }

                return Conflict();
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LocationPoint>> UpdateLocationPoint(long? id, [FromBody] JsonObject data)
        {
            double? latitude = JsonObjectParser<double>.Parse(data, nameof(latitude));
            double? longitude = JsonObjectParser<double>.Parse(data, nameof(longitude));

            if (id != null && (id > 0 || ValidateRequestDatas(latitude, longitude)))
            {
                LocationPoint? locationPoint = _locationPoints.FirstOrDefault(p => p.Id == id);

                if (locationPoint != null)
                {
                    if (_locationPoints.FirstOrDefault(p => p.Latitude == latitude && p.Longitude == longitude) == null)
                    {
                        if (ValidateRequestDatas(latitude, longitude))
                        {
                            int index = _locationPoints.IndexOf(locationPoint);

                            _locationPoints.ElementAt(index).Longitude = (double)longitude!;
                            _locationPoints.ElementAt(index).Latitude = (double)latitude!;
                            await _locationPointRepository.Update(_locationPoints.ElementAt(index));

                            return Ok(locationPoint);
                        }

                        return BadRequest();
                    }

                    return Conflict();
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> DeleteLocationPoint(long? id)
        {
            if (id != null && id > 0)
            {
                if (PointNotUsed(id))
                {
                    LocationPoint? locationPoint = _locationPoints.FirstOrDefault(p => p.Id == id);

                    if (locationPoint == null)
                        return NotFound();

                    _locationPoints.Remove(locationPoint);
                    await _locationPointRepository.Delete((long)id);
                    return Ok();
                }
            }

            return BadRequest();
        }

        private long SetNewId() => _locationPoints.Select(x => x.Id).Max() + 1;

        public bool PointNotUsed(long? id, IEnumerable<Animal>? a = null)
        {
            IEnumerable<Animal>? animals = a;

            if (a == null)
                animals = _animalRepository.GetAll();

            if (animals != null)
                return !animals.Any(a => a.VisitedLocations.Any(locationId => locationId.VisitedLocationId == id));

            return false;
        }

        private static bool ValidateRequestDatas(double? latitude, double? longitude)
        {
            if (latitude != null && longitude != null)
            {
                if (latitude >= -90 && latitude <= 90)
                {
                    if (longitude >= -180 && longitude <= 180)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}