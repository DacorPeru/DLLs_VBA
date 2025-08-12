using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DataConnector.SqlServer
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class DBConnectionManager
    {
        private SqlConnection _connection;

        public DBResult Connect(string connectionString)
        {
            DBResult result = new DBResult();

            try
            {
                _connection = new SqlConnection(connectionString);
                _connection.Open();
                result.Success = true;
                result.Message = "Conexión exitosa";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error de conexión: " + ex.Message;
            }

            return result;
        }

        public void Disconnect()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                _connection.Close();
        }

        public bool IsConnected()
        {
            return _connection != null && _connection.State == System.Data.ConnectionState.Open;
        }

        public SqlConnection GetConnection()
        {
            return _connection;
        }
    }
}
