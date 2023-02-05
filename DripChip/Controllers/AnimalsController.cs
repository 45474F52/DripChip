using DripChip.Core.Serialization;
using DripChip.DataBase;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Animals")]
    public sealed class AnimalsController : BaseController<Animal>
    {
        protected internal override string PathToCurrentEntities => "DataBase/Animals.json";
        private static string VisitedLocationsPath => "DataBase/AnimalsVisitedLocations.json";
        private static string LocationPointsPath => "Database/LocationPoints.json";

        private IAsyncSerializer<IEnumerable<Animal>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<Animal>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public AnimalsController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<Animal>>()
            { Path = Path.Combine(Environment.CurrentDirectory, PathToCurrentEntities) };

            InitializeEntities();
            foreach (var animal in Entities)
                animal.InitializeFilterModel();
        }

        [HttpGet("{id}")]
        public ActionResult<Animal> GetAnimal(long? id)
        {
            if (Entities == null)
                return Unauthorized();

            if (id == null || id <= 0)
                return BadRequest();

            Animal? animal = Entities.FirstOrDefault(a => a.Id == id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpGet("{id}/locations")]
        public async Task<ActionResult<IEnumerable<AnimalVisitedLocations>>> GetAnimalVisitedLocations(
            long? id,
            [FromQuery] string startDateTime,
            [FromQuery] string endDateTime,
            [FromQuery] int? from,
            [FromQuery] int? size)
        {
            if (Entities == null)
                return Unauthorized();

            if (id == null || id <= 0)
                return BadRequest();

            if (from < 0 && from == null && size <= 0 && size == null)
                return BadRequest();

            IEnumerable<long> visitedLocationsId = Entities.Single(a => a.Id == id).VisitedLocations;
            /////
            JsonAsyncSerializer<IEnumerable<AnimalVisitedLocations>> vlSerializer = new()
            { Path = Path.Combine(Environment.CurrentDirectory, VisitedLocationsPath) };

            IEnumerable<AnimalVisitedLocations>? visitedLocations = await new GetEntities<AnimalVisitedLocations>(vlSerializer).ReceiveEnumerable();
            /////
            if (visitedLocations != null)
            {
                IEnumerable<AnimalVisitedLocations> animalVisitedLocations = new List<AnimalVisitedLocations>();

                foreach (var vl in visitedLocationsId.SelectMany(vlId => visitedLocations.Where(vl => vlId == vl.Id)))
                {
                    animalVisitedLocations = animalVisitedLocations.Append(vl);
                }

                visitedLocations = visitedLocations.Skip(from ?? 0);
                visitedLocations = visitedLocations.Take(size ?? 10);
                visitedLocations = visitedLocations.OrderBy(a => a.Id);
            }

            return Ok(visitedLocations);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<AnimalsFilterModel>> SearchAnimals(
            [FromQuery] string startDateTime,
            [FromQuery] string endDateTime,
            [FromQuery] int chipperId,
            [FromQuery] long chippingLocationId,
            [FromQuery] string lifeStatus,
            [FromQuery] string gender,
            [FromQuery] int? from,
            [FromQuery] int? size)
        {
            if (Entities == null)
                return Unauthorized();

            if (!ValidateRequestDatas(chipperId, chippingLocationId, lifeStatus, gender, from, size))
                return BadRequest();

            //DateTime start;
            //DateTime end;
            //if (DateTime.TryParseExact(startDateTime, "O", null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime outStart)
            //    && DateTime.TryParseExact(endDateTime, "O", null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime outEnd))
            //{
            //    start = outStart;
            //    end = outEnd;
            //}
            //else
            //    return BadRequest();

            AnimalsFilterModel sendedFilterModel = new()
            {
                ChipperId = chipperId,
                ChippingLocationId = chippingLocationId,
                LifeStatus = (LifeStatus)Enum.Parse(typeof(LifeStatus), lifeStatus),
                Gender = (Gender)Enum.Parse(typeof(Gender), gender)
            };

            IEnumerable<AnimalsFilterModel> filterModels = new List<AnimalsFilterModel>();

            foreach (var animal in Entities)
            {
                //DateTime parsedTime = DateTime.ParseExact(account.Model.ChippingDateTime, "O", null, System.Globalization.DateTimeStyles.RoundtripKind);
                //if (parsedTime >= start && parsedTime <= end)
                //{
                //    if (account.Model.Contains(sendedFilterModel))
                //    {
                //        filterModels = filterModels.Append(account.Model);
                //    }
                //}

                if (animal.Model.Contains(sendedFilterModel))
                {
                    filterModels = filterModels.Append(animal.Model);
                }
            }

            filterModels = filterModels.Skip(from ?? 0);
            filterModels = filterModels.Take(size ?? 10);
            filterModels = filterModels.OrderBy(a => a.Id);

            return Ok(filterModels);
        }

        private long SetNewId() => Entities.Select(x => x.Id).Max() + 1;
        private static bool ValidateDate(string[] dates, string pattern) => throw new NotImplementedException();
        private bool ValidateRequestDatas(int chipperId, long chippingLocationId, string lifeStatus, string gender, int? from, int? size)
        {
            if (ValidateFromSize(from, size))
            {
                if (chipperId > 0 & chippingLocationId > 0)
                {
                    if (Enum.IsDefined(typeof(LifeStatus), lifeStatus))
                    {
                        if (Enum.IsDefined(typeof(Gender), gender))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}