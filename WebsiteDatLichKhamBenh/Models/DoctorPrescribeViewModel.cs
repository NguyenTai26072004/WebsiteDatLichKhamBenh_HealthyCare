using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteDatLichKhamBenh.Models
{
    public class DoctorPrescriptionViewModel
    {
        public int MaLichKham { get; set; }
        public DateTime? NgayKham { get; set; }
        public string KhungGio { get; set; }
        public string tenBenhNhan { get; set; }
        public string TrangThai { get; set; }
        public bool CanPrescribe { get; set; }
        public bool HasPrescription { get; set; }
        public string GhiChu { get; set; }

        // Danh sách thuốc để hiển thị trong DropdownList
        public List<SelectListItem> ThuocList { get; set; }

        // Danh sách chi tiết đơn thuốc
        public List<DoctorPrescriptionDetailViewModel> PrescriptionDetails { get; set; }
    }


}