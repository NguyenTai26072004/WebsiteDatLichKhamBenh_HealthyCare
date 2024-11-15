using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class DoctorPrescriptionDetailViewModel
    {
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string LieuLuong { get; set; }
        public int SoLuong { get; set; }
    }
}