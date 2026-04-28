using QL_KhamChuaBenhNgoaiTru.DBContext;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Controllers
{
    public class MockLISController : Controller
    {
        private readonly CLSDB db = new CLSDB();

        [HttpPost]
        public ActionResult NhanKetQuaTuMay(string maKetQua)
        {
            try
            {
                var thongTin = db.GetThongTinChiTietCLS(maKetQua);
                if (thongTin == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy yêu cầu cận lâm sàng." });
                }

                if (string.Equals(Convert.ToString(thongTin.TrangThai), "Đã có kết quả", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Dịch vụ này đã có kết quả." });
                }

                string tenDichVu = Convert.ToString(thongTin.TenDichVu ?? thongTin.TenDV ?? string.Empty);
                DemoResult result = TaoKetQuaMoPhong(tenDichVu);

                return Json(new
                {
                    success = true,
                    message = "Máy CLS đã trả kết quả mô phỏng thành công.",
                    data = result.Data,
                    ketluan = result.KetLuan,
                    images = result.Images,
                    sampleOptions = result.SampleOptions,
                    sampleDefault = result.SampleDefault,
                    chatLuong = result.ChatLuong,
                    actionLabel = result.ActionLabel,
                    resultType = result.ResultType,
                    hasMetaColumns = result.Data.Any(x =>
                        !string.IsNullOrWhiteSpace(x.DonVi) ||
                        !string.IsNullOrWhiteSpace(x.ThamChieu))
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi mô phỏng kết quả CLS: " + ex.Message });
            }
        }

        private DemoResult TaoKetQuaMoPhong(string tenDichVu)
        {
            string normalized = NormalizeText(tenDichVu);

            if (normalized.Contains("tong phan tich te bao mau"))
            {
                double wbc = RandomRange(4.6, 9.8, 1);
                bool abnormal = RandomChance(0.35);
                if (abnormal)
                {
                    wbc = RandomRange(10.8, 13.6, 1);
                }

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Máu toàn phần" },
                    SampleDefault = "Máu toàn phần",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả huyết học",
                    ResultType = "lab",
                    KetLuan = abnormal
                        ? "Tăng nhẹ bạch cầu, gợi ý theo dõi thêm tình trạng viêm."
                        : "Công thức máu trong giới hạn tham chiếu.",
                    Data = new List<DemoRow>
                    {
                        Row("WBC", wbc.ToString("F1"), "10^9/L", "4.0 - 10.0", abnormal),
                        Row("RBC", RandomRange(4.2, 5.5, 2).ToString("F2"), "10^12/L", "4.0 - 5.8"),
                        Row("HGB", RandomRange(122, 156, 0).ToString("F0"), "g/L", "120 - 160"),
                        Row("HCT", RandomRange(37, 46, 1).ToString("F1"), "%", "37 - 47"),
                        Row("PLT", RandomRange(180, 320, 0).ToString("F0"), "10^9/L", "150 - 400")
                    }
                };
            }

            if (normalized.Contains("hba1c"))
            {
                double hba1c = RandomRange(5.1, 6.8, 1);
                bool warning = hba1c >= 6.5;

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Máu toàn phần" },
                    SampleDefault = "Máu toàn phần",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả HbA1c",
                    ResultType = "lab",
                    KetLuan = warning
                        ? "Chỉ số HbA1c tăng, cần đối chiếu với đường huyết và lâm sàng."
                        : "Chỉ số HbA1c chưa ghi nhận bất thường.",
                    Data = new List<DemoRow>
                    {
                        Row("HbA1c", hba1c.ToString("F1"), "%", "4.0 - 6.0", warning),
                        Row("Đường huyết ước tính", RandomRange(5.0, 7.3, 2).ToString("F2"), "mmol/L", "4.0 - 7.0", warning)
                    }
                };
            }

            if (normalized.Contains("glucose"))
            {
                double glucose = RandomRange(4.3, 7.8, 2);
                bool warning = glucose > 6.4;

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Huyết tương", "Huyết thanh" },
                    SampleDefault = "Huyết tương",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả sinh hóa",
                    ResultType = "lab",
                    KetLuan = warning
                        ? "Đường huyết tăng nhẹ, đề nghị đối chiếu bối cảnh ăn uống và tiền sử."
                        : "Đường huyết nằm trong giới hạn tham chiếu.",
                    Data = new List<DemoRow>
                    {
                        Row("Glucose máu", glucose.ToString("F2"), "mmol/L", "3.9 - 6.4", warning)
                    }
                };
            }

            if (normalized.Contains("cholesterol"))
            {
                double cholesterol = RandomRange(4.2, 6.4, 2);
                bool warning = cholesterol > 5.2;

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Huyết thanh", "Huyết tương" },
                    SampleDefault = "Huyết thanh",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả mỡ máu",
                    ResultType = "lab",
                    KetLuan = warning
                        ? "Mỡ máu tăng nhẹ, cần tư vấn chế độ ăn và tái khám."
                        : "Bộ chỉ số mỡ máu trong giới hạn tham chiếu.",
                    Data = new List<DemoRow>
                    {
                        Row("Cholesterol toàn phần", cholesterol.ToString("F2"), "mmol/L", "< 5.2", warning),
                        Row("Triglyceride", RandomRange(0.8, 1.9, 2).ToString("F2"), "mmol/L", "< 1.7"),
                        Row("HDL-C", RandomRange(1.0, 1.6, 2).ToString("F2"), "mmol/L", "> 1.0"),
                        Row("LDL-C", RandomRange(2.1, 3.8, 2).ToString("F2"), "mmol/L", "< 3.4", warning)
                    }
                };
            }

            if (normalized.Contains("chuc nang gan") || normalized.Contains("ast/alt") || normalized.Contains("ast"))
            {
                double ast = RandomRange(20, 42, 0);
                double alt = RandomRange(18, 45, 0);
                bool warning = ast > 37 || alt > 40;

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Huyết thanh", "Huyết tương" },
                    SampleDefault = "Huyết thanh",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả chức năng gan",
                    ResultType = "lab",
                    KetLuan = warning
                        ? "Men gan tăng nhẹ, cần theo dõi thêm chức năng gan."
                        : "Chưa ghi nhận bất thường men gan.",
                    Data = new List<DemoRow>
                    {
                        Row("AST (GOT)", ast.ToString("F0"), "U/L", "< 37", ast > 37),
                        Row("ALT (GPT)", alt.ToString("F0"), "U/L", "< 40", alt > 40),
                        Row("GGT", RandomRange(15, 58, 0).ToString("F0"), "U/L", "< 60")
                    }
                };
            }

            if (normalized.Contains("creatinin"))
            {
                double creatinine = RandomRange(60, 116, 0);
                bool warning = creatinine > 106;

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Huyết thanh", "Huyết tương" },
                    SampleDefault = "Huyết thanh",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả chức năng thận",
                    ResultType = "lab",
                    KetLuan = warning
                        ? "Creatinin tăng nhẹ, đề nghị đối chiếu chức năng thận."
                        : "Chức năng thận tạm trong giới hạn chấp nhận.",
                    Data = new List<DemoRow>
                    {
                        Row("Creatinin", creatinine.ToString("F0"), "µmol/L", "53 - 106", warning),
                        Row("Ure", RandomRange(3.1, 7.8, 2).ToString("F2"), "mmol/L", "2.5 - 7.5"),
                        Row("eGFR ước tính", RandomRange(72, 104, 0).ToString("F0"), "mL/phút/1.73m²", "> 60")
                    }
                };
            }

            if (normalized.Contains("nuoc tieu"))
            {
                bool hasRbc = RandomChance(0.2);

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Nước tiểu" },
                    SampleDefault = "Nước tiểu",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả nước tiểu",
                    ResultType = "lab",
                    KetLuan = hasRbc
                        ? "Tổng phân tích nước tiểu có hồng cầu vi thể, cần đối chiếu lâm sàng."
                        : "Tổng phân tích nước tiểu chưa ghi nhận bất thường rõ ràng.",
                    Data = new List<DemoRow>
                    {
                        Row("pH", RandomRange(5.5, 6.8, 1).ToString("F1"), null, "4.8 - 7.4"),
                        Row("Tỷ trọng", RandomRange(1.010, 1.025, 3).ToString("F3"), null, "1.005 - 1.030"),
                        Row("Protein", "Âm tính", null, "Âm tính"),
                        Row("Bạch cầu (LEU)", "Âm tính", "cells/µL", "Âm tính"),
                        Row("Hồng cầu (ERY)", hasRbc ? "1+" : "Âm tính", "cells/µL", "Âm tính", hasRbc)
                    }
                };
            }

            if (normalized.Contains("viem gan b"))
            {
                bool positive = RandomChance(0.18);

                return new DemoResult
                {
                    SampleOptions = new List<string> { "Huyết thanh" },
                    SampleDefault = "Huyết thanh",
                    ChatLuong = "Đạt",
                    ActionLabel = "Lấy kết quả test nhanh",
                    ResultType = "lab",
                    KetLuan = positive
                        ? "Test nhanh HBsAg dương tính, đề nghị làm xét nghiệm khẳng định."
                        : "Test nhanh HBsAg âm tính.",
                    Data = new List<DemoRow>
                    {
                        Row("HBsAg", positive ? "Dương tính" : "Âm tính", "S/CO", "Âm tính", positive)
                    }
                };
            }

            if (normalized.Contains("sieu am o bung"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả siêu âm",
                    ResultType = "image",
                    KetLuan = "Siêu âm ổ bụng tổng quát chưa ghi nhận bất thường đáng kể.",
                    Images = new List<DemoImage>
                    {
                        Image("Siêu âm ổ bụng tổng quát", "sieu_am_bung_tong_quat.png", "Ảnh minh họa gan, mật, tụy, thận trong ca demo.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Gan", "Kích thước không to, nhu mô đồng đều"),
                        Row("Túi mật", "Thành mỏng, không sỏi"),
                        Row("Tụy", "Không thấy khối bất thường"),
                        Row("Thận", "Không ứ nước, không sỏi")
                    }
                };
            }

            if (normalized.Contains("sieu am tuyen giap"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả siêu âm",
                    ResultType = "image",
                    KetLuan = "Cấu trúc tuyến giáp đều, chưa thấy nhân giáp nghi ngờ.",
                    Images = new List<DemoImage>
                    {
                        Image("Siêu âm tuyến giáp", "sieu_am_tuyen_giap.png", "Ảnh demo mặt cắt dọc và ngang tuyến giáp.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Thùy phải", "12 x 14 x 42 mm"),
                        Row("Thùy trái", "11 x 13 x 41 mm"),
                        Row("Eo tuyến", "3 mm"),
                        Row("Nhân giáp", "Chưa thấy nhân đặc hiệu")
                    }
                };
            }

            if (normalized.Contains("doppler tim"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả siêu âm tim",
                    ResultType = "image",
                    KetLuan = "Chức năng tâm thu thất trái bảo tồn, chưa ghi nhận bất thường van tim rõ.",
                    Images = new List<DemoImage>
                    {
                        Image("Siêu âm Doppler tim", "sieu_am_doppler_tim.png", "Ảnh demo mặt cắt 4 buồng tim và Doppler màu.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("EF", RandomRange(58, 68, 0).ToString("F0") + "%"),
                        Row("Van hai lá", "Đóng mở tốt, không hở nghịch dòng đáng kể"),
                        Row("Van động mạch chủ", "Không hẹp, không hở"),
                        Row("Dịch màng ngoài tim", "Không có")
                    }
                };
            }

            if (normalized.Contains("x-quang nguc") || normalized.Contains("x quang nguc"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả X-quang",
                    ResultType = "image",
                    KetLuan = "X-quang ngực chưa thấy tổn thương tim phổi cấp tính.",
                    Images = new List<DemoImage>
                    {
                        Image("X-quang ngực thẳng", "X_quang_nguc_thang.png", "Ảnh phim ngực thẳng kỹ thuật số trong bộ dữ liệu demo.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Tim", "Không to"),
                        Row("Nhu mô phổi", "Không thâm nhiễm khu trú"),
                        Row("Màng phổi", "Không tràn dịch"),
                        Row("Xương lồng ngực", "Không thấy tổn thương rõ")
                    }
                };
            }

            if (normalized.Contains("x-quang cot song") || normalized.Contains("x quang cot song"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả X-quang",
                    ResultType = "image",
                    KetLuan = "Hình ảnh thoái hóa cột sống thắt lưng mức độ nhẹ.",
                    Images = new List<DemoImage>
                    {
                        Image("X-quang cột sống", "X-quang_cot_song.png", "Ảnh phim cột sống đứng nghiêng trong ca demo.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Đường cong sinh lý", "Bảo tồn"),
                        Row("Khe đĩa đệm", "Hẹp nhẹ L4-L5"),
                        Row("Gai xương", "Có gai xương nhỏ bờ thân đốt sống"),
                        Row("Phần mềm quanh cột sống", "Không bất thường")
                    }
                };
            }

            if (normalized.Contains("dien tam do") || normalized.Contains("ecg"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả điện tâm đồ",
                    ResultType = "image",
                    KetLuan = "Điện tâm đồ nhịp xoang, chưa thấy rối loạn dẫn truyền rõ.",
                    Images = new List<DemoImage>
                    {
                        Image("Điện tâm đồ 12 chuyển đạo", "noi_soi_dien_tam_do.png", "Ảnh demo bản ECG 12 chuyển đạo.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Nhịp", "Xoang đều"),
                        Row("Tần số tim", RandomRange(68, 92, 0).ToString("F0") + " lần/phút"),
                        Row("PR", RandomRange(150, 180, 0).ToString("F0") + " ms"),
                        Row("QRS", RandomRange(82, 102, 0).ToString("F0") + " ms"),
                        Row("QTc", RandomRange(390, 430, 0).ToString("F0") + " ms")
                    }
                };
            }

            if (normalized.Contains("noi soi tai mui hong"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả nội soi",
                    ResultType = "image",
                    KetLuan = "Nội soi Tai Mũi Họng ghi nhận viêm niêm mạc mức độ nhẹ.",
                    Images = new List<DemoImage>
                    {
                        Image("Nội soi Tai Mũi Họng", "noi_soi_tai_mui_hong.png", "Ảnh demo nội soi vùng mũi họng.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Hốc mũi", "Thông, niêm mạc sung nhẹ"),
                        Row("Vách ngăn", "Lệch nhẹ sang trái"),
                        Row("VA", "Không quá phát"),
                        Row("Thành sau họng", "Không xuất tiết bất thường")
                    }
                };
            }

            if (normalized.Contains("noi soi da day"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả nội soi",
                    ResultType = "image",
                    KetLuan = "Nội soi dạ dày ghi nhận viêm sung huyết niêm mạc hang vị mức độ nhẹ.",
                    Images = new List<DemoImage>
                    {
                        Image("Nội soi dạ dày", "noi_soi_da_day.png", "Ảnh demo niêm mạc dạ dày trong ca nội soi.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Thực quản", "Niêm mạc hồng, không loét"),
                        Row("Thân vị", "Niêm mạc sung huyết nhẹ"),
                        Row("Hang vị", "Không thấy ổ loét sâu"),
                        Row("Tá tràng", "Hành tá tràng sạch, không biến dạng")
                    }
                };
            }

            if (normalized.Contains("sieu am"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả siêu âm",
                    ResultType = "image",
                    KetLuan = "Kết quả siêu âm demo chưa ghi nhận bất thường nổi bật.",
                    Images = new List<DemoImage>
                    {
                        Image("Siêu âm tổng quát", "sieu_am_bung_tong_quat.png", "Ảnh demo placeholder cho dịch vụ siêu âm.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Cấu trúc mô", "Đồng đều"),
                        Row("Khối bất thường", "Chưa ghi nhận"),
                        Row("Dịch", "Không thấy")
                    }
                };
            }

            if (normalized.Contains("x-quang") || normalized.Contains("x quang"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả X-quang",
                    ResultType = "image",
                    KetLuan = "Kết quả X-quang demo chưa ghi nhận tổn thương bất thường rõ.",
                    Images = new List<DemoImage>
                    {
                        Image("X-quang demo", "X_quang_nguc_thang.png", "Ảnh demo placeholder cho dịch vụ X-quang.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Tư thế chụp", "Đạt yêu cầu kỹ thuật"),
                        Row("Tổn thương xương", "Không thấy rõ"),
                        Row("Phần mềm", "Không bất thường")
                    }
                };
            }

            if (normalized.Contains("noi soi"))
            {
                return new DemoResult
                {
                    SampleOptions = new List<string> { "Không áp dụng" },
                    SampleDefault = "Không áp dụng",
                    ChatLuong = "Không áp dụng",
                    ActionLabel = "Lấy kết quả nội soi",
                    ResultType = "image",
                    KetLuan = "Nội soi demo chưa ghi nhận tổn thương bất thường nổi bật.",
                    Images = new List<DemoImage>
                    {
                        Image("Nội soi demo", "noi_soi_da_day.png", "Ảnh demo placeholder cho dịch vụ nội soi.")
                    },
                    Data = new List<DemoRow>
                    {
                        Row("Niêm mạc", "Hồng, ẩm, không loét"),
                        Row("Xuất tiết", "Không đáng kể"),
                        Row("Tổn thương khu trú", "Chưa thấy")
                    }
                };
            }

            return new DemoResult
            {
                SampleOptions = new List<string> { "Huyết thanh", "Huyết tương" },
                SampleDefault = "Huyết thanh",
                ChatLuong = "Đạt",
                ActionLabel = "Lấy kết quả CLS",
                ResultType = "lab",
                KetLuan = "Kết quả demo đã được sinh theo cấu hình mặc định của dịch vụ CLS.",
                Data = new List<DemoRow>
                {
                    Row("Glucose", RandomRange(4.2, 6.2, 2).ToString("F2"), "mmol/L", "3.9 - 6.4"),
                    Row("Ure", RandomRange(3.1, 6.8, 2).ToString("F2"), "mmol/L", "2.5 - 7.5"),
                    Row("Creatinin", RandomRange(64, 102, 0).ToString("F0"), "µmol/L", "53 - 106")
                }
            };
        }

        private DemoRow Row(string ten, string giaTri, string donVi = null, string thamChieu = null, bool warning = false)
        {
            return new DemoRow
            {
                Ten = ten,
                GiaTri = giaTri,
                DonVi = donVi,
                ThamChieu = thamChieu,
                Warning = warning
            };
        }

        private DemoImage Image(string title, string fileName, string caption)
        {
            return new DemoImage
            {
                Title = title,
                Url = Url.Content("~/Images/cls/" + fileName),
                Caption = caption
            };
        }

        private double RandomRange(double min, double max, int decimals)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            double value = (rnd.NextDouble() * (max - min)) + min;
            return Math.Round(value, decimals);
        }

        private bool RandomChance(double probability)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.NextDouble() < probability;
        }

        private string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c == 'đ' ? 'd' : c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private class DemoResult
        {
            public List<DemoRow> Data { get; set; } = new List<DemoRow>();
            public string KetLuan { get; set; }
            public List<DemoImage> Images { get; set; } = new List<DemoImage>();
            public List<string> SampleOptions { get; set; } = new List<string>();
            public string SampleDefault { get; set; }
            public string ChatLuong { get; set; }
            public string ActionLabel { get; set; }
            public string ResultType { get; set; }
        }

        private class DemoRow
        {
            public string Ten { get; set; }
            public string GiaTri { get; set; }
            public string DonVi { get; set; }
            public string ThamChieu { get; set; }
            public bool Warning { get; set; }
        }

        private class DemoImage
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Caption { get; set; }
        }
    }
}
