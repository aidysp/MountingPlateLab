// PlateBuilder.cs - полная реализация построения
using System;
using System.Windows.Forms;
using Teigha.Runtime;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using MountingPlatePlugin.Model;
using MountingPlatePlugin.View;

namespace MountingPlatePlugin.Builder
{
    public class PlateBuilder
    {
        [CommandMethod("CreateMountingPlate")]
        public static void CreatePlate()
        {
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            
            try
            {
                ed.WriteMessage("\n📝 Открываю конструктор монтажной пластины...");
                
                // Создаем форму
                using (var form = new MainForm())
                {
                    // Показываем диалог
                    if (HostMgd.ApplicationServices.Application.ShowModalDialog(form) == DialogResult.OK)
                    {
                        // Получаем параметры из формы
                        var parameters = form.PlateParameters;
                        
                        // Строим пластину
                        BuildRealPlate(parameters);
                        
                        ed.WriteMessage("\n✅ 3D-пластина успешно создана в nanoCAD!");
                    }
                    else
                    {
                        ed.WriteMessage("\n❌ Построение отменено");
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\n❌ Ошибка: {ex.Message}");
            }
        }
        
        // Реальный метод построения 3D-пластины
       // Реальный метод построения 3D-пластины
public static void BuildRealPlate(MountingPlateParameters parameters)
{
    var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
    var db = doc.Database;
    var ed = doc.Editor;
    
    using (var tr = db.TransactionManager.StartTransaction())
    {
        try
        {
            ed.WriteMessage($"\n🔨 Создаю пластину {parameters.Length}x{parameters.Width}x{parameters.Thickness} мм...");
            
            // 1. Получаем ModelSpace
            var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            var modelSpace = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], 
                                        OpenMode.ForWrite) as BlockTableRecord;
            
            // 2. Создаём пластину ЦЕНТРИРОВАННУЮ в начале координат
            var plate3D = CreatePlateBody(parameters);
            
            // 3. Проверяем границы пластины (для отладки)
            CheckPlateBounds(plate3D, "пластины (до отверстий)");
            
            // 4. Создаём отверстия (если нужно)
            if (parameters.HolesLength > 0 && parameters.HolesWidth > 0)
            {
                CreateProperHoles(plate3D, parameters);
            }
            
            // 5. Проверяем границы после отверстий
            CheckPlateBounds(plate3D, "пластины (после отверстий)");
            
            // 6. Добавляем в чертёж
            modelSpace.AppendEntity(plate3D);
            tr.AddNewlyCreatedDBObject(plate3D, true);
            
            tr.Commit();
            
            // 7. Перестраиваем вид
            doc.Editor.Regen();
            ed.WriteMessage($"\n✅ Готово! Отверстий: {parameters.TotalHoles}");
        }
        catch (System.Exception ex)
        {
            ed.WriteMessage($"\n❌ Ошибка при построении: {ex.Message}");
            ed.WriteMessage($"\n❌ StackTrace: {ex.StackTrace}");
        }
    }
}

private static void CheckPlateBounds(Solid3d plate, string plateName)
{
    try
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        
        // Получаем габариты пластины
        if (plate.Bounds.HasValue)
        {
            Extents3d extents = plate.Bounds.Value;
            Point3d min = extents.MinPoint;
            Point3d max = extents.MaxPoint;
            
            // Вычисляем центр
            double centerX = (min.X + max.X) / 2;
            double centerY = (min.Y + max.Y) / 2;
            double centerZ = (min.Z + max.Z) / 2;
            
            doc.Editor.WriteMessage($"\n📌 Центр {plateName}: X={centerX:F2}, Y={centerY:F2}, Z={centerZ:F2}");
            doc.Editor.WriteMessage($"\n📏 Габариты: от ({min.X:F1},{min.Y:F1}) до ({max.X:F1},{max.Y:F1})");
            doc.Editor.WriteMessage($"\n📐 Размеры: {max.X - min.X:F1} x {max.Y - min.Y:F1} мм");
        }
        else
        {
            doc.Editor.WriteMessage($"\n⚠️ Не удалось получить границы {plateName}");
        }
    }
    catch (System.Exception ex)
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        doc.Editor.WriteMessage($"\n⚠️ Ошибка проверки границ: {ex.Message}");
    }
}

      // Обновляем метод CreatePlateBody
