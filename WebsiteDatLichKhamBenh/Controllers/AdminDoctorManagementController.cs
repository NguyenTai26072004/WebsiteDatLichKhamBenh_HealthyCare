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
        public ActionResult Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var doctorsQuery = db.BacSis.AsQueryable();

            // Tìm kiếm theo ID hoặc tên bác sĩ
            if (!string.IsNullOrEmpty(searchTerm))
            {
                int id;
                if (int.TryParse(searchTerm, out id))
                {
                    // Nếu searchTerm là số, tìm theo ID
                    doctorsQuery = doctorsQuery.Where(d => d.idBS == id);
                }
                else
                {
                    // Tìm theo tên bác sĩ
                    doctorsQuery = doctorsQuery.Where(d => d.tenBS.Contains(searchTerm));
                }
            }

            // Thực hiện phân trang
            var doctors = doctorsQuery
                            .OrderBy(d => d.idBS)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.TotalCount = doctorsQuery.Count(); // Tổng số bác sĩ
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm; // Lưu từ khóa tìm kiếm

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BacSi doctor, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingDoctor = db.BacSis.Find(doctor.idBS);
                if (existingDoctor == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các trường không thay đổi
                existingDoctor.tenBS = doctor.tenBS;
                existingDoctor.chuyenKhoa = doctor.chuyenKhoa;
                existingDoctor.idAccount = doctor.idAccount; // Giữ lại id tài khoản
                existingDoctor.luotDat = doctor.luotDat; // Giữ lại lượt đặt
                existingDoctor.diemDanhGia = doctor.diemDanhGia; // Giữ lại điểm đánh giá

                // Nếu có file ảnh mới, tải lên Firebase
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var downloadUrl = await UploadImageToFirebase(imageFile);
                    existingDoctor.anhBS = downloadUrl; // Cập nhật URL ảnh mới
                }

                db.Entry(existingDoctor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }


        // GET: Xem chi tiết bác sĩ
        public ActionResult Details(int id)
        {
            var doctor = db.BacSis.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
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