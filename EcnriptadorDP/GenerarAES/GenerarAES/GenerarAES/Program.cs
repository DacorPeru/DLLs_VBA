using System;
using System.IO;
using System.Security.Cryptography;

namespace GenerarAES
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("🔐 Generando clave AES cifrada con RSA...");

            try
            {
                // ✅ Leer la clave pública RSA (debe estar en la misma carpeta que el .exe)
                string publicKeyXml = File.ReadAllText("publicKey.xml");

                // ✅ Generar una clave AES aleatoria de 32 bytes (AES-256)
                byte[] aesKey = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(aesKey);
                }

                // ✅ Cifrar la clave AES con la clave pública RSA
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXml);
                    byte[] encryptedKey = rsa.Encrypt(aesKey, true);

                    // ✅ Guardar la clave cifrada en un archivo
                    File.WriteAllBytes("aesKey.enc", encryptedKey);
                }

                Console.WriteLine("✅ Clave AES cifrada creada correctamente.");
                Console.WriteLine("📄 Archivo generado: aesKey.enc (guárdalo junto a tu DLL).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