private static Solid3d CreatePlateBody(MountingPlateParameters parameters)
{
    var plate = new Solid3d();
    
    try
    {
        // Создаем полилинию прямоугольника
        var plateProfile = new Polyline();
        
        // Координаты углов относительно центра
        double halfLength = parameters.Length / 2.0;
        double halfWidth = parameters.Width / 2.0;
        
        // Создаем прямоугольник с центром в (0,0)
        plateProfile.AddVertexAt(0, new Point2d(-halfLength, -halfWidth), 0, 0, 0);
        plateProfile.AddVertexAt(1, new Point2d(halfLength, -halfWidth), 0, 0, 0);
        plateProfile.AddVertexAt(2, new Point2d(halfLength, halfWidth), 0, 0, 0);
        plateProfile.AddVertexAt(3, new Point2d(-halfLength, halfWidth), 0, 0, 0);
        plateProfile.Closed = true;
        
        // Создаем регион из полилинии
        var curves = new DBObjectCollection();
        curves.Add(plateProfile);
        var regions = Region.CreateFromCurves(curves);
        
        if (regions.Count > 0)
        {
            var region = regions[0] as Region;
            
            // Выдавливаем регион для создания твердого тела
            plate.Extrude(region, parameters.Thickness, 0);
        }
    }
    catch (System.Exception ex)
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        doc.Editor.WriteMessage($"\n❌ Ошибка создания пластины: {ex.Message}");
    }
    
    return plate;
}
        
 private static void CreateProperHoles(Solid3d plate, MountingPlateParameters parameters)
{
    try
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        
        // ВЫВОД ДЛЯ ОТЛАДКИ - убедимся в правильности параметров
        doc.Editor.WriteMessage($"\n🔍 ПАРАМЕТРЫ ПЛАСТИНЫ:");
        doc.Editor.WriteMessage($"\n  Длина: {parameters.Length}мм");
        doc.Editor.WriteMessage($"\n  Ширина: {parameters.Width}мм");
        doc.Editor.WriteMessage($"\n  Толщина: {parameters.Thickness}мм");
        doc.Editor.WriteMessage($"\n  Отверстия: {parameters.HolesLength}x{parameters.HolesWidth}");
        doc.Editor.WriteMessage($"\n  Тип: {parameters.HoleTypeValue}");
        
        // Проверяем границы пластины для отладки
        if (plate.Bounds.HasValue)
        {
            Extents3d bounds = plate.Bounds.Value;
            doc.Editor.WriteMessage($"\n🎯 ГРАНИЦЫ ПЛАСТИНЫ:");
            doc.Editor.WriteMessage($"\n  Min: X={bounds.MinPoint.X:F1}, Y={bounds.MinPoint.Y:F1}");
            doc.Editor.WriteMessage($"\n  Max: X={bounds.MaxPoint.X:F1}, Y={bounds.MaxPoint.Y:F1}");
            
            // Проверяем, что пластина центрирована
            double centerX = (bounds.MinPoint.X + bounds.MaxPoint.X) / 2;
            double centerY = (bounds.MinPoint.Y + bounds.MaxPoint.Y) / 2;
            doc.Editor.WriteMessage($"\n📍 Вычисленный центр: X={centerX:F2}, Y={centerY:F2}");
        }
        
        // 1. Отступ от края (минимальный зазор)
        float edgeMargin = Math.Min(parameters.Length, parameters.Width) * 0.15f;
        if (edgeMargin < 5) edgeMargin = 5; // Минимум 5мм
        
        // 2. Рабочая область для размещения отверстий
        float workLength = parameters.Length - 2 * edgeMargin;
        float workWidth = parameters.Width - 2 * edgeMargin;
        
        // 3. Рассчитываем шаг между отверстиями
        float stepX = 0;
        float stepY = 0;
        
        if (parameters.HolesLength > 1)
        {
            stepX = workLength / (parameters.HolesLength - 1);
        }
        else
        {
            // Если одно отверстие - ставим в центре
            stepX = 0;
        }
        
        if (parameters.HolesWidth > 1)
        {
            stepY = workWidth / (parameters.HolesWidth - 1);
        }
        else
        {
            // Если одно отверстие - ставим в центре
            stepY = 0;
        }
        
        doc.Editor.WriteMessage($"\n📐 Рабочая область: {workLength:F1}x{workWidth:F1}мм");
        doc.Editor.WriteMessage($"\n📏 Шаг отверстий: {stepX:F1}x{stepY:F1}мм");
        doc.Editor.WriteMessage($"\n📍 Отступ от края: {edgeMargin:F1}мм");
        
        // 4. Размер отверстия
        float holeSize;
        if (parameters.HoleTypeValue == MountingPlateParameters.HoleType.Round)
        {
            // Используем вычисленный диаметр, но ограничиваем
            holeSize = parameters.HoleDiameter;
            float maxDiameter = Math.Min(stepX, stepY) * 0.8f;
            if (holeSize > maxDiameter && maxDiameter > 0)
                holeSize = maxDiameter;
        }
        else
        {
            // Для квадратных и щелевых
            float maxSize = Math.Min(stepX, stepY) * 0.7f;
            holeSize = Math.Max(5.0f, Math.Min(maxSize, 15.0f));
        }
        
        doc.Editor.WriteMessage($"\n⚫ Размер отверстия: {holeSize:F1}мм");
        
        // 5. Создаем отверстия
        int holeCount = 0;
        for (int i = 0; i < parameters.HolesLength; i++)
        {
            for (int j = 0; j < parameters.HolesWidth; j++)
            {
                // Координаты ОТ ЦЕНТРА пластины
                // Пластина у нас центрирована в точке (0,0)
                
                double xCoord, yCoord;
                
                if (parameters.HolesLength == 1)
                {
                    // Одно отверстие по длине - в центре
                    xCoord = 0;
                }
                else
                {
                    // Несколько отверстий - равномерно распределяем
                    // От левого края рабочей области до правого
                    float xFromLeft = edgeMargin + i * stepX;
                    // Преобразуем в координату от центра
                    xCoord = xFromLeft - (parameters.Length / 2);
                }
                
                if (parameters.HolesWidth == 1)
                {
                    // Одно отверстие по ширине - в центре
                    yCoord = 0;
                }
                else
                {
                    // Несколько отверстий - равномерно распределяем
                    float yFromBottom = edgeMargin + j * stepY;
                    // Преобразуем в координату от центра
                    yCoord = yFromBottom - (parameters.Width / 2);
                }
                
                doc.Editor.WriteMessage($"\n📍 Отверстие [{i},{j}]: X={xCoord:F1}, Y={yCoord:F1}");
                
                // Создаем отверстие
                switch (parameters.HoleTypeValue)
                {
                    case MountingPlateParameters.HoleType.Round:
                        CreateCircularHole(plate, xCoord, yCoord, holeSize, parameters.Thickness);
                        break;
                    case MountingPlateParameters.HoleType.Square:
                        CreateSquareHole(plate, xCoord, yCoord, holeSize, parameters.Thickness);
                        break;
                    case MountingPlateParameters.HoleType.Slot:
                        CreateSlottedHole(plate, xCoord, yCoord, holeSize, parameters.Thickness);
                        break;
                    default:
                        CreateCircularHole(plate, xCoord, yCoord, holeSize, parameters.Thickness);
                        break;
                }
                
                holeCount++;
            }
        }
        
        doc.Editor.WriteMessage($"\n✅ Создано отверстий: {holeCount}");
        
    }
    catch (System.Exception ex)
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        doc.Editor.WriteMessage($"\n❌ Ошибка создания отверстий: {ex.Message}");
        doc.Editor.WriteMessage($"\n❌ StackTrace: {ex.StackTrace}");
    }
}
// Определяем тип отверстия в зависимости от позиции
private static MountingPlateParameters.HoleType GetHoleTypeForPosition(int i, int j, MountingPlateParameters parameters)
{
    // Используем значение из параметров, если оно задано
    if (parameters.HoleTypeValue != MountingPlateParameters.HoleType.Round)
        return parameters.HoleTypeValue;
    
    // По умолчанию используем логику распределения
    if (parameters.HolesLength <= 1 && parameters.HolesWidth <= 1)
        return MountingPlateParameters.HoleType.Round;
    
    // Шахматный порядок: квадратные на четных позициях
    if ((i + j) % 2 == 0)
        return MountingPlateParameters.HoleType.Square;
    
    // Щелевые по краям
    if (i == 0 || i == parameters.HolesLength - 1 || 
        j == 0 || j == parameters.HolesWidth - 1)
        return MountingPlateParameters.HoleType.Slot;
    
    return MountingPlateParameters.HoleType.Round;
}

