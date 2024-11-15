using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerPersonalController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerPersonal
        // GET: CustomerPersonal
        public ActionResult Index()
        {
            int? accountId = Session["UserID"] as int?;
            if (!accountId.HasValue)
            {
                return HttpNotFound("Không tìm thấy thông tin tài khoản.");
            }

            var patient = db.BenhNhans.FirstOrDefault(p => p.idAccount == accountId.Value);
            if (patient == null)
            {
                return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
            }

            // Khởi tạo PersonalViewModel với thông tin bệnh nhân
            var viewModel = new PersonalViewModel
            {
                idBenhNhan = patient.idBenhNhan,
                tenBenhNhan = patient.tenBenhNhan,
                ngaySinh = patient.ngaySinh,
                SDT = patient.SDT,
                Email = patient.Email,
                GioiTinh = patient.GioiTinh,
                TienSuBenh = db.BenhNhan_TienSuBenh
                                .Where(bts => bts.idBenhNhan == patient.idBenhNhan)
                                .Select(bts => bts.TienSuBenh.tenTSB)
                                .ToList(),
                DiUng = db.BenhNhan_DiUng
                          .Where(bd => bd.idBenhNhan == patient.idBenhNhan)
                          .Select(bd => bd.DiUng.tenDiUng)
                          .ToList()
            };

            return View(viewModel); // Truyền PersonalViewModel vào View
        }

        // GET: CustomerPersonal/Edit
        public ActionResult Edit()
        {
            int? accountId = Session["UserID"] as int?;
            if (!accountId.HasValue)
            {
                return HttpNotFound("Không tìm thấy thông tin tài khoản.");
            }

            var patient = db.BenhNhans.FirstOrDefault(p => p.idAccount == accountId.Value);
            if (patient == null)
            {
                return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
            }

            var viewModel = new PersonalViewModel
            {
                idBenhNhan = patient.idBenhNhan,
                tenBenhNhan = patient.tenBenhNhan,
                ngaySinh = patient.ngaySinh,
                SDT = patient.SDT,
                Email = patient.Email,
                GioiTinh = patient.GioiTinh,
            };

            // Lấy danh sách tất cả các Tiền Sử Bệnh và Dị Ứng dưới dạng SelectListItem
            ViewBag.AllTienSuBenhs = db.TienSuBenhs
                .Select(tsb => new SelectListItem
                {
                    Value = tsb.idTienSuBenh.ToString(),
                    Text = tsb.tenTSB
                })
                .ToList();

            ViewBag.AllDiUngs = db.DiUngs
                .Select(di => new SelectListItem
                {
                    Value = di.idDiUng.ToString(),
                    Text = di.tenDiUng
                })
                .ToList();

            // Lấy danh sách Tiền Sử Bệnh và Dị Ứng đã chọn của bệnh nhân và gán vào ViewBag
            // Sử dụng ToList() để truyền các id đã chọn vào ViewBag
            ViewBag.SelectedTienSuBenhs = db.BenhNhan_TienSuBenh
                .Where(bts => bts.idBenhNhan == patient.idBenhNhan)
                .Select(bts => bts.idTienSuBenh)
                .ToList();

            ViewBag.SelectedDiUngs = db.BenhNhan_DiUng
                .Where(bd => bd.idBenhNhan == patient.idBenhNhan)
                .Select(bd => bd.idDiUng)
                .ToList();

            return View(viewModel);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var patient = db.BenhNhans.Find(model.idBenhNhan);
                if (patient == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin bệnh nhân.");
                }

                // Cập nhật thông tin bệnh nhân
                patient.tenBenhNhan = model.tenBenhNhan;
                patient.ngaySinh = model.ngaySinh;
                patient.SDT = model.SDT;
                patient.Email = model.Email;
                patient.GioiTinh = model.GioiTinh;

                // Xóa tất cả các bản ghi Tiền Sử Bệnh hiện tại của bệnh nhân trước khi thêm mới
                var currentTienSuBenhs = db.BenhNhan_TienSuBenh.Where(bts => bts.idBenhNhan == patient.idBenhNhan).ToList();
                db.BenhNhan_TienSuBenh.RemoveRange(currentTienSuBenhs);

                // Lưu thông tin Tiền Sử Bệnh mới
                if (Request["SelectedTienSuBenhs"] != null)
                {
                    var selectedTienSuBenhIds = Request["SelectedTienSuBenhs"].Split(',');
                    foreach (var id in selectedTienSuBenhIds)
                    {
                        int tienSuBenhId = int.Parse(id);
                        db.BenhNhan_TienSuBenh.Add(new BenhNhan_TienSuBenh
                        {
                            idBenhNhan = patient.idBenhNhan,
                            idTienSuBenh = tienSuBenhId
                        });
                    }

                    // Xử lý Tiền Sử Bệnh "Khác"
                    var tienSuBenhKhac = Request["tienSuBenhKhac"];
                    if (!string.IsNullOrEmpty(tienSuBenhKhac))
                    {
                        var tienSuBenhKhacRecord = db.TienSuBenhs.FirstOrDefault(tsb => tsb.tenTSB == "Khác");
                        if (tienSuBenhKhacRecord == null)
                        {
                            tienSuBenhKhacRecord = new TienSuBenh { tenTSB = "Khác" };
                            db.TienSuBenhs.Add(tienSuBenhKhacRecord);
                            db.SaveChanges();
                        }
                        db.BenhNhan_TienSuBenh.Add(new BenhNhan_TienSuBenh
                        {
                            idBenhNhan = patient.idBenhNhan,
                            idTienSuBenh = tienSuBenhKhacRecord.idTienSuBenh
                        });
                    }
                }

                // Xóa tất cả các bản ghi Dị Ứng hiện tại của bệnh nhân trước khi thêm mới
                var currentDiUngs = db.BenhNhan_DiUng.Where(bd => bd.idBenhNhan == patient.idBenhNhan).ToList();
                db.BenhNhan_DiUng.RemoveRange(currentDiUngs);

                // Lưu thông tin Dị Ứng mới
                if (Request["SelectedDiUngs"] != null)
                {
                    var selectedDiUngIds = Request["SelectedDiUngs"].Split(',');
                    foreach (var id in selectedDiUngIds)
                    {
                        int diUngId = int.Parse(id);
                        db.BenhNhan_DiUng.Add(new BenhNhan_DiUng
                        {
                            idBenhNhan = patient.idBenhNhan,
                            idDiUng = diUngId
                        });
                    }

                    // Xử lý Dị Ứng "Khác"
                    var diUngKhac = Request["diUngKhac"];
                    if (!string.IsNullOrEmpty(diUngKhac))
                    {
                        var diUngKhacRecord = db.DiUngs.FirstOrDefault(di => di.tenDiUng == "Khác");
                        if (diUngKhacRecord == null)
                        {
                            diUngKhacRecord = new DiUng { tenDiUng = "Khác" };
                            db.DiUngs.Add(diUngKhacRecord);
                            db.SaveChanges();
                        }
                        db.BenhNhan_DiUng.Add(new BenhNhan_DiUng
                        {
                            idBenhNhan = patient.idBenhNhan,
                            idDiUng = diUngKhacRecord.idDiUng
                        });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }







    }
}
