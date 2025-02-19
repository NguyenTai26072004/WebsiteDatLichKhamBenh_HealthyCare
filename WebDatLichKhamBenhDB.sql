DROP DATABASE WebDatLichKhamBenhDB 

-- Tạo cơ sở dữ liệu
CREATE DATABASE WebDatLichKhamBenhDB;
GO

USE WebDatLichKhamBenhDB;
GO


-- Tạo bảng Account
CREATE TABLE Account (
    idAccount INT PRIMARY KEY IDENTITY(1,1),
    TaiKhoan NVARCHAR(50) NOT NULL,
    MatKhau NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);

-- Tạo bảng CoSo
CREATE TABLE CoSo (
    idCoSo INT PRIMARY KEY IDENTITY(1,1),
    DiaChi NVARCHAR(200),
    tenBenhVien NVARCHAR(100)
);

-- Tạo bảng BenhNhan
CREATE TABLE BenhNhan (
    idBenhNhan INT PRIMARY KEY IDENTITY(1,1),
    tenBenhNhan NVARCHAR(100) NOT NULL,
    ngaySinh DATE,
    SDT NVARCHAR(15),
    Email NVARCHAR(100),
    GioiTinh NVARCHAR(10),
    idAccount INT,
    FOREIGN KEY (idAccount) REFERENCES Account(idAccount) 
);

-- Tạo bảng BacSi
CREATE TABLE BacSi (
    idBS INT PRIMARY KEY IDENTITY(1,1),
    tenBS NVARCHAR(100),
    anhBS NVARCHAR(500),
    chuyenKhoa NVARCHAR(100),
    diemDanhGia DECIMAL(3,1),  -- Giới hạn số thập phân là 1
    luotDat INT DEFAULT 0,
    idAccount INT,
    idCoSo INT,
    FOREIGN KEY (idAccount) REFERENCES Account(idAccount),
    FOREIGN KEY (idCoSo) REFERENCES CoSo(idCoSo)
);

-- Tạo bảng KhungGio
CREATE TABLE KhungGio (
    MaKhungGio INT PRIMARY KEY IDENTITY(1,1),
    GioKham NVARCHAR(20)
);

-- Tạo bảng CaKham
CREATE TABLE CaKham (
    MaCaKham INT PRIMARY KEY IDENTITY(1,1),
    idBS INT,
    NgayKham DATE,
    MaKhungGio INT,
    TrangThai NVARCHAR(50),
    FOREIGN KEY (idBS) REFERENCES BacSi(idBS),  
    FOREIGN KEY (MaKhungGio) REFERENCES KhungGio(MaKhungGio)
);

-- Tạo bảng Thuoc
CREATE TABLE Thuoc (
    MaThuoc INT PRIMARY KEY IDENTITY(1,1),
    TenThuoc NVARCHAR(100)
);

CREATE TABLE DonThuoc (
    MaDonThuoc INT PRIMARY KEY IDENTITY(1,1),
    GhiChu NVARCHAR(500),
    NgayKeDon DATE,
);

CREATE TABLE ChiTietDonThuoc (
    MaChiTietDonThuoc INT PRIMARY KEY IDENTITY(1,1),
    MaDonThuoc INT,
    MaThuoc INT,
    SoLuong INT,
    LieuLuong NVARCHAR(100),
    FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc),
    FOREIGN KEY (MaThuoc) REFERENCES Thuoc(MaThuoc)
);

-- Tạo bảng LichKham
CREATE TABLE LichKham (
    MaLichKham INT PRIMARY KEY IDENTITY(1,1),
    MaBenhNhan INT,
    MaCaKham INT,
    NgayDatLich DATE,
    GioDatLich TIME,  
    TrangThai NVARCHAR(50),
	MaDonThuoc INT,
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(idBenhNhan),
    FOREIGN KEY (MaCaKham) REFERENCES CaKham(MaCaKham),
	FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc)
);

-- Tạo bảng DiUng
CREATE TABLE DiUng (
    idDiUng INT PRIMARY KEY IDENTITY(1,1),
    tenDiUng NVARCHAR(100) NOT NULL,
	TrangThai NVARCHAR(50),
);

-- Tạo bảng TienSuBenh
CREATE TABLE TienSuBenh (
    idTienSuBenh INT PRIMARY KEY IDENTITY(1,1),
    tenTSB NVARCHAR(100) NOT NULL,
	TrangThai NVARCHAR(50),
);

-- Tạo bảng trung gian BenhNhan_DiUng để quản lý mối quan hệ giữa BenhNhan và DiUng
CREATE TABLE BenhNhan_DiUng (
    idBenhNhan INT,
    idDiUng INT,
	GhiChu NVARCHAR(255),
    PRIMARY KEY (idBenhNhan, idDiUng),
    FOREIGN KEY (idBenhNhan) REFERENCES BenhNhan(idBenhNhan),
    FOREIGN KEY (idDiUng) REFERENCES DiUng(idDiUng) 
);

-- Tạo bảng trung gian BenhNhan_TienSuBenh để quản lý mối quan hệ giữa BenhNhan và TienSuBenh
CREATE TABLE BenhNhan_TienSuBenh (
    idBenhNhan INT,
    idTienSuBenh INT,
	GhiChu NVARCHAR(255),
    PRIMARY KEY (idBenhNhan, idTienSuBenh),
    FOREIGN KEY (idBenhNhan) REFERENCES BenhNhan(idBenhNhan) ,
    FOREIGN KEY (idTienSuBenh) REFERENCES TienSuBenh(idTienSuBenh) 
);


