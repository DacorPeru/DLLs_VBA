using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Common;

namespace SmartCodeLib.Core
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class BarcodeGeneratorLib
    {
        /// <summary>
        /// Genera un código de barras y devuelve la imagen como Base64.
        /// </summary>
        public string GenerateBarcodeBase64(
            string text,
            int width,
            int height,
            string format = "CODE128",
            string label = "",
            string foregroundColor = "#000000",
            string backgroundColor = "#FFFFFF")
        {
            BarcodeFormat barcodeFormat = GetBarcodeFormat(format);

            var writer = new BarcodeWriterPixelData
            {
                Format = barcodeFormat,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 2,
                    PureBarcode = string.IsNullOrEmpty(label)
                }
            };

            var pixelData = writer.Write(text);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb))
            {
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // Aplicar colores y agregar etiqueta
                Bitmap finalImage = ApplyColors(bitmap, foregroundColor, backgroundColor);
                if (!string.IsNullOrEmpty(label))
                {
                    Bitmap labeledImage = AddLabelToBarcode(finalImage, label, foregroundColor);
                    finalImage.Dispose();
                    finalImage = labeledImage;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    finalImage.Save(ms, ImageFormat.Png);
                    finalImage.Dispose();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Guarda un código de barras directamente en un archivo PNG.
        /// </summary>
        public void GenerateBarcodeToFile(
            string text,
            string filePath,
            int width,
            int height,
            string format = "CODE128",
            string label = "",
            string foregroundColor = "#000000",
            string backgroundColor = "#FFFFFF")
        {
            string base64 = GenerateBarcodeBase64(text, width, height, format, label, foregroundColor, backgroundColor);
            byte[] bytes = Convert.FromBase64String(base64);
            File.WriteAllBytes(filePath, bytes);
        }

        /// <summary>
        /// Convierte el nombre del formato en BarcodeFormat (compatible con C# 7.3).
        /// </summary>
        private BarcodeFormat GetBarcodeFormat(string format)
        {
            switch (format.ToUpper())
            {
                case "EAN13": return BarcodeFormat.EAN_13;
                case "EAN8": return BarcodeFormat.EAN_8;
                case "UPC_A": return BarcodeFormat.UPC_A;
                case "UPC_E": return BarcodeFormat.UPC_E;
                case "CODE39": return BarcodeFormat.CODE_39;
                case "CODE93": return BarcodeFormat.CODE_93;
                case "ITF": return BarcodeFormat.ITF;
                default: return BarcodeFormat.CODE_128;
            }
        }

        /// <summary>
        /// Aplica colores personalizados al código de barras.
        /// </summary>
        private Bitmap ApplyColors(Bitmap original, string foregroundHex, string backgroundHex)
        {
            Bitmap coloredBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            Color fg = ColorTranslator.FromHtml(foregroundHex);
            Color bg = ColorTranslator.FromHtml(backgroundHex);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    coloredBitmap.SetPixel(x, y, pixel.R < 128 ? fg : bg);
                }
            }

            return coloredBitmap;
        }

        /// <summary>
        /// Agrega una etiqueta debajo del código de barras.
        /// </summary>
        private Bitmap AddLabelToBarcode(Bitmap barcode, string label, string colorHex)
        {
            Bitmap newBitmap = new Bitmap(barcode.Width, barcode.Height + 30);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(barcode, 0, 0);

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(colorHex)))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(label, font, brush, new RectangleF(0, barcode.Height, barcode.Width, 30), sf);
                }
            }

            return newBitmap;
        }
    }
}
