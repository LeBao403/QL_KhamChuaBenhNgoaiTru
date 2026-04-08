using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using QL_KhamChuaBenhNgoaiTru.DBContext;
using QL_KhamChuaBenhNgoaiTru.Services;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class BackupFileInfo
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public DateTime CreationTime { get; set; }
        public string FullPath { get; set; }
        public bool IsDifferential { get; set; }
    }

    public class BackupController : BaseAdminController
    {
        private BackupDB db = new BackupDB();

        // GET: Admin/Backup
        public ActionResult Index()
        {
            string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            var files = Directory.GetFiles(backupFolder, "*.bak")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Select(f => new BackupFileInfo
                {
                    FileName = f.Name,
                    FileSize = (f.Length / 1024f / 1024f).ToString("0.00") + " MB",
                    CreationTime = f.CreationTime,
                    FullPath = f.FullName,
                    IsDifferential = f.Name.Contains("_Diff_")
                }).ToList();

            ViewBag.BackupFiles = files;
            ViewBag.Config = BackupScheduler.GetConfig();
            
            return View();
        }

        [HttpPost]
        public JsonResult CreateBackup(bool isDifferential)
        {
            try
            {
                string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string dbName = db.GetDatabaseName();
                string typeStr = isDifferential ? "Diff" : "Full";
                string fileName = $"{dbName}_{typeStr}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string fullPath = Path.Combine(backupFolder, fileName);

                bool res = db.CreateBackup(fullPath, isDifferential);
                if (res)
                {
                    return Json(new { success = true, message = $"Đã tạo bản sao lưu {(isDifferential ? "Thay đổi (Diff)" : "Toàn bộ (Full)")} thành công!" });
                }
                return Json(new { success = false, message = "Lỗi khi sao lưu!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RestoreBackup(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return Json(new { success = false, message = "Không tìm thấy file!" });

                string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
                string fullPath = Path.Combine(backupFolder, fileName);

                if (!System.IO.File.Exists(fullPath))
                    return Json(new { success = false, message = "File backup không tồn tại trên hệ thống!" });

                bool isDiff = fileName.Contains("_Diff_");
                string fullBackupPath = null;

                if (isDiff)
                {
                    // Lấy tất cả các File Full Backup trước thời điểm của file Diff này
                    var diffFileInfo = new FileInfo(fullPath);
                    var previousFullFiles = Directory.GetFiles(backupFolder, "*.bak")
                        .Select(f => new FileInfo(f))
                        .Where(f => !f.Name.Contains("_Diff_") && f.CreationTime <= diffFileInfo.CreationTime)
                        .OrderByDescending(f => f.CreationTime)
                        .FirstOrDefault();

                    if (previousFullFiles == null)
                    {
                        return Json(new { success = false, message = "KHÔNG TÌM THẤY BẢN FULL BACKUP GỐC! Bản Differential này không thể khôi phục độc lập vì kiến trúc đã bị mất file gốc." });
                    }
                    
                    fullBackupPath = previousFullFiles.FullName;
                }

                bool res = db.RestoreDatabase(fullPath, fullBackupPath);
                if (res)
                {
                    return Json(new { success = true, message = "Đã khôi phục dữ liệu hệ thống chuẩn xác!" });
                }
                return Json(new { success = false, message = "Lỗi khi khôi phục!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteBackup(string fileName)
        {
            try
            {
                string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup");
                string fullPath = Path.Combine(backupFolder, fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "File không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveSchedule(bool isFullAuto, string backupFullTime, bool isDiffAuto, string backupDiffTime)
        {
            try
            {
                var config = BackupScheduler.GetConfig();
                
                config.FullBackup.IsEnabled = isFullAuto;
                config.FullBackup.BackupTime = backupFullTime;
                
                config.DiffBackup.IsEnabled = isDiffAuto;
                config.DiffBackup.BackupTime = backupDiffTime;
                
                BackupScheduler.SaveConfig(config);
                return Json(new { success = true, message = "Đã cập nhật hệ thống lịch trình sao lưu phức hợp!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
