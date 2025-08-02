using SmartValidator.Core;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SmartValidator.Modules
{
    /// <summary>
    /// Validador de campos de texto para sistemas empresariales.
    /// Incluye validaciones de campos obligatorios, longitud, correo, etc.
    /// </summary>
    [ComVisible(true)] // Necesario para usarlo desde VBA
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class TextValidator
    {
        /// <summary>
        /// Valida que el texto no esté vacío ni contenga solo espacios.
        /// </summary>
        public ValidationResult CampoObligatorio(string valor, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new ValidationResult(false, "Empty", $"El campo {nombreCampo} es obligatorio.");

            return new ValidationResult(true, "None", $"{nombreCampo} válido.");
        }

        /// <summary>
        /// Valida que el texto tenga una longitud mínima y máxima.
        /// </summary>
        public ValidationResult Longitud(string valor, int minimo, int maximo, string nombreCampo = "Campo")
        {
            if (valor == null) valor = "";

            if (valor.Length < minimo || valor.Length > maximo)
                return new ValidationResult(false, "InvalidLength",
                    $"{nombreCampo} debe tener entre {minimo} y {maximo} caracteres.");

            return new ValidationResult(true, "None", $"{nombreCampo} tiene una longitud válida.");
        }

        /// <summary>
        /// Valida si el texto es un correo electrónico válido.
        /// </summary>
        public ValidationResult CorreoElectronico(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return new ValidationResult(false, "Empty", "El correo electrónico está vacío.");

            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(correo, patron))
                return new ValidationResult(false, "InvalidFormat", "El correo electrónico no tiene un formato válido.");

            return new ValidationResult(true, "None", "Correo electrónico válido.");
        }

        /// <summary>
        /// Valida que el texto solo contenga letras (sin números ni símbolos).
        /// </summary>
        public ValidationResult SoloLetras(string valor, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new ValidationResult(false, "Empty", $"El campo {nombreCampo} es obligatorio.");

            if (!Regex.IsMatch(valor, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                return new ValidationResult(false, "InvalidCharacters", $"{nombreCampo} solo debe contener letras.");

            return new ValidationResult(true, "None", $"{nombreCampo} válido.");
        }

        /// <summary>
        /// Valida que el texto solo contenga letras y números (sin símbolos).
        /// </summary>
        public ValidationResult SoloAlfanumerico(string valor, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new ValidationResult(false, "Empty", $"El campo {nombreCampo} es obligatorio.");

            if (!Regex.IsMatch(valor, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$"))
                return new ValidationResult(false, "InvalidCharacters", $"{nombreCampo} solo debe contener letras y números.");

            return new ValidationResult(true, "None", $"{nombreCampo} válido.");
        }

        /// <summary>
        /// Valida que el texto no contenga caracteres especiales no permitidos.
        /// </summary>
        public ValidationResult SinCaracteresEspeciales(string valor, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new ValidationResult(false, "Empty", $"El campo {nombreCampo} es obligatorio.");

            if (Regex.IsMatch(valor, @"[^a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s@._-]"))
                return new ValidationResult(false, "InvalidCharacters", $"{nombreCampo} contiene caracteres no permitidos.");

            return new ValidationResult(true, "None", $"{nombreCampo} válido.");
        }

        /// <summary>
        /// Valida si el texto cumple con un patrón de expresión regular personalizado.
        /// </summary>
        public ValidationResult CoincidePatron(string valor, string patron, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
                return new ValidationResult(false, "Empty", $"El campo {nombreCampo} es obligatorio.");

            if (!Regex.IsMatch(valor, patron))
                return new ValidationResult(false, "InvalidPattern", $"{nombreCampo} no cumple con el formato esperado.");

            return new ValidationResult(true, "None", $"{nombreCampo} válido.");
        }
    }
}
