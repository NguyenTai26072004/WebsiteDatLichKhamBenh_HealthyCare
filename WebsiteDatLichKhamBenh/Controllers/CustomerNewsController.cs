using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class CustomerNewsController : Controller
    {
        WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();

        // GET: CustomerNews
        public ActionResult Index()
        {
            var newsList = db.TinTucs.ToList();
            return View(newsList);
        }

        // GET: CustomerNews/Details/5
        public ActionResult Details(int id)
        {
            var news = db.TinTucs.Find(id);

            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);  // Trả về view với model là TinTuc (không cần lấy chi tiết riêng nữa)
        }
    }
}
