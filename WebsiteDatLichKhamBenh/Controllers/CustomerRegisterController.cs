using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerRegisterController : Controller
    {
        private readonly WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                // Trả về view với các thông báo lỗi
                return View("Index", model);
            }

            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            var existingAccount = db.Accounts.FirstOrDefault(a => a.TaiKhoan == model.Username);
            if (existingAccount != null)
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
                return View("Index", model);
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingEmail = db.BenhNhans.FirstOrDefault(b => b.Email == model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được đăng ký. Vui lòng chọn email khác.");
                return View("Index", model);
            }

            // Kiểm tra xem số điện thoại đã tồn tại chưa
            var existingPhone = db.BenhNhans.FirstOrDefault(b => b.SDT == model.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được đăng ký. Vui lòng chọn số điện thoại khác.");
                return View("Index", model);
            }

            // Nếu không có lỗi, tạo tài khoản mới
            var newAccount = new Account
            {
                TaiKhoan = model.Username,
                MatKhau = model.Password,
                Role = "BenhNhan"
            };

            try
            {
                db.Accounts.Add(newAccount);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu tài khoản: " + ex.Message);
                return View("Index", model);
            }

            // Lấy ID của tài khoản vừa thêm
            var accountId = newAccount.idAccount;

            // Tạo đối tượng BenhNhan, sử dụng trường FullName từ RegisterViewModel
            var newBenhNhan = new BenhNhan
            {
                tenBenhNhan = model.FullName,
                Email = model.Email,
                SDT = model.Phone,
                idAccount = accountId,
                ngaySinh = model.BirthDate,
                GioiTinh = model.Gender
            };

            try
            {
                db.BenhNhans.Add(newBenhNhan);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu thông tin bệnh nhân: " + ex.Message);
                return View("Index", model);
            }

            // Lưu thông báo thành công vào TempData
            TempData["MessageSuccess"] = "Bạn đã đăng ký tài khoản thành công!";

            // Sau khi đăng ký thành công, chuyển hướng đến trang đăng nhập
            return RedirectToAction("Index", "CustomerLogin");
        }



    }
}
