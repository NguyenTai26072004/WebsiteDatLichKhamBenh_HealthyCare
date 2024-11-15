using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class DoctorExaminationManagementController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: DoctorExaminationManagement
        public ActionResult Index(string searchId, int page = 1)
        {
            // Lấy thông tin bác sĩ từ session
            var doctorId = Session["UserID"] != null ? int.Parse(Session["UserID"].ToString()) : 0;

            if (doctorId == 0)
            {
                return RedirectToAction("Index", "CustomerLogin");
            }

            // Lấy tất cả các ca khám của bác sĩ
            var caKhams = db.CaKhams.Where(c => c.idBS == doctorId).AsQueryable();

            // Nếu có searchId, thực hiện tìm kiếm theo ID
            if (!string.IsNullOrEmpty(searchId))
            {
                int searchIdInt;
                if (int.TryParse(searchId, out searchIdInt))
                {
                    caKhams = caKhams.Where(c => c.MaCaKham == searchIdInt);
                }
            }

            // Phân trang (10 ca khám mỗi trang)
            int pageSize = 10;
            int totalRecords = caKhams.Count();
            var pagedCaKhams = caKhams.OrderByDescending(c => c.MaCaKham)  // Sắp xếp theo mã ca khám
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();

            // Tạo thông tin phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)totalRecords / pageSize);

            return View(pagedCaKhams);
        }


        // GET: DoctorExaminationManagement/Create
        public ActionResult Create()
        {
            // Lấy danh sách khung giờ từ bảng KhungGio
            var khungGioList = db.KhungGios.ToList();
            ViewBag.KhungGioList = new SelectList(khungGioList, "MaKhungGio", "GioKham");

            return View();
        }

        // POST: DoctorExaminationManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CaKham model)
        {
            if (ModelState.IsValid)
            {
                // Gắn thông tin bác sĩ vào ca khám từ session
                model.idBS = int.Parse(Session["UserID"].ToString()); // Lấy thông tin bác sĩ từ session
                model.TrangThai = "Đang chờ duyệt"; // Trạng thái mặc định khi tạo ca khám

                db.CaKhams.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu có lỗi, trả lại view với danh sách khung giờ để bác sĩ chọn
            var khungGioList = db.KhungGios.ToList();
            ViewBag.KhungGioList = new SelectList(khungGioList, "MaKhungGio", "GioKham");

            return View(model);
        }

        // GET: DoctorExaminationManagement/Edit/5
        public ActionResult Edit(int id)
        {
            var caKham = db.CaKhams.Find(id);
            if (caKham == null)
            {
                return HttpNotFound();
            }

            var khungGioList = db.KhungGios.ToList();
            ViewBag.KhungGioList = new SelectList(khungGioList, "MaKhungGio", "GioKham", caKham.MaKhungGio);
            ViewBag.NgayKham = caKham.NgayKham;


            return View(caKham);
        }

        // POST: DoctorExaminationManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CaKham model)
        {
            if (ModelState.IsValid)
            {
                var caKham = db.CaKhams.Find(id);
                if (caKham == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật giá trị Ngày Khám
                caKham.MaKhungGio = model.MaKhungGio;
                caKham.NgayKham = model.NgayKham; // Lưu lại Ngày Khám

                caKham.TrangThai = model.TrangThai;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var khungGioList = db.KhungGios.ToList();
            ViewBag.KhungGioList = new SelectList(khungGioList, "MaKhungGio", "GioKham", model.MaKhungGio);

            return View(model);
        }

    }
}
