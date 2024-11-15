using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerLoginController : Controller
    {
        WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerLogin
        public ActionResult Index()
        {
            return View();
        }

        // POST: CustomerLogin/Authenticate
        [HttpPost]
        public ActionResult Authenticate(string username, string password)
        {
            // Kiểm tra tên đăng nhập và mật khẩu trong database
            var account = db.Accounts.FirstOrDefault(a => a.TaiKhoan == username && a.MatKhau == password);

            if (account != null)
            {
                // Đăng nhập thành công, lưu thông tin vào session
                Session["UserID"] = account.idAccount;
                Session["Username"] = account.TaiKhoan;

                // Kiểm tra vai trò của người dùng và chuyển hướng
                switch (account.Role)
                {
                    case "BenhNhan":
                        // Kiểm tra nếu có ReturnUrl trong TempData, chuyển hướng về đó
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        // Nếu không có ReturnUrl, chuyển hướng về trang chủ
                        return RedirectToAction("Index", "CustomerHome");

                    case "Admin":
                        return RedirectToAction("Index", "AdminDashboard");

                    case "BacSi":
                        // Chuyển hướng đến giao diện bác sĩ
                        return RedirectToAction("Index", "DoctorExaminationManagement");

                    default:
                        // Nếu không tìm thấy vai trò hợp lệ, có thể chuyển đến một trang mặc định hoặc thông báo lỗi
                        ViewBag.Error = "Vai trò người dùng không hợp lệ.";
                        return View("Index");
                }
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View("Index");
            }
        }


        // Action Đăng Xuất
        public ActionResult Logout()
        {
            // Xóa toàn bộ session
            Session.Clear();
            return RedirectToAction("Index", "CustomerHome");
        }
    }
}
