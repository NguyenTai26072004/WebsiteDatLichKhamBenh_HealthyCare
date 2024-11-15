using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class BookingDetailsViewModel
    {
        public CaKham CaKham { get; set; }  // Thông tin về ca khám
        public BacSi Doctor { get; set; }  // Thông tin về bác sĩ
        public BenhNhan Patient { get; set; }  // Thông tin về bệnh nhân
        public string GioKham { get; set; }
    }
}