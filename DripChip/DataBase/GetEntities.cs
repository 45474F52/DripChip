using DripChip.Core.Serialization;

namespace DripChip.DataBase
{
    public class GetEntities<T> where T : new()
    {
        private readonly IAsyncSerializer<IEnumerable<T>> _serializer;

        public GetEntities(IAsyncSerializer<IEnumerable<T>> serializer) => _serializer = serializer;

        public async Task<IEnumerable<T>?> ReceiveEnumerable()
        {
            IEnumerable<T>? values = await _serializer.DeserializeAsync();
            return values;
        }
    }
}