using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers.Customer
{
    public class CustomerFindDoctorController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();
        private const int DoctorsPerPage = 12; // Số bác sĩ trên mỗi trang

        // GET: Booking
        public ActionResult Index(string searchTerm = "", int page = 1)
        {
            // Lấy danh sách bác sĩ từ cơ sở dữ liệu và lọc theo từ khóa nếu có
            var allDoctors = string.IsNullOrWhiteSpace(searchTerm)
                ? db.BacSis.ToList()
                : db.BacSis.Where(d => d.tenBS.Contains(searchTerm) || d.chuyenKhoa.Contains(searchTerm)).ToList();

            // Tính toán phân trang
            int totalDoctors = allDoctors.Count;
            int totalPages = (int)Math.Ceiling((double)totalDoctors / DoctorsPerPage);

            // Lấy danh sách bác sĩ của trang hiện tại
            var doctorsToDisplay = allDoctors
                .Skip((page - 1) * DoctorsPerPage)
                .Take(DoctorsPerPage)
                .ToList();

            // Đưa dữ liệu phân trang và từ khóa vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Doctors = doctorsToDisplay;

            return View(doctorsToDisplay);
        }
    }
}
