using System.Collections.Generic;
using System.Web.Mvc;
using System;
using WebsiteDatLichKhamBenh.Models;
using System.Linq;

public class DoctorPrescribeManagementController : Controller
{
    private readonly WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

    public ActionResult Index(string search)
    {
        // Kiểm tra nếu có giá trị UserId trong session
        var userId = Session["UserId"] != null ? (int?)Session["UserId"] : null;
        if (!userId.HasValue)
        {
            return RedirectToAction("Index", "CustomerLogin");
        }

        // Lấy tất cả các lịch hẹn đã được duyệt hoặc đã khám của bệnh nhân đối với bác sĩ này
        var appointmentList = db.LichKhams
            .Where(l =>
                l.CaKham.BacSi.idAccount.HasValue &&
                l.CaKham.BacSi.idAccount.Value == userId &&
                (l.TrangThai == "Đã khám" || l.TrangThai == "Đã được duyệt")) // Điều kiện
            .Select(l => new DoctorPrescriptionViewModel
            {
                MaLichKham = l.MaLichKham,
                NgayKham = l.CaKham.NgayKham,
                KhungGio = l.CaKham.KhungGio.GioKham,
                tenBenhNhan = l.BenhNhan.tenBenhNhan, // Lấy tên bệnh nhân
                TrangThai = l.TrangThai,
                CanPrescribe = l.TrangThai == "Đã khám",
                HasPrescription = l.MaDonThuoc.HasValue // Kiểm tra đã có đơn thuốc hay chưa
            }).AsQueryable();

        // Tìm kiếm theo mã lịch hẹn hoặc tên bệnh nhân
        if (!string.IsNullOrEmpty(search))
        {
            int searchIdInt;
            if (int.TryParse(search, out searchIdInt))
            {
                appointmentList = appointmentList.Where(a => a.MaLichKham == searchIdInt);
            }
            else
            {
                appointmentList = appointmentList.Where(a => a.tenBenhNhan.Contains(search));
            }
        }

        return View(appointmentList.ToList());
    }




    public ActionResult KeThuoc(int maLichKham)
    {
        var model = new DoctorPrescriptionViewModel
        {
            MaLichKham = maLichKham,
            ThuocList = db.Thuocs.Select(t => new SelectListItem
            {
                Value = t.MaThuoc.ToString(),
                Text = t.TenThuoc
            }).ToList(),
            PrescriptionDetails = new List<DoctorPrescriptionDetailViewModel>()
        };
        return View(model);
    }

    [HttpPost]
    public ActionResult SavePrescription(DoctorPrescriptionViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Lưu đơn thuốc vào bảng DonThuoc
            var donThuoc = new DonThuoc
            {
                GhiChu = model.GhiChu,
                NgayKeDon = DateTime.Now
            };

            db.DonThuocs.Add(donThuoc);
            db.SaveChanges();

            // Lưu chi tiết đơn thuốc vào bảng ChiTietDonThuoc
            foreach (var detail in model.PrescriptionDetails)
            {
                var donThuocChiTiet = new ChiTietDonThuoc
                {
                    MaDonThuoc = donThuoc.MaDonThuoc,
                    MaThuoc = detail.MaThuoc,
                    LieuLuong = detail.LieuLuong,
                    SoLuong = detail.SoLuong
                };
                db.ChiTietDonThuocs.Add(donThuocChiTiet);
            }

            db.SaveChanges();

            // Cập nhật bảng LichKham để liên kết đơn thuốc
            var lichKham = db.LichKhams.Find(model.MaLichKham);
            if (lichKham != null)
            {
                lichKham.MaDonThuoc = donThuoc.MaDonThuoc;  // Gán MaDonThuoc vào lịch khám
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        model.ThuocList = db.Thuocs.Select(t => new SelectListItem
        {
            Value = t.MaThuoc.ToString(),
            Text = t.TenThuoc
        }).ToList();

        return View("KeThuoc", model);
    }

    public ActionResult XemDonThuoc(int maLichKham)
    {
        var lichKham = db.LichKhams.Include("DonThuoc.ChiTietDonThuocs.Thuoc")
            .FirstOrDefault(l => l.MaLichKham == maLichKham);

        if (lichKham == null || lichKham.MaDonThuoc == null)
        {
            return HttpNotFound("Không tìm thấy đơn thuốc cho lịch khám này.");
        }

        var donThuoc = db.DonThuocs.Include("ChiTietDonThuocs.Thuoc")
            .Where(d => d.MaDonThuoc == lichKham.MaDonThuoc)
            .Select(d => new DoctorPrescriptionViewModel
            {
                MaLichKham = lichKham.MaLichKham,
                GhiChu = d.GhiChu,
                PrescriptionDetails = d.ChiTietDonThuocs.Select(c => new DoctorPrescriptionDetailViewModel
                {
                    MaThuoc = c.MaThuoc ?? 0,
                    TenThuoc = c.Thuoc.TenThuoc,
                    LieuLuong = c.LieuLuong,
                    SoLuong = c.SoLuong ?? 0
                }).ToList()
            })
            .FirstOrDefault();

        return View(donThuoc);
    }




}
