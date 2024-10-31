using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminExamenController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: AdminExamen
        public ActionResult Index()
        {
            var caKhams = db.CaKhams.Include(c => c.BacSi).Include(c => c.CoSo).Include(c => c.KhungGio);
            return View(caKhams.ToList());
        }

        // GET: AdminExamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaKham caKham = db.CaKhams.Find(id);
            if (caKham == null)
            {
                return HttpNotFound();
            }
            return View(caKham);
        }

        // GET: AdminExamen/Create
        public ActionResult Create()
        {
            ViewBag.idBS = new SelectList(db.BacSis, "idBS", "tenBS");
            ViewBag.idCoSo = new SelectList(db.CoSoes, "idCoSo", "DiaChi");
            ViewBag.MaKhungGio = new SelectList(db.KhungGios, "MaKhungGio", "GioKham");
            return View();
        }

        // POST: AdminExamen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCaKham,idBS,NgayKham,MaKhungGio,TrangThai,idCoSo")] CaKham caKham)
        {
            if (ModelState.IsValid)
            {
                db.CaKhams.Add(caKham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idBS = new SelectList(db.BacSis, "idBS", "tenBS", caKham.idBS);
            ViewBag.idCoSo = new SelectList(db.CoSoes, "idCoSo", "DiaChi", caKham.idCoSo);
            ViewBag.MaKhungGio = new SelectList(db.KhungGios, "MaKhungGio", "GioKham", caKham.MaKhungGio);
            return View(caKham);
        }

        // GET: AdminExamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaKham caKham = db.CaKhams.Find(id);
            if (caKham == null)
            {
                return HttpNotFound();
            }
            ViewBag.idBS = new SelectList(db.BacSis, "idBS", "tenBS", caKham.idBS);
            ViewBag.idCoSo = new SelectList(db.CoSoes, "idCoSo", "DiaChi", caKham.idCoSo);
            ViewBag.MaKhungGio = new SelectList(db.KhungGios, "MaKhungGio", "GioKham", caKham.MaKhungGio);
            return View(caKham);
        }

        // POST: AdminExamen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCaKham,idBS,NgayKham,MaKhungGio,TrangThai,idCoSo")] CaKham caKham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caKham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idBS = new SelectList(db.BacSis, "idBS", "tenBS", caKham.idBS);
            ViewBag.idCoSo = new SelectList(db.CoSoes, "idCoSo", "DiaChi", caKham.idCoSo);
            ViewBag.MaKhungGio = new SelectList(db.KhungGios, "MaKhungGio", "GioKham", caKham.MaKhungGio);
            return View(caKham);
        }

        // GET: AdminExamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaKham caKham = db.CaKhams.Find(id);
            if (caKham == null)
            {
                return HttpNotFound();
            }
            return View(caKham);
        }

        // POST: AdminExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CaKham caKham = db.CaKhams.Find(id);
            db.CaKhams.Remove(caKham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
