using System;
using System.Security.Cryptography; // ProtectedData y DataProtectionScope
using System.Text;

namespace DataConnector.Core
{
    /// <summary>
    /// Utilidades para proteger/recuperar secretos usando DPAPI (ámbito: maquina).
    /// - Protect: recibe texto plano y devuelve Base64 cifrado con DPAPI.
    /// - Unprotect: recibe Base64 y devuelve el texto original.
    ///
    /// Nota:
    ///  • DataProtectionScope.LocalMachine permite descifrar en cualquier usuario de la MISMA máquina.
    ///  • Es ideal para guardar contraseñas en archivos de configuración en servidores/equipos.
    /// </summary>
    internal static class SecureString
    {
        /// <summary>Protege (cifra) un string y devuelve Base64 DPAPI (LocalMachine).</summary>
        public static string Protect(string plain)
        {
            if (string.IsNullOrWhiteSpace(plain)) return string.Empty;
            var bytes = Encoding.UTF8.GetBytes(plain);
            var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(protectedBytes);
        }

        /// <summary>Desprotege (descifra) un Base64 DPAPI (LocalMachine) y devuelve texto plano.</summary>
        public static string Unprotect(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
            var data = Convert.FromBase64String(base64);
            var raw = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(raw);
        }
    }
}
