using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class LoaiDichVu
    {
        [Key]
        public string MaLoaiDV { get; set; }
        public string TenLoaiDV { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
    }
    public class DichVu
    {
        [Key]
        [DisplayName("Mã dịch vụ")]
        public string MaDV { get; set; }

        [DisplayName("Tên dịch vụ")]
        public string TenDV { get; set; }

        [DisplayName("Loại dịch vụ")]
        public string MaLoaiDV { get; set; }

        [DisplayName("Giá dịch vụ")]
        public decimal GiaDichVu { get; set; }

        [DisplayName("Đơn vị tính")]
        public string DonViTinh { get; set; }

        public bool? TrangThai { get; set; }
    }
}