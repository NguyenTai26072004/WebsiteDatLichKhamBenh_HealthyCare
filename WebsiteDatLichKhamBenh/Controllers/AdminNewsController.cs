using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

public class AdminNewsController : Controller
{
    private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

    // GET: Danh sách tin tức
    public ActionResult Index()
    {
        var newsList = db.TinTucs.ToList(); // Lấy danh sách tin tức
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
    public ActionResult Create(TinTuc tinTuc)
    {
        if (ModelState.IsValid)
        {
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
    public ActionResult Edit(TinTuc tinTuc)
    {
        if (ModelState.IsValid)
        {
            db.Entry(tinTuc).State = EntityState.Modified;
            db.SaveChanges();
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
            db.TinTucs.Remove(tinTuc);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Tin tức đã được xóa thành công.";
        }
        return RedirectToAction("Index");
    }
}
