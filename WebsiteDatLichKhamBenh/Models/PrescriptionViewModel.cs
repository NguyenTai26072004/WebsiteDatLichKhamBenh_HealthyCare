using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteDatLichKhamBenh.Models
{
    public class PrescriptionViewModel
    {
        public int MaLichKham { get; set; }
        public string GhiChu { get; set; }  // Ghi chú cho toàn bộ đơn thuốc
        public List<SelectListItem> ThuocList { get; set; }
        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; }
    }

    public class PrescriptionDetailViewModel
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string LieuLuong { get; set; }
        public int SoLuong { get; set; }
    }

}