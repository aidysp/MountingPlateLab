using System;
using MountingPlatePlugin.Model;

namespace MountingPlatePlugin.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование модели монтажной пластины ===\n");
            
            var plate = new MountingPlateParameters();
            
            try
            {
                // Устанавливаем параметры
                plate.Length = 200.0f;
                plate.Width = 100.0f;
                plate.Thickness = 10.0f;
                plate.HolesLength = 4;
                plate.HolesWidth = 3;
                plate.HoleTypeValue = MountingPlateParameters.HoleType.Round;
                
                // Выводим информацию
                Console.WriteLine("=== Параметры монтажной пластины ===");
                Console.WriteLine($"Длина: {plate.Length} мм");
                Console.WriteLine($"Ширина: {plate.Width} мм");
                Console.WriteLine($"Толщина: {plate.Thickness} мм");
                Console.WriteLine($"Отверстий по длине: {plate.HolesLength}");
                Console.WriteLine($"Отверстий по ширине: {plate.HolesWidth}");
                Console.WriteLine($"Всего отверстий: {plate.TotalHoles}");
                Console.WriteLine($"Тип отверстий: {plate.HoleTypeValue}");
                Console.WriteLine($"Диаметр отверстий: {plate.HoleDiameter:F2} мм");
                Console.WriteLine($"Расстояние между отверстиями по длине: {plate.HoleSpacingLength:F2} мм");
                Console.WriteLine($"Расстояние между отверстиями по ширине: {plate.HoleSpacingWidth:F2} мм");
                Console.WriteLine($"Отступ от края: {plate.EdgeOffset:F2} мм");
                
                Console.WriteLine($"\nВалидация: {(plate.ValidateAll() ? "ПРОЙДЕНА" : "НЕ ПРОЙДЕНА")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }
        }
    }
}