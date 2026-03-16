using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class KhachHang
    {
        public string MaKH { get; set; }

        [Required(ErrorMessage = "Họ tên khách hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được dài quá 100 ký tự.")]
        public string HoTen { get; set; }

        [StringLength(20)]
        public string CCCD { get; set; }

        [Phone(ErrorMessage = "SĐT không hợp lệ.")]
        public string SDT { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string DiaChi { get; set; }
        public bool BHYT { get; set; }
        public int? MaTK { get; set; }
        public string MaNGH { get; set; }
    }


    public class DanhMucTienSuYTe
    {
        public string MaTSYT { get; set; }
        public string TenTSYT { get; set; }
    }

    public class TienSuYTeBenhNhan
    {
        public string MaTSYT { get; set; }
        public string MaKH { get; set; }
    }

    public class NguoiGiamHo : IValidatableObject
    {
        public string MaNGH { get; set; }

        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string MoiQuanHe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Nếu người giám hộ có nhập ít nhất 1 trường
            if (!string.IsNullOrWhiteSpace(HoTen) || !string.IsNullOrWhiteSpace(SDT) ||
                !string.IsNullOrWhiteSpace(DiaChi) || !string.IsNullOrWhiteSpace(GioiTinh) || !string.IsNullOrWhiteSpace(MoiQuanHe))
            {
                if (string.IsNullOrWhiteSpace(HoTen))
                    yield return new ValidationResult("Họ tên người giám hộ bắt buộc nếu điền thông tin.", new[] { nameof(HoTen) });

                if (string.IsNullOrWhiteSpace(SDT))
                    yield return new ValidationResult("SĐT người giám hộ bắt buộc nếu điền thông tin.", new[] { nameof(SDT) });

                if (string.IsNullOrWhiteSpace(DiaChi))
                    yield return new ValidationResult("Địa chỉ người giám hộ bắt buộc nếu điền thông tin.", new[] { nameof(DiaChi) });
            }
        }
    }

    public class KhachHangManageViewModel
    {
        public KhachHang KhachHang { get; set; } = new KhachHang();

        public NguoiGiamHo NguoiGiamHo { get; set; } = new NguoiGiamHo();

        public TaiKhoan TaiKhoan { get; set; } = new TaiKhoan();
    }
    //===============PROFILE===================
    public class ProfileViewModel
    {
        public string MaKH { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "CCCD")]
        public string CCCD { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string MatKhauHienTai { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [System.Web.Mvc.Compare("MatKhauMoi", ErrorMessage = "Xác nhận mật khẩu mới không khớp")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhauMoi { get; set; }
    }
    

}