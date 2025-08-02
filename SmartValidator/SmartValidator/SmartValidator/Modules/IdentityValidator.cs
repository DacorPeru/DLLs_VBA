using SmartValidator.Core;
using SmartValidator.Enums;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SmartValidator.Modules
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class IdentityValidator
    {
        public ValidationResult DNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return new ValidationResult(false, DNIErrorCodes.Empty.ToString(), "El DNI está vacío.");

            if (!Regex.IsMatch(dni, @"^\d+$"))
                return new ValidationResult(false, DNIErrorCodes.InvalidCharacters.ToString(), "El DNI solo debe contener números.");

            if (dni.Length != 8)
                return new ValidationResult(false, DNIErrorCodes.InvalidLength.ToString(), "El DNI debe tener exactamente 8 dígitos.");

            return new ValidationResult(true, DNIErrorCodes.None.ToString(), "DNI válido.");
        }

        public ValidationResult RUC(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc))
                return new ValidationResult(false, RUCErrorCodes.Empty.ToString(), "El RUC está vacío.");

            if (!Regex.IsMatch(ruc, @"^\d+$"))
                return new ValidationResult(false, RUCErrorCodes.InvalidCharacters.ToString(), "El RUC solo debe contener números.");

            if (ruc.Length != 11)
                return new ValidationResult(false, RUCErrorCodes.InvalidLength.ToString(), "El RUC debe tener 11 dígitos.");

            string prefix = ruc.Substring(0, 2);
            if (!(prefix == "10" || prefix == "15" || prefix == "17" || prefix == "20"))
                return new ValidationResult(false, RUCErrorCodes.InvalidPrefix.ToString(), "El RUC debe comenzar con 10, 15, 17 o 20.");

            if (!IsValidChecksum(ruc))
                return new ValidationResult(false, RUCErrorCodes.InvalidChecksum.ToString(), "El RUC no es válido (falló la verificación).");

            return new ValidationResult(true, RUCErrorCodes.None.ToString(), "RUC válido.");
        }

        private bool IsValidChecksum(string ruc)
        {
            int[] factors = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int sum = 0;

            for (int i = 0; i < factors.Length; i++)
                sum += (ruc[i] - '0') * factors[i];

            int remainder = 11 - (sum % 11);
            int checkDigit = remainder == 10 ? 0 : (remainder == 11 ? 1 : remainder);

            return checkDigit == (ruc[10] - '0');
        }
    }
}
