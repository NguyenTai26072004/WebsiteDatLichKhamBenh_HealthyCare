using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class DoctorBookingManagementController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        public ActionResult Index(string search)
        {
            // Lấy thông tin bác sĩ từ session
            var doctorId = Session["UserID"] != null ? int.Parse(Session["UserID"].ToString()) : 0;
            if (doctorId == 0)
            {
                return RedirectToAction("Index", "CustomerLogin");
            }

            // Lấy tất cả các lịch hẹn đã được duyệt hoặc đã khám của bệnh nhân đối với bác sĩ này
            var appointments = db.LichKhams
                .Where(l => l.MaCaKham.HasValue && l.CaKham.idBS == doctorId &&
                            (l.TrangThai == "Đã được duyệt" || l.TrangThai == "Đã khám"))
                .Select(l => new AppointmentViewModel
                {
                    MaLichKham = l.MaLichKham,
                    tenBenhNhan = l.BenhNhan.tenBenhNhan,
                    NgayKham = l.CaKham.NgayKham,
                    GioKham = l.CaKham.KhungGio.GioKham,
                    TrangThai = l.TrangThai
                }).AsQueryable();

            // Tìm kiếm theo mã lịch hẹn hoặc tên bệnh nhân
            if (!string.IsNullOrEmpty(search))
            {
                // Kiểm tra nếu người dùng nhập vào một số (mã lịch hẹn)
                int searchIdInt;
                if (int.TryParse(search, out searchIdInt))
                {
                    appointments = appointments.Where(a => a.MaLichKham == searchIdInt);
                }
                else
                {
                    // Nếu không phải số, tìm kiếm theo tên bệnh nhân
                    appointments = appointments.Where(a => a.tenBenhNhan.Contains(search));
                }
            }

            return View(appointments.ToList());
        }


        public ActionResult Details(int id)
        {
            var appointment = db.LichKhams
                .Where(l => l.MaLichKham == id)
                .Select(l => new AppointmentViewModel
                {
                    MaLichKham = l.MaLichKham,
                    tenBenhNhan = l.BenhNhan.tenBenhNhan,
                    NgayKham = l.CaKham.NgayKham,
                    GioKham = l.CaKham.KhungGio.GioKham,
                    TrangThai = l.TrangThai,
                    tenBS = l.CaKham.BacSi.tenBS // Lấy tên bác sĩ
                }).FirstOrDefault();

            if (appointment == null)
            {
                return HttpNotFound();
            }

            return View(appointment);
        }


    }
}
