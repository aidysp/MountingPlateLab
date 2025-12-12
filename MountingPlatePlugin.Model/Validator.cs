namespace MountingPlatePlugin.Model
{
    /// <summary>
    /// Статический класс для валидации параметров.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Проверяет, находится ли значение в допустимом диапазоне параметра.
        /// </summary>
        /// <typeparam name="T">Тип параметра.</typeparam>
        /// <param name="parameter">Параметр с границами.</param>
        /// <param name="value">Проверяемое значение.</param>
        /// <returns>true если значение валидно, иначе false.</returns>
        public static bool ValidateParameter<T>(Parameter<T> parameter, T value) 
            where T : IComparable<T>
        {
            return value.CompareTo(parameter.MinValue) >= 0 && 
                   value.CompareTo(parameter.MaxValue) <= 0;
        }
    }
}