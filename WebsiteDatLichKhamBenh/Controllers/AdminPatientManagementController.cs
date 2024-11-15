using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminPatientManagementController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminPatientManagement
        // GET: AdminPatientManagement
        public ActionResult Index(string searchTerm, int page = 1)
        {
            var patients = db.BenhNhans.AsQueryable();

            // Tìm kiếm theo tên bệnh nhân hoặc ID bệnh nhân
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Tìm kiếm theo ID bệnh nhân hoặc tên bệnh nhân
                patients = patients.Where(p => p.tenBenhNhan.Contains(searchTerm) || p.idBenhNhan.ToString().Contains(searchTerm));
            }

            int pageSize = 10;
            int totalCount = patients.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            var result = patients.OrderBy(p => p.idBenhNhan)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            return View(result);
        }


        // GET: AdminPatientManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminPatientManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BenhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.BenhNhans.Add(benhNhan);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm bệnh nhân thành công!";
                return RedirectToAction("Index");
            }
            return View(benhNhan);
        }

        // GET: AdminPatientManagement/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                TempData["ErrorMessage"] = "ID bệnh nhân không hợp lệ.";
                return RedirectToAction("Index");
            }

            var patient = db.BenhNhans.Find(id);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Bệnh nhân không tồn tại.";
                return HttpNotFound();
            }

            // Đảm bảo ngày sinh được truyền đúng định dạng (yyyy-MM-dd)
            ViewBag.NgaySinh = patient.ngaySinh?.ToString("yyyy-MM-dd");

            return View(patient);
        }

        // POST: AdminPatientManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BenhNhan benhNhan)
        {
            if (benhNhan.idBenhNhan == 0)
            {
                ModelState.AddModelError("", "ID bệnh nhân không hợp lệ.");
                return View(benhNhan);
            }

            if (ModelState.IsValid)
            {
                // Lấy bệnh nhân từ cơ sở dữ liệu
                var patient = db.BenhNhans.Find(benhNhan.idBenhNhan);

                if (patient == null)
                {
                    ModelState.AddModelError("", "Bệnh nhân không tồn tại.");
                    return View(benhNhan);
                }

                // Giữ nguyên trường idAccount hiện tại, không cho phép sửa
                benhNhan.idAccount = patient.idAccount;

                // Cập nhật các trường khác
                patient.tenBenhNhan = benhNhan.tenBenhNhan;
                patient.ngaySinh = benhNhan.ngaySinh;
                patient.SDT = benhNhan.SDT;
                patient.Email = benhNhan.Email;
                patient.GioiTinh = benhNhan.GioiTinh;

                // Đánh dấu bệnh nhân đã thay đổi và cần cập nhật
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thông tin bệnh nhân thành công!";
                return RedirectToAction("Index");
            }

            return View(benhNhan);
        }




        // GET: AdminPatientManagement/Delete/5
        public ActionResult Delete(int id)
        {
            var patient = db.BenhNhans.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: AdminPatientManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Kiểm tra xem bệnh nhân có lịch hẹn nào liên kết không
            var patient = db.BenhNhans.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem bệnh nhân có liên kết với lịch hẹn nào không
            bool hasAppointment = db.LichKhams.Any(l => l.MaBenhNhan == id);

            if (hasAppointment)
            {
                // Nếu có lịch hẹn chưa hoàn thành, không cho phép xóa
                TempData["ErrorMessage"] = "Không thể xóa bệnh nhân vì bệnh này có lịch khám!";
                return RedirectToAction("Index");
            }

            // Nếu không có lịch hẹn liên kết, tiếp tục xóa bệnh nhân
            db.BenhNhans.Remove(patient);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa bệnh nhân thành công!";
            return RedirectToAction("Index");
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
