// MountingPlatePlugin.View/Program.cs
using System;
using System.Windows.Forms;

namespace MountingPlatePlugin.View
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Для .NET 6.0 используем старый способ
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}