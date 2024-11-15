using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminBookingListController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminBookingList
        public ActionResult Index(string searchTerm, int page = 1)
        {
            var bookings = db.LichKhams.Include(b => b.BenhNhan).Include(b => b.CaKham); // Liên kết với bảng bệnh nhân và ca khám

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Kiểm tra xem searchTerm là số hay chuỗi
                int parsedSearchTerm;
                bool isNumber = int.TryParse(searchTerm, out parsedSearchTerm);

                if (isNumber)
                {
                    // Nếu searchTerm là số, tìm theo mã lịch khám
                    bookings = bookings.Where(b => b.MaLichKham == parsedSearchTerm);
                }
                else
                {
                    // Nếu searchTerm là chuỗi, tìm theo tên bệnh nhân
                    bookings = bookings.Where(b =>
                        b.BenhNhan.tenBenhNhan.Contains(searchTerm) || // Tìm theo tên bệnh nhân
                        b.MaLichKham.ToString().Contains(searchTerm)   // Tìm theo mã lịch khám dưới dạng chuỗi
                    );
                }
            }

            int pageSize = 10;
            ViewBag.TotalCount = bookings.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            var result = bookings.OrderBy(b => b.NgayDatLich)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            return View(result);
        }

        // Cập nhật trạng thái đặt lịch qua AJAX
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int lichKhamId, string newStatus)
        {
            var booking = await db.LichKhams.FindAsync(lichKhamId);
            if (booking != null)
            {
                booking.TrangThai = newStatus;
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công";
                return Json(new { success = true });
            }
            else
            {
                TempData["ErrorMessage"] = "Lịch khám không tồn tại.";
                return Json(new { success = false });
            }
        }

        // Xóa lịch khám
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var booking = db.LichKhams.Find(id);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Lịch khám không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra trạng thái và các điều kiện xóa
            if (booking.TrangThai != "Đã từ chối")
            {
                TempData["ErrorMessage"] = "Chỉ có thể xóa lịch khám với trạng thái 'Đã từ chối'.";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu tồn tại liên kết với đơn thuốc
            var relatedPrescriptions = db.DonThuocs.Any(p => p.LichKhams.Any(lk => lk.MaLichKham == id));
            if (relatedPrescriptions)
            {
                TempData["ErrorMessage"] = "Không thể xóa lịch khám vì tồn tại liên kết với đơn thuốc.";
                return RedirectToAction("Index");
            }

            db.LichKhams.Remove(booking);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xóa lịch khám thành công.";
            return RedirectToAction("Index");
        }
    }
}
