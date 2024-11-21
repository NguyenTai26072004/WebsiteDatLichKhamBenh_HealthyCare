using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteDatLichKhamBenh.Models
{
    public class AppointmentHistoryViewModel
    {
        public int Id { get; set; }                      // ID của lịch khám
        public DateTime NgayDatLich { get; set; }        // Ngày đặt lịch
        public DateTime NgayKhamBenh { get; set; }       // Ngày khám bệnh
        public TimeSpan GioKham { get; set; }            // Giờ khám bệnh
        public TimeSpan GioDatLich { get; set; }         // Giờ đặt lịch
        public string BacSi { get; set; }                // Tên bác sĩ phụ trách
        public string DiaChi { get; set; }               //Tên địa chỉ bệnh viên bác sĩ làm việc
        public string ChuyenKhoa { get; set; }           // Chuyên khoa của bác sĩ
        public string TrangThai { get; set; }            // Trạng thái của lịch khám
        public string ReviewStatus { get; set; }         // Trạng thái đánh giá
    }

}