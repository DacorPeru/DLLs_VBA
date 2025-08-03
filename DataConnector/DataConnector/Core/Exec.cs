// ==================================
// 📜 Core / Exec.cs
// ==================================
using System;
using System.Data;
using System.Linq;

namespace DataConnector.Core
{
    /// <summary>
    /// Ejecuta comandos y consultas parametrizadas.
    /// </summary>
    public class Exec
    {
        public Models.Result Execute(IDbConnection conn, string sql, string[] fields, object[] values)
        {
            var result = new Models.Result();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    if (fields != null && values != null)
                    {
                        for (int i = 0; i < fields.Length; i++)
                        {
                            var p = cmd.CreateParameter();
                            p.ParameterName = "@" + fields[i];
                            p.Value = values[i] ?? DBNull.Value;
                            cmd.Parameters.Add(p);
                        }
                    }

                    if (sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var dt = new System.Data.DataTable();
                            dt.Load(reader);
                            result.Ok = true;
                            result.Rows = To2DArray(dt);
                            result.Cols = dt.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName).ToArray();
                        }
                    }
                    else
                    {
                        result.Val = cmd.ExecuteNonQuery();
                        result.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Ok = false;
                result.Msg = ex.Message;
            }

            return result;
        }

        private object[,] To2DArray(System.Data.DataTable dt)
        {
            var rows = new object[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
                for (int j = 0; j < dt.Columns.Count; j++)
                    rows[i, j] = dt.Rows[i][j];
            return rows;
        }
    }
}