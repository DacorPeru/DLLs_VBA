using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace PasswordLib
{
    [ComVisible(true)]
    [Guid("a1b2c3d4-e5f6-7890-ab12-c3d4e5f67890")]    // Genera un GUID único en Herramientas > Crear GUID
    [ProgId("PasswordLib.PasswordHelper")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PasswordHelper
    {
        /// <summary>
        /// Genera un hash seguro para almacenar en la base de datos
        /// </summary>
        public string GeneratePasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            // Generar SALT aleatorio
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            // Derivar clave con PBKDF2 (SHA256)
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] key = pbkdf2.GetBytes(32);

                // Combinar SALT + HASH
                byte[] hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(key, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado
        /// </summary>
        public bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                return false;

            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extraer SALT
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Derivar clave con la contraseña ingresada
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] key = pbkdf2.GetBytes(32);

                // Comparar byte a byte
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != key[i])
                        return false;
                }
                return true;
            }
        }
    }
}
