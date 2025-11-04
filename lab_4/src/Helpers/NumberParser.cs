using System;
using System.Globalization;

namespace lab_4.Helpers
{
    public static class NumberParser
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Заменяем запятую на точку для унификации
            value = value.Replace(',', '.');
            
            if (double.TryParse(value, NumberStyles.Any, Culture, out double result))
                return result;
            
            throw new FormatException($"Невозможно преобразовать '{value}' в число");
        }

        public static int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (int.TryParse(value, out int result))
                return result;
            
            throw new FormatException($"Невозможно преобразовать '{value}' в целое число");
        }

        public static string FormatDouble(double value)
        {
            return value.ToString(Culture);
        }
    }
}