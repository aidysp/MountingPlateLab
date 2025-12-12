namespace MountingPlatePlugin.Model
{
    /// <summary>
    /// Обобщенный класс, представляющий параметр с допустимым диапазоном.
    /// </summary>
    /// <typeparam name="T">Тип параметра (float, int и т.д.).</typeparam>
    public class Parameter<T> where T : IComparable<T>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Parameter{T}"/>.
        /// </summary>
        /// <param name="min">Минимально допустимое значение.</param>
        /// <param name="value">Текущее значение.</param>
        /// <param name="max">Максимально допустимое значение.</param>
        public Parameter(T min, T value, T max)
        {
            MinValue = min;
            Value = value;
            MaxValue = max;
        }
        
        /// <summary>
        /// Получает или задает минимально допустимое значение.
        /// </summary>
        public T MinValue { get; set; }
        
        /// <summary>
        /// Получает или задает текущее значение параметра.
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Получает или задает максимально допустимое значение.
        /// </summary>
        public T MaxValue { get; set; }
    }
}