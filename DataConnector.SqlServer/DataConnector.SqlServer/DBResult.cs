using System;
using System.Runtime.InteropServices;

namespace DataConnector.SqlServer
{
    [ComVisible(true)]  // Necesario para exponer la clase a COM
    [ClassInterface(ClassInterfaceType.AutoDual)] // Permite acceso directo desde VBA
    public class DBResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public int RowsAffected { get; set; }
    }
}
