// ==================================
// 📜 Core / Conn.cs
// ==================================
using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace DataConnector.Core
{
    /// <summary>
    /// Maneja conexiones dinámicas a SQL Server, MySQL y PostgreSQL.
    /// </summary>
    public class Conn
    {
        private IDbConnection _connection;

        /// <summary>
        /// Abre una conexión según el tipo de base de datos.
        /// </summary>
        public IDbConnection Open(string tipoBD, string connStr)
        {
            switch (tipoBD.ToLower())
            {
                case "sqlserver":
                    _connection = new SqlConnection(connStr);
                    break;
                case "mysql":
                    _connection = new MySqlConnection(connStr);
                    break;
                case "postgresql":
                    _connection = new NpgsqlConnection(connStr);
                    break;
                default:
                    throw new Exception("Tipo de base de datos no soportado.");
            }

            _connection.Open();
            return _connection;
        }

        /// <summary>
        /// Devuelve la conexión activa.
        /// </summary>
        public IDbConnection GetConnection() => _connection;

        /// <summary>
        /// Cierra la conexión activa.
        /// </summary>
        public void Close()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}
