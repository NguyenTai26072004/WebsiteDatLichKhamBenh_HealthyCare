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
            // Kiểm tra nếu người dùng đã đăng nhập
            if (Session["UserID"] == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập và truyền thông báo
                TempData["Message"] = "Bạn cần đăng nhập để đặt lịch khám.";
                return RedirectToAction("Index", "CustomerLogin");
            }

            var doctor = db.BacSis.Include(d => d.CoSo).FirstOrDefault(d => d.idBS == doctorId);
            if (doctor == null)
            {
                return HttpNotFound();
            }

            // Lấy lịch khám của bác sĩ
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
                                Comment = d.binhLuan                  // Nội dung bình luận
                            })
                            .ToList();

            // Cập nhật số lượt đặt vào ViewModel
            var viewModel = new CustomerBookingViewModel
            {
                Doctor = doctor,
                Schedule = schedule,
                CoSo = doctor.CoSo,
                Ratings = ratings, // Thêm thông tin đánh giá
                SoLuotDat = doctor.luotDat ?? 0 // Thêm số lượt đặt vào ViewModel
            };

            return View(viewModel);
        }



        // Kiểm tra trạng thái đăng nhập
        public ActionResult BookingCheck()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["Username"] == null)
            {
                // Nếu chưa đăng nhập, trả về thông báo yêu cầu đăng nhập
                return Json(new { redirectToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            // Nếu đã đăng nhập, trả về kết quả không chuyển hướng
            return Json(new { redirectToLogin = false }, JsonRequestBehavior.AllowGet);
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
            if (Session["UserId"] == null || Session["Username"] == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để đặt lịch khám";
                return RedirectToAction("Index", "CustomerLogin");
            }

            int userId = (int)Session["UserId"];
            var caKham = db.CaKhams.Include(c => c.KhungGio).FirstOrDefault(c => c.MaCaKham == caKhamId);
            if (caKham == null)
            {
                TempData["Error"] = "Lịch khám không tồn tại. Vui lòng thử lại.";
                return RedirectToAction("Index", "CustomerFindDoctor");
            }

            var patient = db.BenhNhans.FirstOrDefault(b => b.idAccount == userId);
            if (patient == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bệnh nhân. Vui lòng liên hệ hỗ trợ.";
                return RedirectToAction("Index", "CustomerFindDoctor");
            }

            var existingAppointment = db.LichKhams
                .Include(lk => lk.CaKham.KhungGio)
                .FirstOrDefault(lk => lk.MaBenhNhan == patient.idBenhNhan &&
                                      lk.CaKham.NgayKham == caKham.NgayKham &&
                                      lk.CaKham.KhungGio.GioKham == caKham.KhungGio.GioKham &&
                                      lk.TrangThai != "Đã từ chối");

            if (existingAppointment != null)
            {
                TempData["Error"] = "Bạn đã có lịch khám vào thời điểm này. Vui lòng chọn thời gian khác.";
                return RedirectToAction("BookingDetails", new { caKhamId });
            }

            try
            {
                var lichKham = new LichKham
                {
                    MaBenhNhan = patient.idBenhNhan,
                    MaCaKham = caKham.MaCaKham,
                    NgayDatLich = DateTime.Now,
                    GioDatLich = DateTime.Now.TimeOfDay,
                    TrangThai = "Đang chờ duyệt"
                };

                db.LichKhams.Add(lichKham);

                // Cập nhật số lượt đặt khám cho bác sĩ
                var doctor = db.BacSis.FirstOrDefault(d => d.idBS == caKham.idBS);
                if (doctor != null)
                {
                    doctor.luotDat += 1; // Tăng số lượt đặt khám
                }

                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                return RedirectToAction("BookingSuccess");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Đã xảy ra lỗi trong quá trình đặt lịch. Vui lòng thử lại.";
                return RedirectToAction("BookingDetails", new { caKhamId });
            }
        }


        public ActionResult BookingSuccess()
        {
            return View();
        }


    }
}
