using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace SecureLib
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PasswordGenerator
    {
        public string Generate(int length, bool includeLower, bool includeUpper, bool includeNumbers, bool includeSymbols, bool avoidConfusing, bool ensureAllTypes)
        {
            string lowers = avoidConfusing ? "abcdefghijkmnopqrstuvwxyz" : "abcdefghijklmnopqrstuvwxyz";
            string uppers = avoidConfusing ? "ABCDEFGHJKLMNPQRSTUVWXYZ" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = avoidConfusing ? "23456789" : "0123456789";
            string symbols = "!@#$%^&*()-_=+[]{};:,.<>?";

            string allChars = "";
            if (includeLower) allChars += lowers;
            if (includeUpper) allChars += uppers;
            if (includeNumbers) allChars += numbers;
            if (includeSymbols) allChars += symbols;

            if (string.IsNullOrEmpty(allChars))
                throw new ArgumentException("Debe incluir al menos un tipo de carácter.");

            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                if (ensureAllTypes)
                {
                    if (includeLower) password.Append(GetSecureChar(lowers, rng));
                    if (includeUpper) password.Append(GetSecureChar(uppers, rng));
                    if (includeNumbers) password.Append(GetSecureChar(numbers, rng));
                    if (includeSymbols) password.Append(GetSecureChar(symbols, rng));
                }

                while (password.Length < length)
                    password.Append(GetSecureChar(allChars, rng));
            }

            return Shuffle(password.ToString());
        }

        private static char GetSecureChar(string chars, RandomNumberGenerator rng)
        {
            byte[] data = new byte[4];
            rng.GetBytes(data);
            uint value = BitConverter.ToUInt32(data, 0);
            return chars[(int)(value % (uint)chars.Length)];
        }

        private static string Shuffle(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    byte[] data = new byte[4];
                    rng.GetBytes(data);
                    int j = (int)(BitConverter.ToUInt32(data, 0) % (uint)(i + 1));

                    char temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
            return new string(array);
        }
    }
}
