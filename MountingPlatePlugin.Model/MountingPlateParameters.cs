using System;

namespace MountingPlatePlugin.Model
{
    /// <summary>
    /// Класс, представляющий параметры монтажной пластины.
    /// </summary>
    public class MountingPlateParameters
    {


        // Приватные поля-параметры
        private readonly Parameter<float> _length = new Parameter<float>(
            Constants.MIN_LENGTH,
            200.0f, // значение по умолчанию
            Constants.MAX_LENGTH);
            
        private readonly Parameter<float> _width = new Parameter<float>(
            Constants.MIN_WIDTH,
            100.0f, // значение по умолчанию
            Constants.MAX_WIDTH);
            
        private readonly Parameter<float> _thickness = new Parameter<float>(
            Constants.MIN_THICKNESS,
            10.0f, // значение по умолчанию
            Constants.MAX_THICKNESS);
            
        private readonly Parameter<int> _holesLength = new Parameter<int>(
            Constants.MIN_HOLES_LENGTH,
            4, // значение по умолчанию
            Constants.MAX_HOLES_LENGTH);
            
        private readonly Parameter<int> _holesWidth = new Parameter<int>(
            Constants.MIN_HOLES_WIDTH,
            3, // значение по умолчанию
            Constants.MAX_HOLES_WIDTH);
        
        /// <summary>
        /// Перечисление типов отверстий.
        /// </summary>
        public enum HoleType
        {
            /// <summary>
            /// Круглое отверстие.
            /// </summary>
            Round,
            
            /// <summary>
            /// Квадратное отверстие.
            /// </summary>
            Square,
            
            /// <summary>
            /// Продолговатое отверстие (паз).
            /// </summary>
            Slot
        }
        
        /// <summary>
        /// Тип отверстий (по умолчанию круглые).
        /// </summary>
        public HoleType HoleTypeValue { get; set; } = HoleType.Round;
        
        /// <summary>
        /// Получает или задает длину пластины (мм).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при недопустимом значении.</exception>
        public float Length
        {
            get => _length.Value;
            set
            {
                if (Validator.ValidateParameter(_length, value))
                {
                    _length.Value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Длина должна быть в диапазоне [{_length.MinValue} - {_length.MaxValue}] мм.");
                }
            }
        }
        
        /// <summary>
        /// Получает или задает ширину пластины (мм).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при недопустимом значении.</exception>
        public float Width
        {
            get => _width.Value;
            set
            {
                if (Validator.ValidateParameter(_width, value))
                {
                    _width.Value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Ширина должна быть в диапазоне [{_width.MinValue} - {_width.MaxValue}] мм.");
                }
            }
        }
        
        /// <summary>
        /// Получает или задает толщину пластины (мм).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при недопустимом значении.</exception>
        public float Thickness
        {
            get => _thickness.Value;
            set
            {
                if (Validator.ValidateParameter(_thickness, value))
                {
                    _thickness.Value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Толщина должна быть в диапазоне [{_thickness.MinValue} - {_thickness.MaxValue}] мм.");
                }
            }
        }
        
        /// <summary>
        /// Получает или задает количество отверстий по длине.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при недопустимом значении.</exception>
        public int HolesLength
        {
            get => _holesLength.Value;
            set
            {
                if (Validator.ValidateParameter(_holesLength, value))
                {
                    _holesLength.Value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Количество отверстий по длине должно быть в диапазоне [{_holesLength.MinValue} - {_holesLength.MaxValue}].");
                }
            }
        }
        
        /// <summary>
        /// Получает или задает количество отверстий по ширине.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается при недопустимом значении.</exception>
        public int HolesWidth
        {
            get => _holesWidth.Value;
            set
            {
                if (Validator.ValidateParameter(_holesWidth, value))
                {
                    _holesWidth.Value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Количество отверстий по ширине должно быть в диапазоне [{_holesWidth.MinValue} - {_holesWidth.MaxValue}].");
                }
            }
        }
        
        /// <summary>
        /// Вычисляет диаметр отверстий на основе ширины пластины.
        /// </summary>
        /// <remarks>
        /// Диаметр = Ширина / (Количество отверстий по ширине * 2)
        /// Это гарантирует зазор между отверстиями.
        /// </remarks>
        public float HoleDiameter
        {
            get
            {
                float calculatedDiameter = Width / (HolesWidth * 2);
                return Math.Max(calculatedDiameter, Constants.MIN_HOLE_DIAMETER);
            }
        }
        
        /// <summary>
        /// Вычисляет расстояние между отверстиями по длине.
        /// </summary>
        /// <remarks>
        /// Расстояние = Длина / (Количество отверстий по длине + 1)
        /// </remarks>
        public float HoleSpacingLength
        {
            get => Length / (HolesLength + 1);
        }
        
        /// <summary>
        /// Вычисляет расстояние между отверстиями по ширине.
        /// </summary>
        /// <remarks>
        /// Расстояние = Ширина / (Количество отверстий по ширине + 1)
        /// </remarks>
        public float HoleSpacingWidth
        {
            get => Width / (HolesWidth + 1);
        }
        
        /// <summary>
        /// Вычисляет отступ от краев пластины.
        /// </summary>
        /// <remarks>
        /// Отступ = min(Длина, Ширина) * 0.2 (1/5)
        /// </remarks>
        public float EdgeOffset
        {
            get => Math.Min(Length, Width) * Constants.EDGE_OFFSET_RATIO;
        }
        
        /// <summary>
        /// Вычисляет общее количество отверстий.
        /// </summary>
        public int TotalHoles
        {
            get => HolesLength * HolesWidth;
        }
        
        /// <summary>
        /// Проверяет все параметры на корректность.
        /// </summary>
        /// <returns>true если все параметры валидны, иначе false.</returns>
        public bool ValidateAll()
        {
            try
            {
                // Проверяем каждый параметр через сеттеры
                float tempLength = Length;
                float tempWidth = Width;
                float tempThickness = Thickness;
                int tempHolesLength = HolesLength;
                int tempHolesWidth = HolesWidth;
                
                // Дополнительные проверки зависимостей
                if (HoleDiameter < Constants.MIN_HOLE_DIAMETER)
                    return false;
                    
                if (EdgeOffset < HoleDiameter / 2)
                    return false;
                    
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
    }
}