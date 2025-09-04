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
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;
using iText.IO.Font;



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
        // Retrieve the appointment with related prescription data
        var lichKham = db.LichKhams
            .Include("BenhNhan")
            .Include("CaKham.KhungGio")
            .Include("CaKham.BacSi")
            .Include("DonThuoc.ChiTietDonThuocs.Thuoc")
            .FirstOrDefault(l => l.MaLichKham == maLichKham);

        if (lichKham == null || lichKham.MaDonThuoc == null)
        {
            return HttpNotFound("Không tìm thấy đơn thuốc cho lịch khám này.");
        }

        var donThuoc = db.DonThuocs
            .Include("ChiTietDonThuocs.Thuoc")
            .FirstOrDefault(d => d.MaDonThuoc == lichKham.MaDonThuoc);

        if (donThuoc == null)
        {
            return HttpNotFound("Không tìm thấy thông tin đơn thuốc.");
        }

        // Get doctor information
        var bacSi = db.BacSis.FirstOrDefault(b => b.idBS == lichKham.CaKham.idBS);

        using (MemoryStream ms = new MemoryStream())
        {
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Set up Vietnamese font support
            var fontPath = Server.MapPath("~/Content/Fonts/arial.ttf");
            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
            var fontBold = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            // Add hospital/clinic header
            document.Add(new Paragraph("CÔNG TY CỔ PHẦN DỊCH VỤ SỨC KHOẺ HEALTHYLIFE")
                .SetFont(fontBold).SetFontSize(16).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("ĐỊA CHỈ: 123 Đường Khám Bệnh, Quận 1, TP HCM")
                .SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("ĐIỆN THOẠI: 028.1234.5678")
                .SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER));

            // Add divider
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            // Title
            document.Add(new Paragraph("ĐƠN THUỐC")
                .SetFont(fontBold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(15).SetMarginBottom(15));

            // Patient information
            Table patientTable = new Table(2).UseAllAvailableWidth();
            patientTable.AddCell(new Cell().Add(new Paragraph("Họ và tên bệnh nhân:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.BenhNhan.tenBenhNhan).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            // Add patient ID if available
            patientTable.AddCell(new Cell().Add(new Paragraph("Mã bệnh nhân:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.MaBenhNhan.ToString()).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            // Add patient age/DOB if available
            if (lichKham.BenhNhan.ngaySinh.HasValue)
            {
                int age = DateTime.Now.Year - lichKham.BenhNhan.ngaySinh.Value.Year;
                patientTable.AddCell(new Cell().Add(new Paragraph("Năm sinh:").SetFont(fontBold).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.BenhNhan.ngaySinh.Value.Year.ToString() + " (" + age + " tuổi)").SetFont(font).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
            }

            // Add patient contact info if available
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.SDT))
            {
                patientTable.AddCell(new Cell().Add(new Paragraph("Số điện thoại:").SetFont(fontBold).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.BenhNhan.SDT).SetFont(font).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
            }


            if (!string.IsNullOrEmpty(lichKham.BenhNhan.GioiTinh))
            {
                patientTable.AddCell(new Cell().Add(new Paragraph("Giới tính:").SetFont(fontBold).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
                patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.BenhNhan.GioiTinh).SetFont(font).SetFontSize(12))
                    .SetBorder(Border.NO_BORDER));
            }

            // Prescription details
            patientTable.AddCell(new Cell().Add(new Paragraph("Mã lịch khám:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.MaLichKham.ToString()).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            patientTable.AddCell(new Cell().Add(new Paragraph("Ngày khám:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.CaKham.NgayKham.ToString()).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            patientTable.AddCell(new Cell().Add(new Paragraph("Khung giờ:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(lichKham.CaKham.KhungGio.GioKham).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            patientTable.AddCell(new Cell().Add(new Paragraph("Ngày kê đơn:").SetFont(fontBold).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));
            patientTable.AddCell(new Cell().Add(new Paragraph(donThuoc.NgayKeDon.ToString()).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER));

            document.Add(patientTable);

            // Add prescription diagnosis/notes if available
            if (!string.IsNullOrEmpty(donThuoc.GhiChu))
            {
                document.Add(new Paragraph("Chẩn đoán / Ghi chú:").SetFont(fontBold).SetFontSize(12).SetMarginTop(10));
                document.Add(new Paragraph(donThuoc.GhiChu).SetFont(font).SetFontSize(12));
            }

            // Add medications title
            document.Add(new Paragraph("CHI TIẾT ĐƠN THUỐC").SetFont(fontBold).SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER).SetMarginTop(15).SetMarginBottom(10));

            // Create medication table
            Table medicationTable = new Table(4).UseAllAvailableWidth();

            // Add header cells
            medicationTable.AddHeaderCell(new Cell().Add(new Paragraph("STT").SetFont(fontBold).SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)));
            medicationTable.AddHeaderCell(new Cell().Add(new Paragraph("Tên thuốc").SetFont(fontBold).SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)));
            medicationTable.AddHeaderCell(new Cell().Add(new Paragraph("Liều lượng / Cách dùng").SetFont(fontBold).SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)));
            medicationTable.AddHeaderCell(new Cell().Add(new Paragraph("Số lượng").SetFont(fontBold).SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)));

            // Add medication rows
            int stt = 1;
            foreach (var detail in donThuoc.ChiTietDonThuocs)
            {
                medicationTable.AddCell(new Cell().Add(new Paragraph(stt.ToString()).SetFont(font).SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)));
                medicationTable.AddCell(new Cell().Add(new Paragraph(detail.Thuoc.TenThuoc).SetFont(font).SetFontSize(12)));
                medicationTable.AddCell(new Cell().Add(new Paragraph(detail.LieuLuong).SetFont(font).SetFontSize(12)));
                medicationTable.AddCell(new Cell().Add(new Paragraph(detail.SoLuong.ToString()).SetFont(font).SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)));
                stt++;
            }

            document.Add(medicationTable);

            // Add additional instructions if needed
            document.Add(new Paragraph("Lưu ý:").SetFont(fontBold).SetFontSize(12).SetMarginTop(15));
            document.Add(new Paragraph("- Uống thuốc đúng theo liều lượng được kê.")
                .SetFont(font).SetFontSize(12));
            document.Add(new Paragraph("- Thông báo cho bác sĩ nếu có bất kỳ phản ứng phụ nào.")
                .SetFont(font).SetFontSize(12));
            document.Add(new Paragraph("- Giữ đơn thuốc này để tham khảo trong tương lai.")
                .SetFont(font).SetFontSize(12));

            // Add doctor signature section
            document.Add(new Paragraph(" ").SetMarginTop(30));
            Table signatureTable = new Table(2).UseAllAvailableWidth();

            signatureTable.AddCell(new Cell().Add(new Paragraph("Bệnh nhân\n(Ký và ghi rõ họ tên)")
                .SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));

            string doctorName = bacSi != null ? bacSi.tenBS : "Bác sĩ";
            string doctorTitle = bacSi != null && !string.IsNullOrEmpty(bacSi.chuyenKhoa) ?
                "BS. Chuyên khoa " + bacSi.chuyenKhoa : "Bác sĩ";

            signatureTable.AddCell(new Cell().Add(new Paragraph("Bác sĩ kê đơn\n(Ký và đóng dấu)\n\n\n\n" + doctorName + "\n" + doctorTitle)
                .SetFont(font).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));

            document.Add(signatureTable);

            // Close document
            document.Close();

            // Return as downloadable PDF
            byte[] fileBytes = ms.ToArray();
            return File(fileBytes, "application/pdf", "DonThuoc_" + lichKham.MaLichKham + ".pdf");
        }
    }

}
