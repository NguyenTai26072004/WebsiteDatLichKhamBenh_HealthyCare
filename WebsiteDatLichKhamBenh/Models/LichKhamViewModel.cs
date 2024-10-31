using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class LichKhamViewModel
    {
        public int MaLichKham { get; set; }
        public string TenBenhNhan { get; set; }
        public string TenBacSi { get; set; }
        public DateTime NgayKham { get; set; }
        public string GioKham { get; set; }
        public string TrangThai { get; set; }
    }
}