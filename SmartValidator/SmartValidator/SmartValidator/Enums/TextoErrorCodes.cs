namespace SmartValidator.Enums
{
    /// <summary>
    /// Códigos de error estándar para validaciones de texto.
    /// </summary>
    public enum TextoErrorCodes
    {
        Ninguno,            // ✅ Sin error
        Vacio,              // ❌ Campo vacío
        LongitudInvalida,    // ❌ Longitud fuera de rango
        FormatoInvalido,     // ❌ Formato incorrecto (ej. correo)
        CaracteresInvalidos, // ❌ Contiene caracteres no permitidos
        PatronInvalido       // ❌ No cumple con la expresión regular
    }
}
