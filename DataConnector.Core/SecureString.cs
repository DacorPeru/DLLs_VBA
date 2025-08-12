using System;
using System.Security.Cryptography;   // ← IMPORTANTE
using System.Text;

namespace DataConnector.Core
{
    internal static class SecureString
    {
        public static string Protect(string plain)
        {
            if (string.IsNullOrWhiteSpace(plain)) return string.Empty;
            var bytes = Encoding.UTF8.GetBytes(plain);
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(protectedBytes);
        }

        public static string Unprotect(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
            var data = Convert.FromBase64String(base64);
            var raw = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(raw);
        }
    }
}
