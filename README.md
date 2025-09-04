# HealthyCare - Đồ án môn học Dự án Phần mềm

Chào mừng bạn đã đến với HealthyCare! Đây là sản phẩm của tụi mình cho môn học Dự án Phần mềm tại trường Đại học Ngoại ngữ - Tin học TP. Hồ Chí Minh (HUFLIT). Ứng dụng mô phỏng một hệ thống đặt lịch khám bệnh trực tuyến, được xây dựng bằng ASP.NET MVC 5.

---

> ### Về Tụi Mình & Dự Án Này
>
> Dự án này được phát triển bởi hai thành viên với sự phân chia công việc rõ ràng để mỗi người có thể tập trung vào thế mạnh của mình:
>
> *   **Nguyễn Anh Tài:** Phụ trách chính phần **Backend, Database và toàn bộ hệ thống quản trị (Admin Panel)**.
> *   **Nguyễn Quang Huy:** Đảm nhiệm phần **Frontend và xây dựng trải nghiệm cho người dùng cuối (Bệnh nhân)**.
>
> **Một lưu ý nhỏ:** Do trong quá trình làm đồ án, tụi mình thường ngồi code chung trên một máy nên đa số lịch sử commit đều được ghi nhận dưới một tài khoản. Repository gốc của dự án có thể được tìm thấy tại **[đây](https://github.com/nQhuy04/WebsiteDatLichKhamBenh)**.

---


## 🚀 Các chức năng chính của Hệ thống

Đây là những gì tụi mình đã xây dựng, được phân chia theo hai mảng chính:

### ⚙️ **Hệ thống Quản trị & Nghiệp vụ Bác sĩ (Backend-focused)**
Đây là "trung tâm đầu não" của ứng dụng, nơi Admin và Bác sĩ làm việc. Phần này do **Tài** phát triển chính, tập trung vào việc xử lý logic phức tạp và quản lý dữ liệu.

#### **Dành cho Quản trị viên (Admin):**
*   **Dashboard Thống kê:**
    *   Cung cấp một giao diện tổng quan, hiển thị các số liệu chính như tổng số bác sĩ, bệnh nhân, và các lịch hẹn mới trong ngày.

*   **Quản lý Dữ liệu lõi (CRUD):**
    *   **Quản lý Bác sĩ:** Thêm mới, xem danh sách, cập nhật thông tin chi tiết (chuyên khoa, kinh nghiệm) và xóa thông tin bác sĩ.
    *   **Quản lý Bệnh nhân:** Xem danh sách, chỉnh sửa thông tin cá nhân và xóa tài khoản bệnh nhân khi cần thiết.
    *   **Quản lý Tài khoản:** Toàn quyền quản lý các tài khoản trong hệ thống, bao gồm cả việc tạo tài khoản mới cho bác sĩ và phân quyền.
    *   **Quản lý Cơ sở y tế:** Thêm, sửa, xóa thông tin các bệnh viện, phòng khám liên kết.

*   **Vận hành & Kiểm duyệt:**
    *   **Quản lý Lịch hẹn:** Theo dõi toàn bộ lịch hẹn trên hệ thống, thực hiện các hành động nghiệp vụ như **Duyệt** hoặc **Từ chối** một lịch hẹn. Các lịch hẹn bị từ chối có thể được xóa để dọn dẹp hệ thống.
    *   **Quản lý Ca khám:** Xem và kiểm duyệt các lịch làm việc (ca khám) mà bác sĩ đăng ký, đảm bảo lịch làm việc hợp lệ.

#### **Dành cho Bác sĩ:**
*   **Cổng thông tin cho Bác sĩ:**
    *   **Tự quản lý Lịch làm việc:** Cung cấp giao diện để bác sĩ có thể chủ động đăng ký các ca làm việc trống theo ngày và khung giờ, cũng như chỉnh sửa lại khi cần.
    *   **Quản lý Bệnh nhân của mình:** Xem danh sách chi tiết các lịch hẹn mà bệnh nhân đã đặt với mình.
    *   **Quy trình khám bệnh:**
        *   Thực hiện kê đơn thuốc cho bệnh nhân sau khi lịch hẹn đã được đánh dấu là "Đã khám".
        *   Tích hợp thư viện **iTextSharp** để tự động **tạo và xuất đơn thuốc ra file PDF** một cách chuyên nghiệp, có thể in ra hoặc gửi cho bệnh nhân.

### 👤 **Trải nghiệm dành cho Bệnh nhân (Frontend-focused)**
Đây là giao diện mà người dùng cuối sẽ tương tác, được xây dựng với mục tiêu thân thiện và dễ sử dụng. Phần này do **Huy** phát triển chính.

*   **Hành trình của Bệnh nhân:**
    *   **Tìm kiếm và Đặt lịch thông minh:**
        *   Cung cấp thanh tìm kiếm cho phép bệnh nhân dễ dàng tìm bác sĩ theo **tên** hoặc **chuyên khoa**.
        *   Xây dựng một quy trình đặt lịch gồm nhiều bước, từ chọn bác sĩ, xem lịch trống, đến xác nhận thông tin.
    *   **Trang quản lý cá nhân "Tất cả trong một":**
        *   Cho phép bệnh nhân tự cập nhật thông tin cá nhân, số điện thoại, email.
        *   Quản lý **Hồ sơ sức khỏe**, bao gồm các thông tin quan trọng như tiền sử bệnh và dị ứng.
        *   Xem lại toàn bộ **lịch sử khám bệnh**, bao gồm các lịch hẹn đã hoàn thành, đã hủy và các đơn thuốc tương ứng.
    *   **Hệ thống Đánh giá & Tương tác:**
        *   Sau khi lịch khám hoàn thành, bệnh nhân sẽ có quyền **đánh giá bác sĩ** qua thang điểm sao và **để lại bình luận** chi tiết về trải nghiệm của mình.
*   **Quản lý Nội dung & Tin tức:**
    *   Xây dựng module tin tức với đầy đủ chức năng CRUD, cho phép Admin đăng tải các bài viết, thông tin sức khỏe để bệnh nhân có thể đọc và tham khảo.

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ / Phương pháp |
| :--- | :--- |
| **Nền tảng** | .NET Framework 4.7.2 |
| **Backend** | C#, ASP.NET MVC 5, Entity Framework 6 (Database First) |
| **Frontend** | HTML5, CSS3, JavaScript, jQuery, Bootstrap 3 |
| **Database** | Microsoft SQL Server |
| **Thư viện phụ trợ** | iTextSharp (Xuất PDF), BCrypt.Net (Băm mật khẩu) |

---

## 📖 Hướng dẫn Cài đặt & Vận hành Dự án

Dự án này sử dụng Entity Framework theo phương pháp **Database First**. Vui lòng làm theo các bước dưới đây để cài đặt.

### Cài đặt Dự án:

Đây là các bước để chạy dự án từ đầu trên một máy tính mới.

1.  **Clone repository về máy:**
    ```bash
    git clone https://github.com/NguyenTai26072004/WebsiteDatLichKhamBenh_HealthyCare.git
    ```

2.  **Chuẩn bị Cơ sở dữ liệu:**
    *   Mở **SQL Server Management Studio (SSMS)**.
    *   Mở file script SQL có sẵn trong project (`WebDatLichKhamBenhDB.sql`) và thực thi (Execute) toàn bộ script này trên database vừa tạo. Script sẽ tạo tất cả các bảng và chèn dữ liệu mẫu.

3.  **Cấu hình Connection String (Quan trọng nhất):**
    *   Mở project bằng **Visual Studio 2019** (hoặc mới hơn).
    *   Trong Solution Explorer, mở file `Web.config` ở thư mục gốc.
    *   Tìm đến dòng có `connectionStrings`. Bạn sẽ thấy một chuỗi kết nối tương tự như sau:
    ```xml
    <connectionStrings>
      <add name="WebDatLichKhamBenhDBEntities" 
           connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=ADMIN-PC;initial catalog=WebDatLichKhamBenhDB;integrated security=True;...&quot;" 
           providerName="System.Data.EntityClient" />
    </connectionStrings>
    ```
    *   Bạn **chỉ cần sửa lại đúng một chỗ**: `data source=...`. Hãy thay phần `...` bằng **"Server Name" trong SQL Server trên máy của bạn**. (Bạn có thể lấy tên này trong cửa sổ đăng nhập của SSMS).

4.  **Build và Chạy:**
    *   Trong Visual Studio, vào menu `Build` -> `Rebuild Solution` để đảm bảo mọi thứ được biên dịch lại.
    *   Nhấn `F5` hoặc nút ▶ (Start) để chạy dự án. Tài khoản admin mặc định là `admin` / `@adminpass`.


---

## 👥 Nhóm Phát triển

*   **Nguyễn Anh Tài** - *Backend & Admin Panel*
    *   GitHub: [@NguyenTai26072004](https://github.com/NguyenTai26072004)
*   **Nguyễn Quang Huy** - *Frontend & User Experience*
    *   GitHub: [@nQhuy04](https://github.com/nQhuy04)

Cảm ơn bạn đã ghé thăm project môn học của tụi mình!
