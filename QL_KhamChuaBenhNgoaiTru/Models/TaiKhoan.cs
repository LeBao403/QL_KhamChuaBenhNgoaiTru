using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class TaiKhoan
    {
        public int MaTK { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string Username { get; set; } // Dùng cho cả KH (SĐT) và NV

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class BenhNhanRegisterModel
    {

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Remote("IsPhoneAvailable", "Account", HttpMethod = "POST", ErrorMessage = "Số điện thoại đã được sử dụng.")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [Remote("IsEmailAvailable", "Account", HttpMethod = "POST", ErrorMessage = "Email đã được sử dụng.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu và Nhập lại mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class NhanVienAuthInfo
    {
        public string MaNV { get; set; }
        public int MaTK { get; set; }
        public string HoTen { get; set; }
        public int? MaChucVu { get; set; }
        public string TenChucVu { get; set; } // Tên chức vụ (Rất quan trọng)
        public int? MaCoSo { get; set; } // Mã cơ sở (Rất quan trọng)
        public int? MaPhong { get; set; }
    }

    public class BenhNhanAuthInfo
    {
        public string MaKH { get; set; }
        public int MaTK { get; set; }
        public string HoTen { get; set; }
    }
}