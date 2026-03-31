using System;
using System.IO;
using System.Timers;
using System.Web;
using Newtonsoft.Json;
using QL_KhamChuaBenhNgoaiTru.DBContext;

namespace QL_KhamChuaBenhNgoaiTru.Services
{
    public class AutoScheduleInfo
    {
        public bool IsEnabled { get; set; } = false;
        public string BackupTime { get; set; } = "00:00"; 
        public DateTime? LastBackupDate { get; set; } = null;
    }

    public class BackupConfig
    {
        public AutoScheduleInfo FullBackup { get; set; } = new AutoScheduleInfo { BackupTime = "00:00" };
        public AutoScheduleInfo DiffBackup { get; set; } = new AutoScheduleInfo { BackupTime = "12:00" };
    }

    public static class BackupScheduler
    {
        private static Timer _timer;
        private static string _backupFolderPath;
        private static string _configFilePath;

        public static void Start()
        {
            try
            {
                _backupFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
                if (!Directory.Exists(_backupFolderPath))
                {
                    Directory.CreateDirectory(_backupFolderPath);
                }

                _configFilePath = Path.Combine(_backupFolderPath, "backup_config.json");

                // Check every minute
                _timer = new Timer(60000); 
                _timer.Elapsed += ExecuteTask;
                _timer.AutoReset = true;
                _timer.Start();
            }
            catch (Exception)
            {
            }
        }

        private static void ExecuteTask(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!File.Exists(_configFilePath)) return;

                var json = File.ReadAllText(_configFilePath);
                var config = JsonConvert.DeserializeObject<BackupConfig>(json);
                if (config == null) return;

                DateTime now = DateTime.Now;
                bool configChanged = false;

                // 1. Kiểm tra Full Backup
                if (config.FullBackup != null && config.FullBackup.IsEnabled)
                {
                    TimeSpan fTime;
                    if (TimeSpan.TryParse(config.FullBackup.BackupTime, out fTime))
                    {
                        if (now.TimeOfDay >= fTime)
                        {
                            if (!config.FullBackup.LastBackupDate.HasValue || config.FullBackup.LastBackupDate.Value.Date < now.Date)
                            {
                                PerformBackup(false);
                                config.FullBackup.LastBackupDate = now;
                                configChanged = true;
                            }
                        }
                    }
                }

                // 2. Kiểm tra Diff Backup
                if (config.DiffBackup != null && config.DiffBackup.IsEnabled)
                {
                    TimeSpan dTime;
                    if (TimeSpan.TryParse(config.DiffBackup.BackupTime, out dTime))
                    {
                        if (now.TimeOfDay >= dTime)
                        {
                            if (!config.DiffBackup.LastBackupDate.HasValue || config.DiffBackup.LastBackupDate.Value.Date < now.Date)
                            {
                                PerformBackup(true);
                                config.DiffBackup.LastBackupDate = now;
                                configChanged = true;
                            }
                        }
                    }
                }

                if (configChanged)
                {
                    File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(config));
                }
            }
            catch (Exception) { }
        }

        private static void PerformBackup(bool isDifferential)
        {
            try
            {
                BackupDB db = new BackupDB();
                string dbName = db.GetDatabaseName();
                string typeStr = isDifferential ? "Diff" : "Full";
                string fileName = $"{dbName}_{typeStr}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string fullPath = Path.Combine(_backupFolderPath, fileName);
                
                db.CreateBackup(fullPath, isDifferential);
            }
            catch (Exception) { }
        }

        public static BackupConfig GetConfig()
        {
            try
            {
                _backupFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
                _configFilePath = Path.Combine(_backupFolderPath, "backup_config.json");
                
                if (File.Exists(_configFilePath))
                {
                    var json = File.ReadAllText(_configFilePath);
                    return JsonConvert.DeserializeObject<BackupConfig>(json) ?? new BackupConfig();
                }
            }
            catch (Exception) { }
            return new BackupConfig();
        }

        public static void SaveConfig(BackupConfig config)
        {
            _backupFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
            if (!Directory.Exists(_backupFolderPath)) Directory.CreateDirectory(_backupFolderPath);
            
            _configFilePath = Path.Combine(_backupFolderPath, "backup_config.json");
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(config));
        }
    }
}
