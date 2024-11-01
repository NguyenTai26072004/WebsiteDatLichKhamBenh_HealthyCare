using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;


public class AdminFacilityManagementController : Controller
{
    private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

    // GET: Danh sách cơ sở
    public ActionResult Index()
    {
        var coSoList = db.CoSoes.ToList(); // Lấy danh sách cơ sở
        return View(coSoList);
    }

    // GET: Tạo cơ sở mới
    public ActionResult Create()
    {
        return View();
    }

    // POST: Tạo cơ sở mới
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CoSo coSo)
    {
        if (ModelState.IsValid)
        {
            db.CoSoes.Add(coSo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(coSo);
    }

    // GET: Sửa cơ sở
    public ActionResult Edit(int id)
    {
        var coSo = db.CoSoes.Find(id);
        if (coSo == null)
        {
            return HttpNotFound();
        }
        return View(coSo);
    }

    // POST: Sửa cơ sở
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(CoSo coSo)
    {
        if (ModelState.IsValid)
        {
            db.Entry(coSo).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(coSo);
    }

    // GET: Xóa cơ sở
    public ActionResult Delete(int id)
    {
        var coSo = db.CoSoes.Find(id);
        if (coSo == null)
        {
            return HttpNotFound();
        }
        return View(coSo);
    }

    // POST: Xóa cơ sở
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        var coSo = db.CoSoes.Find(id);
        db.CoSoes.Remove(coSo);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
}
