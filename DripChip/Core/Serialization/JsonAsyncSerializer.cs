using System.Text.Json;

namespace DripChip.Core.Serialization
{
    public class JsonAsyncSerializer<T> : IAsyncSerializer<T>
    {
        private readonly JsonSerializerOptions? _options;

        public string? Path { get; set; }

        public JsonAsyncSerializer() { }

        public JsonAsyncSerializer(JsonSerializerOptions options) => _options = options;

        public async Task<T?> DeserializeAsync()
        {
            CheckPath();
            if (File.Exists(Path))
            {
                using FileStream stream = new(Path!, FileMode.Open, FileAccess.Read, FileShare.Read);
                T? value = (T?)await JsonSerializer.DeserializeAsync(stream, typeof(T?), _options);
                return value;
            }

            return default;
        }

        public async Task SerializeAsync(T value)
        {
            CheckPath();

            using FileStream stream = new(
                Path!, File.Exists(Path) ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, value, _options);
        }

        public async Task OverwriteFileAsync(T value)
        {
            CheckPath();

            using FileStream stream = new(Path!, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, value, _options);
        }

        private void CheckPath()
        {
            if (Path == null)
                throw new ArgumentNullException(nameof(Path), "Путь для сериализации данных не может быть равным NULL");
        }
    }
}