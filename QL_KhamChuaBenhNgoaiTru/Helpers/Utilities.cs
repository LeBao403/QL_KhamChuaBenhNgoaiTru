using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Helpers
{
    public static class Utilities
    {
        public static string Generate(SqlConnection conn, SqlTransaction trans, string prefix, string tableName, string idColumnName, int padLength = 4)
        {
            string datePart = DateTime.Now.ToString("yyMMdd");
            string pattern = prefix + datePart + "%";

            string sql = $@"
                SELECT TOP 1 {idColumnName} 
                FROM {tableName} WITH (UPDLOCK, HOLDLOCK) 
                WHERE {idColumnName} LIKE @Pattern 
                ORDER BY {idColumnName} DESC";

            using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("@Pattern", pattern);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string lastId = result.ToString();
                    // Cắt lấy phần số ở đuôi (Prefix length + 6 ký tự ngày tháng)
                    string numPart = lastId.Substring(prefix.Length + 6);
                    if (int.TryParse(numPart, out int lastNum))
                    {
                        return prefix + datePart + (lastNum + 1).ToString().PadLeft(padLength, '0');
                    }
                }
                // Nếu ngày hôm nay chưa có mã nào thì bắt đầu từ số 1
                return prefix + datePart + 1.ToString().PadLeft(padLength, '0');
            }
        }
    }
}