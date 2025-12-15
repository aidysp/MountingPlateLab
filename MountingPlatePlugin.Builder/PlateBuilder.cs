// PlateBuilder.cs - с явными пространствами имён
using System;  // System.Exception
using System.Windows.Forms;  // Для MessageBox
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
            // Явно указываем nanoCAD Application
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            
            try
            {
                // Создаем и показываем форму
                var form = new MainForm();
                
                // Используем nanoCAD Application для диалога
                HostMgd.ApplicationServices.Application.ShowModalDialog(form);
                
                // Здесь будет код построения пластины
                ed.WriteMessage("\nФорма закрыта, можно начинать построение...");
            }
            catch (System.Exception ex)  // Явно System.Exception
            {
                ed.WriteMessage($"\nОшибка: {ex.Message}");
            }
        }
        
        // Метод для построения пластины
        public static void BuildPlate(MountingPlateParameters parameters)
        {
            var doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            
            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    ed.WriteMessage($"\nНачинаем построение пластины: {parameters.Length}x{parameters.Width} мм");
                    
                    // TODO: Здесь будет код создания 3D-тела
                    
                    tr.Commit();
                    ed.WriteMessage("\nПластина успешно создана!");
                }
                catch (System.Exception ex)  // Явно System.Exception
                {
                    ed.WriteMessage($"\nОшибка при построении: {ex.Message}");
                }
            }
        }
    }
}