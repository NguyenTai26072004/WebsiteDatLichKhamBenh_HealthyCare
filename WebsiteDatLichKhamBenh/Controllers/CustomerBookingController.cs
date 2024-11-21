using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
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

        // Lấy lịch khám theo ngày
        [HttpGet]
        public ActionResult GetSchedulesByDate(int doctorId, string selectedDate)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(selectedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var schedules = db.CaKhams
                    .Include(ca => ca.KhungGio)
                    .Where(ca => ca.idBS == doctorId && DbFunctions.TruncateTime(ca.NgayKham) == parsedDate.Date && ca.TrangThai == "Đang hoạt động")
                    .Select(ca => new
                    {
                        CaKhamId = ca.MaCaKham,
                        GioKham = ca.KhungGio.GioKham
                    })
                    .ToList();

                return Json(schedules, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Hiển thị trang đặt lịch khám
        public ActionResult Index(int doctorId)
        {
            var doctor = db.BacSis.Include(d => d.CoSo).FirstOrDefault(d => d.idBS == doctorId);
            if (doctor == null)
            {
                return HttpNotFound();
            }

            var schedule = db.CaKhams
                             .Where(c => c.idBS == doctorId && c.TrangThai == "Đang hoạt động")
                             .Include(c => c.KhungGio)
                             .Select(c => new ScheduleViewModel
                             {
                                 CaKhamId = c.MaCaKham,
                                 GioKham = c.KhungGio.GioKham
                             })
                             .ToList();

            // Lấy danh sách đánh giá từ bảng DanhGiaBacSi
            var ratings = db.DanhGiaBacSis
                                    .Where(d => d.idBacSi == doctorId)  // Sử dụng idBacSi thay vì idBS
                                    .Select(d => new Rating
                                    {
                                        PatientName = d.BenhNhan.tenBenhNhan, // Tên bệnh nhân
                                        Score = d.diemDanhGia.HasValue ? d.diemDanhGia.Value : 0,  // Kiểm tra null và gán giá trị mặc định nếu null
                                        Comment = d.binhLuan                  // Nội dung bình luận (dùng binhLuan thay vì NoiDung)
                                    })
                                    .ToList();


            var viewModel = new CustomerBookingViewModel
            {
                Doctor = doctor,
                Schedule = schedule,
                CoSo = doctor.CoSo,
                Ratings = ratings // Thêm thông tin đánh giá
            };

            return View(viewModel);
        }


        // Kiểm tra trạng thái đăng nhập
        public ActionResult BookingCheck(int doctorId)
        {
            if (Session["Username"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để tiếp tục.";
                return RedirectToAction("Index", "CustomerLogin");
            }

            return RedirectToAction("Index", new { doctorId });
        }

        // Chi tiết đặt lịch
        public ActionResult BookingDetails(int caKhamId)
        {
            var caKham = db.CaKhams.Include(c => c.KhungGio).FirstOrDefault(c => c.MaCaKham == caKhamId);
            if (caKham == null)
            {
                return HttpNotFound();
            }

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "CustomerLogin");
            }

            int userId = (int)Session["UserId"];

            var patient = db.BenhNhans.FirstOrDefault(b => b.idAccount == userId);
            var doctor = db.BacSis.FirstOrDefault(d => d.idBS == caKham.idBS);

            var bookingDetails = new BookingDetailsViewModel
            {
                CaKham = caKham,
                Doctor = doctor,
                Patient = patient
            };

            return View(bookingDetails);
        }

        [HttpPost]
        public ActionResult ConfirmBooking(int caKhamId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "CustomerLogin");
            }

            int userId = (int)Session["UserId"];
            var caKham = db.CaKhams.Include(c => c.KhungGio).FirstOrDefault(c => c.MaCaKham == caKhamId);
            if (caKham == null)
            {
                return HttpNotFound();
            }

            var patient = db.BenhNhans.FirstOrDefault(b => b.idAccount == userId);

            if (patient == null)
            {
                return HttpNotFound();
            }

            var lichKham = new LichKham
            {
                MaBenhNhan = patient.idBenhNhan,
                MaCaKham = caKham.MaCaKham,
                NgayDatLich = DateTime.Now,
                GioDatLich = DateTime.Now.TimeOfDay,
                TrangThai = "Đang chờ duyệt"
            };

            db.LichKhams.Add(lichKham);
            db.SaveChanges();

            return RedirectToAction("BookingSuccess");
        }

        public ActionResult BookingSuccess()
        {
            return View();
        }
    }
}
