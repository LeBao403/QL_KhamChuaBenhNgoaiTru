using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace QL_KhamChuaBenhNgoaiTru.Models
{
    public class DanhMucBenh
    {
        [Key]
        [DisplayName("Mã bệnh (ICD)")]
        public string MaBenh { get; set; }

        [DisplayName("Tên bệnh")]
        public string TenBenh { get; set; }
        public string TrieuChung { get; set; }
        public string MoTa { get; set; }
        public bool? SoGiaiDoan { get; set; }
    }
}