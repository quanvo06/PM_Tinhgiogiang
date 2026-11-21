using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PM_Tinhgiogiang_Lib.DataAccess
{
    /// <summary>
    /// Lớp hỗ trợ kết nối SQL Server (PM_Tinhgiogiang) thông qua System.Data.SqlClient.
    /// Tự động đọc chuỗi kết nối từ App.config ("PM_TinhgiogiangConnectionString").
    /// </summary>
    public class DatabaseHelper
    {
        public const string DefaultConnectionName = "PM_TinhgiogiangConnectionString";

        public string ConnectionString { get; private set; }

        public DatabaseHelper(string connectionString = null)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ConnectionString = connectionString;
                return;
            }

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[DefaultConnectionName];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Không tìm thấy chuỗi kết nối '" + DefaultConnectionName + "' trong tệp App.config.");
            }

            ConnectionString = settings.ConnectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public SqlConnection OpenConnection()
        {
            try
            {
                SqlConnection conn = CreateConnection();
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Không thể kết nối tới máy chủ SQL Server (PM_Tinhgiogiang): " + ex.Message, ex);
            }
        }
    }
}
