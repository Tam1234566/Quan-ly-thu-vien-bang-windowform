# Hệ thống quản lý thư viện

## 1. Giới thiệu

Hệ thống quản lý thư viện là ứng dụng desktop Windows Forms dùng để quản lý sách, độc giả và phiếu mượn/trả. Ứng dụng kết nối trực tiếp đến SQL Server và hiện đang sử dụng:

- **C# 7.3**
- **.NET Framework 4.7.2**
- **Microsoft.Data.SqlClient**
- **SQL Server**
- Project legacy non-SDK

Ứng dụng không triển khai Web API. Thay vào đó, phần nghiệp vụ và truy cập dữ liệu được tách thành project class library riêng để phù hợp với cấu trúc WinForms hiện tại.

## 2. Kiến trúc tổng thể

```text
Actor / Người dùng
        |
        v
Frontend / Windows Forms
        |
        v
Backend / Quanlythuvien.Services
        |
        v
Database / SQL Server - quanlythuvien
```

### Các thành phần chính

| Thành phần | Trách nhiệm |
|---|---|
| **Actor** | Người sử dụng hệ thống, đăng nhập và thực hiện các chức năng thư viện |
| **Frontend** | Hiển thị giao diện, nhận thao tác người dùng, kiểm tra dữ liệu nhập cơ bản và gọi service |
| **Backend** | Xử lý nghiệp vụ, CRUD, tìm kiếm, xác thực, mượn/trả, thống kê và truy vấn SQL |
| **Database** | Lưu trữ tài khoản, sách, độc giả và phiếu mượn/trả |

Frontend không chứa câu SQL hoặc tự mở kết nối database. Mọi xử lý dữ liệu được thực hiện thông qua project `Quanlythuvien.Services`. Với User, các form nghiệp vụ chỉ hiển thị khu vực tìm kiếm và kết quả; khu vực nhập liệu cùng các thao tác thay đổi dữ liệu được ẩn.

## 3. Actor và các chức năng của Actor

### 3.1. Actor: Quản trị viên (Admin)

Admin là tài khoản có `VaiTro = 'Admin'` trong bảng `dbo.TaiKhoan`.

Các chức năng của Admin:

- Đăng nhập hệ thống.
- Xem danh sách sách.
- Thêm sách.
- Sửa thông tin sách.
- Xóa sách.
- Tìm kiếm sách theo nhiều trường.
- Xem danh sách độc giả.
- Thêm độc giả.
- Sửa thông tin độc giả.
- Xóa độc giả.
- Tìm kiếm độc giả.
- Tạo phiếu mượn.
- Sửa phiếu mượn.
- Ghi nhận trả sách.
- Xóa phiếu mượn.
- Tìm kiếm phiếu mượn/trả.
- Xem thống kê tổng quan.
- Xem sách được mượn nhiều và phiếu mượn gần đây.

### 3.2. Actor: Người dùng/Nhân viên (User)

User là tài khoản có `VaiTro = 'User'` trong bảng `dbo.TaiKhoan`.

Các thao tác hiện có trên giao diện:

- Đăng nhập.
- Xem và tìm kiếm sách.
- Xem và tìm kiếm độc giả.
- Chọn hồ sơ độc giả của mình sau khi đăng nhập.
- Chỉ xem các phiếu mượn có `MaDocGia` trùng với hồ sơ đã chọn.

Admin được phép sử dụng các thao tác thay đổi dữ liệu. User chỉ nhìn thấy menu sách, thống kê và mượn/trả; không nhìn thấy menu quản lý độc giả. User phải chọn hồ sơ độc giả sau khi đăng nhập, sau đó màn hình mượn/trả chỉ tải các phiếu có `MaDocGia` tương ứng. Các nút thêm, sửa, xóa và trả sách được ẩn đối với User. Vai trò và hồ sơ được truyền từ màn hình đăng nhập xuống `MainForm` và các form nghiệp vụ.

## 4. Frontend

Frontend là phần giao diện Windows Forms. Frontend chỉ chịu trách nhiệm hiển thị và điều phối thao tác người dùng; không chịu trách nhiệm viết SQL.

### 4.1. Các form chính

