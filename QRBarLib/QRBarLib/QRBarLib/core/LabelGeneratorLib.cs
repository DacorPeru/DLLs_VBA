using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartCodeLib.Core
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class LabelGeneratorLib
    {
        /// <summary>
        /// Genera una etiqueta con QR o código de barras, texto adicional y configuración avanzada.
        /// Devuelve la imagen como Base64.
        /// </summary>
        public string GenerateLabelBase64(
            string codeData, string codeType,
            string labelText, string fontName = "Arial", int fontSize = 14, bool bold = true,
            int labelWidth = 400, int labelHeight = 250,
            string foregroundColor = "#000000", string backgroundColor = "#FFFFFF",
            bool transparentBackground = false)
        {
            // 1️⃣ Generar imagen del código (QR o Barras) usando las otras clases
            string tempPath = Path.GetTempFileName();
            string base64Code = (codeType.ToUpper() == "QR")
                ? new QRCodeGeneratorLib().GenerateQRCodeBase64(codeData, 20, foregroundColor, backgroundColor, transparentBackground)
                : new BarcodeGeneratorLib().GenerateBarcodeBase64(codeData, 300, 100, "CODE128", foregroundColor, backgroundColor);

            byte[] codeBytes = Convert.FromBase64String(base64Code);
            Bitmap codeImage;
            using (MemoryStream ms = new MemoryStream(codeBytes))
                codeImage = new Bitmap(ms);

            // 2️⃣ Crear la etiqueta
            Bitmap labelBitmap = new Bitmap(labelWidth, labelHeight, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(labelBitmap))
            {
                g.Clear(transparentBackground ? Color.Transparent : ColorTranslator.FromHtml(backgroundColor));

                // 📌 Dibujar el código centrado en la parte superior
                int codeX = (labelWidth - codeImage.Width) / 2;
                g.DrawImage(codeImage, codeX, 10);

                // 📌 Dibujar texto debajo
                using (Font font = new Font(fontName, fontSize, bold ? FontStyle.Bold : FontStyle.Regular))
                using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(foregroundColor)))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(labelText, font, brush,
                        new RectangleF(0, codeImage.Height + 20, labelWidth, labelHeight - (codeImage.Height + 20)), sf);
                }
            }

            // 3️⃣ Convertir a Base64
            using (MemoryStream ms = new MemoryStream())
            {
                labelBitmap.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Genera una etiqueta y la guarda como archivo PNG.
        /// </summary>
        public void GenerateLabelToFile(
            string codeData, string codeType, string labelText, string filePath,
            string fontName = "Arial", int fontSize = 14, bool bold = true,
            int labelWidth = 400, int labelHeight = 250,
            string foregroundColor = "#000000", string backgroundColor = "#FFFFFF",
            bool transparentBackground = false)
        {
            string base64 = GenerateLabelBase64(codeData, codeType, labelText, fontName, fontSize, bold,
                                                labelWidth, labelHeight, foregroundColor, backgroundColor,
                                                transparentBackground);
            File.WriteAllBytes(filePath, Convert.FromBase64String(base64));
        }
    }
}