// Метод создания круглого отверстия
private static void CreateCircularHole(Solid3d plate, double x, double y, float size, float thickness)
{
    try
    {
        var hole = new Solid3d();
        // Преобразуем размер в double для использования в Circle
        double radius = (double)size / 2.0;
        
        var circle = new Circle(
            new Point3d(x, y, -thickness / 2),
            Vector3d.ZAxis,
            radius);
        
        var curves = new DBObjectCollection();
        curves.Add(circle);
        var regions = Region.CreateFromCurves(curves);
        
        if (regions.Count > 0)
        {
            var region = regions[0] as Region;
            hole.Extrude(region, thickness * 1.5, 0);
            plate.BooleanOperation(BooleanOperationType.BoolSubtract, hole);
        }
    }
    catch { }
}

// Метод создания квадратного отверстия
private static void CreateSquareHole(Solid3d plate, double x, double y, float size, float thickness)
{
    try
    {
        var hole = new Solid3d();
        // Преобразуем размер в double
        double halfSize = (double)size / 2.0;
        
        // Создаем полилинию квадрата
        var square = new Polyline();
        square.AddVertexAt(0, new Point2d(x - halfSize, y - halfSize), 0, 0, 0);
        square.AddVertexAt(1, new Point2d(x + halfSize, y - halfSize), 0, 0, 0);
        square.AddVertexAt(2, new Point2d(x + halfSize, y + halfSize), 0, 0, 0);
        square.AddVertexAt(3, new Point2d(x - halfSize, y + halfSize), 0, 0, 0);
        square.Closed = true;
        
        var curves = new DBObjectCollection();
        curves.Add(square);
        var regions = Region.CreateFromCurves(curves);
        
        if (regions.Count > 0)
        {
            var region = regions[0] as Region;
            hole.Extrude(region, thickness * 1.5, 0);
            plate.BooleanOperation(BooleanOperationType.BoolSubtract, hole);
        }
    }
    catch { }
}

