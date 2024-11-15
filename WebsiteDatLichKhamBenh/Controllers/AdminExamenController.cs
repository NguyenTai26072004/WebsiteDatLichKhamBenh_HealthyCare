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
            var examen = db.CaKhams.Include(b => b.BacSi);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Kiểm tra kiểu dữ liệu của searchTerm và thực hiện tìm kiếm tương ứng
                int parsedSearchTerm;
                bool isNumber = int.TryParse(searchTerm, out parsedSearchTerm);

                if (isNumber)
                {
                    // Nếu searchTerm là số, tìm kiếm theo mã ca khám (MaCaKham là kiểu số)
                    examen = examen.Where(b => b.MaCaKham == parsedSearchTerm || b.BacSi.tenBS.Contains(searchTerm));
                }
                else
                {
                    // Nếu searchTerm là chuỗi, tìm kiếm theo tên bác sĩ và mã ca khám dưới dạng chuỗi
                    examen = examen.Where(b =>
                        b.MaCaKham.ToString().Contains(searchTerm) || // Tìm theo mã ca khám
                        b.BacSi.tenBS.Contains(searchTerm)           // Tìm theo tên Bác sĩ
                    );
                }
            }

            int pageSize = 10;
            ViewBag.TotalCount = examen.Count();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            var result = examen.OrderBy(b => b.NgayKham)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            return View(result);
        }

        // Cập nhật trạng thái đặt lịch qua AJAX
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int caKhamId, string newStatus)
        {
            var examen = await db.CaKhams.FindAsync(caKhamId);
            if (examen != null)
            {
                examen.TrangThai = newStatus;
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công";
                return Json(new { success = true });
            }
            else
            {
                TempData["ErrorMessage"] = "Ca khám không tồn tại";
                return Json(new { success = false });
            }
        }

        // Xóa ca khám
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Tìm ca khám theo id
            var examen = db.CaKhams.Find(id);

            if (examen == null)
            {
                TempData["ErrorMessage"] = "Ca khám không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem có tồn tại lịch khám nào liên kết với ca khám này không
            var relatedBookings = db.LichKhams.Where(lk => lk.MaCaKham == examen.MaCaKham).ToList();

            // Nếu có bản ghi trong LichKham liên kết với MaCaKham thì không cho phép xóa
            if (relatedBookings.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa ca khám này vì nó đang được liên kết với một lịch khám.";
                return RedirectToAction("Index");
            }

            // Kiểm tra trạng thái và các điều kiện xóa
            if (examen.TrangThai != "Đã từ chối")
            {
                TempData["ErrorMessage"] = "Chỉ có thể xóa ca khám với trạng thái 'Đã từ chối'.";
                return RedirectToAction("Index");
            }

            // Tiến hành xóa ca khám
            db.CaKhams.Remove(examen);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xóa ca khám thành công.";
            return RedirectToAction("Index");
        }
    }
}
