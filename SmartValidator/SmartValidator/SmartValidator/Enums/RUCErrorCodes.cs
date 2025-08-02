namespace SmartValidator.Enums
{
    /// <summary>
    /// Códigos de error estándar para validación de RUC.
    /// </summary>
    public enum RUCErrorCodes
    {
        None,
        Empty,
        InvalidCharacters,
        InvalidLength,
        InvalidPrefix,
        InvalidChecksum
    }
}
