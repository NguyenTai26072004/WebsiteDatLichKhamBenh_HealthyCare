using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminExamenController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminExamen
        public ActionResult Index(string searchTerm, int page = 1)
        {
            // Tìm kiếm theo mã ca khám hoặc tên bác sĩ
            var caKhams = db.CaKhams.Include(c => c.BacSi).Include(c => c.CoSo).Include(c => c.KhungGio);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Kiểm tra xem searchTerm có phải là số không
                if (int.TryParse(searchTerm, out int maCaKham))
                {
                    // Tìm kiếm theo mã ca khám nếu searchTerm là số
                    caKhams = caKhams.Where(c => c.MaCaKham == maCaKham);
                }
                else
                {
                    // Tìm kiếm theo tên bác sĩ
                    caKhams = caKhams.Where(c => c.BacSi.tenBS.Contains(searchTerm));
                }
            }

            int pageSize = 10;
            ViewBag.TotalCount = caKhams.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            var result = caKhams.OrderBy(c => c.NgayKham)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            return View(result);
        }


        // Phê duyệt ca khám
        public async Task<ActionResult> Approve(int id) // Thay đổi kiểu dữ liệu id
        {
            var appointment = await db.CaKhams.FindAsync(id); // Tìm kiếm theo id int
            if (appointment != null)
            {
                appointment.TrangThai = "Đang hoạt động";
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Từ chối ca khám
        public async Task<ActionResult> Reject(int id)
        {
            var appointment = await db.CaKhams.FindAsync(id);
            if (appointment != null)
            {
                appointment.TrangThai = "Đã từ chối";
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Cập nhật trạng thái ca khám qua AJAX
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int caKhamId, string newStatus)
        {
            var appointment = await db.CaKhams.FindAsync(caKhamId);
            if (appointment != null)
            {
                appointment.TrangThai = newStatus;
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
