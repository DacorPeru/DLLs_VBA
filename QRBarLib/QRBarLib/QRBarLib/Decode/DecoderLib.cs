using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Common;

namespace SmartCodeLib.Core
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class DecoderLib
    {
        /// <summary>
        /// 📂 Decodifica un QR o código de barras desde un archivo de imagen.
        /// </summary>
        public string DecodeFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("No se encontró la imagen especificada.", filePath);

            using (Bitmap bitmap = new Bitmap(filePath))
            {
                return DecodeBitmap(bitmap);
            }
        }

        /// <summary>
        /// 🖼️ Decodifica un QR o código de barras desde una cadena Base64.
        /// </summary>
        public string DecodeFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(bytes))
            using (Bitmap bitmap = new Bitmap(ms))
            {
                return DecodeBitmap(bitmap);
            }
        }

        /// <summary>
        /// 🔍 Lógica central para decodificar desde un objeto Bitmap.
        /// </summary>
        private string DecodeBitmap(Bitmap bitmap)
        {
            var options = new DecodingOptions
            {
                TryHarder = true,
                TryInverted = true, // ✅ Ahora se usa dentro de Options
                PossibleFormats = new[]
                {
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.EAN_13,
                    BarcodeFormat.EAN_8,
                    BarcodeFormat.UPC_A,
                    BarcodeFormat.UPC_E,
                    BarcodeFormat.CODE_39,
                    BarcodeFormat.CODE_93,
                    BarcodeFormat.ITF
                }
            };

            var reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = options
            };

            var result = reader.Decode(bitmap);
            return result?.Text ?? string.Empty;
        }
    }
}
