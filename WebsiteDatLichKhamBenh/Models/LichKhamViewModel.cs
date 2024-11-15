using System;
using System.Collections.Generic;

namespace WebsiteDatLichKhamBenh.Models
{
    // ViewModel cho trang CustomerBooking
    public class CustomerBookingViewModel
    {
        public BacSi Doctor { get; set; }
        public List<ScheduleViewModel> Schedule { get; set; }

        // Thêm thông tin cơ sở y tế
        public CoSo CoSo { get; set; }

        // Thêm các thông tin bổ sung cho giao diện người dùng
        public DateTime NgayChon { get; set; } // Ngày khám mà người dùng chọn
        public string TenBenhVien => CoSo?.tenBenhVien ?? "N/A"; // Tên cơ sở y tế
        public string DiaChiBenhVien => CoSo?.DiaChi ?? "Chưa xác định"; // Địa chỉ cơ sở y tế
    }

    // ViewModel cho từng ca khám
    public class ScheduleViewModel
    {
        public int CaKhamId { get; set; } // ID của ca khám
        public string GioKham { get; set; } // Thời gian khám

        // Thêm các thông tin bổ sung để hiển thị trạng thái
        public bool IsAvailable { get; set; } // Trạng thái có sẵn của ca khám
        public string TrangThai => IsAvailable ? "Còn chỗ" : "Hết chỗ"; // Trạng thái text
    }
}
