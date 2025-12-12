using System.Linq;

namespace MountingPlatePlugin.View
{
    public static class InputValidator
    {
        public static string ValidateNumericInput(string value)
        {
            const string allowedChars = ".1234567890";
            return new string(value.Where(character => 
                allowedChars.Contains(character)).ToArray());
        }
        
        public static bool IsValidFloat(string value)
        {
            return float.TryParse(value, out _);
        }
        
        public static bool IsValidInt(string value)
        {
            return int.TryParse(value, out _);
        }
    }
}