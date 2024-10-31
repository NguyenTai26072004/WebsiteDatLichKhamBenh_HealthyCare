using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebsiteDatLichKhamBenh.Models;

namespace WebsiteDatLichKhamBenh.Controllers
{
    public class AdminDoctorManagementController : Controller
    {
        private WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();
        private readonly string _firebaseBucket = "websitedatlichkhambenh.appspot.com";

        // GET: AdminDoctorManagement
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var doctors = db.BacSis
                            .OrderBy(d => d.idBS)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.TotalCount = db.BacSis.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(doctors);
        }

        // GET: Tạo tài khoản bác sĩ
        public ActionResult CreateAccount()
        {
            return View();
        }

        // POST: Tạo tài khoản bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAccount(string taiKhoan, string matKhau)
        {
            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    TaiKhoan = taiKhoan,
                    MatKhau = matKhau, // Mã hóa mật khẩu trước khi lưu
                    Role = "BacSi" // Vai trò bác sĩ
                };
                db.Accounts.Add(account);
                db.SaveChanges();

                // Chuyển hướng đến trang thêm bác sĩ
                return RedirectToAction("CreateDoctor", new { accountId = account.idAccount });
            }
            return View();
        }

        // GET: Thêm bác sĩ
        public ActionResult CreateDoctor(int accountId)
        {
            var model = new BacSi { idAccount = accountId }; // Liên kết với tài khoản
            return View(model);
        }

        // POST: Thêm bác sĩ mới và tải hình ảnh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDoctor(BacSi doctor, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Nếu có file ảnh thì tải lên Firebase
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var downloadUrl = await UploadImageToFirebase(imageFile);
                    doctor.anhBS = downloadUrl; // Lưu URL hình ảnh từ Firebase vào thông tin bác sĩ
                }

                db.BacSis.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // Phương thức tải ảnh lên Firebase
        private async Task<string> UploadImageToFirebase(HttpPostedFileBase imageFile)
        {
            var serviceAccountKeyPath = Server.MapPath("~/firebase-service-account.json");

            if (!System.IO.File.Exists(serviceAccountKeyPath))
                throw new FileNotFoundException("Không tìm thấy file JSON của Service Account tại: " + serviceAccountKeyPath);

            var auth = GoogleCredential.FromFile(serviceAccountKeyPath)
                                       .CreateScoped(new[] { "https://www.googleapis.com/auth/firebase.storage" });

            var firebaseStorage = new FirebaseStorage(
                _firebaseBucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = async () => await auth.UnderlyingCredential.GetAccessTokenForRequestAsync(),
                    ThrowOnCancel = true
                });

            var stream = imageFile.InputStream;
            var upload = await firebaseStorage
                .Child("images")
                .Child(imageFile.FileName)
                .PutAsync(stream);

            return await firebaseStorage
                .Child("images")
                .Child(imageFile.FileName)
                .GetDownloadUrlAsync();
        }

        // GET: Sửa bác sĩ
        public ActionResult Edit(int id)
        {
            var doctor = db.BacSis.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Sửa bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BacSi doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: Xóa bác sĩ
        public ActionResult Delete(int id)
        {
            var doctor = db.BacSis.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Xóa bác sĩ
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var doctor = db.BacSis.Find(id);
            db.BacSis.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
