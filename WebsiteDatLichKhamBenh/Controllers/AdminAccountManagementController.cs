using System;
using System.Collections.Generic;
using System.Data.Entity; // Thêm thư viện này để sử dụng EntityState
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminAccountManagementController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        public ActionResult Index(string searchTerm, int page = 1)
        {
            var accounts = db.Accounts.AsQueryable();

            // Tìm kiếm theo ID hoặc Tên Tài Khoản
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Tìm kiếm theo ID (idAccount) hoặc Tên Tài Khoản (TaiKhoan)
                accounts = accounts.Where(a =>
                    a.idAccount.ToString().Contains(searchTerm) || // Tìm kiếm theo idAccount
                    a.TaiKhoan.Contains(searchTerm));            // Tìm kiếm theo tên tài khoản
            }

            // Số lượng tài khoản trên mỗi trang
            int pageSize = 10;
            int totalCount = accounts.Count();  // Tổng số tài khoản
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Tổng số trang

            // Lưu thông tin phân trang vào ViewBag để sử dụng trong view
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            // Truy vấn dữ liệu cho trang hiện tại
            var result = accounts.OrderBy(a => a.idAccount)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            return View(result); // Trả về kết quả cho view
        }

        // GET: AdminAccountManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminAccountManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();

                // Thêm thông báo thành công khi tạo tài khoản mới
                TempData["SuccessMessage"] = "Thêm tài khoản mới thành công!";
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: AdminAccountManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: AdminAccountManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();

                // Thêm thông báo thành công khi cập nhật tài khoản
                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: AdminAccountManagement/Delete/5
        public ActionResult Delete(int id)
        {
            var account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            return View(account); // Trả về view để xác nhận xóa
        }

        // POST: AdminAccountManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem tài khoản có liên kết với bảng BenhNhan không
            var hasBenhNhanReferences = db.BenhNhans.Any(b => b.idAccount == id);
            // Kiểm tra xem tài khoản có liên kết với bảng BacSi không
            var hasBacSiReferences = db.BacSis.Any(b => b.idAccount == id);

            if (hasBenhNhanReferences || hasBacSiReferences)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản này vì nó đang được sử dụng trong bảng BenhNhan hoặc BacSi.";
                return View(account); // Trả về view Delete với thông báo lỗi
            }

            // Kiểm tra xem tài khoản có đang được tham chiếu trong bảng CaKhams không
            var hasCaKhamReferences = db.CaKhams.Any(c => c.idBS == id);
            if (hasCaKhamReferences)
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản này vì nó đang được sử dụng trong bảng CaKham.");
                return View(account); // Trả về view Delete với thông báo lỗi
            }

            // Nếu không có liên kết nào, thực hiện xóa tài khoản
            db.Accounts.Remove(account);
            db.SaveChanges();

            // Thêm thông báo thành công khi xóa tài khoản
            TempData["SuccessMessage"] = "Tài khoản đã được xóa thành công!";
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
