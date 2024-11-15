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
        public ActionResult Index(string searchTerm = "", string chuyenKhoa = "", int page = 1)
        {
            // Lấy danh sách bác sĩ từ cơ sở dữ liệu và lọc theo từ khóa tìm kiếm
            var allDoctors = string.IsNullOrWhiteSpace(searchTerm)
                ? db.BacSis.AsQueryable()
                : db.BacSis.Where(d => d.tenBS.Contains(searchTerm) || d.chuyenKhoa.Contains(searchTerm));

            // Lọc theo chuyên khoa nếu có
            if (!string.IsNullOrWhiteSpace(chuyenKhoa))
            {
                allDoctors = allDoctors.Where(d => d.chuyenKhoa == chuyenKhoa);
            }

            // Đảm bảo danh sách bác sĩ được sắp xếp trước khi gọi Skip và Take
            allDoctors = allDoctors.OrderBy(d => d.tenBS); // Thêm phần sắp xếp ở đây

            // Tính toán phân trang
            int totalDoctors = allDoctors.Count();
            int totalPages = (int)Math.Ceiling((double)totalDoctors / DoctorsPerPage);

            // Lấy danh sách bác sĩ của trang hiện tại
            var doctorsToDisplay = allDoctors
                .Skip((page - 1) * DoctorsPerPage)
                .Take(DoctorsPerPage)
                .ToList();

            // Đưa dữ liệu phân trang, từ khóa tìm kiếm, và chuyên khoa vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.ChuyenKhoa = chuyenKhoa;
            ViewBag.Doctors = doctorsToDisplay;

            // Lấy danh sách chuyên khoa để hiển thị trong select box
            ViewBag.Specialties = new List<string>
    {
        "Da liễu", "Tâm lý", "Phục hồi chức năng", "Nội soi tiêu hóa", "Nhi khoa", "Nội khoa", "Nội cơ xương khớp",
        "Tim mạch", "Hô hấp", "Thần kinh", "Tai mũi họng", "Nội tiết", "Y học cổ truyền", "Sản phụ khoa",
        "Nha khoa", "Răng hàm mặt", "Gây mê hồi sức", "Huyết học"
    };

            return View(doctorsToDisplay);
        }



    }
}