// Метод создания щелевого отверстия
// Метод создания щелевого (овального) отверстия
// Метод создания щелевого (овального) отверстия
private static void CreateSlottedHole(Solid3d plate, double x, double y, float size, float thickness)
{
    try
    {
        var hole = new Solid3d();
        
        // Размеры щелевого отверстия (длинное в направлении X)
        double length = (double)size * 2.0; // Длина
        double width = (double)size * 0.6;  // Ширина (уже)
        
        // Создаем эллипс или прямоугольник со скругленными концами
        var slot = new Polyline();
        
        // Добавляем точки для создания овальной формы
        int segments = 20;
        
        // Верхняя половина
        for (int i = 0; i <= segments; i++)
        {
            double t = (double)i / segments * Math.PI; // от 0 до π
            double pointX = x + (length / 2) * Math.Cos(t);
            double pointY = y + (width / 2) * Math.Sin(t);
            slot.AddVertexAt(slot.NumberOfVertices, new Point2d(pointX, pointY), 0, 0, 0);
        }
        
        // Нижняя половина
        for (int i = segments; i >= 0; i--)
        {
            double t = (double)i / segments * Math.PI; // от π до 0
            double pointX = x + (length / 2) * Math.Cos(t);
            double pointY = y - (width / 2) * Math.Sin(t);
            slot.AddVertexAt(slot.NumberOfVertices, new Point2d(pointX, pointY), 0, 0, 0);
        }
        
        slot.Closed = true;
        
        var curves = new DBObjectCollection();
        curves.Add(slot);
        var regions = Region.CreateFromCurves(curves);
        
        if (regions.Count > 0)
        {
            var region = regions[0] as Region;
            hole.Extrude(region, thickness * 1.5, 0);
            plate.BooleanOperation(BooleanOperationType.BoolSubtract, hole);
        }
    }
    catch (System.Exception ex) // Явно указываем System.Exception
    {
        var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        doc.Editor.WriteMessage($"\n⚠️ Ошибка при создании щелевого отверстия: {ex.Message}");
        
        // В случае ошибки создаем простое круглое отверстие
        CreateCircularHole(plate, x, y, size, thickness);
    }
}

