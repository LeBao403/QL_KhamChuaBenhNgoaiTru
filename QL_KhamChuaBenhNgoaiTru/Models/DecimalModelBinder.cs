using System;
using System.Web.Mvc;
using System.Globalization;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult != null && !string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            {
                string value = valueResult.AttemptedValue.Trim();

                // Lấy tên của biến đang được gửi lên (Ví dụ: "Sang", "DonGia", "TongTien"...)
                string propName = bindingContext.ModelMetadata.PropertyName;

                // KIỂM TRA ĐÍCH DANH: Chỉ đổi dấu chấm -> phẩy nếu đó là các biến Liều Lượng Thuốc
                if (propName == "Sang" || propName == "Trua" ||
                    propName == "Chieu" || propName == "Toi" ||
                    propName == "SoLuongSang" || propName == "SoLuongTrua" ||
                    propName == "SoLuongChieu" || propName == "SoLuongToi")
                {
                    value = value.Replace(".", ",");
                }

                // Tiến hành ép kiểu với chuẩn vi-VN gốc (Các biến tiền tệ có dấu chấm sẽ vẫn được hiểu là phân cách hàng nghìn)
                if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("vi-VN"), out decimal result))
                {
                    return result;
                }
            }

            // Trả về mặc định nếu lỗi hoặc rỗng
            return bindingContext.ModelType == typeof(decimal?) ? (decimal?)null : 0m;
        }
    }
}