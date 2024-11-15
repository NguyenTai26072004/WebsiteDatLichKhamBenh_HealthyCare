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
            TempData["SuccessMessage"] = "Tạo cơ sở thành công.";
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
            TempData["SuccessMessage"] = "Cập nhật cơ sở thành công.";
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
        // Tìm cơ sở theo id
        var coSo = db.CoSoes.Find(id);

        if (coSo == null)
        {
            return HttpNotFound();
        }

        // Kiểm tra xem có bác sĩ nào đang liên kết với cơ sở này không
        var relatedDoctors = db.BacSis.Any(b => b.idCoSo == id); // Kiểm tra có bác sĩ nào với MaCoSo này không

        if (relatedDoctors)
        {
            // Nếu có bác sĩ liên kết, không cho phép xóa và thông báo lỗi
            TempData["ErrorMessage"] = "Không thể xóa cơ sở vì có bác sĩ liên kết với cơ sở này.";
            return RedirectToAction("Index");
        }

        // Nếu không có bác sĩ liên kết, tiến hành xóa cơ sở
        db.CoSoes.Remove(coSo);
        db.SaveChanges();

        // Thông báo thành công
        TempData["SuccessMessage"] = "Cơ sở đã được xóa thành công.";
        return RedirectToAction("Index");
    }
}
