using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.QrCode;

namespace SmartCodeLib.Core
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class QRCodeGeneratorLib
    {
        /// <summary>
        /// Genera un código QR y devuelve la imagen como Base64.
        /// </summary>
        /// <param name="text">Texto o URL a codificar.</param>
        /// <param name="pixelsPerModule">Tamaño de cada módulo del QR.</param>
        /// <param name="foregroundColor">Color del QR (HEX #000000).</param>
        /// <param name="backgroundColor">Color de fondo (HEX #FFFFFF).</param>
        /// <param name="transparentBackground">Fondo transparente si es TRUE.</param>
        /// <param name="label">Texto opcional debajo del QR.</param>
        /// <returns>Imagen en Base64.</returns>
        public string GenerateQRCodeBase64(string text, int pixelsPerModule = 20,
                                           string foregroundColor = "#000000",
                                           string backgroundColor = "#FFFFFF",
                                           bool transparentBackground = false,
                                           string label = "")
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = pixelsPerModule * 10,
                    Width = pixelsPerModule * 10,
                    Margin = 1,
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                }
            };

            var pixelData = writer.Write(text);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                                 ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                Bitmap finalImage = ApplyColors(bitmap, foregroundColor, backgroundColor, transparentBackground);
                finalImage = AddLabelToQR(finalImage, label, foregroundColor);

                using (MemoryStream ms = new MemoryStream())
                {
                    finalImage.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Genera y guarda el QR como archivo PNG.
        /// </summary>
        public void GenerateQRCodeToFile(string text, string filePath, int pixelsPerModule = 20,
                                         string foregroundColor = "#000000",
                                         string backgroundColor = "#FFFFFF",
                                         bool transparentBackground = false,
                                         string label = "")
        {
            string base64 = GenerateQRCodeBase64(text, pixelsPerModule, foregroundColor,
                                                 backgroundColor, transparentBackground, label);
            byte[] bytes = Convert.FromBase64String(base64);
            File.WriteAllBytes(filePath, bytes);
        }

        /// <summary>
        /// Aplica colores personalizados al QR.
        /// </summary>
        private Bitmap ApplyColors(Bitmap bitmap, string foregroundHex, string backgroundHex, bool transparent)
        {
            Color fg = ColorTranslator.FromHtml(foregroundHex);
            Color bg = transparent ? Color.Transparent : ColorTranslator.FromHtml(backgroundHex);

            Bitmap colored = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    colored.SetPixel(x, y, pixel.R < 128 ? fg : bg);
                }
            }
            return colored;
        }

        /// <summary>
        /// Agrega una etiqueta opcional debajo del QR.
        /// </summary>
        private Bitmap AddLabelToQR(Bitmap qrImage, string label, string colorHex)
        {
            if (string.IsNullOrEmpty(label))
                return qrImage;

            Bitmap newBitmap = new Bitmap(qrImage.Width, qrImage.Height + 30);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(qrImage, 0, 0);

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (Brush brush = new SolidBrush(ColorTranslator.FromHtml(colorHex)))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(label, font, brush, new RectangleF(0, qrImage.Height, qrImage.Width, 30), sf);
                }
            }
            return newBitmap;
        }
    }
}