[CommandMethod("QuickBuildWithHoles")]
public static void QuickBuildWithHoles()
{
    // Быстрое тестовое построение с разными типами отверстий
    var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
    var ed = doc.Editor;
    
    try
    {
        ed.WriteMessage("\n🔧 Тестовое построение пластины с отверстиями...");
        
        // Тест 1: Круглые отверстия
        ed.WriteMessage("\n\n🎯 ТЕСТ 1: Круглые отверстия");
        var test1 = new MountingPlateParameters();
        test1.Length = 200;
        test1.Width = 100;
        test1.Thickness = 10;
        test1.HolesLength = 3;
        test1.HolesWidth = 2;
        test1.HoleTypeValue = MountingPlateParameters.HoleType.Round;
        BuildRealPlate(test1);
        
        // Тест 2: Квадратные отверстия
        ed.WriteMessage("\n\n🎯 ТЕСТ 2: Квадратные отверстия");
        var test2 = new MountingPlateParameters();
        test2.Length = 150;
        test2.Width = 80;
        test2.Thickness = 8;
        test2.HolesLength = 4;
        test2.HolesWidth = 3;
        test2.HoleTypeValue = MountingPlateParameters.HoleType.Square;
        BuildRealPlate(test2);
        
        // Тест 3: Щелевые отверстия
        ed.WriteMessage("\n\n🎯 ТЕСТ 3: Щелевые отверстия");
        var test3 = new MountingPlateParameters();
        test3.Length = 180;
        test3.Width = 90;
        test3.Thickness = 12;
        test3.HolesLength = 2;
        test3.HolesWidth = 2;
        test3.HoleTypeValue = MountingPlateParameters.HoleType.Slot;
        BuildRealPlate(test3);
        
        ed.WriteMessage("\n\n✅ Все тестовые пластины созданы!");
    }
    catch (System.Exception ex)
    {
        ed.WriteMessage($"\n❌ Ошибка тестирования: {ex.Message}");
    }
}

[CommandMethod("CreateSimplePlate")]
public static void CreateSimplePlate()
{
    var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
    var db = doc.Database;
    var ed = doc.Editor;
    
    using (var tr = db.TransactionManager.StartTransaction())
    {
        try
        {
            ed.WriteMessage("\n🔧 Создаю простую пластину без отверстий...");
            
            var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            var modelSpace = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], 
                                        OpenMode.ForWrite) as BlockTableRecord;
            
            // Создаем простую пластину 100x50x10 мм
            var plate = new Solid3d();
            
            var profile = new Polyline();
            profile.AddVertexAt(0, new Point2d(-50, -25), 0, 0, 0);
            profile.AddVertexAt(1, new Point2d(50, -25), 0, 0, 0);
            profile.AddVertexAt(2, new Point2d(50, 25), 0, 0, 0);
            profile.AddVertexAt(3, new Point2d(-50, 25), 0, 0, 0);
            profile.Closed = true;
            
            var curves = new DBObjectCollection();
            curves.Add(profile);
            var regions = Region.CreateFromCurves(curves);
            
            if (regions.Count > 0)
            {
                var region = regions[0] as Region;
                plate.Extrude(region, 10, 0);
                
                modelSpace.AppendEntity(plate);
                tr.AddNewlyCreatedDBObject(plate, true);
                
                tr.Commit();
                doc.Editor.Regen();
                ed.WriteMessage("\n✅ Простая пластина создана!");
                
                // Проверяем границы
                if (plate.Bounds.HasValue)
                {
                    var bounds = plate.Bounds.Value;
                    ed.WriteMessage($"\n📏 Границы: от ({bounds.MinPoint.X:F1},{bounds.MinPoint.Y:F1}) до ({bounds.MaxPoint.X:F1},{bounds.MaxPoint.Y:F1})");
                }
            }
        }
        catch (System.Exception ex)
        {
            ed.WriteMessage($"\n❌ Ошибка: {ex.Message}");
        }
    }
}
        [CommandMethod("TestPlate")]
        public static void TestCommand()
        {
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            ed.WriteMessage("\n✅ Плагин MountingPlate готов к работе!");
        }
        
        [CommandMethod("QuickBuild")]
        public static void QuickBuild()
        {
            // Быстрое тестовое построение
            var testParams = new MountingPlateParameters();
            testParams.Length = 200;
            testParams.Width = 100;
            testParams.Thickness = 10;
            testParams.HolesLength = 3;
            testParams.HolesWidth = 2;
            
            BuildRealPlate(testParams);
        }
    }
}