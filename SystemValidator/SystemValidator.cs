using System;
using System.IO;
using System.Runtime.InteropServices;
using DataConnector.Core;

namespace DataConnector.Validation
{
    /// <summary>
    /// Fachada COM expuesta a VBA (ProgId estable).
    /// Mantiene el último resultado de operación:
    ///  • GetLastCode(): "OK" | "CFG-001" | "SQL-001"
    ///  • GetLastMessage(): mensaje para usuario
    ///  • GetLastTechnical(): detalle técnico para soporte
    ///
    /// MÉTODOS:
    ///  • LoadConfig(jsonPath): carga desde una ruta específica.
    ///  • LoadConfigAuto(): busca el JSON en ubicaciones estándar (ProgramData, junto a la DLL).
    ///  • PingSql(): valida la conexión con la config cargada.
    /// </summary>
    [ComVisible(true)]
    [Guid("F5B7CBE3-7E9D-4E35-9E58-3C6C9F4C8D21")] // Cambia por tu propio GUID
    [ProgId("DataConnector.Validation.SystemValidator")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SystemValidator
    {
        private string _code = "OK";
        private string _message = string.Empty;
        private string _technical = string.Empty;

        /// <summary>
        /// Carga el archivo de configuración JSON desde una ruta dada.
        /// Úsalo si tu instalador conoce la ruta final (pasada desde VBA).
        /// </summary>
        public void LoadConfig(string jsonPath)
        {
            Reset();
            try
            {
                ConfigManager.LoadFromFile(jsonPath);
                _message = "Configuración cargada.";
            }
            catch (Exception ex)
            {
                _code = "CFG-001";
                _message = "No se pudo cargar la configuración.";
                _technical = ex.ToString();
                throw new COMException($"{_code}: {_message}"); // propagación COM
            }
        }

        /// <summary>
        /// Carga automáticamente el JSON desde ubicaciones estándar (sin rutas fijas):
        ///  1) %PROGRAMDATA%\MiEmpresa\SystemValidator\systemvalidator.json
        ///  2) Carpeta de la DLL COM (junto a DataConnector.Validation.dll)
        /// Lanza error si no encuentra en ninguna ubicación.
        /// </summary>
        public void LoadConfigAuto()
        {
            Reset();
            try
            {
                var candidates = new System.Collections.Generic.List<string>();

                // 1) ProgramData (todos los usuarios)
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                candidates.Add(Path.Combine(programData, "MiEmpresa", "SystemValidator", "systemvalidator.json"));

                // 2) Junto a la DLL COM
                var dllDir = Path.GetDirectoryName(typeof(SystemValidator).Assembly.Location);
                if (!string.IsNullOrEmpty(dllDir))
                    candidates.Add(Path.Combine(dllDir, "systemvalidator.json"));

                string found = null;
                foreach (var p in candidates)
                    if (File.Exists(p)) { found = p; break; }

                if (found == null)
                    throw new FileNotFoundException("No se encontró systemvalidator.json en ubicaciones conocidas.");

                ConfigManager.LoadFromFile(found);
                _message = $"Configuración cargada: {found}";
            }
            catch (Exception ex)
            {
                _code = "CFG-001";
                _message = "No se pudo cargar la configuración (auto).";
                _technical = ex.ToString();
                throw new COMException($"{_code}: {_message}");
            }
        }

        /// <summary>
        /// Valida la conectividad a SQL Server usando la configuración cargada.
        /// Retorna true/false para uso directo en VBA.
        /// </summary>
        public bool PingSql()
        {
            Reset();
            var (ok, msg, tech) = ConnectionTester.Test();
            _code = ok ? "OK" : "SQL-001";
            _message = msg;
            _technical = tech;
            return ok;
        }

        // --- Diagnóstico para VBA ---
        public string GetLastCode() => _code;
        public string GetLastMessage() => _message;
        public string GetLastTechnical() => _technical;

        private void Reset()
        {
            _code = "OK";
            _message = string.Empty;
            _technical = string.Empty;
        }
    }
}
