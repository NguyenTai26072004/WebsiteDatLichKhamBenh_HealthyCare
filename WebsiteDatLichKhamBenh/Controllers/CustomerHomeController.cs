using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerHomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // Action kiểm tra đăng nhập trước khi xem thông tin sức khỏe
        public ActionResult PersonalInfo()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["Username"] == null)
            {
                // Lưu thông báo trong TempData để hiện thông báo sau khi chuyển hướng
                TempData["Message"] = "Bạn cần đăng nhập để xem Thông tin sức khỏe";
                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("Index", "CustomerLogin");
            }
            // Nếu đã đăng nhập, chuyển hướng đến trang Thông Tin Sức Khỏe
            return RedirectToAction("Index", "CustomerPersonal");
        }

       
    }
}
