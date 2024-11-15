using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminDashboardController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: Dashboard
        public ActionResult Index()
        {
            var today = DateTime.Now.Date;  // Lấy ngày hiện tại mà không có thời gian

            // Lấy số lịch hẹn xác nhận trong hôm nay
            var confirmedAppointmentsToday = db.LichKhams
                .Where(l => DbFunctions.TruncateTime(l.NgayDatLich) == today && l.TrangThai == "Đã được duyệt")
                .Count();

            // Lấy số bệnh nhân đã đăng ký trong hôm nay
            var registeredPatientsToday = db.LichKhams
                .Where(l => DbFunctions.TruncateTime(l.NgayDatLich) == today)
                .Select(l => l.MaBenhNhan)  // Chọn mã bệnh nhân
                .Distinct()  // Chỉ lấy các mã bệnh nhân duy nhất
                .Count();

            // Lấy số bác sĩ hiện có
            var totalDoctors = db.BacSis.Count();  // Số lượng bác sĩ trong hệ thống

            // Lấy số ca khám hiện hành
            var activeSchedules = db.CaKhams
                .Where(c => c.TrangThai == "Đang hoạt động")
                .Count();

            // Truyền các giá trị vào ViewBag
            ViewBag.ConfirmedAppointmentsToday = confirmedAppointmentsToday;
            ViewBag.RegisteredPatientsToday = registeredPatientsToday;
            ViewBag.TotalDoctors = totalDoctors;
            ViewBag.ActiveSchedules = activeSchedules;

            return View();
        }

        public ActionResult Logout()
        {
            // Xóa thông tin người dùng trong session hoặc cookie
            Session.Clear(); // Nếu bạn sử dụng session để lưu trữ thông tin người dùng
                             // Hoặc nếu dùng cookie thì bạn có thể làm như sau:
                             // Response.Cookies["username"].Expires = DateTime.Now.AddDays(-1); // Ví dụ xóa cookie tên người dùng

            // Chuyển hướng về trang đăng nhập sau khi đăng xuất
            return RedirectToAction("Index", "CustomerLogin"); // Điều hướng về trang đăng nhập
        }


    }
}
