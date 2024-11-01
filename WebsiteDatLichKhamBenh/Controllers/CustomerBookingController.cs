using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerBookingController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        public CustomerBookingController()
        {
            db = new WebDatLichKhamBenhDBEntities();
        }
        public ActionResult Index(int doctorId)
        {
            // Lấy thông tin bác sĩ dựa trên doctorId
            var doctor = db.BacSis.FirstOrDefault(d => d.idBS == doctorId);
            if (doctor == null)
            {
                return HttpNotFound(); // Trả về 404 nếu không tìm thấy bác sĩ
            }

            // Lấy danh sách ca khám của bác sĩ
            var schedule = db.CaKhams.Where(c => c.idBS == doctorId && c.TrangThai == "Available").ToList();

            // Tạo ViewModel để truyền dữ liệu vào View
            var viewModel = new CustomerBookingViewModel
            {
                Doctor = doctor,
                Schedule = schedule
            };

            return View(viewModel);
        }
    }
    // ViewModel cho trang CustomerBooking
    public class CustomerBookingViewModel
    {
        public BacSi Doctor { get; set; }
        public List<CaKham> Schedule { get; set; }
    }
}