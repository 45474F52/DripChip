using DripChip.Core.Serialization;
using DripChip.DataBase;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Locations")]
    public sealed class LocationsController : BaseController<LocationPoint>
    {
        protected internal override string PathToCurrentEntities => "DataBase/LocationPoints.json";

        private IAsyncSerializer<IEnumerable<LocationPoint>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<LocationPoint>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public LocationsController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<LocationPoint>>()
            { Path = Path.Combine(Environment.CurrentDirectory, PathToCurrentEntities) };

            InitializeEntities();
        }

        [HttpGet("{id}")]
        public ActionResult<LocationPoint> GetLocationPoint(int? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            LocationPoint? locationPoint = Entities.FirstOrDefault(a => a.Id == id);
            if (locationPoint == null)
                return NotFound();

            return Ok(locationPoint);
        }

        [HttpPost]
        public ActionResult<LocationPoint> AddLocationPoint([FromBody] (double? latitude, double? longitude) param)
        {
            if (!ValidateRequestDatas(param.latitude, param.longitude))
                return BadRequest();

            if (Entities.FirstOrDefault(p => p.Latitude == param.latitude && p.Longitude == param.longitude) != null)
                return Conflict();

            LocationPoint locationPoint = new() { Id = SetNewId(), Latitude = (double)param.latitude!, Longitude = (double)param.longitude! };
            //Сохранить новую точку в файл
            return CreatedAtAction(nameof(AddLocationPoint), locationPoint);
        }

        [HttpPut("{id}")]
        public ActionResult<LocationPoint> UpdateLocationPoint(long? id, [FromBody] (double? latitude, double? longitude) param)
        {
            if (id == null || id <= 0 && !ValidateRequestDatas(param.latitude, param.longitude))
                return BadRequest();

            LocationPoint? locationPoint = Entities.FirstOrDefault(p => p.Id == id);

            if (locationPoint == null)
                return NotFound();

            if (Entities.FirstOrDefault(p => p.Latitude == param.latitude && p.Longitude == param.longitude) != null)
                return Conflict();

            locationPoint.Longitude = (double)param.longitude!;
            locationPoint.Latitude = (double)param.latitude!;
            //Сохранить изменённую точку в файле
            return Ok(locationPoint);
        }

        [HttpDelete("{id}")]
        public StatusCodeResult DeleteLocationPoint(long? id)
        {
            if (id != null && id > 0)
            {
                if (PointNotUsed(id))
                {
                    LocationPoint? locationPoint = Entities.FirstOrDefault(p => p.Id == id);

                    if (locationPoint == null)
                        return NotFound();

                    Entities.Remove(locationPoint);
                    //Сохранить изменения в файле
                    return Ok();
                }
            }

            return BadRequest();
        }

        public static bool PointNotUsed(long? id, IEnumerable<Animal>? a = null)
        {
            IEnumerable<Animal>? animals = a;

            if (a == null)
            {
                JsonAsyncSerializer<IEnumerable<Animal>> animalsSerializer = new() { Path = Path.Combine(Environment.CurrentDirectory, "DataBase/Animals.json") };
                animals = new GetEntities<Animal>(animalsSerializer).ReceiveEnumerable().Result;
            }

            if (animals != null)
                return !animals.Any(a => a.VisitedLocations.Any(locationId => locationId == id));

            return false;
        }

        private long SetNewId() => Entities.Select(x => x.Id).Max() + 1;

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