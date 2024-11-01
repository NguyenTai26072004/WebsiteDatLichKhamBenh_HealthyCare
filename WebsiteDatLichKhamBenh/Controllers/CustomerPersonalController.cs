using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerPersonalController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerPersonal
        public ActionResult Index()
        {
            int? accountId = Session["UserID"] as int?; // Lấy ID người dùng dưới dạng int

            if (!accountId.HasValue)
            {
                return HttpNotFound("Không tìm thấy thông tin tài khoản.");
            }

            var patient = db.BenhNhans.FirstOrDefault(p => p.idAccount == accountId.Value); // Tìm bệnh nhân bằng idAccount kiểu int

            if (patient == null)
            {
                return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
            }

            return View(patient);
        }

        // GET: CustomerPersonal/Edit
        public ActionResult Edit()
        {
            int? accountId = Session["UserID"] as int?; // Lấy ID người dùng dưới dạng int

            if (!accountId.HasValue)
            {
                return HttpNotFound("Không tìm thấy thông tin tài khoản.");
            }

            var patient = db.BenhNhans.FirstOrDefault(p => p.idAccount == accountId.Value); // Tìm bệnh nhân bằng idAccount kiểu int

            if (patient == null)
            {
                return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
            }

            return View(patient);
        }

        // POST: CustomerPersonal/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BenhNhan model, string customTienSuBenh, string customDiUng)
        {
            if (ModelState.IsValid)
            {
                // Tìm bệnh nhân trong cơ sở dữ liệu
                var patient = db.BenhNhans.Find(model.idBenhNhan); // Tìm kiếm theo idBenhNhan kiểu int
                if (patient == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
                }

                // Cập nhật các thông tin bệnh nhân
                patient.tenBenhNhan = model.tenBenhNhan;
                patient.ngaySinh = model.ngaySinh;
                patient.SDT = model.SDT;
                patient.Email = model.Email;
                patient.GioiTinh = model.GioiTinh;

                // Kiểm tra và cập nhật giá trị của "Tiền sử bệnh án" và "Dị ứng"
                // Nếu người dùng chọn "Khác..." và nhập dữ liệu vào ô bổ sung, dùng dữ liệu nhập
                if (model.tienSuBenh == "Khác..." && !string.IsNullOrEmpty(customTienSuBenh))
                {
                    patient.tienSuBenh = customTienSuBenh;
                }
                else
                {
                    patient.tienSuBenh = model.tienSuBenh;
                }

                if (model.DiUng == "Khác..." && !string.IsNullOrEmpty(customDiUng))
                {
                    patient.DiUng = customDiUng;
                }
                else
                {
                    patient.DiUng = model.DiUng;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                // Chuyển hướng về trang chỉ định sau khi lưu thành công
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, trả về lại model để hiển thị lỗi
            return View(model);
        }
    }
}
