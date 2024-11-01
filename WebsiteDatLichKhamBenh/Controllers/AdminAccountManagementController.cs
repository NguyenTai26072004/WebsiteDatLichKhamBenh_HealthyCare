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

        // GET: AdminAccountManagement
        public ActionResult Index()
        {
            var accounts = db.Accounts.ToList(); 
            return View(accounts); 
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

            // Kiểm tra xem tài khoản có đang được tham chiếu trong bảng khác không
            var hasReferences = db.CaKhams.Any(c => c.idBS == id);
            if (hasReferences)
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản này vì nó đang được sử dụng.");
                return View(account); // Trả về view Delete với thông báo lỗi
            }

            db.Accounts.Remove(account);
            db.SaveChanges();
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
