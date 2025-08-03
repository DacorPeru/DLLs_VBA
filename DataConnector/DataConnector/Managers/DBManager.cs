// ==================================
// 📜 Managers / DBManager.cs
// ==================================
using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using DataConnector.Core;
using DataConnector.Models;

namespace DataConnector.Managers
{
    /// <summary>
    /// Clase COM visible para VBA. Expone métodos simples para conexión y CRUD dinámico.
    /// </summary>
    [ComVisible(true)]
    [Guid("A2B12345-AAAA-BBBB-CCCC-DDDD12345678")] // ⚠️ Usa tu GUID único
    [ProgId("DataConnector.DBManager")] // ✅ Necesario para CreateObject
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class DBManager
    {
        // Campos inmutables → marcados como readonly para evitar advertencias
        private readonly Conn _conn = new Conn();
        private readonly Exec _exec = new Exec();
        private IDbConnection _activeConn;

        /// <summary>
        /// Configura manualmente la conexión a la base de datos.
        /// </summary>
        public Result ConfigConn(string tipoBD, string connStr)
        {
            var result = new Result();
            try
            {
                _activeConn = _conn.Open(tipoBD, connStr);
                result.Ok = true;
                result.Msg = "Conexión establecida correctamente.";
            }
            catch (Exception ex)
            {
                result.Ok = false;
                result.Msg = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Inserta dinámicamente un registro en cualquier tabla.
        /// </summary>
        public Result InsertRow(string table, object fields, object values)
        {
            var f = ((object[])fields).Cast<string>().ToArray();
            var v = ((object[])values);
            string sql = QueryBuilder.BuildInsert(table, f);
            return _exec.Execute(_activeConn, sql, f, v);
        }

        /// <summary>
        /// Actualiza dinámicamente registros en cualquier tabla.
        /// </summary>
        public Result UpdateRow(string table, object fields, object values, string where)
        {
            var f = ((object[])fields).Cast<string>().ToArray();
            var v = ((object[])values);
            string sql = QueryBuilder.BuildUpdate(table, f, where);
            return _exec.Execute(_activeConn, sql, f, v);
        }

        /// <summary>
        /// Elimina registros de una tabla según condición.
        /// </summary>
        public Result DeleteRow(string table, string where)
        {
            string sql = QueryBuilder.BuildDelete(table, where);
            return _exec.Execute(_activeConn, sql, null, null);
        }

        /// <summary>
        /// Obtiene registros como object[,] para VBA.
        /// </summary>
        public Result GetRows(string table, object fields, string where)
        {
            var f = ((object[])fields).Cast<string>().ToArray();
            string sql = QueryBuilder.BuildSelect(table, f, where);
            return _exec.Execute(_activeConn, sql, null, null);
        }

        /// <summary>
        /// Obtiene un valor escalar (COUNT, MAX, etc.).
        /// </summary>
        public Result GetScalar(string table, string field, string where)
        {
            string sql = QueryBuilder.BuildSelect(table, new[] { field }, where);
            return _exec.Execute(_activeConn, sql, null, null);
        }
    }
}