CREATE TABLE DanhGiaBacSi (
    idDanhGia INT PRIMARY KEY IDENTITY(1,1),
    idBacSi INT,
    idBenhNhan INT,
    MaLichKham INT,  -- Thêm cột MaLichKham để tham chiếu LichKham
    diemDanhGia FLOAT CHECK (diemDanhGia >= 1 AND diemDanhGia <= 5), -- Thêm ràng buộc điểm đánh giá
    binhLuan NVARCHAR(500), -- Bình luận của bệnh nhân
    ngayDanhGia DATETIME DEFAULT GETDATE(), -- Ngày đánh giá, mặc định là ngày hiện tại
    TrangThai NVARCHAR(50),
    FOREIGN KEY (idBacSi) REFERENCES BacSi(idBS),
    FOREIGN KEY (idBenhNhan) REFERENCES BenhNhan(idBenhNhan),
    FOREIGN KEY (MaLichKham) REFERENCES LichKham(MaLichKham)  -- Khóa ngoại tham chiếu LichKham
);





-- Tạo bảng TinTuc
CREATE TABLE TinTuc (
    idTinTuc INT PRIMARY KEY IDENTITY(1,1),
    TieuDe NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(500),
	NoiDung NVARCHAR(MAX),
    HinhAnh NVARCHAR(500),
);



-- Chèn dữ liệu vào bảng Account
INSERT INTO Account (TaiKhoan, MatKhau, Role) VALUES
('bacsi1', '@bacsi1', 'BacSi'),
('bacsi2', '@bacsi2', 'BacSi'),
('bacsi3', '@bacsi3', 'BacSi'),
('bacsi4', '@bacsi4', 'BacSi'),
('bacsi5', '@bacsi5', 'BacSi'),
('bacsi6', '@bacsi6', 'BacSi'),
('bacsi7', '@bacsi7', 'BacSi'),
('bacsi8', '@bacsi8', 'BacSi'),
('bacsi9', '@bacsi9', 'BacSi'),
('bacsi10', '@bacsi10', 'BacSi'),
('bacsi11', '@bacsi11', 'BacSi'),
('bacsi12', '@bacsi12', 'BacSi'),
('bacsi13', '@bacsi13', 'BacSi'),
('bacsi14', '@bacsi14', 'BacSi'),
('bacsi15', '@bacsi15', 'BacSi'),
('bacsi16', '@bacsi16', 'BacSi'),
('bacsi17', '@bacsi17', 'BacSi'),
('bacsi18', '@bacsi18', 'BacSi'),
('bacsi19', '@bacsi19', 'BacSi'),
('bacsi20', '@bacsi20', 'BacSi'),
('bacsi21', '@bacsi21', 'BacSi'),
('bacsi22', '@bacsi22', 'BacSi'),
('bacsi23', '@bacsi23', 'BacSi'),
('bacsi24', '@bacsi24', 'BacSi'),
('bacsi25', '@bacsi25', 'BacSi'),
('bacsi26', '@bacsi26', 'BacSi'),
('bacsi27', '@bacsi27', 'BacSi'),
('bacsi28', '@bacsi28', 'BacSi'),
('bacsi29', '@bacsi29', 'BacSi'),
('bacsi30', '@bacsi30', 'BacSi'),
('bacsi31', '@bacsi31', 'BacSi'),
('bacsi32', '@bacsi32', 'BacSi'),
('bacsi33', '@bacsi33', 'BacSi'),
('bacsihuy', 'Bacsihuy@123', 'BacSi'),
('bacsitai', 'Bacsitai@123', 'BacSi'),
('bacsikhai', 'Bacsikhai@123', 'BacSi'),
('benhnhan1', '@benhnhan1', 'BenhNhan'),
('benhnhan2', '@benhnhan2', 'BenhNhan'),
('benhnhan3', '@benhnhan3', 'BenhNhan'),
('benhnhan4', '@benhnhan4', 'BenhNhan'),
('admin', '@adminpass', 'Admin');

-- Chèn dữ liệu vào bảng CoSo
INSERT INTO CoSo (DiaChi, tenBenhVien) VALUES
(N'828 Sư Vạn Hạnh, Phường 13, Quận 10, TPHCM', N'Bệnh Viện Sư Vạn Hạnh'),
(N'52-70 Ba Gia, Phường 7, Tân Bình, Hồ Chí Minh', N'Bệnh Viện Ba Gia'),
(N'806 QL22, ấp Mỹ Hoà 3, Hóc Môn, Hồ Chí Minh', N'Bệnh Viện Hóc Môn');

-- Chèn dữ liệu vào bảng KhungGio
INSERT INTO KhungGio (GioKham) VALUES
('08:00'),
('09:00'),
('10:00'),
('11:00'),
('13:00'),
('14:00'),
('15:00'),
('16:00'),
('17:00'),
('18:00'),
('19:00'),
('20:00');

