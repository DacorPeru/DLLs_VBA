using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace SmartCodeLib.Config
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SmartCodeLib", "config.json");

        private Dictionary<string, dynamic> config;

        public ConfigManager()
        {
            if (!File.Exists(ConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
                config = GetDefaultConfig();
                SaveConfig();
            }
            else
            {
                LoadConfig();
            }
        }

        /// <summary>
        /// Establece un valor de configuración global o por empresa.
        /// </summary>
        public void SetConfig(string key, object value, string company = "GLOBAL")
        {
            if (!config.ContainsKey(company))
                config[company] = new Dictionary<string, object>();

            config[company][key] = value;
            SaveConfig();
        }

        /// <summary>
        /// Obtiene un valor de configuración global o por empresa.
        /// </summary>
        public object GetConfig(string key, string company = "GLOBAL")
        {
            if (config.ContainsKey(company) && config[company].ContainsKey(key))
                return config[company][key];

            if (config.ContainsKey("GLOBAL") && config["GLOBAL"].ContainsKey(key))
                return config["GLOBAL"][key];

            return null;
        }

        /// <summary>
        /// Restaura configuraciones predeterminadas.
        /// </summary>
        public void ResetConfig(string company = "GLOBAL")
        {
            if (company == "GLOBAL")
                config = GetDefaultConfig();
            else if (config.ContainsKey(company))
                config[company] = new Dictionary<string, object>();

            SaveConfig();
        }

        /// <summary>
        /// Obtiene la configuración predeterminada inicial.
        /// </summary>
        private Dictionary<string, dynamic> GetDefaultConfig()
        {
            return new Dictionary<string, dynamic>
            {
                ["GLOBAL"] = new Dictionary<string, object>
                {
                    {"DefaultPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Codigos")},
                    {"DefaultDPI", 300},
                    {"DefaultQRSize", 250},
                    {"DefaultBarcodeWidth", 300},
                    {"DefaultBarcodeHeight", 100},
                    {"ForegroundColor", "#000000"},
                    {"BackgroundColor", "#FFFFFF"},
                    {"DefaultFont", "Arial"},
                    {"DefaultFontSize", 14},
                    {"BoldText", true},
                    {"OutputFormat", "PNG"}
                }
            };
        }

        /// <summary>
        /// Guarda la configuración en un archivo JSON.
        /// </summary>
        private void SaveConfig()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }

        /// <summary>
        /// Carga la configuración desde el archivo JSON.
        /// </summary>
        private void LoadConfig()
        {
            string json = File.ReadAllText(ConfigPath);
            config = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json);
        }
    }
}
