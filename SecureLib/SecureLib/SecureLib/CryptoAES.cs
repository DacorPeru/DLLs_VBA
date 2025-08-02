using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace SecureLib
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class CryptoAES
    {
        public string EncryptToBase64(string plainText, string keyBase64, string ivBase64)
        {
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }

        public string DecryptFromBase64(string cipherTextBase64, string keyBase64, string ivBase64)
        {
            byte[] cipherText = Convert.FromBase64String(cipherTextBase64);
            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