-- Chèn dữ liệu vào bảng BacSi
INSERT INTO BacSi (tenBS, anhBS, chuyenKhoa, diemDanhGia, luotDat, idAccount, idCoSo) VALUES
(N'BSCKII TRẦN THỊ THANH NHO', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi1.jpg?alt=media&token=f4e19ce0-706d-4030-986b-6ff4ad07fa73', N'Da liễu', 4.9, 359, 1, 1),
(N'ThsBSNT PHẠM THỊ QUỲNH', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi2.jpg?alt=media&token=85998ad2-5f00-4f99-89e6-d565b6a40e98', N'Tâm lý', 4.8, 104, 2, 2),
(N'BS TRƯƠNG THỊ HƯƠNG GIANG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi3.jpg?alt=media&token=3049611c-3c42-44ec-b5ad-2f8259b9968f', N'Phục hồi chức năng', 5, 100, 3, 1),
(N'BSCKII NGUYỄN THỊ MỸ LÝ', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi4.jpg?alt=media&token=0d8f5c77-6969-4edf-9bed-e12f693e8d51', N'Nội soi tiêu hóa', 5, 23, 4, 3),
(N'BS PHAN NGỌC SƠN', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi5.jpg?alt=media&token=850419ae-5c8d-4208-bb42-f235d9f0fd40', N'Phục hồi chức năng', 4.2, 150, 5, 3),
(N'ThsBSNT NGUYỄN SỸ ĐỨC', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi6.jpg?alt=media&token=dc7f6605-a4d7-45fe-aff6-68e4266dd75c', N'Nhi Khoa', 4, 200, 6, 1),
(N'ThS LÊ VĂN VINH', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi7.jpg?alt=media&token=a8b8ca5b-3b91-4fb5-9ed1-248a5e1264c2', N'Nội khoa', 4.7, 300, 7, 2),
(N'BS TS.BSCKII LÊ QUỐC VIỆT', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi8.png?alt=media&token=e6ca084d-45ff-4c7f-90a4-f804ec4825be', N'Nội Cơ Xương Khớp', 5, 50, 8, 3),
(N'THS LÊ ĐỨC VIỆT', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi9.png?alt=media&token=d64c5601-096f-4c80-b265-2585417f4711', N'Tim mạch', 4.8, 222, 9, 2),
(N'BS NGUYỄN TRUNG VIỆT', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi10.jpg?alt=media&token=da7bcc74-8f82-40a3-a0c7-d0975480c76f', N'Hô hấp', 4.9, 2727, 10, 2),
(N'ThS HOÀNG MINH TRUNG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi11.png?alt=media&token=ee249612-3f75-4365-8448-4e8ba9e61f3f', N'Tim mạch', 4.4, 60, 11, 2),
(N'ThSBSNT TRẦN THỊ HOA', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi12.png?alt=media&token=2f9ec59a-edac-45af-a653-b0c3a13abfc1', N'Thần kinh', 4.8, 150, 12, 3),
(N'ThSBS HÀ LƯƠNG UYÊN', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi13.jpg?alt=media&token=07930894-e9c2-4939-979c-e490910726b1', N'Tai mũi họng', 4.3, 110, 13, 1),
(N'BS ĐÀM NHẬT THANH', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi14.jpg?alt=media&token=3d7b4cea-b003-45c0-bc68-dd71519502bf', N'Nội thần kinh', 4.7, 95, 14, 1),
(N'BS ĐẶNG TRUNG THÀNH', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi15.jpg?alt=media&token=d318a152-c838-46af-a889-9148dcfaa440g', N'Nội tiết', 4.6, 78, 15, 3),
(N'BS HOÀNG THỊ BẠCH DƯƠNG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi16.jpg?alt=media&token=b04ec90b-9dd7-4364-a7ba-eadcb086baf8', N'Nhi Khoa', 4.5, 140, 16, 2),
(N'BS TRẦN THẢO TRANG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi17.webp?alt=media&token=231e4751-94ce-4557-9c66-45c9e1c6eb2f', N'Phục hồi chức năng', 4.9, 125, 17, 1),
(N'BS ĐOÀN THỊ HẰNG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi18.jpg?alt=media&token=33d172a7-de04-4b9d-9a5b-c563f421f3ac', N'Y học cổ truyền', 4.8, 115, 18, 2),
(N'BS MAI DUY HIỀN', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi19.png?alt=media&token=c0f036fc-936e-4380-9118-b26a8aea8c84', N'Sản phụ Khoa', 4.6, 135, 19, 3),
(N'ThSBSNT LÊ XUÂN QUÝ', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi20.jpg?alt=media&token=7fbb0790-fa1c-49e0-a622-cad1907fb378', N'Nha khoa', 4.5, 130, 20, 3),
(N'ThSBS NGUYỄN HUYỀN NHUNG', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsi21.png?alt=media&token=7c0cb2bf-dbde-4784-8195-3030afd80451', N'Răng hàm mặt', 4.7, 98, 21, 1),
(N'BS NGUYỄN QUANG HUY', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsihuy.jpg?alt=media&token=ae378aef-0e6c-45ac-96d1-44078a561fd7', N'Sản phụ khoa', 5, 250, 22, 3),
(N'BS NGUYỄN ANH TÀI', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsitai.jpg?alt=media&token=33f94bc5-2762-4c53-905b-ab306d64bcd4', N'Gây mê hồi sức', 5, 100, 23, 3),
(N'BS TRẦN QUANG KHẢI', 'https://firebasestorage.googleapis.com/v0/b/websitedatlichkhambenh.appspot.com/o/images%2Fbacsikhai.jpg?alt=media&token=20eb9c12-7064-4073-8380-3f99be4923df', N'Huyết học', 5, 70, 24, 3);



-- Chèn dữ liệu vào bảng BenhNhan
INSERT INTO BenhNhan (tenBenhNhan, ngaySinh, SDT, Email, GioiTinh, idAccount) 
VALUES 
(N'Bệnh Nhân 1', '1990-01-01', '0971129574', 'benhnhan1@example.com', N'Nam', 
    (SELECT idAccount FROM Account WHERE TaiKhoan = 'benhnhan1')),
(N'Bệnh Nhân 2', '1991-02-02', '0967847239', 'benhnhan2@example.com', N'Nữ', 
    (SELECT idAccount FROM Account WHERE TaiKhoan = 'benhnhan2')),
(N'Bệnh Nhân 3', '1992-03-03', '0958256523', 'benhnhan3@example.com', N'Nam', 
    (SELECT idAccount FROM Account WHERE TaiKhoan = 'benhnhan3')),
(N'Bệnh Nhân 4', '1993-04-04', '0965329834', 'benhnhan4@example.com', N'Nữ', 
    (SELECT idAccount FROM Account WHERE TaiKhoan = 'benhnhan4'));


-- Chèn dữ liệu vào bảng DiUng
INSERT INTO DiUng (tenDiUng, TrangThai) VALUES
(N'Không có', N'Mặc định'),
(N'Dị ứng với penicillin', N'Mặc định'),
(N'Dị ứng với thức ăn', N'Mặc định'),
(N'Dị ứng với phấn hoa', N'Mặc định'),
(N'Dị ứng với thuốc giảm đau', N'Mặc định'),
(N'Dị ứng với lông động vật', N'Mặc định'),
(N'Dị ứng với mạt bụi nhà', N'Mặc định'),
(N'Dị ứng với thuốc kháng sinh', N'Mặc định'),
(N'Dị ứng với kim loại', N'Mặc định'),
(N'Dị ứng với nọc độc côn trùng', N'Mặc định'),
(N'Dị ứng với sữa', N'Mặc định'),
(N'Khác', N'Mặc định');

-- Chèn dữ liệu vào bảng TienSuBenh
INSERT INTO TienSuBenh (tenTSB, TrangThai) VALUES
(N'Không có', N'Mặc định'),
(N'Tiểu đường', N'Mặc định'),
(N'Huyết áp cao', N'Mặc định'),
(N'Bệnh tim mạch', N'Mặc định'),
(N'Viêm khớp', N'Mặc định'),
(N'Trầm cảm', N'Mặc định'),
(N'Chứng đau nửa đầu', N'Mặc định'),
(N'Bệnh về gan', N'Mặc định'),
(N'Loãng xương', N'Mặc định'),
(N'Bệnh về thận', N'Mặc định'),
(N'Rối loạn tiêu hóa', N'Mặc định'),
(N'Khác', N'Mặc định');

-- Chèn dữ liệu vào bảng BenhNhan_DiUng
INSERT INTO BenhNhan_DiUng (idBenhNhan, idDiUng) VALUES
(1, 5),
(2, 3),
(3, 1),
(4, 7);

-- Chèn dữ liệu vào bảng BenhNhan_TienSuBenh
INSERT INTO BenhNhan_TienSuBenh (idBenhNhan, idTienSuBenh) VALUES
(1, 2),
(2, 1),
(3, 6),
(4, 8);


-- Chèn dữ liệu vào bảng Thuoc
INSERT INTO Thuoc (TenThuoc) VALUES
(N'Aspirin'),
(N'Paracetamol'),
(N'Amoxicillin'),
(N'Ibuprofen'),
(N'Ciprofloxacin');

-- Chèn dữ liệu vào bảng DonThuoc
INSERT INTO DonThuoc (GhiChu, NgayKeDon) VALUES
( N'Bệnh nhân cần nghỉ ngơi, uống thuốc đầy đủ', '2024-11-05'),
( N'Uống thuốc khi có triệu chứng đau đầu', '2024-11-06'),
( N'Diều trị kháng sinh', '2024-11-07'),
( N'Điều trị viêm khớp', '2024-11-08');



/*INSERT INTO LichKham (MaBenhNhan, MaCaKham, NgayDatLich, GioDatLich, TrangThai,MaDonThuoc) 
VALUES
(1, 1, '2024-11-05', '13:20', N'Đang chờ duyệt',1), -- Chưa Khám
(1, 3, '2024-11-05', '20:11', N'Đã được duyệt',1), -- Chưa khám
(1, 5, '2024-11-05', '08:54', N'Đã khám',1), -- Đã khám
(1, 4, '2024-11-05', '09:15', N'Đã từ chối',1), -- Chưa khám
(2, 2, '2024-11-06', '18:30', N'Đang chờ duyệt',1), -- Chưa khám
(3, 3, '2024-11-07', '23:42', N'Đã được duyệt',1), -- Chưa khám
(4, 4, '2024-11-08', '11:24', N'Đang chờ duyệt',1); -- Chưa khám */




-- Chèn dữ liệu vào bảng TinTuc
INSERT INTO TinTuc (TieuDe, MoTa, NoiDung, HinhAnh) VALUES
(N'Bà bầu mất ngủ do đâu? Làm gì để cải thiện', N'Mất ngủ khi mang thai là nỗi lo của nhiều bà bầu, khiến sức khỏe và tinh thần bị ảnh hưởng. Tình trạng này đôi khi có thể xảy ra điển hình vào 3 tháng đầu hoặc 3 tháng cuối thai kỳ.',N'Mất ngủ khi mang thai có thể xuất phát từ nhiều nguyên nhân khác nhau, chẳng hạn như thay đổi hormone, áp lực về tâm lý, hoặc khó chịu về thể chất khi thai nhi ngày càng lớn. Tình trạng này có thể làm giảm chất lượng cuộc sống của mẹ bầu và ảnh hưởng đến sức khỏe của cả mẹ và bé. Để cải thiện, mẹ bầu có thể thử tập các bài tập yoga nhẹ nhàng, luyện thiền để giảm stress, hoặc duy trì thói quen đi ngủ đúng giờ. Ngoài ra, nên tránh các chất kích thích như cafein và tạo một môi trường ngủ thoải mái, yên tĩnh. Một số mẹ bầu có thể cảm thấy lo lắng về giấc ngủ không đủ, nhưng thực tế, giấc ngủ ngắn và nhiều lần cũng có thể mang lại hiệu quả tốt. Thực hiện các bài tập thư giãn trước khi ngủ, như hít thở sâu hoặc nghe nhạc nhẹ nhàng, có thể giúp cải thiện chất lượng giấc ngủ. Tuy nhiên, nếu tình trạng mất ngủ kéo dài, mẹ bầu nên tham khảo ý kiến bác sĩ để tìm giải pháp an toàn cho cả mẹ và bé. Các phương pháp điều trị không sử dụng thuốc như liệu pháp ánh sáng và liệu pháp hành vi cũng có thể là một lựa chọn hữu ích.', 'https://storage.googleapis.com/a1aa/image/u0s00TJyPy4nDNVsJDXy2eGL3CAKJdSKWuShMJhWSFje89tTA.jpg'),
(N'Lá vông trị mất ngủ có tốt không? Cách dùng hiệu quả nhất', N'Lá vông trị mất ngủ là một phương pháp tự nhiên được nhiều người tin dùng. Tuy nhiên, để đạt hiệu quả cao, cần dùng đúng cách, đúng liều lượng và kiên trì trong thời gian dài.', N'Lá vông là một loại thảo dược có tính an thần và đã được sử dụng trong y học dân gian để hỗ trợ giấc ngủ. Nghiên cứu cho thấy, lá vông chứa các thành phần giúp thư giãn cơ thể, giảm căng thẳng và hỗ trợ cải thiện chất lượng giấc ngủ. Tuy nhiên, người sử dụng cần cẩn thận về liều lượng và không nên lạm dụng. Để đạt hiệu quả tốt nhất, nên tham khảo ý kiến bác sĩ hoặc chuyên gia, đồng thời duy trì sử dụng kiên trì trong một khoảng thời gian. Lá vông còn được biết đến với tác dụng làm giảm lo âu, căng thẳng và giúp thư giãn hệ thần kinh. Tuy nhiên, việc sử dụng lá vông không nên thay thế các phương pháp điều trị y tế khi cần thiết. Trong y học hiện đại, lá vông được nghiên cứu như một liệu pháp bổ sung trong điều trị mất ngủ, đặc biệt là khi người bệnh gặp phải chứng mất ngủ do stress. Thông thường, lá vông được sử dụng dưới dạng trà hoặc viên nén để dễ dàng tiêu thụ. Ngoài ra, việc kết hợp lá vông với các biện pháp tự nhiên khác như massage thư giãn cũng giúp tăng cường hiệu quả trong việc cải thiện giấc ngủ.','https://storage.googleapis.com/a1aa/image/75kLEufZX8QANiev6DbyCqOp2pmRBdiiuyvtyXej5DT257bnA.jpg'),
(N'Bị mất ngủ sau sinh có sao không? Cách chữa khỏi', N'Sau khi sinh, nhiều bà mẹ phải đối mặt với tình trạng mất ngủ kéo dài. Đây không chỉ là một hiện tượng thông thường mà còn có thể ảnh hưởng nghiêm trọng đến sức khỏe nếu không được điều trị kịp thời.', N'Sau sinh, nhiều bà mẹ thường phải đối mặt với tình trạng mất ngủ kéo dài, có thể do sự thay đổi hormone, áp lực từ việc chăm sóc con cái, hoặc căng thẳng tâm lý. Mất ngủ sau sinh không chỉ ảnh hưởng đến sức khỏe của mẹ mà còn gây mệt mỏi, ảnh hưởng đến khả năng chăm sóc bé. Để điều trị, các bà mẹ có thể tìm đến các liệu pháp như tư vấn tâm lý, yoga, hoặc kỹ thuật thiền để thư giãn. Khi cần thiết, nên tham khảo bác sĩ để sử dụng các loại thuốc an thần an toàn, không ảnh hưởng đến sữa mẹ. Một số bà mẹ gặp phải chứng mất ngủ do không thể ngủ khi em bé thức dậy trong đêm, điều này có thể dẫn đến việc ngủ thiếu chất lượng. Các bài tập thư giãn, thiền định, hoặc nghe nhạc nhẹ nhàng trước khi ngủ có thể giúp các bà mẹ giảm căng thẳng và dễ dàng đi vào giấc ngủ hơn. Tuy nhiên, nếu tình trạng mất ngủ không cải thiện, bà mẹ cần tìm kiếm sự hỗ trợ từ các chuyên gia sức khỏe để đảm bảo có thể chăm sóc bản thân tốt nhất, đồng thời duy trì sức khỏe trong suốt quá trình nuôi con.','https://storage.googleapis.com/a1aa/image/zdTowXf7CMSbaanpZhCqpTbsJk3JqwW6FfSe70CnxyU857bnA.jpg'),
(N'Thiền định giúp cải thiện giấc ngủ như thế nào?', N'Thiền định là một phương pháp hiệu quả để cải thiện giấc ngủ. Bài viết này sẽ hướng dẫn bạn cách thực hiện thiền định để có giấc ngủ ngon hơn.', N'Thiền định là một phương pháp hiệu quả giúp giảm căng thẳng và tăng cường sự thư giãn, từ đó cải thiện chất lượng giấc ngủ. Khi thực hiện thiền định đều đặn trước khi ngủ, cơ thể và tâm trí sẽ được thư giãn, giúp dễ dàng chìm vào giấc ngủ sâu và kéo dài. Bài viết này sẽ hướng dẫn bạn từng bước thực hành thiền định một cách đơn giản nhưng hiệu quả để có giấc ngủ ngon hơn. Thiền định giúp bạn tập trung vào hơi thở, xóa bỏ lo âu và sự căng thẳng tích tụ trong suốt ngày dài. Đặc biệt, thiền giúp cân bằng cảm xúc, điều này rất quan trọng đối với những người gặp phải khó khăn trong việc thư giãn và nghỉ ngơi. Một nghiên cứu đã chỉ ra rằng, người thực hành thiền đều đặn có xu hướng dễ dàng chìm vào giấc ngủ hơn và ngủ sâu hơn, giảm thiểu các rối loạn giấc ngủ. Bên cạnh việc thực hành thiền, bạn cũng có thể kết hợp các yếu tố khác như ánh sáng dịu nhẹ, âm nhạc thư giãn để tối ưu hóa môi trường ngủ của mình.','https://storage.googleapis.com/a1aa/image/fPoxDEYfZymhVkoz1Y6EBXtXVwii9YhsIZDI4BXLIfuoA8bnA.jpg'),
(N'Các loại trà thảo mộc giúp ngủ ngon', N'Trà thảo mộc là một trong những phương pháp tự nhiên giúp cải thiện giấc ngủ. Dưới đây là danh sách các loại trà thảo mộc tốt nhất cho giấc ngủ của bạn.', N'Các loại trà thảo mộc như trà hoa cúc, trà bạc hà, và trà tim sen là những lựa chọn tuyệt vời để hỗ trợ giấc ngủ. Những loại trà này chứa các hoạt chất có tác dụng thư giãn thần kinh, giúp cơ thể và tâm trí dễ dàng chìm vào giấc ngủ. Trước khi ngủ, thưởng thức một tách trà ấm sẽ giúp bạn giảm căng thẳng và chuẩn bị tinh thần tốt hơn cho giấc ngủ sâu. Trà hoa cúc được biết đến với khả năng làm dịu hệ thần kinh và giảm lo âu, rất thích hợp cho những người gặp phải tình trạng mất ngủ do căng thẳng. Trà bạc hà lại giúp thư giãn cơ bắp và tạo cảm giác thoải mái, trong khi trà tim sen có tác dụng an thần, giúp tăng cường chất lượng giấc ngủ. Bạn có thể kết hợp các loại trà này với thói quen thư giãn khác như đọc sách hoặc tắm nước ấm để tăng hiệu quả cải thiện giấc ngủ. Tuy nhiên, để đạt được hiệu quả tốt nhất, bạn nên tránh uống trà quá gần giờ đi ngủ, vì một số loại trà có thể gây tác dụng phụ nếu uống quá nhiều.','https://storage.googleapis.com/a1aa/image/bd8ke2ZSEW2LYC19UF56YvkSma0zLyW0tqqbeEgPQOwcAebnA.jpg'),
(N'Đọc sách trước khi ngủ có lợi ích gì?', N'Đọc sách trước khi ngủ là một thói quen tốt giúp bạn thư giãn và dễ dàng đi vào giấc ngủ. Hãy cùng tìm hiểu những lợi ích của việc đọc sách trước khi ngủ.', N'Đọc sách trước khi đi ngủ là một thói quen giúp cơ thể và tâm trí thư giãn, làm giảm bớt căng thẳng tích tụ trong ngày. Đặc biệt, khi chọn đọc các sách nhẹ nhàng hoặc sách mang tính chất giải trí, bạn sẽ dễ dàng buông bỏ lo âu và chìm vào giấc ngủ nhanh hơn. Tuy nhiên, nên tránh đọc sách quá kích thích hoặc sử dụng thiết bị điện tử để đọc, vì ánh sáng xanh từ màn hình có thể làm bạn khó ngủ. Đọc sách không chỉ giúp bạn thư giãn mà còn mở rộng kiến thức và phát triển tư duy. Các nghiên cứu đã chỉ ra rằng thói quen đọc sách trước khi ngủ có thể giúp cải thiện chất lượng giấc ngủ và giảm thiểu các rối loạn giấc ngủ. Nếu bạn có xu hướng thức khuya hoặc lo lắng về giấc ngủ, hãy thử thay đổi thói quen đọc sách vào buổi tối, chọn những câu chuyện nhẹ nhàng và thư giãn để có một giấc ngủ ngon.','https://storage.googleapis.com/a1aa/image/40d6y7xZv2qePaGNVxrMoaKFuTeFG13jXlYPxaX4lu5bAebnA.jpg'),
(N'Yoga giúp cải thiện giấc ngủ như thế nào?', N'Yoga không chỉ giúp cơ thể khỏe mạnh mà còn cải thiện giấc ngủ. Bài viết này sẽ giới thiệu các bài tập yoga giúp bạn có giấc ngủ ngon hơn.', N'Yoga là một phương pháp kết hợp giữa thể chất và tinh thần, giúp cơ thể thư giãn và giải phóng căng thẳng. Những bài tập yoga nhẹ nhàng trước khi ngủ có thể giúp hệ thần kinh bình tĩnh, giảm căng thẳng và chuẩn bị cho một giấc ngủ ngon. Một số tư thế yoga như tư thế đứa trẻ, tư thế ngọn núi và tư thế xác chết có thể là lựa chọn lý tưởng giúp bạn cải thiện chất lượng giấc ngủ. Những động tác yoga nhẹ nhàng làm thư giãn cơ thể, giảm đau lưng và tăng cường lưu thông máu. Nhiều nghiên cứu đã chỉ ra rằng yoga có thể giảm bớt căng thẳng và lo âu, giúp cải thiện giấc ngủ tự nhiên mà không cần dùng đến thuốc. Nếu bạn không có thời gian để tham gia lớp học yoga, bạn có thể thực hành các bài tập yoga đơn giản tại nhà trước khi đi ngủ để cảm nhận sự thư giãn và dễ dàng chìm vào giấc ngủ.','https://storage.googleapis.com/a1aa/image/2eDvDl0yLq2yVyjupbHL2eLe8LelaheQtEfyup2HoeWhLAftTA.jpg'),
(N'Uống sữa ấm trước khi ngủ có tốt không?', N'Uống sữa ấm trước khi ngủ là một thói quen được nhiều người áp dụng để cải thiện giấc ngủ. Hãy cùng tìm hiểu xem thói quen này có thực sự tốt không.', N'Uống sữa ấm trước khi ngủ là một cách phổ biến để cải thiện giấc ngủ, vì sữa chứa tryptophan, một axit amin có thể giúp sản sinh melatonin, hormone hỗ trợ giấc ngủ. Bên cạnh đó, sữa ấm còn có tác dụng làm dịu hệ thần kinh, giúp cơ thể thư giãn. Tuy nhiên, bạn nên uống một lượng vừa phải và tránh uống quá no để không gây khó chịu dạ dày trong đêm. Một số nghiên cứu cho thấy, sữa có thể giúp cải thiện chất lượng giấc ngủ của những người gặp phải tình trạng khó ngủ do căng thẳng hoặc lo âu. Ngoài ra, bạn cũng có thể kết hợp uống sữa ấm với một chút mật ong hoặc một ít gia vị như quế để tăng thêm tác dụng thư giãn. Tuy nhiên, nếu bạn không dung nạp lactose, bạn có thể thay thế bằng các loại sữa thực vật như sữa hạnh nhân hoặc sữa đậu nành.','https://storage.googleapis.com/a1aa/image/YCkPUbPolqrwA15DJrSeIP87Fplfh4EJIAfGwMoDHAyzA8bnA.jpg'),
(N'Tinh dầu giúp cải thiện giấc ngủ như thế nào?', N'Tinh dầu là một phương pháp tự nhiên giúp cải thiện giấc ngủ. Bài viết này sẽ giới thiệu các loại tinh dầu tốt nhất cho giấc ngủ của bạn.', N'Tinh dầu là một phương pháp tự nhiên giúp thư giãn và cải thiện giấc ngủ. Một số loại tinh dầu phổ biến như tinh dầu hoa oải hương, tinh dầu cam và tinh dầu trầm hương có tác dụng làm dịu hệ thần kinh và tạo cảm giác thư giãn. Bạn có thể sử dụng tinh dầu bằng cách xông phòng hoặc pha loãng rồi thoa lên da. Tinh dầu hoa oải hương được biết đến với tác dụng an thần mạnh mẽ, giúp giảm căng thẳng và lo âu. Tinh dầu cam lại mang đến cảm giác thư giãn nhưng cũng đầy tươi mới, giúp bạn cảm thấy dễ chịu trước khi đi ngủ. Tinh dầu trầm hương có tác dụng cân bằng cảm xúc, giúp cơ thể thư giãn sâu hơn và dễ dàng chìm vào giấc ngủ. Ngoài ra, bạn có thể kết hợp các loại tinh dầu với các phương pháp thư giãn khác như tắm nước ấm để tăng cường hiệu quả.','https://storage.googleapis.com/a1aa/image/Xxi8H2W68p5rJN8coLJSj4S0G6STIhL5eYFr2uURDWTMAftTA.jpg'),
(N'Sử dụng mặt nạ ngủ có hiệu quả không?', N'Mặt nạ ngủ là một công cụ hữu ích giúp bạn có giấc ngủ ngon hơn. Hãy cùng tìm hiểu cách sử dụng mặt nạ ngủ hiệu quả.', N'Mặt nạ ngủ là một công cụ hỗ trợ giúp ngăn chặn ánh sáng và tạo môi trường tối cho giấc ngủ sâu hơn. Khi đeo mặt nạ ngủ, cơ thể dễ dàng thư giãn hơn vì ít bị phân tâm từ môi trường xung quanh. Để sử dụng hiệu quả, hãy chọn mặt nạ mềm mại, vừa vặn và thoải mái. Mặt nạ ngủ không chỉ giúp tạo môi trường tối để giảm ánh sáng mà còn hỗ trợ giữ ấm nhẹ cho mắt, giúp thư giãn vùng mắt và khu vực xung quanh. Một số loại mặt nạ còn được thiết kế với tính năng massage nhẹ nhàng giúp giảm căng thẳng cho mắt, rất hữu ích cho những người phải làm việc nhiều với màn hình. Việc sử dụng mặt nạ ngủ kết hợp với một môi trường ngủ yên tĩnh và thoải mái có thể giúp cải thiện chất lượng giấc ngủ đáng kể, đặc biệt đối với những người sống trong môi trường có nhiều tiếng ồn hoặc ánh sáng mạnh.','https://storage.googleapis.com/a1aa/image/ulv5qn3XO0baG9abkki7tauNwTiQJqei8NMpb0mTZ2JLAftTA.jpg');

------------------------------------------------------------------------------
--Thêm bình luận của Bệnh nhân 1 2 3 4
DECLARE @i INT = 1; -- Bắt đầu với bác sĩ có idBS = 1
DECLARE @maxDoctorId INT = 24; -- Tổng số bác sĩ có idBS từ 1 đến 24
DECLARE @maxPatientId INT = 4; -- Tổng số bệnh nhân có idBenhNhan từ 1 đến 4

DECLARE @diemDanhGia FLOAT = 5; -- Điểm đánh giá mặc định
DECLARE @binhLuan NVARCHAR(500); -- Nội dung bình luận
DECLARE @patientId INT; -- Bệnh nhân hiện tại

WHILE @i <= @maxDoctorId
BEGIN
    SET @patientId = 1; -- Bắt đầu với bệnh nhân 1
    WHILE @patientId <= @maxPatientId
    BEGIN
        -- Tùy chỉnh bình luận dựa trên bệnh nhân
        SET @binhLuan = 
            CASE @patientId
                WHEN 1 THEN N'Thật sự hài lòng với bác sĩ này.'
                WHEN 2 THEN N'Dịch vụ rất tốt, tôi cảm thấy an tâm.'
                WHEN 3 THEN N'Bác sĩ chuyên nghiệp và thân thiện.'
                WHEN 4 THEN N'Tôi rất ấn tượng với sự tận tâm của bác sĩ.'
                ELSE N'Đánh giá tốt.'
            END;

        -- Thực hiện INSERT vào bảng DanhGiaBacSi
        INSERT INTO DanhGiaBacSi (idBacSi, idBenhNhan, diemDanhGia, binhLuan, ngayDanhGia, TrangThai)
        VALUES (@i, @patientId, @diemDanhGia, @binhLuan, GETDATE(), N'Đang hoạt động');
        
        -- Tăng idBenhNhan
        SET @patientId = @patientId + 1;
    END;

    -- Tăng idBS
    SET @i = @i + 1;
END;




-----------------------------------------------------------------------
-- Thêm ngẫu nhiên các ca khám của các bác sĩ
GO
DECLARE @idBS INT, @NgayKham DATE, @MaKhungGio INT, @TrangThai NVARCHAR(50);

SET @TrangThai = N'Đang hoạt động';

-- Lấy ngày hiện tại
DECLARE @StartDate DATE = CAST(GETDATE() AS DATE);

-- Duyệt qua tất cả các bác sĩ từ ID 1 đến 24
DECLARE @i INT = 1;

WHILE @i <= 24
BEGIN
    -- Mỗi bác sĩ có 10 ca khám ngẫu nhiên
    DECLARE @j INT = 1;
    WHILE @j <= 10
    BEGIN
        -- Tạo ngày khám ngẫu nhiên trong khoảng từ hôm nay đến 30 ngày sau
        SET @NgayKham = DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 30, @StartDate);

        -- Chọn khung giờ ngẫu nhiên (giả sử từ 1 đến 12)
        SET @MaKhungGio = ABS(CHECKSUM(NEWID())) % 12 + 1;

        -- Chèn ca khám ngẫu nhiên vào bảng
        INSERT INTO CaKham (idBS, NgayKham, MaKhungGio, TrangThai)
        VALUES (@i, @NgayKham, @MaKhungGio, @TrangThai);

        SET @j = @j + 1;
    END;

    SET @i = @i + 1;
END;




 