using System;
using System.Collections.Generic;

namespace WebsiteDatLichKhamBenh.Models
{
    public class PersonalViewModel
    {
        public int idBenhNhan { get; set; }
        public string tenBenhNhan { get; set; }
        public DateTime? ngaySinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string GioiTinh { get; set; }

        public List<string> TienSuBenh { get; set; } = new List<string>();
        public List<string> DiUng { get; set; } = new List<string>();
    }
}
