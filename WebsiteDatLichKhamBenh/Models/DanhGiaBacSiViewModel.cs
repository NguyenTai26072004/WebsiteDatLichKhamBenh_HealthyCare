using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class DanhGiaBacSiViewModel
    {
        public int MaLichKham { get; set; }  // Mã lịch khám (để biết bệnh nhân đang đánh giá cho lịch khám nào)
        public int MaBacSi { get; set; }     // Mã bác sĩ (để xác định bác sĩ mà bệnh nhân sẽ đánh giá)
        public string TenBacSi { get; set; } // Tên bác sĩ để hiển thị trong view

        // Các thuộc tính cho việc nhập đánh giá
        public float DiemDanhGia { get; set; }  // Điểm đánh giá (từ 1 đến 5)
        public string BinhLuan { get; set; }    // Bình luận của bệnh nhân về bác sĩ
    }
}