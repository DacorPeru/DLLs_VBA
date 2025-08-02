using System;
using System.IO;
using System.Security.Cryptography;

namespace GenerarRSA
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("🔐 Generando claves RSA (2048 bits)...");

            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    // ✅ Generar claves
                    string privateKeyXml = rsa.ToXmlString(true);  // Con clave privada
                    string publicKeyXml = rsa.ToXmlString(false); // Solo clave pública

                    // ✅ Guardar en archivos
                    File.WriteAllText("privateKey.xml", privateKeyXml);
                    File.WriteAllText("publicKey.xml", publicKeyXml);

                    Console.WriteLine("✅ Claves generadas correctamente.");
                    Console.WriteLine("📄 Archivos creados en la carpeta bin\\Debug:");
                    Console.WriteLine("   - privateKey.xml (solo para la DLL)");
                    Console.WriteLine("   - publicKey.xml (para generar la clave AES cifrada)");
                }
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
