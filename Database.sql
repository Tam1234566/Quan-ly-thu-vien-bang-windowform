-- Tạo bảng tài khoản
CREATE TABLE dbo.TaiKhoan
(
    ID INT PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL
        CONSTRAINT CK_TaiKhoan_VaiTro
        CHECK (VaiTro IN (N'Admin', N'User'))
);
GO

-- Tạo bảng sách
CREATE TABLE dbo.Sach
(
    MaSach INT PRIMARY KEY,
    TenSach NVARCHAR(255) NOT NULL,
    TacGia NVARCHAR(100) NULL,
    TheLoai NVARCHAR(100) NULL,
    NamXuatBan INT NULL
        CONSTRAINT CK_Sach_NamXuatBan
        CHECK (NamXuatBan >= 1900),
    NhaXuatBan NVARCHAR(255) NULL
);
GO

-- Tạo bảng độc giả
CREATE TABLE dbo.DocGia
(
    MaDocGia INT PRIMARY KEY,
    HoTen NVARCHAR(255) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL
        CONSTRAINT CK_DocGia_GioiTinh
        CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
    SDT NVARCHAR(15) NULL UNIQUE,
    Email NVARCHAR(255) NULL UNIQUE
);
GO

-- Tạo bảng phiếu mượn
CREATE TABLE dbo.PhieuMuon
(
    MaPhieu INT PRIMARY KEY,
    MaDocGia INT NOT NULL,
    HoTen NVARCHAR(255) NULL,
    TenSach NVARCHAR(255) NULL,
    MaSach INT NOT NULL,
    NgayMuon DATE NULL,
    NgayTra DATE NULL,
    TrangThai NVARCHAR(50) NOT NULL
        CONSTRAINT DF_PhieuMuon_TrangThai
        DEFAULT N'Đang mượn',

    CONSTRAINT FK_PhieuMuon_DocGia
        FOREIGN KEY (MaDocGia)
        REFERENCES dbo.DocGia(MaDocGia)
        ON DELETE CASCADE,

    CONSTRAINT FK_PhieuMuon_Sach
        FOREIGN KEY (MaSach)
        REFERENCES dbo.Sach(MaSach)
        ON DELETE CASCADE,

    CONSTRAINT CK_PhieuMuon_NgayTra
        CHECK (NgayTra IS NULL OR NgayMuon IS NULL OR NgayTra >= NgayMuon)
);
GO

-- Thêm dữ liệu tài khoản
INSERT INTO dbo.TaiKhoan
    (ID, TenDangNhap, MatKhau, VaiTro)
VALUES
    (1, N'admin', N'123456', N'Admin'),
    (2, N'user1', N'password1', N'User'),
    (3, N'user2', N'password2', N'User');
GO

-- Thêm dữ liệu sách
INSERT INTO dbo.Sach
    (MaSach, TenSach, TacGia, TheLoai, NamXuatBan, NhaXuatBan)
VALUES
    (1, N'Lập trình C#', N'Nguyễn Văn A', N'Công nghệ', 2020, N'NXB Giáo Dục'),
    (2, N'Toán học cao cấp', N'Trần Thị B', N'Giáo trình', 2018, N'NXB Đại Học Quốc Gia'),
    (3, N'Lịch sử Việt Nam', N'Phạm Văn C', N'Lịch sử', 2015, N'NXB Chính Trị Quốc Gia');
GO

-- Thêm dữ liệu độc giả
INSERT INTO dbo.DocGia
    (MaDocGia, HoTen, NgaySinh, GioiTinh, SDT, Email)
VALUES
    (1, N'Nguyễn Văn Nam', '19950615', N'Nam',
        N'0912345678', N'namnguyen@gmail.com'),
    (2, N'Trần Thị Hoa', '20000922', N'Nữ',
        N'0987654321', N'hoatran@yahoo.com'),
    (3, N'Lê Văn An', '19981205', N'Nam',
        N'0909123456', N'anle@hotmail.com');
GO

-- Thêm thêm dữ liệu độc giả
INSERT INTO dbo.DocGia
    (MaDocGia, HoTen, NgaySinh, GioiTinh, SDT, Email)
VALUES
    (4, N'Phạm Minh Tuấn', '19970318', N'Nam', N'0911223344', N'tuanpham@gmail.com'),
    (5, N'Đỗ Ngọc Mai', '20010625', N'Nữ', N'0933445566', N'maido@gmail.com'),
    (6, N'Võ Hoàng Long', '19991130', N'Nam', N'0977889900', N'longvo@gmail.com');
GO

-- Thêm thêm dữ liệu phiếu mượn
INSERT INTO dbo.PhieuMuon
    (MaPhieu, MaDocGia, HoTen, TenSach, MaSach, NgayMuon, NgayTra, TrangThai)
VALUES
    (4, 4, N'Phạm Minh Tuấn', N'Lập trình C#', 1, '2024-03-01', '2024-03-12', N'Đã trả'),
    (5, 5, N'Đỗ Ngọc Mai', N'Toán học cao cấp', 2, '2024-03-10', NULL, N'Đang mượn'),
    (6, 6, N'Võ Hoàng Long', N'Lịch sử Việt Nam', 3, '2024-03-15', NULL, N'Đang mượn');
GO

-- Kiểm tra dữ liệu
SELECT * FROM dbo.TaiKhoan;
SELECT * FROM dbo.Sach;
SELECT * FROM dbo.DocGia;
SELECT * FROM dbo.PhieuMuon;