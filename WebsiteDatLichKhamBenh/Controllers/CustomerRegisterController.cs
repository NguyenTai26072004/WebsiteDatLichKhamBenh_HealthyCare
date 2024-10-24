using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerRegisterController : Controller
    {
        // Kết nối tới database
        private readonly WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerRegister
        public ActionResult Index()
        {
            return View();
        }

        // POST: Đăng ký tài khoản
        [HttpPost]
        public ActionResult Register(string username, string password, string confirmPassword, string email, string phone, DateTime birthDate, string gender)
        {
            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp nhau không
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu và nhập lại mật khẩu không khớp.";
                return View("Index");
            }

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            var existingAccount = db.Accounts.FirstOrDefault(a => a.TaiKhoan == username);
            if (existingAccount != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.";
                return View("Index");
            }

            // Tạo mới đối tượng Account
            var newAccount = new Account
            {
                TaiKhoan = username,
                MatKhau = password, // Lưu mật khẩu gốc
                Role = "BenhNhan"  // Mặc định vai trò của tài khoản là Bệnh nhân
            };

            try
            {
                // Lưu thông tin Account vào database
                db.Accounts.Add(newAccount);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi khi lưu tài khoản: " + ex.Message;
                return View("Index");
            }

            // Sau khi thêm Account, cần lấy idAccount để thêm vào bảng BenhNhan
            var accountId = newAccount.idAccount;

            // Tạo mới đối tượng BenhNhan
            var newBenhNhan = new BenhNhan
            {
                tenBenhNhan = username,
                Email = email,
                SDT = phone,
                idAccount = accountId,
                ngaySinh = birthDate,
                GioiTinh = gender
            };

            try
            {
                // Lưu thông tin BenhNhan vào database
                db.BenhNhans.Add(newBenhNhan);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đã xảy ra lỗi khi lưu thông tin bệnh nhân: " + ex.Message;
                return View("Index");
            }

            // Sau khi đăng ký thành công, chuyển hướng đến trang đăng nhập hoặc trang chủ
            return RedirectToAction("Index", "CustomerLogin");
        }
    }
}
