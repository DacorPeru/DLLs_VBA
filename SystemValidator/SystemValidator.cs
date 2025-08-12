using System;
using System.Runtime.InteropServices;
using DataConnector.Core;

namespace DataConnector.Validation
{
    /// <summary>
    /// Fachada COM expuesta a VBA (ProgId estable).
    /// Mantiene el último resultado de operación con:
    ///   - GetLastCode(): "OK" | "CFG-001" | "SQL-001"
    ///   - GetLastMessage(): texto amigable para usuario
    ///   - GetLastTechnical(): detalle técnico para soporte
    /// </summary>
    [ComVisible(true)]
    [Guid("F5B7CBE3-7E9D-4E35-9E58-3C6C9F4C8D21")] // Genera tu propio GUID (Herramientas > Crear GUID)
    [ProgId("DataConnector.Validation.SystemValidator")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SystemValidator
    {
        private string _code = "OK";
        private string _message = string.Empty;
        private string _technical = string.Empty;

        /// <summary>
        /// Carga el archivo de configuración JSON desde disco.
        /// Ej: C:\Secure\Config\systemvalidator.json
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

                // Importante para que VBA pueda capturar el error COM si quiere
                throw new COMException($"{_code}: {_message}");
            }
        }

        /// <summary>
        /// Valida la conectividad a SQL Server usando la config cargada.
        /// Retorna True/False para facilitar su uso en VBA.
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
