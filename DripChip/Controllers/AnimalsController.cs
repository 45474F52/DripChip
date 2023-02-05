using DripChip.Core.Serialization;
using DripChip.DataBase;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Animals")]
    public sealed class AnimalsController : BaseController<Animal>
    {
        protected internal override string CurrentPath => "DataBase/Animals.json";

        private IAsyncSerializer<IEnumerable<Animal>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<Animal>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public AnimalsController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<Animal>>(new System.Text.Json.JsonSerializerOptions()
            { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })
            { Path = Path.Combine(Environment.CurrentDirectory, CurrentPath) };

            InitializeEntities();
            foreach (var animal in Entities)
            {
                animal.InitializeFilterModel();
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Animal>> GetAnimals() => Get();

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
        public async Task<ActionResult<IEnumerable<AnimalVisitedLocations>>> GetAnimalVisitedLocations(long? id,
            string startDateTime,
            string endDateTime,
            int? from,
            int? size)
        {
            if (Entities == null)
                return Unauthorized();

            if (id == null || id <= 0)
                return BadRequest();

            if (from >= 0 && from != null && size > 0 && size != null)
                return BadRequest();

            IEnumerable<long> visitedLocationsId = Entities.Single(a => a.Id == id).VisitedLocations;

            JsonAsyncSerializer<IEnumerable<AnimalVisitedLocations>> serializer = new(new System.Text.Json.JsonSerializerOptions()
            { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })
            { Path = Path.Combine(Environment.CurrentDirectory, CurrentPath) };

            IEnumerable<AnimalVisitedLocations>? visitedLocations = await new GetEntities<AnimalVisitedLocations>(serializer).Receive();
            
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
        public ActionResult<IEnumerable<AnimalsFilterModel>> SearchAccounts(
            string startDateTime,
            string endDateTime,
            int chipperId,
            long chippingLocationId,
            string lifeStatus,
            string gender,
            int? from,
            int? size)
        {
            if (Entities == null)
                return Unauthorized();

            if (!Validate(
                chipperId,
                chippingLocationId,
                lifeStatus,
                gender,
                from,
                size))
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
                LifeStatus = lifeStatus,
                Gender = gender
            };

            IEnumerable<AnimalsFilterModel> filterModels = new List<AnimalsFilterModel>();

            foreach (var account in Entities)
            {
                //DateTime parsedTime = DateTime.ParseExact(account.Model.ChippingDateTime, "O", null, System.Globalization.DateTimeStyles.RoundtripKind);
                //if (parsedTime >= start && parsedTime <= end)
                //{
                //    if (account.Model.Contains(sendedFilterModel))
                //    {
                //        filterModels = filterModels.Append(account.Model);
                //    }
                //}

                if (account.Model.Contains(sendedFilterModel))
                {
                    filterModels = filterModels.Append(account.Model);
                }
            }

            filterModels = filterModels.Skip(from ?? 0);
            filterModels = filterModels.Take(size ?? 10);
            filterModels = filterModels.OrderBy(a => a.Id);

            return Ok(filterModels);
        }

        private static bool ValidateData(string[] dates, string pattern) => throw new NotImplementedException();
        private static bool Validate(int chipperId, long chippingLocationId, string lifeStatus, string gender, int? from, int? size)
        {
            if (from >= 0 && from != null && size > 0 && size != null)
            {
                if (chipperId > 0 & chippingLocationId > 0)
                {
                    if (lifeStatus == "ALIVE" || lifeStatus == "DEAD")
                    {
                        if (gender == "MALE" || gender == "FEMALE" || gender == "OTHER")
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