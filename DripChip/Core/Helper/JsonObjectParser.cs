using System.Text.Json.Nodes;

namespace DripChip.Core.Helper
{
    /// <summary>
    /// Класс, представляющий методы получения значений из объекта <see cref="JsonObject"/>
    /// </summary>
    /// <typeparam name="OuterType">Тип выходного значения из <see cref="JsonObject"/></typeparam>
    public static class JsonObjectParser<OuterType>
    {
        /// <summary>
        /// Получает значение типа <typeparamref name="OuterType"/> из <paramref name="data"/>, путём поиска его по имени <paramref name="propertyName"/>
        /// </summary>
        /// <param name="data"><see cref="JsonObject"/> объект, содержащий некоторые данные</param>
        /// <param name="propertyName">Имя значения, содержащегося в <paramref name="data"/></param>
        /// <returns>Возвращает <typeparamref name="OuterType"/>, если значение с именем <paramref name="propertyName"/> было найдено
        /// в <paramref name="data"/>, иначе значение <typeparamref name="OuterType"/> будет равным <see langword="null"/></returns>
        public static OuterType? Parse(in JsonObject data, string propertyName)
        {
            OuterType? value = default;
            data.TryGetPropertyValue(propertyName, out JsonNode? node);
            node?.AsValue().TryGetValue(out value);
            return value;
        }
        /// <summary>
        /// Получает коллекцию пар &lt;Имя значения, Значение типа <typeparamref name="OuterType"/>&gt; из <paramref name="data"/>
        /// </summary>
        /// <param name="data"><see cref="JsonObject"/> объект, содержащий некоторые данные</param>
        /// <param name="propertyNames">Имена значений, содержащихся в <paramref name="data"/></param>
        /// <returns>Возвращает коллекцию с заполенными значениями, если их имена <paramref name="propertyName"/> были найдены
        /// в <paramref name="data"/>, иначе значение значения будут равными <see langword="null"/></returns>
        public static Dictionary<string, OuterType?> ParseObjects(in JsonObject data, string[] propertyNames)
        {
            Dictionary<string, OuterType?> result = new();
            for (int i = 0; i < propertyNames.Length; i++)
            {
                OuterType? value = default;
                data.TryGetPropertyValue(propertyNames[i], out JsonNode? node);
                node?.AsValue().TryGetValue(out value);
                result.Add(propertyNames[i], value);
            }
            return result;
        }
    }
}