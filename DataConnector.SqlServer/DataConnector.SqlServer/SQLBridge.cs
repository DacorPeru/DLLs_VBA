using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataConnector.SqlServer
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SQLBridge
    {
        // ✅ Probar conexión
        public bool TestConnection(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // ✅ Ejecutar SELECT y devolver resultado como JSON
        public string RunQuery(string sql, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return JsonConvert.SerializeObject(dt);
                }
            }
        }

        // ✅ Ejecutar INSERT/UPDATE/DELETE y devolver filas afectadas
        public int RunNonQuery(string sql, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // ✅ Ejecutar Stored Procedure y devolver resultado como JSON
        public string RunStoredProcedure(string procedureName, string parametersJson, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (!string.IsNullOrWhiteSpace(parametersJson))
                    {
                        var jsonParams = JObject.Parse(parametersJson);
                        foreach (var kv in jsonParams)
                        {
                            object valor = kv.Value.Type == JTokenType.Null
                                ? DBNull.Value
                                : kv.Value.ToObject<object>();
                            cmd.Parameters.AddWithValue("@" + kv.Key, valor ?? DBNull.Value);
                        }
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        return JsonConvert.SerializeObject(dt);
                    }
                }
            }
        }

        // ✅ Ejecutar SELECT y exportar resultados directamente a una hoja de Excel
        public bool RunQueryToSheet(string sql, string connectionString, string sheetName)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        var excelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");

                        if (excelApp == null || excelApp.ActiveWorkbook == null)
                            throw new Exception("Excel no está abierto o no hay un libro activo.");

                        Excel.Workbook workbook = excelApp.ActiveWorkbook;
                        Excel.Worksheet sheet = null;

                        // Eliminar hoja existente con el mismo nombre si existe
                        foreach (Excel.Worksheet ws in workbook.Sheets)
                        {
                            if (ws.Name == sheetName)
                            {
                                ws.Delete();
                                break;
                            }
                        }

                        // Agregar nueva hoja y renombrarla
                        sheet = (Excel.Worksheet)workbook.Sheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                        sheet.Name = sheetName;

                        // Escribir encabezados
                        for (int col = 0; col < dt.Columns.Count; col++)
                            sheet.Cells[1, col + 1].Value = dt.Columns[col].ColumnName;

                        // Escribir datos
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                sheet.Cells[i + 2, j + 1].Value = dt.Rows[i][j];
                            }
                        }

                        // Ajustar columnas automáticamente
                        sheet.Columns.AutoFit();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Error en RunQueryToSheet: " + ex.Message);
                return false;
            }
        }
    }
}
