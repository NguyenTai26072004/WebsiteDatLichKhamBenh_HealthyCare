using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class CustomerPrescribeViewModel
    {
        public int MaDonThuoc { get; set; }
        public DateTime? NgayKeDon { get; set; }
        public string GhiChu { get; set; }
        public List<CustomerPrescriptionDetail> CustomerPrescriptionDetails { get; set; }
    }

    public class CustomerPrescriptionDetail
    {
        public string TenThuoc { get; set; }
        public string LieuLuong { get; set; }
        public int SoLuong { get; set; }
    }
}