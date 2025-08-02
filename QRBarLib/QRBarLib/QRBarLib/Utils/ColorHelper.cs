using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SmartCodeLib.Utils
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ColorHelper
    {
        /// <summary>
        /// Valida si una cadena es un color HEX válido (#RRGGBB o #RGB).
        /// </summary>
        public bool IsValidHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return false;

            string pattern = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            return Regex.IsMatch(hex, pattern);
        }

        /// <summary>
        /// Convierte un color HEX a objeto Color.
        /// </summary>
        public Color HexToColor(string hex)
        {
            if (!IsValidHex(hex))
                throw new ArgumentException("Color HEX inválido. Debe ser #RRGGBB o #RGB.");

            return ColorTranslator.FromHtml(hex);
        }

        /// <summary>
        /// Convierte un objeto Color a cadena HEX (#RRGGBB).
        /// </summary>
        public string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Convierte valores RGB a HEX (#RRGGBB).
        /// </summary>
        public string RgbToHex(int r, int g, int b)
        {
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                throw new ArgumentException("Los valores RGB deben estar entre 0 y 255.");

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        /// <summary>
        /// Convierte un color HEX a valores RGB en formato "R,G,B".
        /// </summary>
        public string HexToRgbString(string hex)
        {
            Color c = HexToColor(hex);
            return $"{c.R},{c.G},{c.B}";
        }
    }
}
