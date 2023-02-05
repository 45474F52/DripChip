using DripChip.Core.Serialization;
using DripChip.DataBase;
using Microsoft.AspNetCore.Mvc;

namespace DripChip.Controllers
{
    public abstract class BaseController<T> : ControllerBase where T : new()
    {
        protected internal List<T> Entities { get; set; } = new List<T>();
        protected internal abstract string PathToCurrentEntities { get; }
        protected internal abstract IAsyncSerializer<IEnumerable<T>> Serializer { get; set; }

        protected internal void InitializeEntities()
        {
            IEnumerable<T>? entities = new GetEntities<T>(Serializer).ReceiveEnumerable().Result;

            if (entities != null)
            {
                Entities = entities.ToList();
            }
        }

        public virtual ActionResult<IEnumerable<T>> Get() => Ok(Entities);
        public virtual bool ValidateFromSize(int? from, int? size) =>
            from != null && size != null && from >= 0 && size > 0;
    }
}