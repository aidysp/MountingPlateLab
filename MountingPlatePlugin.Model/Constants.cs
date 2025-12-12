namespace MountingPlatePlugin.Model
{
    /// <summary>
    /// Класс, хранящий константные значения для параметров монтажной пластины.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Минимальная длина пластины, мм.
        /// </summary>
        public const float MIN_LENGTH = 50.0f;
        
        /// <summary>
        /// Максимальная длина пластины, мм.
        /// </summary>
        public const float MAX_LENGTH = 1000.0f;
        
        /// <summary>
        /// Минимальная ширина пластины, мм.
        /// </summary>
        public const float MIN_WIDTH = 30.0f;
        
        /// <summary>
        /// Максимальная ширина пластины, мм.
        /// </summary>
        public const float MAX_WIDTH = 500.0f;
        
        /// <summary>
        /// Минимальная толщина пластины, мм.
        /// </summary>
        public const float MIN_THICKNESS = 3.0f;
        
        /// <summary>
        /// Максимальная толщина пластины, мм.
        /// </summary>
        public const float MAX_THICKNESS = 50.0f;
        
        /// <summary>
        /// Минимальное количество отверстий по длине.
        /// </summary>
        public const int MIN_HOLES_LENGTH = 2;
        
        /// <summary>
        /// Максимальное количество отверстий по длине.
        /// </summary>
        public const int MAX_HOLES_LENGTH = 20;
        
        /// <summary>
        /// Минимальное количество отверстий по ширине.
        /// </summary>
        public const int MIN_HOLES_WIDTH = 2;
        
        /// <summary>
        /// Максимальное количество отверстий по ширине.
        /// </summary>
        public const int MAX_HOLES_WIDTH = 10;
        
        /// <summary>
        /// Минимальный диаметр отверстия, мм.
        /// </summary>
        public const float MIN_HOLE_DIAMETER = 3.0f;
        
        /// <summary>
        /// Коэффициент отступа от края.
        /// </summary>
        public const float EDGE_OFFSET_RATIO = 0.2f; // 1/5 = 0.2
    }
}