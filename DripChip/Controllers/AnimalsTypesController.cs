using DripChip.Core.Serialization;
using DripChip.DataBase;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("animals/types")]
    public class AnimalsTypesController : BaseController<AnimalsType>
    {
        protected internal override string PathToCurrentEntities => "DataBase/AnimalsTypes.json";

        private IAsyncSerializer<IEnumerable<AnimalsType>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<AnimalsType>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public AnimalsTypesController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<AnimalsType>>()
            { Path = Path.Combine(Environment.CurrentDirectory, PathToCurrentEntities) };

            InitializeEntities();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalsType>> GetAnimalTypes(long? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            IEnumerable<AnimalsType>? animalsTypes = await new GetEntities<AnimalsType>(Serializer).ReceiveEnumerable();
            if (animalsTypes != null)
            {
                AnimalsType? animalsType = animalsTypes.FirstOrDefault(t => t.Id == id);
                if (animalsType != null)
                {
                    return Ok(animalsType);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<AnimalsType>> AddAnimalsType([FromBody] JsonObject type)
        {
            string? stype = string.Empty;
            type.TryGetPropertyValue("type", out JsonNode? jsonNode);
            jsonNode?.AsValue().TryGetValue(out stype);

            if (string.IsNullOrWhiteSpace(stype))
                return BadRequest();

            IEnumerable<AnimalsType>? animalsTypes = await new GetEntities<AnimalsType>(Serializer).ReceiveEnumerable();

            if (animalsTypes != null)
            {
                if (!animalsTypes.Any(t => t.Type.ToLower().Contains(stype.ToLower())))
                {
                    AnimalsType animalsType = new() { Id = SetNewId(), Type = stype };
                    Entities.Add(animalsType);
                    //Сохранить новый тип в файл
                    return Ok(animalsType);
                }

                return Conflict();
            }

            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AnimalsType>> UpdateAnimalsType(long? id, [FromBody] string? type)
        {
            if (id == null || id <= 0)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(type))
                return BadRequest();

            IEnumerable<AnimalsType>? animalsTypes = await new GetEntities<AnimalsType>(Serializer).ReceiveEnumerable();

            if (animalsTypes != null)
            {
                AnimalsType? animalsType = animalsTypes.FirstOrDefault(t => t.Id == id);
                if (animalsType != null)
                {
                    if (!animalsTypes.Any(t => t.Type.ToLower().Contains(type.ToLower())))
                    {
                        animalsType.Type = type;
                        //Сохранить изменение типа в файл
                        return Ok(animalsType);
                    }

                    return Conflict();
                }
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> DeleteAnimalsType(long? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            if (!TypeNotUsed(id))
                return BadRequest();

            IEnumerable<AnimalsType>? animalsTypes = await new GetEntities<AnimalsType>(Serializer).ReceiveEnumerable();

            if (animalsTypes != null)
            {
                AnimalsType? animalsType = animalsTypes.FirstOrDefault(t => t.Id == id);
                if (animalsType != null)
                {
                    Entities.Remove(animalsType);
                    //Удалить из файла
                    return Ok();
                }
            }

            return NotFound();
        }

        private long SetNewId() => Entities.Select(x => x.Id).Max() + 1;
        private static bool TypeNotUsed(long? id, IEnumerable<Animal>? a = null)
        {
            IEnumerable<Animal>? animals = a;

            if (a == null)
            {
                JsonAsyncSerializer<IEnumerable<Animal>> animalsSerializer = new() { Path = Path.Combine(Environment.CurrentDirectory, "DataBase/Animals.json") };
                animals = new GetEntities<Animal>(animalsSerializer).ReceiveEnumerable().Result;
            }

            if (animals != null)
                return !animals.Any(a => a.AnimalTypes.Any(typeId => typeId == id));

            return false;
        }
    }
}