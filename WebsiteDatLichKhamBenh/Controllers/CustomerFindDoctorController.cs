using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers.Customer
{
    public class CustomerFindDoctorController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();
        // GET: Booking
        public ActionResult Index()
        {
            var doctors = db.BacSis.ToList();
            return View(doctors);
        }
    }
}