| File | Vai trò |
|---|---|
| `database.cs` | Chứa giao diện `LoginForm`; gọi `AuthService` để xác thực |
| `Program.cs` | Khởi động ứng dụng và mở màn hình đăng nhập |
| `MainForm.cs` | Menu chính, mở các màn hình nghiệp vụ |
| `FrmSach.cs` | Hiển thị, thêm, sửa, xóa và tìm kiếm sách |
| `FrmDocGia.cs` | Hiển thị, thêm, sửa, xóa và tìm kiếm độc giả |
| `FrmMuonTra.cs` | Quản lý phiếu mượn, sửa phiếu, trả sách và tìm kiếm |
| `FrmThongKe.cs` | Hiển thị số liệu tổng quan và các bảng thống kê |

### 4.2. Quy trình hoạt động của frontend

1. `Program.cs` mở `LoginForm`.
2. Người dùng nhập tên đăng nhập và mật khẩu.
3. `LoginForm` gọi `AuthService.Authenticate`.
4. Nếu đăng nhập thành công, ứng dụng mở `MainForm`.
5. Người dùng chọn chức năng trên menu.
6. Form tương ứng đọc dữ liệu nhập, kiểm tra quyền giao diện, gọi service và gán kết quả vào `DataGridView`.
7. Khi thêm, sửa hoặc xóa thành công, form tải lại dữ liệu.

### 4.3. Các chức năng tìm kiếm trên frontend

- **Sách:** tất cả thông tin, mã sách, tên sách, tác giả, thể loại, năm xuất bản, nhà xuất bản.
- **Độc giả:** tất cả thông tin, mã độc giả, họ tên, số điện thoại, email, giới tính.
- **Phiếu mượn/trả:** mã phiếu, mã độc giả, tên độc giả, mã sách, tên sách, trạng thái, ngày mượn, ngày trả.

## 5. Backend và Service layer

Backend nằm trong project `Quanlythuvien.Services`. Đây là class library .NET Framework 4.7.2 tương thích với project WinForms non-SDK.

Backend gồm:

| File/Class | Trách nhiệm |
|---|---|
| `DatabaseConnection` | Tạo kết nối, kiểm tra database và cung cấp kết nối SQL Server |
| `AuthService` | Xác thực tên đăng nhập và mật khẩu |
| `BookService` | CRUD và tìm kiếm sách |
| `ReaderService` | CRUD và tìm kiếm độc giả |
| `LoanService` | CRUD, tìm kiếm phiếu mượn và ghi nhận trả sách |
| `StatisticsService` | Lấy số liệu tổng quan, sách phổ biến và phiếu gần đây |
| `LibraryStatistics` | Đối tượng chứa kết quả thống kê |

### 5.1. Các phương thức nghiệp vụ

#### `AuthService`

- `Authenticate(username, password)`: kiểm tra tài khoản trong `dbo.TaiKhoan`.

#### `BookService`

- `Search(keyword, field)`: tìm kiếm sách.
- `Add(...)`: thêm sách.
- `Update(...)`: cập nhật sách.
- `Delete(id)`: xóa sách.

#### `ReaderService`

- `Search(keyword, field)`: tìm kiếm độc giả.
- `Add(...)`: thêm độc giả.
- `Update(...)`: cập nhật độc giả.
- `Delete(id)`: xóa độc giả.

#### `LoanService`

- `Search(keyword, field)`: tìm kiếm phiếu mượn/trả.
- `Add(...)`: tạo phiếu mượn.
- `Update(...)`: sửa phiếu hoặc ghi nhận trả sách.
- `Delete(receipt)`: xóa phiếu mượn.

Khi trả sách, `NgayTra` được cập nhật ngày trả và `TrangThai` được cập nhật thành `Đã trả`.

#### `StatisticsService`

- `Get()`: trả về tổng số sách, tổng số độc giả, số phiếu đang mượn, số phiếu đã trả, top sách được mượn và các phiếu gần đây.

### 5.2. Nguyên tắc xử lý backend

