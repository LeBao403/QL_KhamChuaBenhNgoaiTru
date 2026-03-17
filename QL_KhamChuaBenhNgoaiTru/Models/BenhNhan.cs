using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class BenhNhan
    {
        [StringLength(10)]
        public string MaBN { get; set; }

        [Required(ErrorMessage = "Họ tên khách hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được dài quá 100 ký tự.")]
        public string HoTen { get; set; }

        [StringLength(20, ErrorMessage = "CCCD không được dài quá 20 ký tự.")]
        public string CCCD { get; set; }

        [Phone(ErrorMessage = "SĐT không hợp lệ.")]
        [StringLength(15, ErrorMessage = "SĐT không được dài quá 15 ký tự.")]
        public string SDT { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được dài quá 100 ký tự.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        [StringLength(10)]
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được dài quá 200 ký tự.")]
        public string DiaChi { get; set; }

        // --- CÁC TRƯỜNG THÔNG TIN BẢO HIỂM Y TẾ ---
        public bool BHYT { get; set; }

        [StringLength(50, ErrorMessage = "Số thẻ BHYT không được dài quá 50 ký tự.")]
        public string SoTheBHYT { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HanSuDungBHYT { get; set; } // Phải để nullable (DateTime?) vì nếu không có BHYT thì trường này rỗng

        [StringLength(50)]
        public string TuyenKham { get; set; }

        public int? MucHuongBHYT { get; set; } // Phải để nullable (int?) vì nếu không có BHYT thì trường này rỗng

        public int? MaTK { get; set; }

    }


    public class DanhMucTienSuYTe
    {
        public string MaTSYT { get; set; }
        public string TenTSYT { get; set; }
    }

    public class TienSuYTeBenhNhan
    {
        public string MaTSYT { get; set; }
        public string MaBN { get; set; }
    }

    

    public class BenhNhanManageViewModel
    {
        public BenhNhan BenhNhan { get; set; } = new BenhNhan();

        public TaiKhoan TaiKhoan { get; set; } = new TaiKhoan();
    }
    //===============PROFILE===================
    public class ProfileViewModel
    {
        public string MaBN { get; set; }

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