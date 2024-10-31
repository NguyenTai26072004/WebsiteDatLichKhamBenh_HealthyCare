using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models; // Chỉnh đường dẫn namespace của DbContext nếu khác

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminDashboardController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminDashboard
        public ActionResult Index()
        {
            // Lịch hẹn hôm nay với trạng thái "Đã xác nhận"
            var today = DateTime.Today;
            var confirmedAppointmentsToday = db.LichKhams
                .Where(l => l.NgayDatLich == today && l.TrangThai == "Đã xác nhận")
                .Count();

            // Bệnh nhân đã đăng ký hôm nay
            var registeredPatientsToday = db.LichKhams
                .Where(l => l.NgayDatLich == today)
                .Select(l => l.MaBenhNhan)
                .Distinct()
                .Count();

            // Tổng số bác sĩ hiện có
            var totalDoctors = db.BacSis.Count();

            // Số ca khám với trạng thái "Đang hoạt động"
            var activeSchedules = db.CaKhams
                .Where(c => c.TrangThai == "Đang hoạt động")
                .Count();

            // Truyền dữ liệu sang View thông qua ViewBag
            ViewBag.ConfirmedAppointmentsToday = confirmedAppointmentsToday;
            ViewBag.RegisteredPatientsToday = registeredPatientsToday;
            ViewBag.TotalDoctors = totalDoctors;
            ViewBag.ActiveSchedules = activeSchedules;

            return View();
        }
    }
}
