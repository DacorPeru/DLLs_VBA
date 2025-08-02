using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace SecureLib
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PasswordHasher
    {
        public string HashPassword(string password, int iterations)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, 16, iterations))
            {
                byte[] salt = rfc2898.Salt;
                byte[] key = rfc2898.GetBytes(32);

                return iterations + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(key);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('.');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] key = Convert.FromBase64String(parts[2]);

            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] keyToCheck = rfc2898.GetBytes(32);
                return FixedTimeEquals(key, keyToCheck);
            }
        }

        private bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int result = 0;
            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];
            return result == 0;
        }
    }
}
