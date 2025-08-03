// ==================================
// 📜 Core / QueryBuilder.cs
// ==================================
using System.Linq;

namespace DataConnector.Core
{
    /// <summary>
    /// Construye dinámicamente sentencias SQL para CRUD.
    /// </summary>
    public static class QueryBuilder
    {
        public static string BuildInsert(string table, string[] fields)
        {
            string columns = string.Join(",", fields);
            string parameters = string.Join(",", fields.Select(f => "@" + f));
            return $"INSERT INTO {table} ({columns}) VALUES ({parameters});";
        }

        public static string BuildUpdate(string table, string[] fields, string where)
        {
            string set = string.Join(",", fields.Select(f => $"{f}=@{f}"));
            return $"UPDATE {table} SET {set} WHERE {where};";
        }

        public static string BuildDelete(string table, string where)
        {
            return $"DELETE FROM {table} WHERE {where};";
        }

        public static string BuildSelect(string table, string[] fields, string where)
        {
            string columns = fields.Length > 0 ? string.Join(",", fields) : "*";
            string sql = $"SELECT {columns} FROM {table}";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $" WHERE {where}";
            return sql;
        }
    }
}
