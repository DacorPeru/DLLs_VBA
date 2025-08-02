using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartCodeLib.Utils
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ImageHelper
    {
        /// <summary>
        /// Convierte una imagen en Base64.
        /// </summary>
        public string ImageToBase64(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("No se encontró la imagen especificada.", filePath);

            byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Guarda una imagen desde una cadena Base64.
        /// </summary>
        public void Base64ToImage(string base64, string outputPath)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            File.WriteAllBytes(outputPath, bytes);
        }

        /// <summary>
        /// Redimensiona una imagen manteniendo la relación de aspecto.
        /// </summary>
        public void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("No se encontró la imagen especificada.", inputPath);

            using (Bitmap original = new Bitmap(inputPath))
            using (Bitmap resized = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, width, height);
                resized.Save(outputPath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Agrega un texto sobre una imagen (marca, etiqueta, precio, etc.).
        /// </summary>
        public void AddTextToImage(string inputPath, string outputPath, string text,
                                   string fontName = "Arial", int fontSize = 14, bool bold = true,
                                   string hexColor = "#000000", int x = 10, int y = 10)
        {
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("No se encontró la imagen especificada.", inputPath);

            using (Bitmap bitmap = new Bitmap(inputPath))
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font(fontName, fontSize, bold ? FontStyle.Bold : FontStyle.Regular))
            using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(hexColor)))
            {
                g.DrawString(text, font, brush, new PointF(x, y));
                bitmap.Save(outputPath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Combina dos imágenes (por ejemplo, agregar un logo sobre un QR).
        /// </summary>
        public void OverlayImage(string baseImagePath, string overlayImagePath, string outputPath, int x, int y, int width, int height)
        {
            if (!File.Exists(baseImagePath) || !File.Exists(overlayImagePath))
                throw new FileNotFoundException("No se encontró una de las imágenes especificadas.");

            using (Bitmap baseImage = new Bitmap(baseImagePath))
            using (Bitmap overlay = new Bitmap(overlayImagePath))
            using (Graphics g = Graphics.FromImage(baseImage))
            {
                g.DrawImage(overlay, new Rectangle(x, y, width, height));
                baseImage.Save(outputPath, ImageFormat.Png);
            }
        }
    }
}
