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
            // Lấy danh sách lịch khám từ database
            var bookings = db.LichKhams.Include(b => b.BenhNhan).Include(b => b.CaKham); // Giả sử bạn có liên kết với bảng bệnh nhân và ca khám

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bookings = bookings.Where(b =>
                    b.MaLichKham.Equals(searchTerm) || // Tìm theo mã lịch khám
                    b.BenhNhan.tenBenhNhan.Contains(searchTerm) //theo tên bệnh nhân
                );
            }

            int pageSize = 10;
            ViewBag.TotalCount = bookings.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm; // Để giữ giá trị tìm kiếm trong view

            var result = bookings.OrderBy(b => b.NgayDatLich)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            return View(result);
        }

        // Phê duyệt đặt lịch
        public async Task<ActionResult> Approve(int id) // Chuyển đổi kiểu dữ liệu thành int
        {
            var booking = await db.LichKhams.FindAsync(id);
            if (booking != null)
            {
                booking.TrangThai = "Đã xác nhận";
                await db.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Đặt lịch đã được xác nhận thành công.";
            return RedirectToAction("Index");
        }

        // Hủy đặt lịch
        public async Task<ActionResult> Cancel(int id) // Chuyển đổi kiểu dữ liệu thành int
        {
            var booking = await db.LichKhams.FindAsync(id);
            if (booking != null)
            {
                booking.TrangThai = "Đã hủy";
                await db.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Đặt lịch đã được hủy thành công.";
            return RedirectToAction("Index");
        }

        // Cập nhật trạng thái đặt lịch qua AJAX
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int lichKhamId, string newStatus) // Chuyển đổi kiểu dữ liệu thành int
        {
            var booking = await db.LichKhams.FindAsync(lichKhamId);
            if (booking != null)
            {
                booking.TrangThai = newStatus;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
