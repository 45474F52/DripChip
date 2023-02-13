using DripChip.Core.Helper;
using DripChip.DataBase.Repositories;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("animals/types")]
    public class AnimalsTypesController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IAnimalTypeRepository _animalTypeRepository;
        private readonly List<AnimalType> _animalTypes;

        public AnimalsTypesController(IAnimalRepository animalRepository, IAnimalTypeRepository animalTypeRepository)
        {
            _animalRepository = animalRepository;
            _animalTypeRepository = animalTypeRepository;
            _animalTypes = _animalTypeRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<AnimalType> GetAnimalTypes(long? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            AnimalType? animalsType = _animalTypes.FirstOrDefault(t => t.Id == id);
            return animalsType != null ? Ok(animalsType) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<AnimalType>> AddAnimalType([FromBody] JsonObject data)
        {
            string? type = JsonObjectParser<string>.Parse(data, nameof(type));

            if (string.IsNullOrWhiteSpace(type))
                return BadRequest();

            if (!_animalTypes.Any(t => t.Type.ToLower().Contains(type.ToLower())))
            {
                AnimalType animalType = new() { Id = SetNewId(), Type = type };
                _animalTypes.Add(animalType);
                await _animalTypeRepository.Create(animalType);
                return Ok(animalType);
            }

            return Conflict();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AnimalType>> UpdateAnimalsType(long? id, [FromBody] JsonObject data)
        {
            string? type = JsonObjectParser<string>.Parse(data, nameof(type));

            if (id == null || id <= 0 || string.IsNullOrWhiteSpace(type))
                return BadRequest();

            AnimalType? animalType = _animalTypes.FirstOrDefault(t => t.Id == id);
            if (animalType != null)
            {
                int index = _animalTypes.IndexOf(animalType);
                if (!_animalTypes.Any(t => t.Type.ToLower().Contains(type.ToLower())))
                {
                    _animalTypes.ElementAt(index).Type = type;
                    await _animalTypeRepository.Update(animalType);
                    return Ok(_animalTypes.ElementAt(index));
                }

                return Conflict();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> DeleteAnimalsType(long? id)
        {
            if (id == null || id <= 0 || !TypeNotUsed(id))
                return BadRequest();

            AnimalType? animalType = _animalTypes.FirstOrDefault(t => t.Id == id);
            if (animalType != null)
            {
                _animalTypes.Remove(animalType);
                await _animalTypeRepository.Delete((long)id);
                return Ok();
            }

            return NotFound();
        }

        private long SetNewId() => _animalTypes.Select(x => x.Id).Max() + 1;

        private bool TypeNotUsed(long? id, IEnumerable<Animal>? a = null)
        {
            IEnumerable<Animal>? animals = a;

            if (a == null)
                animals = _animalRepository.GetAll();

            if (animals != null)
                return !animals.Any(a => a.AnimalTypes.Any(typeId => typeId.AnimalTypeId == id));

            return false;
        }
    }
}