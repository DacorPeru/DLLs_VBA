using System;
using System.IO;
using Newtonsoft.Json;

namespace DataConnector.Core
{
    public sealed class AppConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string AuthMode { get; set; } = "SQL";
        public string User { get; set; }
        public string PasswordDpapi { get; set; }
        public bool Encrypt { get; set; } = true;
        public bool TrustServerCertificate { get; set; } = false;
        public int ConnectTimeoutMs { get; set; } = 15000;
    }

    // ▼ HAZLA PUBLIC
    public static class ConfigManager
    {
        private static AppConfig _cfg;
        // ▼ EVITAR target-typed new (C# 7.3)
        private static readonly object _lock = new object();

        public static void LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config no encontrado: {path}");

            var json = File.ReadAllText(path);
            var cfg = JsonConvert.DeserializeObject<AppConfig>(json)
                      ?? throw new InvalidOperationException("Config inválida (JSON nulo).");

            if (string.Equals(cfg.AuthMode, "SQL", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(cfg.User))
                    throw new InvalidOperationException("User faltante para AuthMode=SQL.");
                if (string.IsNullOrWhiteSpace(cfg.PasswordDpapi))
                    throw new InvalidOperationException("PasswordDpapi faltante para AuthMode=SQL.");

                cfg.PasswordDpapi = SecureString.Unprotect(cfg.PasswordDpapi);
            }

            lock (_lock) _cfg = cfg;
        }

        public static AppConfig Current
        {
            get
            {
                lock (_lock)
                {
                    return _cfg ?? throw new InvalidOperationException("Config no cargada. Llama primero a LoadFromFile().");
                }
            }
        }

        public static string BuildConnectionString()
        {
            var c = Current;
            var sb = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = c.Server,
                InitialCatalog = c.Database,
                ConnectTimeout = Math.Max(1, c.ConnectTimeoutMs / 1000),
                Encrypt = c.Encrypt,
                TrustServerCertificate = c.TrustServerCertificate
            };
            if (string.Equals(c.AuthMode, "Windows", StringComparison.OrdinalIgnoreCase))
                sb.IntegratedSecurity = true;
            else { sb.IntegratedSecurity = false; sb.UserID = c.User; sb.Password = c.PasswordDpapi; }
            return sb.ConnectionString;
        }

        public static string GeneratePasswordDpapi(string plainPassword) => SecureString.Protect(plainPassword);
    }
}
