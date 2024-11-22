using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerHistoryController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerHistory
        public ActionResult Index()
        {
            // Kiểm tra nếu không có thông tin user trong session
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "CustomerLogin");
            }

            // Lấy idAccount từ session
            int accountId = (int)Session["UserID"];

            // Lấy idBenhNhan từ bảng BenhNhan dựa trên idAccount
            var benhNhan = db.BenhNhans.FirstOrDefault(b => b.idAccount == accountId);
            if (benhNhan == null)
            {
                // Nếu không tìm thấy bệnh nhân, chuyển hướng về trang login
                return RedirectToAction("Index", "CustomerLogin");
            }

            int benhNhanId = benhNhan.idBenhNhan;

            // Lấy danh sách lịch sử đặt lịch của bệnh nhân
            var patientHistory = db.LichKhams
                .Where(lk => lk.MaBenhNhan == benhNhanId) // Sử dụng idBenhNhan
                .Join(db.CaKhams, lk => lk.MaCaKham, ck => ck.MaCaKham, (lk, ck) => new { lk, ck })
                .Join(db.BacSis, combined => combined.ck.idBS, bs => bs.idBS, (combined, bs) => new { combined.lk, combined.ck, bs })
                .Join(db.KhungGios, combined => combined.ck.MaKhungGio, kg => kg.MaKhungGio, (combined, kg) => new { combined.lk, combined.ck, combined.bs, kg })
                .Join(db.CoSoes, combined => combined.bs.idCoSo, cs => cs.idCoSo, (combined, cs) => new
                {
                    combined.lk,
                    combined.ck,
                    combined.bs,
                    combined.kg,
                    DiaChi = cs.DiaChi
                })
                .ToList()
                .Select(combined => new AppointmentHistoryViewModel
                {
                    Id = combined.lk.MaLichKham,
                    NgayDatLich = combined.lk.NgayDatLich ?? DateTime.MinValue,
                    NgayKhamBenh = combined.ck.NgayKham ?? DateTime.MinValue,
                    GioDatLich = combined.lk.GioDatLich ?? TimeSpan.Zero,
                    GioKham = TimeSpan.TryParse(combined.kg.GioKham, out var gioKham) ? gioKham : TimeSpan.Zero,
                    BacSi = combined.bs.tenBS,
                    ChuyenKhoa = combined.bs.chuyenKhoa,
                    TrangThai = combined.lk.TrangThai, // Trạng thái chính xác của lịch khám
                    DiaChi = combined.DiaChi,
                    ReviewStatus = db.DanhGiaBacSis.Any(d => d.idBenhNhan == benhNhanId && d.idBacSi == combined.bs.idBS && d.ngayDanhGia != null)
                        ? "Đã đánh giá"
                        : "Chưa đánh giá"
                }).ToList();

            return View(patientHistory);
        }



        public ActionResult XemDonThuoc(int maLichKham)
        {
            // Tìm lịch khám và thông tin liên quan
            var lichKham = db.LichKhams.Include("DonThuoc.ChiTietDonThuocs.Thuoc")
                .FirstOrDefault(l => l.MaLichKham == maLichKham);

            // Kiểm tra nếu lịch khám không tồn tại
            if (lichKham == null)
            {
                TempData["ErrorMessage"] = "Lịch khám không tồn tại.";
                return RedirectToAction("Index");
            }


            // Kiểm tra nếu lịch khám chưa có đơn thuốc
            if (!lichKham.MaDonThuoc.HasValue)
            {
                ViewBag.ErrorMessage = "Chưa có đơn thuốc cho lịch khám này.";
                return View("ErrorPrescription");
            }

            // Tìm đơn thuốc dựa trên mã đơn thuốc của lịch khám
            var donThuoc = db.DonThuocs.Include("ChiTietDonThuocs.Thuoc")
                .FirstOrDefault(d => d.MaDonThuoc == lichKham.MaDonThuoc.Value);

            // Kiểm tra nếu đơn thuốc không tồn tại
            if (donThuoc == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy thông tin đơn thuốc.";
                return View("ErrorPrescription");
            }

            // Chuẩn bị dữ liệu cho ViewModel
            var model = new CustomerPrescribeViewModel
            {
                MaDonThuoc = donThuoc.MaDonThuoc,
                NgayKeDon = donThuoc.NgayKeDon,
                GhiChu = string.IsNullOrWhiteSpace(donThuoc.GhiChu) ? "Không có ghi chú." : donThuoc.GhiChu,
                CustomerPrescriptionDetails = donThuoc.ChiTietDonThuocs.Select(c => new CustomerPrescriptionDetail
                {
                    TenThuoc = c.Thuoc.TenThuoc,
                    LieuLuong = c.LieuLuong,
                    SoLuong = c.SoLuong ?? 0
                }).ToList()
            };

            return View(model);
        }


        public ActionResult DanhGiaBacSi(int maLichKham)
        {
            // Lấy thông tin lịch khám từ database
            var lichKham = db.LichKhams.Find(maLichKham);

            if (lichKham == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra trạng thái đánh giá
            var existingReview = db.DanhGiaBacSis
                .FirstOrDefault(d => d.idBenhNhan == lichKham.MaBenhNhan && d.idBacSi == lichKham.CaKham.idBS);

            if (existingReview != null)
            {
                // Nếu đã đánh giá bác sĩ rồi, chuyển hướng về trang lịch sử
                TempData["Message"] = "Bạn đã đánh giá bác sĩ này rồi!";
                return RedirectToAction("Index");
            }

            // Lấy thông tin CaKham từ bảng CaKham qua MaCaKham
            var caKham = db.CaKhams.Find(lichKham.MaCaKham);

            if (caKham == null)
            {
                return HttpNotFound();
            }

            // Lấy thông tin bác sĩ từ bảng BacSi qua idBS trong CaKham
            var bacSi = db.BacSis.Find(caKham.idBS);

            if (bacSi == null)
            {
                return HttpNotFound();
            }

            var model = new DanhGiaBacSiViewModel
            {
                MaLichKham = maLichKham,
                MaBacSi = bacSi.idBS,
                TenBacSi = bacSi.tenBS
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult LuuDanhGia(int maLichKham, int maBacSi, float diemDanhGia, string binhLuan)
        {
            var lichKham = db.LichKhams.Find(maLichKham);

            if (lichKham == null)
            {
                return HttpNotFound();
            }

            var maBenhNhan = lichKham.MaBenhNhan;

            // Tạo bản ghi đánh giá mới
            var danhGia = new DanhGiaBacSi
            {
                idBacSi = maBacSi,
                idBenhNhan = maBenhNhan,
                diemDanhGia = diemDanhGia,
                binhLuan = binhLuan,
                ngayDanhGia = DateTime.Now
            };

            db.DanhGiaBacSis.Add(danhGia);
            db.SaveChanges();

            // Tính toán lại điểm đánh giá trung bình cho bác sĩ
            var bacSi = db.BacSis.Find(maBacSi);
            if (bacSi != null)
            {
                var diemTrungBinh = (decimal?)db.DanhGiaBacSis
                                .Where(d => d.idBacSi == maBacSi)
                                .Average(d => d.diemDanhGia);

                bacSi.diemDanhGia = diemTrungBinh;
                db.SaveChanges();
            }

            // Lưu thông báo vào TempData
            TempData["Message"] = "Cảm ơn bạn đã đánh giá";

            return RedirectToAction("Index", "CustomerHistory");
        }


    }
}
