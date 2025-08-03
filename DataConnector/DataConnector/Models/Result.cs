// ==================================
// 📜 Models / Result.cs
// ==================================
using System;
using System.Runtime.InteropServices;

namespace DataConnector.Models
{
    /// <summary>
    /// Clase estándar para devolver resultados de cualquier operación.
    /// Compatible con VBA mediante COM.
    /// </summary>
    [ComVisible(true)]
    [Guid("F1A12345-1111-2222-3333-444455556666")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Result
    {
        public bool Ok { get; set; }          // Indica si la operación fue exitosa
        public string Msg { get; set; }       // Mensaje de error o éxito
        public object Val { get; set; }       // Valor escalar (COUNT, MAX, etc.)
        public object[,] Rows { get; set; }   // Filas de resultados para VBA
        public string[] Cols { get; set; }    // Nombres de columnas
    }
}
