using System.IO;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

public class AdminNewsController : Controller
{
    private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

    // GET: Danh sách tin tức
    public ActionResult Index()
    {
        var newsList = db.TinTucs.ToList();
        return View(newsList);
    }

    // GET: Tạo tin tức mới
    public ActionResult Create()
    {
        return View();
    }

    // POST: Tạo tin tức mới
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(TinTuc tinTuc, HttpPostedFileBase HinhAnh)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra và xử lý hình ảnh
            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                // Tạo thư mục Uploads nếu chưa có
                var uploadPath = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Lưu hình ảnh vào thư mục "Uploads"
                var fileName = Path.GetFileName(HinhAnh.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                HinhAnh.SaveAs(filePath);

                // Lưu đường dẫn hình ảnh vào cơ sở dữ liệu
                tinTuc.HinhAnh = "/Uploads/" + fileName;
            }

            db.TinTucs.Add(tinTuc);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Tạo tin tức thành công.";
            return RedirectToAction("Index");
        }
        return View(tinTuc);
    }

    // GET: Sửa tin tức
    public ActionResult Edit(int id)
    {
        var tinTuc = db.TinTucs.Find(id);
        if (tinTuc == null)
        {
            return HttpNotFound();
        }
        return View(tinTuc);
    }

    // POST: Sửa tin tức
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(TinTuc tinTuc, HttpPostedFileBase HinhAnh)
    {
        if (ModelState.IsValid)
        {
            var existingTinTuc = db.TinTucs.Find(tinTuc.idTinTuc); // Tìm lại đối tượng trong cơ sở dữ liệu

            if (existingTinTuc == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra và xử lý hình ảnh mới
            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                // Xóa hình ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingTinTuc.HinhAnh))
                {
                    var oldFilePath = Server.MapPath(existingTinTuc.HinhAnh);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Tạo thư mục Uploads nếu chưa có
                var uploadPath = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Lưu hình ảnh mới
                var fileName = Path.GetFileName(HinhAnh.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                HinhAnh.SaveAs(filePath);

                // Cập nhật đường dẫn hình ảnh mới
                existingTinTuc.HinhAnh = "/Uploads/" + fileName;
            }

            // Cập nhật các trường còn lại
            existingTinTuc.TieuDe = tinTuc.TieuDe;
            existingTinTuc.MoTa = tinTuc.MoTa;
            existingTinTuc.NoiDung = tinTuc.NoiDung;

            db.SaveChanges(); // Lưu thay đổi
            TempData["SuccessMessage"] = "Cập nhật tin tức thành công.";
            return RedirectToAction("Index");
        }
        return View(tinTuc);
    }



    // GET: Xóa tin tức
    public ActionResult Delete(int id)
    {
        var tinTuc = db.TinTucs.Find(id);
        if (tinTuc == null)
        {
            return HttpNotFound();
        }
        return View(tinTuc);
    }

    // POST: Xóa tin tức
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        var tinTuc = db.TinTucs.Find(id);
        if (tinTuc != null)
        {
            // Xóa hình ảnh khi xóa tin tức
            if (!string.IsNullOrEmpty(tinTuc.HinhAnh))
            {
                var filePath = Server.MapPath(tinTuc.HinhAnh);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            db.TinTucs.Remove(tinTuc);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Tin tức đã được xóa thành công.";
        }
        return RedirectToAction("Index");
    }
}
