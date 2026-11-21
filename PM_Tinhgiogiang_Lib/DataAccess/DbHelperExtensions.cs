using System;
using System.Data.SqlClient;
using System.Globalization;

namespace PM_Tinhgiogiang_Lib.DataAccess
{
    /// <summary>
    /// Các hàm mở rộng đọc dữ liệu an toàn từ SqlDataReader (decimal, bit, date, time, NULL).
    /// </summary>
    internal static class DbHelperExtensions
    {
        public static SqlCommand CreateCommand(this SqlConnection conn, string sql, SqlTransaction tran = null)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            return cmd;
        }

        public static void AddParam(this SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        public static int GetInt(this SqlDataReader reader, int ordinal)
        {
            return Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        public static int? GetNullableInt(this SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal)
                ? (int?)null
                : Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        public static double GetDoubleVal(this SqlDataReader reader, int ordinal)
        {
            return Convert.ToDouble(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        public static double? GetNullableDoubleVal(this SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal)
                ? (double?)null
                : Convert.ToDouble(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        public static bool GetBool(this SqlDataReader reader, int ordinal, bool defaultValue = false)
        {
            return reader.IsDBNull(ordinal)
                ? defaultValue
                : Convert.ToBoolean(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        public static string GetStr(this SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? string.Empty : (reader.GetValue(ordinal).ToString() ?? string.Empty);
        }

        public static string GetNullableStr(this SqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal).ToString();
        }

        public static string GetDateStr(this SqlDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal)) return string.Empty;
            object val = reader.GetValue(ordinal);
            if (val is DateTime)
            {
                return ((DateTime)val).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return val.ToString() ?? string.Empty;
        }

        public static string GetNullableDateStr(this SqlDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal)) return null;
            object val = reader.GetValue(ordinal);
            if (val is DateTime)
            {
                return ((DateTime)val).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            return val.ToString();
        }

        public static string GetTimeStr(this SqlDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal)) return string.Empty;
            object val = reader.GetValue(ordinal);
            if (val is TimeSpan)
            {
                return ((TimeSpan)val).ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            }
            return val.ToString() ?? string.Empty;
        }
    }
}
