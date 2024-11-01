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

        public ActionResult Index(string searchString, int page = 1)
        {
            int pageSize = 10; // Số lượng bệnh nhân trên mỗi trang
            var patients = db.BenhNhans.AsQueryable(); // Lấy danh sách bệnh nhân từ bảng BenhNhan

            // Nếu có từ khóa tìm kiếm, lọc danh sách bệnh nhân theo tên hoặc ID
            if (!string.IsNullOrEmpty(searchString))
            {
                // Kiểm tra xem từ khóa tìm kiếm có phải là số hay không
                // Chuyển đổi searchString thành số nếu có thể
                if (int.TryParse(searchString, out int patientId))
                {
                    // Nếu là số, tìm kiếm theo ID
                    patients = patients.Where(p => p.idBenhNhan == patientId);
                }
                else
                {
                    // Nếu không phải số, tìm kiếm theo tên
                    patients = patients.Where(p => p.tenBenhNhan.Contains(searchString));
                }
            }

            // Sắp xếp danh sách bệnh nhân theo tên (hoặc một trường khác)
            patients = patients.OrderBy(p => p.tenBenhNhan);

            // Lấy danh sách bệnh nhân đã phân trang
            var pagedPatients = patients.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Lưu số trang hiện tại và tổng số trang vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)patients.Count() / pageSize);
            ViewBag.SearchString = searchString; // Lưu từ khóa tìm kiếm để hiển thị lại trong view

            return View(pagedPatients); // Truyền danh sách bệnh nhân đã phân trang ra view
        }

        // GET: Hiển thị form chỉnh sửa bệnh nhân
        public ActionResult Edit(int id) // Thay đổi kiểu dữ liệu id thành int
        {
            var patient = db.BenhNhans.Find(id); // Tìm kiếm theo id int
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
                // Tìm bản ghi trong cơ sở dữ liệu
                var existingPatient = db.BenhNhans.Find(patient.idBenhNhan); // Tìm kiếm theo id int
                if (existingPatient == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các thuộc tính cần thiết
                existingPatient.tenBenhNhan = patient.tenBenhNhan;
                existingPatient.ngaySinh = patient.ngaySinh;
                existingPatient.GioiTinh = patient.GioiTinh;
                existingPatient.SDT = patient.SDT;
                existingPatient.Email = patient.Email;

                // Lưu thay đổi
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Xác nhận xóa bệnh nhân
        public ActionResult Delete(int id) // Thay đổi kiểu dữ liệu id thành int
        {
            var patient = db.BenhNhans.Find(id); // Tìm kiếm theo id int
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // Thay đổi kiểu dữ liệu id thành int
        {
            var patient = db.BenhNhans.Find(id); // Tìm kiếm theo id int
            if (patient == null)
            {
                return HttpNotFound();
            }

            db.BenhNhans.Remove(patient);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