- SQL chỉ nằm trong service/data-access layer.
- Dữ liệu đầu vào được truyền bằng `SqlParameter`.
- Không ghép trực tiếp dữ liệu người dùng vào câu SQL.
- Lỗi database được trả về form để hiển thị thông báo phù hợp.
- Quyền thao tác giao diện được xác định từ `UserAccount.IsAdmin`.
- Các service dùng `Microsoft.Data.SqlClient` và giữ tương thích C# 7.3.

## 6. Database

Database sử dụng SQL Server với database mặc định là `quanlythuvien`.

Cấu hình kết nối hiện nằm trong `Quanlythuvien.Services/DatabaseConnection.cs`:

```text
Server: LAPTOP-EBNPBAJE
Database: quanlythuvien
Authentication: Windows Integrated Security
Encrypt: True
TrustServerCertificate: True
Connect Timeout: 30 giây
```

Nếu tên SQL Server trên máy khác, thay đổi `ServerName` trong `DatabaseConnection.cs`.

### 6.1. Các bảng chính

#### `dbo.TaiKhoan`

Lưu thông tin đăng nhập:

- `ID`: khóa chính.
- `TenDangNhap`: tên đăng nhập.
- `MatKhau`: mật khẩu.
- `VaiTro`: `Admin` hoặc `User`.

#### `dbo.Sach`

Lưu thông tin sách:

- `MaSach`: khóa chính.
- `TenSach`: tên sách.
- `TacGia`: tác giả.
- `TheLoai`: thể loại.
- `NamXuatBan`: năm xuất bản.
- `NhaXuatBan`: nhà xuất bản.

#### `dbo.DocGia`

Lưu thông tin độc giả:

- `MaDocGia`: khóa chính.
- `HoTen`: họ tên.
- `NgaySinh`: ngày sinh.
- `GioiTinh`: giới tính.
- `SDT`: số điện thoại.
- `Email`: email.

#### `dbo.PhieuMuon`

Lưu thông tin mượn/trả:

- `MaPhieu`: khóa chính.
- `MaDocGia`: mã độc giả.
- `MaSach`: mã sách.
- `HoTen`: tên độc giả tại thời điểm lập phiếu.
- `TenSach`: tên sách tại thời điểm lập phiếu.
- `NgayMuon`: ngày mượn.
- `NgayTra`: ngày trả, có thể rỗng.
- `TrangThai`: trạng thái phiếu.

Quan hệ chính:

```text
DocGia 1 ---- n PhieuMuon
Sach   1 ---- n PhieuMuon
```

### 6.2. Bảo toàn schema và dữ liệu

File `Database.sql` không được thay đổi theo hướng xóa schema hoặc tạo lại bản ghi.

`DatabaseConnection.Initialize()` chỉ tạo database nếu database chưa tồn tại và kiểm tra kết nối sau đó. Phương thức này không:

- Xóa bảng.
- Reset schema.
- Xóa dữ liệu.
- Tạo lại các bản ghi hiện có.

Không nên chạy lại toàn bộ `Database.sql` trên database đang sử dụng nếu chưa kiểm tra nội dung script.

## 7. Build và chạy ứng dụng

1. Mở `Quanlythuvien.slnx` bằng Visual Studio 2026.
2. Kiểm tra SQL Server đang chạy.
3. Kiểm tra Windows account có quyền truy cập database.
4. Đặt project `Quanlythuvien` làm startup project.
5. Restore các package NuGet trong thư mục `packages` nếu cần.
6. Build solution với `Debug` hoặc `Release`.
7. Chạy ứng dụng và đăng nhập bằng tài khoản tồn tại trong `dbo.TaiKhoan`.

Project chính và service project phải tiếp tục sử dụng:

- .NET Framework 4.7.2.
- C# 7.3.
- `Microsoft.Data.SqlClient`.
- Project format non-SDK tương thích với solution hiện tại.

## 8. Nguyên tắc phát triển tiếp theo

- Form chỉ xử lý UI và gọi service.
- Không đưa SQL hoặc `SqlConnection` trực tiếp trở lại form.
- Nghiệp vụ mới phải được thêm vào service phù hợp.
- Mọi truy vấn có dữ liệu người dùng phải sử dụng tham số.
- Không sửa `Database.sql` theo hướng phá hủy schema hoặc dữ liệu.
- Giữ tương thích với .NET Framework 4.7.2 và C# 7.3.
