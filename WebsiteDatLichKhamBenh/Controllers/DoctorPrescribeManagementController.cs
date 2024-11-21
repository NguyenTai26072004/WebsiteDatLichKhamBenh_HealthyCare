    using System.Collections.Generic;
    using System.Web.Mvc;
    using System;
    using WebsiteDatLichKhamBenh.Models;
    using System.Linq;
    using System.Diagnostics;
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using iText.Kernel.Pdf.Canvas;
    using System.IO;
    using iText.Kernel.Font;
    using iText.IO.Font.Constants;



public class DoctorPrescribeManagementController : Controller
    {
        private readonly WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // Trang chính - danh sách lịch khám
        public ActionResult Index(string search)
        {
            var doctorId = Session["UserID"] != null ? int.Parse(Session["UserID"].ToString()) : 0;
            if (doctorId == 0)
            {
                return RedirectToAction("Index", "CustomerLogin");
            }

            var appointmentList = db.LichKhams
                .Where(l => l.MaCaKham.HasValue && l.CaKham.idBS == doctorId &&
                            (l.TrangThai == "Đã được duyệt" || l.TrangThai == "Đã khám"))
                .Select(l => new DoctorPrescriptionViewModel
                {
                    MaLichKham = l.MaLichKham,
                    NgayKham = l.CaKham.NgayKham,
                    KhungGio = l.CaKham.KhungGio.GioKham,
                    tenBenhNhan = l.BenhNhan.tenBenhNhan,
                    TrangThai = l.TrangThai,
                    CanPrescribe = l.TrangThai == "Đã khám",
                    HasPrescription = l.MaDonThuoc.HasValue
                })
                .OrderByDescending(l => l.NgayKham)
                .ToList();

            // Tìm kiếm theo mã lịch hẹn hoặc tên bệnh nhân
            if (!string.IsNullOrEmpty(search))
            {
                int searchIdInt;
                if (int.TryParse(search, out searchIdInt))
                {
                    appointmentList = appointmentList
                        .Where(a => a.MaLichKham == searchIdInt)
                        .ToList();
                }
                else
                {
                    appointmentList = appointmentList
                        .Where(a => a.tenBenhNhan.Contains(search))
                        .ToList();
                }
            }

            return View(appointmentList);
        }
        [HttpGet]
        public ActionResult KeThuoc(int maLichKham)
        {
            // Lấy thông tin lịch khám từ mã lịch khám
            var lichKham = db.LichKhams.FirstOrDefault(l => l.MaLichKham == maLichKham);

            // Kiểm tra nếu lịch khám không tồn tại hoặc đã có đơn thuốc
            if (lichKham == null)
            {
                return HttpNotFound("Không tìm thấy lịch khám.");
            }

            if (lichKham.MaDonThuoc.HasValue)
            {
                // Nếu đã có đơn thuốc, chuyển đến trang xem đơn thuốc
                return RedirectToAction("XemDonThuoc", new { maLichKham = maLichKham });
            }

            // Nếu chưa có đơn thuốc, hiển thị trang kê thuốc
            var model = new DoctorPrescriptionViewModel
            {
                MaLichKham = maLichKham,
                GhiChu = "", // Hoặc dữ liệu mặc định
                PrescriptionDetails = new List<DoctorPrescriptionDetailViewModel>(), // Danh sách chi tiết đơn thuốc rỗng ban đầu
                ThuocList = db.Thuocs.Select(t => new SelectListItem
                {
                    Value = t.MaThuoc.ToString(),
                    Text = t.TenThuoc
                }).ToList()
            };

            return View(model);
        }



        [HttpPost]
        public ActionResult KeThuoc(DoctorPrescriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var lichKham = db.LichKhams.FirstOrDefault(lk => lk.MaLichKham == model.MaLichKham);
                    if (lichKham == null)
                    {
                        return HttpNotFound("Không tìm thấy lịch khám.");
                    }

                    // Kiểm tra nếu đã có đơn thuốc
                    if (lichKham.MaDonThuoc.HasValue)
                    {
                        return RedirectToAction("XemDonThuoc", new { maLichKham = model.MaLichKham });
                    }

                    // Nếu chưa có đơn thuốc, tạo đơn thuốc mới
                    var donThuoc = new DonThuoc
                    {
                        GhiChu = model.GhiChu,
                        NgayKeDon = DateTime.Now
                    };
                    db.DonThuocs.Add(donThuoc);
                    db.SaveChanges();

                    // Lưu chi tiết đơn thuốc
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

                    // Cập nhật mã đơn thuốc vào lịch khám
                    lichKham.MaDonThuoc = donThuoc.MaDonThuoc;
                    db.Entry(lichKham).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    // Chuyển đến trang xem đơn thuốc sau khi tạo đơn mới
                    return RedirectToAction("XemDonThuoc", new { maLichKham = model.MaLichKham });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu đơn thuốc: " + ex.Message);
                }
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
            // Lấy thông tin lịch khám từ maLichKham
            var lichKham = db.LichKhams.Include("DonThuoc.ChiTietDonThuocs.Thuoc")
                .FirstOrDefault(l => l.MaLichKham == maLichKham);

            if (lichKham == null || lichKham.MaDonThuoc == null)
            {
                return HttpNotFound("Không tìm thấy đơn thuốc cho lịch khám này.");
            }

            var donThuoc = db.DonThuocs.Include("ChiTietDonThuocs.Thuoc")
                .FirstOrDefault(d => d.MaDonThuoc == lichKham.MaDonThuoc);

            if (donThuoc == null)
            {
                return HttpNotFound("Không tìm thấy thông tin đơn thuốc.");
            }

            var model = new DoctorPrescriptionViewModel
            {
                MaLichKham = lichKham.MaLichKham,
                GhiChu = donThuoc.GhiChu,
                PrescriptionDetails = donThuoc.ChiTietDonThuocs.Select(c => new DoctorPrescriptionDetailViewModel
                {
                    MaThuoc = c.MaThuoc ?? 0,
                    TenThuoc = c.Thuoc.TenThuoc,
                    LieuLuong = c.LieuLuong,
                    SoLuong = c.SoLuong ?? 0
                }).ToList()
            };

            return View(model);
        }


    public ActionResult InDonThuoc(int maLichKham)
    {
        var lichKham = db.LichKhams.Include("DonThuoc.ChiTietDonThuocs.Thuoc")
            .FirstOrDefault(l => l.MaLichKham == maLichKham);

        if (lichKham == null || lichKham.MaDonThuoc == null)
        {
            return HttpNotFound("Không tìm thấy đơn thuốc cho lịch khám này.");
        }

        var donThuoc = db.DonThuocs.Include("ChiTietDonThuocs.Thuoc")
            .FirstOrDefault(d => d.MaDonThuoc == lichKham.MaDonThuoc);

        if (donThuoc == null)
        {
            return HttpNotFound("Không tìm thấy thông tin đơn thuốc.");
        }

        var model = new DoctorPrescriptionViewModel
        {
            MaLichKham = lichKham.MaLichKham,
            GhiChu = donThuoc.GhiChu,
            PrescriptionDetails = donThuoc.ChiTietDonThuocs.Select(c => new DoctorPrescriptionDetailViewModel
            {
                MaThuoc = c.MaThuoc ?? 0,
                TenThuoc = c.Thuoc.TenThuoc,
                LieuLuong = c.LieuLuong,
                SoLuong = c.SoLuong ?? 0
            }).ToList()
        };

        using (MemoryStream ms = new MemoryStream())
        {
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Đường dẫn đến phông chữ TTF hỗ trợ tiếng Việt (ví dụ Arial hoặc Tahoma)
            var fontPath = Server.MapPath("~/Content/Fonts/arial.ttf");  // Điều chỉnh theo đúng đường dẫn đến phông chữ
            var font = PdfFontFactory.CreateFont(fontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            // Tạo phông chữ in đậm
            var fontBold = PdfFontFactory.CreateFont(fontPath, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            // Tiêu đề đơn thuốc
            document.Add(new Paragraph($"Đơn Thuốc - Mã Lịch Khám: {model.MaLichKham}")
                .SetFont(fontBold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            // Thêm dòng trống
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph($"Ghi chú: {model.GhiChu}")
                .SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.LEFT));

            // Thêm dòng trống
            document.Add(new Paragraph(" "));

            // Tạo bảng chi tiết đơn thuốc
            Table table = new Table(3);
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tên Thuốc").SetFont(fontBold).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Liều Lượng").SetFont(fontBold).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Số Lượng").SetFont(fontBold).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)));

            foreach (var detail in model.PrescriptionDetails)
            {
                table.AddCell(new Cell().Add(new Paragraph(detail.TenThuoc).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.LEFT)));
                table.AddCell(new Cell().Add(new Paragraph(detail.LieuLuong).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)));
                table.AddCell(new Cell().Add(new Paragraph(detail.SoLuong.ToString()).SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)));
            }

            // Thêm bảng vào tài liệu
            document.Add(table);

            // Thêm dòng trống
            document.Add(new Paragraph(" "));

            // Đóng document và xuất ra file PDF
            document.Close();

            // Trả về PDF dưới dạng file download
            byte[] fileBytes = ms.ToArray();
            return File(fileBytes, "application/pdf", "DonThuoc_" + model.MaLichKham + ".pdf");
        }
    }

}
