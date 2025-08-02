using System.Runtime.InteropServices;

namespace SmartValidator.Core
{
    /// <summary>
    /// Resultado estándar de validaciones empresariales.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorCode { get; }
        public string Message { get; }

        public ValidationResult(bool isValid, string errorCode = "", string message = "")
        {
            IsValid = isValid;
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
