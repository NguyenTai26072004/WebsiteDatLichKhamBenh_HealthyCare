using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminPatientManagementController : Controller
    {
        // Đối tượng kết nối với cơ sở dữ liệu
        WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminPatientManagement
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10; // Số lượng bệnh nhân trên mỗi trang
            var patients = db.BenhNhans.ToList(); // Lấy danh sách bệnh nhân từ bảng BenhNhan

            // Phân trang
            var pagedPatients = patients.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Lưu số trang hiện tại và tổng số trang vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)patients.Count / pageSize);

            return View(pagedPatients); // Truyền danh sách bệnh nhân đã phân trang ra view
        }

        // GET: Hiển thị form chỉnh sửa bệnh nhân
        public ActionResult Edit(int id)
        {
            var patient = db.BenhNhans.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Lưu thông tin chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BenhNhan patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Xác nhận xóa bệnh nhân
        public ActionResult Delete(int id)
        {
            var patient = db.BenhNhans.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Thực hiện xóa bệnh nhân
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var patient = db.BenhNhans.Find(id);
            db.BenhNhans.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
