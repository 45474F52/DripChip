namespace DripChip.Core.Serialization
{
    public interface IAsyncSerializer<T>
    {
        string? Path { get; set; }

        Task<T?> DeserializeAsync();
        Task SerializeAsync(object value);
    }
}