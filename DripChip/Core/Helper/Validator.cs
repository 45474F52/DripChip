namespace DripChip.Core.Helper
{
    /// <summary>
    /// Класс, содержащий методы проверки соотвествия объектов условиям
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Определяет не содержит ли последовательность объектов значение <see langword="null"/>
        /// </summary>
        /// <param name="objects">Множество объектов для проверки на <see langword="null"/></param>
        /// <returns>Возвращает <see langword="true"/>, если в значениях <paramref name="objects"/>
        /// нет <see langword="null"/>, иначе — <see langword="false"/></returns>
        public static bool IsNotNull(params object?[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is null)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Определяет содержится ли объект в перечислении
        /// </summary>
        /// <param name="enumType">Тип перечисления, в котором производится поиск объекта</param>
        /// <param name="value">Объект, который может быть в перечислении <paramref name="enumType"/></param>
        /// <returns>Возвращает <see langword="true"/>, если <paramref name="value"/> есть в перечислении типа <paramref name="enumType"/>,
        /// иначе — <see langword="false"/></returns>
        public static bool IsValidEnum(Type enumType, object? value) => value is not null && Enum.IsDefined(enumType, value);
        /// <summary>
        /// Определяет соответсвуют ли числа <typeparamref name="TNumber"/> условию
        /// </summary>
        /// <typeparam name="TNumber">Тип числа</typeparam>
        /// <param name="predicate">Условие, по которому проверяются числа</param>
        /// <param name="numbers">Множество чисел типа <typeparamref name="TNumber"/>, проверяющиеся на условие <paramref name="predicate"/></param>
        /// <returns>Возвращает <see langword="true"/>, если все числа <paramref name="numbers"/> прошли по условию <paramref name="predicate"/>,
        /// иначе — <see langword="false"/></returns>
        public static bool IsValidNums<TNumber>(Func<TNumber, bool> predicate, params TNumber[] numbers)
            where TNumber : unmanaged, IComparable, IEquatable<TNumber>
        {
            try
            {
                foreach (TNumber number in numbers)
                    if (!predicate(number))
                        return false;
                return true;
            }
            catch (Exception) { throw; }
        }
        /// <summary>
        /// Проверяет соответствие даты формату
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="pattern">Формат даты</param>
        /// <returns>Возвращает <see langword="true"/>, если дата <paramref name="date"/> соответствует формату <paramref name="pattern"/>,
        /// иначе — <see langword="false"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool IsValidDate(string date, string pattern)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Проверяет соответствие даты формату
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="pattern">Формат даты</param>
        /// <returns>Возвращает <see langword="true"/>, если дата <paramref name="date"/> соответствует формату <paramref name="pattern"/>,
        /// иначе — <see langword="false"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool IsValidDate(DateTime date, string pattern)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Проверяет соответствие дат формату
        /// </summary>
        /// <param name="dates">Даты</param>
        /// <param name="pattern">Формат даты</param>
        /// <returns>Возвращает <see langword="true"/>, если даты <paramref name="dates"/> соответствует формату <paramref name="pattern"/>,
        /// иначе — <see langword="false"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool IsValidDates(string[] dates, string pattern)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Проверяет соответствие дат формату
        /// </summary>
        /// <param name="dates">Даты</param>
        /// <param name="pattern">Формат даты</param>
        /// <returns>Возвращает <see langword="true"/>, если даты <paramref name="dates"/> соответствует формату <paramref name="pattern"/>,
        /// иначе — <see langword="false"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool IsValidDates(DateTime[] dates, string pattern)
        {
            throw new NotImplementedException();
        }
        [Obsolete("Догоростоящая операция")]
        public static bool IsNumeric(Type type) =>
            type == typeof(sbyte) || type == typeof(byte) ||
            type == typeof(short) || type == typeof(ushort) ||
            type == typeof(int) || type == typeof(uint) ||
            type == typeof(long) || type == typeof(ulong) ||
            type == typeof(float) || type == typeof(double) ||
            type == typeof(decimal);
    }
}