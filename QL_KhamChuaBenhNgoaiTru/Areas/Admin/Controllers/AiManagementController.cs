using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QL_KhamChuaBenhNgoaiTru.Services;

namespace QL_KhamChuaBenhNgoaiTru.Areas.Admin.Controllers
{
    public class AiManagementController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Quản lý AI";
            ViewBag.Config = AiTrainingScheduler.GetConfig();
            ViewBag.AiServiceUrl = AiTrainingScheduler.GetAiServiceUrl();
            return View();
        }

        [HttpGet]
        public ActionResult Overview()
        {
            try
            {
                var status = JObject.Parse(CallAiApi("/api/status", "GET"));
                var models = JObject.Parse(CallAiApi("/api/models", "GET"));
                return JsonNet(new
                {
                    success = true,
                    status,
                    models,
                    config = AiTrainingScheduler.GetConfig()
                });
            }
            catch (Exception ex)
            {
                return JsonNet(new
                {
                    success = false,
                    message = "Không kết nối được AI service: " + ex.Message,
                    config = AiTrainingScheduler.GetConfig()
                });
            }
        }

        [HttpPost]
        public ActionResult TrainNow()
        {
            try
            {
                var result = JObject.Parse(CallAiApi("/api/train", "POST", timeoutMs: 600000));
                return JsonNet(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SelectModel(string modelId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(modelId))
                {
                    return JsonNet(new { success = false, message = "Chưa chọn model." });
                }

                var body = JsonConvert.SerializeObject(new { model_id = modelId });
                var result = JObject.Parse(CallAiApi("/api/models/select", "POST", body));
                return JsonNet(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveSchedule(bool isAutoTrainEnabled, string trainTime, string trainCycle, bool autoActivateAfterTrain)
        {
            try
            {
                var config = AiTrainingScheduler.GetConfig();
                var normalizedTime = string.IsNullOrWhiteSpace(trainTime) ? "01:00" : trainTime;
                var normalizedCycle = NormalizeCycle(trainCycle);
                var scheduleChanged = config.TrainTime != normalizedTime || config.ScheduleCycle != normalizedCycle;

                config.IsAutoTrainEnabled = isAutoTrainEnabled;
                config.TrainTime = normalizedTime;
                config.ScheduleCycle = normalizedCycle;
                if (scheduleChanged) config.LastTrainDate = null;
                config.AutoActivateAfterTrain = autoActivateAfterTrain;
                AiTrainingScheduler.SaveConfig(config);

                return Json(new { success = true, message = "Đã cập nhật cấu hình train AI tự động." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static string NormalizeCycle(string cycle)
        {
            cycle = (cycle ?? "").Trim();
            if (string.Equals(cycle, "Weekly", StringComparison.OrdinalIgnoreCase)) return "Weekly";
            if (string.Equals(cycle, "Monthly", StringComparison.OrdinalIgnoreCase)) return "Monthly";
            return "Daily";
        }

        private static string CallAiApi(string path, string method, string jsonBody = null, int timeoutMs = 10000)
        {
            var url = AiTrainingScheduler.GetAiServiceUrl().TrimEnd('/') + path;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Accept = "application/json";
            request.Timeout = timeoutMs;

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.ContentType = "application/json";
                var bytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            else if (method == "POST")
            {
                request.ContentLength = 0;
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var responseText = reader.ReadToEnd();
                        throw new InvalidOperationException(responseText);
                    }
                }

                throw;
            }
        }

        private ContentResult JsonNet(object value)
        {
            return Content(JsonConvert.SerializeObject(value), "application/json");
        }
    }
}
