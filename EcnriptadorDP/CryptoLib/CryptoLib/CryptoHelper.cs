using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib
{
    // ✅ Clase visible para COM (para usar desde VBA)
    [ComVisible(true)]
    [Guid("A1234567-B89C-4DEF-8123-ABCDEF987654")] // ⚠️ Genera un nuevo GUID desde Herramientas > Crear GUID
    [ProgId("CryptoLib.CryptoHelper")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class CryptoHelper
    {
        private byte[] Key; // Clave AES descifrada
        private byte[] IV;  // Vector de inicialización dinámico

        public CryptoHelper()
        {
            try
            {
                // 📌 1️⃣ Aquí debes pegar TODO el contenido de tu privateKey.xml
                string privateKeyXml = @"<RSAKeyValue>
<Modulus>qsITO85Mosp3IhgYqVNwKUg4Kt22pkkmNapzKYxHueFqFOY1OSmfFcX79TQQWEAZw8TgMVulUmCbkqvdkNfMkwehzJshrKLgXYQFXo+FkUWsKx0oD5RAA075VRk6efDqs1oPxGbg3/haxF8yJh8q9KtR78tMg3EuZV1ihBq1LEHM8UHutBFncibBsG46Wl9sdyU2OFSJw8FrN7Y8K2+GUglm9y8pPplcol4Zlea3xjbIO6xqh6/XSYleFkQASS/HtuIpvXY9AX8Hwv9mN2G1x6XeSn/hkY8tgHbeMZFtifaUtsRqGqVmcaoDeV/VU5MGuz6iZL6iKuirTeLfoLDVvQ==</Modulus>
<Exponent>AQAB</Exponent>
<P>wOiJy9U26Rrmj6jGbeDMagrF8fWpddy2rYYrKHPAf1/lCxEJDLtJLu/irJFAt5DgwnGZJODZRBCl2wA3l2kxFNRlMKfZBNT76b81j1600zCkoYK23mnF5wVJ0K/gnqaq4TX45S0gwa4TnuoNexbzb7njhvXNREGHX7X+FjFfPes=</P>
<Q>4pr7vbqs/uaxk3oeQ3MoZyfT4RW/aziY2Uz88NmUKDWK2oQO7VIaUsvKjAvIObk3/3Dv97CNfNPug8eF2f9Xf7zzPvcFFvQ8uYFHHQ3Nlpz3xZgusaggBXKiapcYCvw5jDP66vDqayjoPXiOlxWgFKblaBZDh9L48ZAcWaqmSPc=</Q>
<DP>gbATa2hU+ROvqybg99+oLSY++zKXkXgni7LmctSWtksmtXDnb5q9UYPOFQu1wXLx12b9ePB8wCidCAU2WmFKsOE1gidlW70/+l4kyNT3pf9OvpIdFJtxg6VngZer35zWxn89a/0Rx4ObBzv8giLk19jVoNbA/8E6HFtCjXL1v68=</DP>
<DQ>TZHEDwRx56fMrbEl3uzyra0iiHBLYEa1e90w0LmKT+FUMMz31yE9xY3hPeNinO/cTWK5ok4bgNmDUjTgiukvaJijo7AnW94rU0z/rOBmueHtpwQbJ9FndLXzBw3Fuyzv/iBlCjuB40DTjwdPuEQ80hT3fpPwdfQwJ3CvY7pYxa0=</DQ>
<InverseQ>tsntqDAnBGql9lS1E5FHLUZaAZU1aC+m52DaJkjLKkf0vFWDkayzSsVQX15EPG2G4I5S/SczZI9K/vqlaBD9jfFi5YBBPOH8Wj5OtO71bme1Toa/gVFwTyuv5og+3w5GT8H/K9Qu+v3f2fnr7MzKTZH6ClFkT+7KldC5gCZ5C1Y=</InverseQ>
<D>GJCa27ut4q1lvg3hXlqWhBnTmrcSk5FqYcYHky6PEvqOnDsR5QtLy4IT4Q6AW3G9QyCLcA5TFiq3tr3qve+NiolKiXi3u3Ii/NEFRLk46dEwF3O2U6dsHg26aLecZfZ2jR6jrgluyEmeJSncuQc60r/ipbCFbVNW7VYl2gNNlPQWAadclXvGHZcm0A29HKMxewbiWY/9UDsqNB0Ofw02hcM/jbDwJRAuUYmYk7Sz3R8koqYljQyuoA9uuwk4fLEQ9pF9qPERtlrKEsxjlf/RmkLKVprIpqjepi/rh/w6En/WD4Li5Cf7qXjsPqYi6PVQ91p/Vn6LkeWpLf1ztsM+MQ==</D>
</RSAKeyValue>";

                // 📌 2️⃣ Buscar automáticamente el archivo aesKey.enc en la carpeta de la DLL
                string dllFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string encFilePath = Path.Combine(dllFolder, "aesKey.enc");

                if (!File.Exists(encFilePath))
                    throw new FileNotFoundException($"No se encontró el archivo aesKey.enc en: {encFilePath}");

                byte[] encryptedKey = File.ReadAllBytes(encFilePath);

                // 📌 3️⃣ Descifrar la clave AES con la clave privada RSA
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKeyXml);
                    Key = rsa.Decrypt(encryptedKey, true);
                }

                // Inicializar IV vacío (se generará uno nuevo en cada cifrado)
                IV = new byte[16];
            }
            catch (Exception ex)
            {
                throw new Exception("❌ Error al inicializar CryptoHelper: " + ex.Message);
            }
        }

        // 🔒 Encripta texto en Base64 con AES-256
        public string EncryptAES(string plainText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;

                    // 🔹 Generar un IV aleatorio para cada cifrado
                    aes.IV = new byte[16];
                    using (var rng = RandomNumberGenerator.Create())
                        rng.GetBytes(aes.IV);

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] encryptedBytes = PerformCryptography(plainBytes, encryptor);

                        // 🔹 Combinar IV + datos cifrados en un solo arreglo
                        byte[] combined = new byte[aes.IV.Length + encryptedBytes.Length];
                        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                        Buffer.BlockCopy(encryptedBytes, 0, combined, aes.IV.Length, encryptedBytes.Length);

                        return Convert.ToBase64String(combined);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        // 🔓 Desencripta texto en Base64 con AES-256
        public string DecryptAES(string cipherText)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(cipherText);

                // 🔹 Separar IV y datos cifrados
                byte[] iv = new byte[16];
                byte[] encryptedData = new byte[combined.Length - 16];
                Buffer.BlockCopy(combined, 0, iv, 0, 16);
                Buffer.BlockCopy(combined, 16, encryptedData, 0, encryptedData.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        byte[] decryptedBytes = PerformCryptography(encryptedData, decryptor);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }

        // 🔹 Método auxiliar para cifrar y descifrar
        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }
}
