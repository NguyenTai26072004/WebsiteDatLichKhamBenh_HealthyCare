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
        private readonly WebDatLichKhamBenhDBEntities db = new WebDatLichKhamBenhDBEntities();
        private readonly string _firebaseBucket = "websitedatlichkhambenh.appspot.com";

        // GET: AdminDoctorManagement
        public ActionResult Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var doctorsQuery = db.BacSis.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int id;
                if (int.TryParse(searchTerm, out id))
                {
                    doctorsQuery = doctorsQuery.Where(d => d.idBS == id);
                }
                else
                {
                    doctorsQuery = doctorsQuery.Where(d => d.tenBS.Contains(searchTerm));
                }
            }

            var doctors = doctorsQuery.OrderBy(d => d.idBS)
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();

            ViewBag.TotalCount = doctorsQuery.Count();
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;

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
            var model = new BacSi { idAccount = accountId };
            // Lấy danh sách cơ sở từ bảng CoSo
            ViewBag.CoSoList = new SelectList(db.CoSoes.ToList(), "idCoSo", "tenBenhVien");
            return View(model);
        }

        // POST: Thêm bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDoctor(BacSi doctor, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                doctor.luotDat = 0;
                // Nếu có file ảnh thì tải lên Firebase
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var downloadUrl = await UploadImageToFirebase(imageFile);
                    doctor.anhBS = downloadUrl; // Lưu URL hình ảnh từ Firebase vào thông tin bác sĩ
                }

                db.BacSis.Add(doctor);
                db.SaveChanges();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm bác sĩ thành công!";
                return RedirectToAction("Index");
            }
            // Trường hợp lỗi, hiển thị lại danh sách cơ sở
            ViewBag.CoSoList = new SelectList(db.CoSoes.ToList(), "idCoSo", "tenCoSo");
            return View(doctor);
        }
     


        // Phương thức tải ảnh lên Firebase
        private async Task<string> UploadImageToFirebase(HttpPostedFileBase imageFile)
        {
            var serviceAccountKeyPath = Server.MapPath("~/firebase-service-account.json");
            if (!System.IO.File.Exists(serviceAccountKeyPath))
                throw new FileNotFoundException("Không tìm thấy file JSON tại: " + serviceAccountKeyPath);

            var auth = GoogleCredential.FromFile(serviceAccountKeyPath)
                                       .CreateScoped("https://www.googleapis.com/auth/firebase.storage");

            var firebaseStorage = new FirebaseStorage(
                _firebaseBucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = async () => await auth.UnderlyingCredential.GetAccessTokenForRequestAsync(),
                    ThrowOnCancel = true
                });

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var stream = imageFile.InputStream;

            await firebaseStorage.Child("images").Child(fileName).PutAsync(stream);
            return await firebaseStorage.Child("images").Child(fileName).GetDownloadUrlAsync();
        }

        // GET: Sửa bác sĩ
        public ActionResult Edit(int id)
        {
            var doctor = db.BacSis.Find(id);
            if (doctor == null) return HttpNotFound();

            ViewBag.CoSoList = new SelectList(db.CoSoes.ToList(), "idCoSo", "tenBenhVien", doctor.idCoSo);
            return View(doctor);
        }

        // POST: Sửa bác sĩ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BacSi doctor, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingDoctor = db.BacSis.Find(doctor.idBS);
                if (existingDoctor == null) return HttpNotFound();

                existingDoctor.tenBS = doctor.tenBS;
                existingDoctor.chuyenKhoa = doctor.chuyenKhoa;
                existingDoctor.idCoSo = doctor.idCoSo;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var downloadUrl = await UploadImageToFirebase(imageFile);
                    existingDoctor.anhBS = downloadUrl;
                }

                db.Entry(existingDoctor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Cập nhật bác sĩ thành công!";

                return RedirectToAction("Index");
            }

            ViewBag.CoSoList = new SelectList(db.CoSoes.ToList(), "idCoSo", "tenBenhVien", doctor.idCoSo);
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

        // GET: AdminDoctorManagement/Delete/{id}
        public ActionResult Delete(int id)
        {
            var doctor = db.BacSis.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();  // Trả về lỗi 404 nếu không tìm thấy bác sĩ
            }
            return View(doctor);  // Truyền đối tượng bác sĩ qua view để hiển thị
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)  // Sử dụng kiểu nullable (int?)
        {
            if (id == null)
            {
                return HttpNotFound();  // Nếu không có id, trả về lỗi 404
            }

            var doctor = db.BacSis.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();  // Nếu không tìm thấy bác sĩ, trả về lỗi 404
            }

            // Kiểm tra xem bác sĩ có bất kỳ ca khám nào không
            var relatedAppointments = db.CaKhams.Any(c => c.idBS == doctor.idBS);
            if (relatedAppointments)
            {
                // Nếu có, không cho phép xóa bác sĩ và hiển thị thông báo lỗi
                TempData["ErrorMessage"] = "Không thể xóa bác sĩ này vì có các ca khám đang tồn tại!";
                return RedirectToAction("Index", "AdminDoctorManagement");
            }

            var account = db.Accounts.Find(doctor.idAccount);
            if (account != null)
            {
                db.BacSis.Remove(doctor);  // Xóa bác sĩ
                db.Accounts.Remove(account);  // Xóa tài khoản
                db.SaveChanges();

                // Lưu thông báo vào TempData để hiển thị trong trang quản lý bác sĩ
                TempData["SuccessMessage"] = "Xóa tài khoản và bác sĩ thành công!";
            }
            else
            {
                // Nếu không tìm thấy tài khoản, hiển thị thông báo lỗi
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản của bác sĩ!";
            }

            // Quay lại trang danh sách bác sĩ
            return RedirectToAction("Index", "AdminDoctorManagement");
        }

    }
}