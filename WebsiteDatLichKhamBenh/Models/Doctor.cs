using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class Doctor
    {
        public int Id { get; set; } // idBS
        public string Name { get; set; } // tenBS
        public string Specialty { get; set; } // chuyenKhoa
        public int AppointmentCount { get; set; } // luotDat
    }
}