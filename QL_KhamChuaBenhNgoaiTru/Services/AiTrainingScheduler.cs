using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Timers;
using System.Web;
using Newtonsoft.Json;

namespace QL_KhamChuaBenhNgoaiTru.Services
{
    public class AiManagementConfig
    {
        public bool IsAutoTrainEnabled { get; set; } = false;
        public string TrainTime { get; set; } = "01:00";
        public DateTime? LastTrainDate { get; set; } = null;
        public bool AutoActivateAfterTrain { get; set; } = true;
        public string LastError { get; set; }
        public DateTime? LastErrorAt { get; set; } = null;
    }

    public static class AiTrainingScheduler
    {
        private static Timer _timer;
        private static readonly object LockObj = new object();
        private static bool _isTraining;

        public static void Start()
        {
            try
            {
                EnsureConfigFolder();
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
            lock (LockObj)
            {
                if (_isTraining) return;
                _isTraining = true;
            }

            try
            {
                var config = GetConfig();
                if (!config.IsAutoTrainEnabled) return;

                TimeSpan trainTime;
                if (!TimeSpan.TryParse(config.TrainTime, out trainTime)) return;

                var now = DateTime.Now;
                if (now.TimeOfDay < trainTime) return;
                if (config.LastTrainDate.HasValue && config.LastTrainDate.Value.Date >= now.Date) return;

                CallAiTrain(config.AutoActivateAfterTrain);
                config.LastTrainDate = now;
                config.LastError = null;
                config.LastErrorAt = null;
                SaveConfig(config);
            }
            catch (Exception ex)
            {
                var config = GetConfig();
                config.LastError = ex.Message;
                config.LastErrorAt = DateTime.Now;
                SaveConfig(config);
            }
            finally
            {
                lock (LockObj)
                {
                    _isTraining = false;
                }
            }
        }

        private static void CallAiTrain(bool autoActivate)
        {
            var url = GetAiServiceUrl().TrimEnd('/') + "/api/train?auto_activate=" + autoActivate.ToString().ToLowerInvariant();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentLength = 0;
            request.Timeout = 600000;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("AI service tra ve trang thai " + response.StatusCode);
                }
            }
        }

        public static AiManagementConfig GetConfig()
        {
            try
            {
                var path = GetConfigPath();
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<AiManagementConfig>(json) ?? new AiManagementConfig();
                }
            }
            catch (Exception)
            {
            }

            return new AiManagementConfig();
        }

        public static void SaveConfig(AiManagementConfig config)
        {
            EnsureConfigFolder();
            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static string GetAiServiceUrl()
        {
            return ConfigurationManager.AppSettings["AiServiceUrl"] ?? "http://127.0.0.1:8000";
        }

        private static string GetConfigPath()
        {
            return Path.Combine(GetAppDataPath(), "ai_management_config.json");
        }

        private static string GetAppDataPath()
        {
            var baseDir = HttpContext.Current != null
                ? HttpContext.Current.Server.MapPath("~/App_Data")
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            return baseDir;
        }

        private static void EnsureConfigFolder()
        {
            var appData = GetAppDataPath();
            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }
        }
    }
}
