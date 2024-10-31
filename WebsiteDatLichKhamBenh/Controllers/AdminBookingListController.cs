using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminBookingListController : Controller
    {
        // GET: AdminBookingList
        public ActionResult Index()
        {
            var bookingList = new List<LichKhamViewModel>();

            using (var context = new WebDatLichKhamBenhDBEntities())
            {
                var bookings = from lk in context.LichKhams
                               join bn in context.BenhNhans on lk.MaBenhNhan equals bn.idBenhNhan
                               join ca in context.CaKhams on lk.MaCaKham equals ca.MaCaKham
                               join bs in context.BacSis on ca.idBS equals bs.idBS
                               select new LichKhamViewModel
                               {
                                   MaLichKham = lk.MaLichKham,
                                   TenBenhNhan = bn.tenBenhNhan,
                                   TenBacSi = bs.tenBS,
                                   NgayKham = ca.NgayKham.GetValueOrDefault(),
                                   GioKham = ca.KhungGio.GioKham,
                                   TrangThai = lk.TrangThai
                               };

                bookingList = bookings.ToList();
            }

            return View(bookingList);
        }

        // Thêm lịch hẹn (GET - Form)
        public ActionResult Create()
        {
            return View();
        }

        // Thêm lịch hẹn (POST - Lưu dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LichKham model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new WebDatLichKhamBenhDBEntities())
                {
                    context.LichKhams.Add(model);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Sửa lịch hẹn (GET - Form)
        public ActionResult Edit(int id)
        {
            using (var context = new WebDatLichKhamBenhDBEntities())
            {
                var booking = context.LichKhams.Find(id);
                if (booking == null)
                {
                    return HttpNotFound();
                }
                return View(booking);
            }
        }

        // Sửa lịch hẹn (POST - Cập nhật dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LichKham model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new WebDatLichKhamBenhDBEntities())
                {
                    context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Xóa lịch hẹn
        public ActionResult Delete(int id)
        {
            using (var context = new WebDatLichKhamBenhDBEntities())
            {
                var booking = context.LichKhams.Find(id);
                if (booking != null)
                {
                    context.LichKhams.Remove(booking);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
