using System;
using System.Data.SqlClient;
using System.Configuration;

namespace QL_KhamChuaBenhNgoaiTru.DBContext
{
    public class BackupDB
    {
        private string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        // Extract Database Name from Connection String
        public string GetDatabaseName()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectStr);
            return builder.InitialCatalog;
        }

        public bool CreateBackup(string backupFilePath, bool isDifferential = false)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectStr))
                {
                    string dbName = GetDatabaseName();
                    string withClause = isDifferential 
                        ? "DIFFERENTIAL, INIT, NAME = N'Differential Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10" 
                        : "FORMAT, INIT, NAME = N'Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10";
                        
                    string sql = $@"BACKUP DATABASE [{dbName}] TO DISK = '{backupFilePath}' WITH {withClause}";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.CommandTimeout = 0; // Backup can take a while
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sao lưu dữ liệu: " + ex.Message);
            }
        }

        public bool RestoreDatabase(string targetFilePath, string fullBackupPath = null)
        {
            try
            {
                // Connect to master database to drop active connections to our target database
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectStr);
                string targetDbName = builder.InitialCatalog;
                builder.InitialCatalog = "master"; // Switch to master

                using (SqlConnection con = new SqlConnection(builder.ConnectionString))
                {
                    con.Open();

                    // Step 1: Set database to SINGLE_USER to drop other connections
                    string sqlSingleUser = $"ALTER DATABASE [{targetDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    using (SqlCommand cmd = new SqlCommand(sqlSingleUser, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Step 2: Restore Data
                    if (!string.IsNullOrEmpty(fullBackupPath))
                    {
                        // Khôi phục bản Full trước định dạng nối tiếp NORECOVERY
                        string sqlRestoreFull = $"RESTORE DATABASE [{targetDbName}] FROM DISK = '{fullBackupPath}' WITH NORECOVERY, REPLACE";
                        using (SqlCommand cmd = new SqlCommand(sqlRestoreFull, con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }

                        // Sau đó đổ bản Differential lên với cờ RECOVERY
                        string sqlRestoreDiff = $"RESTORE DATABASE [{targetDbName}] FROM DISK = '{targetFilePath}' WITH RECOVERY";
                        using (SqlCommand cmd = new SqlCommand(sqlRestoreDiff, con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Khôi phục bản Full độc lập thông thường
                        string sqlRestore = $"RESTORE DATABASE [{targetDbName}] FROM DISK = '{targetFilePath}' WITH REPLACE";
                        using (SqlCommand cmd = new SqlCommand(sqlRestore, con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Step 3: Set database back to MULTI_USER
                    string sqlMultiUser = $"ALTER DATABASE [{targetDbName}] SET MULTI_USER";
                    using (SqlCommand cmd = new SqlCommand(sqlMultiUser, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi phục hồi dữ liệu: " + ex.Message);
            }
        }
    }
}
