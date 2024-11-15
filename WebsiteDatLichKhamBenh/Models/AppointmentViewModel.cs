using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class AppointmentViewModel
    {
        public int MaLichKham { get; set; }
        public string tenBenhNhan { get; set; }
        public DateTime? NgayKham { get; set; }  // Thay đổi từ DateTime thành DateTime?
        public string GioKham { get; set; }
        public string TrangThai { get; set; }
        public string tenBS { get; set; }
    }


}