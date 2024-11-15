using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerHistoryController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerHistory
        public ActionResult Index()
        {
            int userID = (int)Session["UserID"]; // Giả sử UserID được lưu trong session

            var patientHistory = db.LichKhams
                .Where(lk => lk.MaBenhNhan == userID)
                .Join(db.CaKhams, lk => lk.MaCaKham, ck => ck.MaCaKham, (lk, ck) => new { lk, ck })
                .Join(db.BacSis, combined => combined.ck.idBS, bs => bs.idBS, (combined, bs) => new { combined.lk, combined.ck, bs })
                .Join(db.KhungGios, combined => combined.ck.MaKhungGio, kg => kg.MaKhungGio, (combined, kg) => new
                {
                    combined.lk,
                    combined.ck,
                    combined.bs,
                    TenKhungGio = kg.GioKham,  // Lấy tên khung giờ từ bảng KhungGio
                    GioDatLich = combined.lk.GioDatLich ?? TimeSpan.Zero // Giờ đặt lịch
                })
                .ToList()
                .Select(combined => new AppointmentHistoryViewModel
                {
                    Id = combined.lk.MaLichKham,
                    NgayDatLich = combined.lk.NgayDatLich ?? DateTime.MinValue,  // Ngày và giờ đặt lịch
                    NgayKhamBenh = combined.ck.NgayKham ?? DateTime.MinValue,  // Ngày khám
                    GioDatLich = combined.GioDatLich,  // Giờ đặt lịch từ DB
                    GioKham = TimeSpan.TryParse(combined.TenKhungGio, out var gioKham) ? gioKham : TimeSpan.Zero, // Giờ khám bệnh
                    BacSi = combined.bs.tenBS,
                    ChuyenKhoa = combined.bs.chuyenKhoa,
                    TrangThai = combined.lk.TrangThai
                }).ToList();

            return View(patientHistory);
        }


    }
}
