using DripChip.Core.Helper;
using DripChip.DataBase.Repositories;
using DripChip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Animals")]
    public sealed class AnimalsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILocationPointRepository _locationPointRepository;
        private readonly IAnimalTypeRepository _animalTypesRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly List<Animal> _animals;

        public AnimalsController(IAccountRepository accountRepository,
                                 ILocationPointRepository locationPointRepository,
                                 IAnimalTypeRepository animalTypesRepository,
                                 IAnimalRepository animalRepository)
        {
            _accountRepository = accountRepository;
            _locationPointRepository = locationPointRepository;
            _animalTypesRepository = animalTypesRepository;
            _animalRepository = animalRepository;

            _animals = _animalRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Animal> GetAnimal(long? id)
        {
            if (_animals == null)
                return Unauthorized();

            if (id == null || id <= 0)
                return BadRequest();

            Animal? animal = _animals.FirstOrDefault(a => a.Id == id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpGet("{id}/locations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AnimalVisitedLocations>>> GetAnimalVisitedLocations(long? id,
            [FromQuery] string startDateTime,
            [FromQuery] string endDateTime,
            [FromQuery] int? from,
            [FromQuery] int? size)
        {
            throw new NotImplementedException();
            //if (id == null || id <= 0)
            //    return BadRequest();

            //if (!ValidateFromSize(from, size))
            //    return BadRequest();

            //IEnumerable<long> visitedLocationsId = _animals.Single(a => a.Id == id).VisitedLocations;
            ///////
            //JsonAsyncSerializer<IEnumerable<AnimalVisitedLocations>> vlSerializer =
            //    JsonAsyncSerializerFactory<AnimalVisitedLocations, IEnumerable<AnimalVisitedLocations>>.Create();

            //IEnumerable<AnimalVisitedLocations>? visitedLocations = await new GetEntities<AnimalVisitedLocations>(vlSerializer).ReceiveEnumerable();
            ///////
            //if (visitedLocations != null)
            //{
            //    IEnumerable<AnimalVisitedLocations> animalVisitedLocations = new List<AnimalVisitedLocations>();

            //    foreach (var vl in visitedLocationsId.SelectMany(vlId => visitedLocations.Where(vl => vlId == vl.Id)))
            //    {
            //        animalVisitedLocations = animalVisitedLocations.Append(vl);
            //    }

            //    visitedLocations = visitedLocations.Skip(from ?? 0);
            //    visitedLocations = visitedLocations.Take(size ?? 10);
            //    visitedLocations = visitedLocations.OrderBy(a => a.Id);
            //}

            //return Ok(visitedLocations);
        }

        [HttpGet("search")]
        [Authorize]
        public ActionResult<IEnumerable<Animal>> SearchAnimals(
            [FromQuery] string startDateTime,
            [FromQuery] string endDateTime,
            [FromQuery] int chipperId,
            [FromQuery] long chippingLocationId,
            [FromQuery] string lifeStatus,
            [FromQuery] string gender,
            [FromQuery] int? from,
            [FromQuery] int? size)
        {
            if (!ValidateFromSize(from, size)
                || !Validator.IsValidEnum(typeof(Gender), gender) || !Validator.IsValidEnum(typeof(LifeStatus), lifeStatus)
                || !Validator.IsValidNums(n => n > 0, chipperId) || !Validator.IsValidNums(n => n > 0, chippingLocationId))
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

            Animal sendedAnimal = new()
            {
                ChipperId = chipperId,
                ChippingLocationId = chippingLocationId,
                LifeStatus = (LifeStatus)Enum.Parse(typeof(LifeStatus), lifeStatus),
                Gender = (Gender)Enum.Parse(typeof(Gender), gender)
            };

            IEnumerable<Animal> animals = new List<Animal>();

            foreach (var animal in _animals)
            {
                //DateTime parsedTime = DateTime.ParseExact(account.Model.ChippingDateTime, "O", null, System.Globalization.DateTimeStyles.RoundtripKind);
                //if (parsedTime >= start && parsedTime <= end)
                //{
                //    if (account.Model.Contains(sendedFilterModel))
                //    {
                //        filterModels = filterModels.Append(account.Model);
                //    }
                //}

                if (animal.Contains(sendedAnimal))
                    animals = animals.Append(animal);
            }

            animals = animals.Skip(from ?? 0);
            animals = animals.Take(size ?? 10);
            animals = animals.OrderBy(a => a.Id);

            return Ok(animals);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Animal>> AddAnimal([FromBody] JsonObject data)
        {
            long?[]? animalTypes = JsonObjectParser<long?[]>.Parse(data, nameof(animalTypes));
            #region Alternative
            //Dictionary<string, float> values = JsonObjectParser<float>.ParseObjects(data, new string[3] { "weight", "length", "height" });
            //float? weight = values[nameof(weight)];
            //float? length = values[nameof(length)];
            //float? height = values[nameof(height)];
            #endregion Alternative
            float? weight = JsonObjectParser<float>.Parse(data, nameof(weight));
            float? length = JsonObjectParser<float>.Parse(data, nameof(length));
            float? height = JsonObjectParser<float>.Parse(data, nameof(height));
            Enum.TryParse(typeof(Gender), JsonObjectParser<string>.Parse(data, "gender") ?? string.Empty, out object? gender);
            int? chipperId = JsonObjectParser<int>.Parse(data, nameof(chipperId));
            long? chippingLocationId = JsonObjectParser<long>.Parse(data, nameof(chippingLocationId));

            if (Validator.IsNotNull(animalTypes, weight, length, height, gender, chipperId, chippingLocationId) && AnimalTypesIsValid(animalTypes!))
            {
                if (Validator.IsValidNums(value => value > 0, (float)weight, (float)length, (float)height) && chipperId > 0 && chippingLocationId > 0)
                {
                    if (Validator.IsValidEnum(typeof(Gender), gender))
                    {
                        IEnumerable<AnimalType> animalsTypesDB = _animalTypesRepository.GetAll();

                        if (animalsTypesDB.Select(t => t.Id).Except(animalTypes!.Cast<long>()).Any())
                            return NotFound();

                        IEnumerable<Account> accountsDB = _accountRepository.GetAll();

                        if (!accountsDB!.Select(a => a.Id).Contains((int)chipperId))
                            return NotFound();

                        IEnumerable<LocationPoint> locationPointsDB = _locationPointRepository.GetAll();

                        if (!locationPointsDB.Select(l => l.Id).Contains((long)chippingLocationId))
                            return NotFound();

                        long animalId = SetNewId();
                        List<AnimalTypeOnAnimal> animalTypeOnAnimals = new();

                        for (int i = 0; i < animalTypes!.Length; i++)
                        {
                            animalTypeOnAnimals.Add(new AnimalTypeOnAnimal()
                            {
                                AnimalId = animalId,
                                AnimalTypeId = (long)animalTypes[i]!
                            });
                        }

                        Animal animal = new()
                        {
                            Id = animalId,
                            AnimalTypes = animalTypeOnAnimals,
                            Weight = (float)weight,
                            Length = (float)length,
                            Height = (float)height,
                            Gender = (Gender)gender!,
                            ChipperId = (int)chipperId,
                            ChippingLocationId = (int)chippingLocationId,
                            VisitedLocations = new HashSet<VisitedLocationOnAnimal>()
                        };

                        _animals.Add(animal);
                        await _animalRepository.Create(animal);
                        return CreatedAtAction(nameof(AddAnimal), animal);
                    }
                }
            }
            return BadRequest();
        }

        private long SetNewId() => _animals.Select(x => x.Id).Max() + 1;
        public bool ValidateFromSize(int? from, int? size) => from != null && size != null && from >= 0 && size > 0;
        private static bool AnimalTypesIsValid(long?[] animalTypes)
        {
            if (animalTypes.Length > 0)
            {
                foreach (long? type in animalTypes)
                {
                    if (type is not null && type > 0)
                        return true;
                }
            }
            return false;
        }
    }
}