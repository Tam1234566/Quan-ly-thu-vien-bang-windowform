using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Quanlythuvien.Services
{
    public sealed class UserAccount
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsAdmin { get { return string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase); } }
    }

    public sealed class AuthService
    {
        public UserAccount Authenticate(string username, string password)
        {
            DatabaseConnection.Initialize();
            using (SqlConnection connection = DatabaseConnection.Open())
            using (SqlCommand command = new SqlCommand("SELECT TOP 1 TenDangNhap, VaiTro FROM dbo.TaiKhoan WHERE TenDangNhap=@u AND MatKhau=@p", connection))
            {
                command.Parameters.Add("@u", SqlDbType.NVarChar, 100).Value = username.Trim();
                command.Parameters.Add("@p", SqlDbType.NVarChar, 200).Value = password;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new UserAccount { Username = Convert.ToString(reader["TenDangNhap"]), Role = Convert.ToString(reader["VaiTro"]) };
                }
            }
        }
    }

    public sealed class BookService
    {
        public DataTable Search(string keyword, int field)
        {
            string[] fields = { "CONVERT(NVARCHAR(20), MaSach)", "CONVERT(NVARCHAR(20), MaSach)", "TenSach", "TacGia", "TheLoai", "CONVERT(NVARCHAR(10), NamXuatBan)", "NhaXuatBan" };
            string condition = field >= 1 && field <= 6 ? fields[field] + " LIKE @k" : string.Join(" OR ", new[] { fields[0] + " LIKE @k", "TenSach LIKE @k", "TacGia LIKE @k", "TheLoai LIKE @k", "CONVERT(NVARCHAR(10), NamXuatBan) LIKE @k", "NhaXuatBan LIKE @k" });
            return Query("SELECT MaSach,TenSach AS [Tên sách],TacGia AS [Tác giả],TheLoai AS [Thể loại],NamXuatBan AS [Năm XB],NhaXuatBan AS [Nhà XB] FROM dbo.Sach WHERE " + condition + " ORDER BY MaSach", P("@k", "%" + (keyword ?? string.Empty).Trim() + "%"));
        }
        public void Add(int id, string title, string author, string category, int year, string publisher) { Execute("INSERT INTO dbo.Sach(MaSach,TenSach,TacGia,TheLoai,NamXuatBan,NhaXuatBan) VALUES(@m,@t,@a,@g,@n,@x)", P("@m", id), P("@t", title), P("@a", Nullable(author)), P("@g", Nullable(category)), P("@n", year), P("@x", Nullable(publisher))); }
        public void Update(int id, string title, string author, string category, int year, string publisher) { Execute("UPDATE dbo.Sach SET TenSach=@t,TacGia=@a,TheLoai=@g,NamXuatBan=@n,NhaXuatBan=@x WHERE MaSach=@m", P("@m", id), P("@t", title), P("@a", Nullable(author)), P("@g", Nullable(category)), P("@n", year), P("@x", Nullable(publisher))); }
        public void Delete(int id) { Execute("DELETE FROM dbo.Sach WHERE MaSach=@m", P("@m", id)); }
        private static object Nullable(string value) { return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim(); }
        internal static SqlParameter P(string name, object value) { return new SqlParameter(name, value ?? DBNull.Value); }
        internal static DataTable Query(string sql, params SqlParameter[] parameters) { using (SqlConnection c = DatabaseConnection.Open()) using (SqlCommand q = new SqlCommand(sql, c)) using (SqlDataAdapter a = new SqlDataAdapter(q)) { q.Parameters.AddRange(parameters); DataTable t = new DataTable(); a.Fill(t); return t; } }
        internal static void Execute(string sql, params SqlParameter[] parameters) { using (SqlConnection c = DatabaseConnection.Open()) using (SqlCommand q = new SqlCommand(sql, c)) { q.Parameters.AddRange(parameters); q.ExecuteNonQuery(); } }
    }

    public sealed class ReaderService
    {
        public DataTable GetAll() { return BookService.Query("SELECT MaDocGia, HoTen FROM dbo.DocGia ORDER BY HoTen"); }
        public DataTable Search(string keyword, int field)
        {
            string[] fields = { "CONVERT(NVARCHAR(20), MaDocGia)", "CONVERT(NVARCHAR(20), MaDocGia)", "HoTen", "SDT", "Email", "GioiTinh" };
            string condition = field >= 1 && field <= 5 ? fields[field] + " LIKE @k" : string.Join(" OR ", new[] { fields[0] + " LIKE @k", "HoTen LIKE @k", "SDT LIKE @k", "Email LIKE @k", "GioiTinh LIKE @k" });
            return BookService.Query("SELECT MaDocGia,HoTen AS [Họ tên],NgaySinh AS [Ngày sinh],GioiTinh AS [Giới tính],SDT,Email FROM dbo.DocGia WHERE " + condition + " ORDER BY MaDocGia", BookService.P("@k", "%" + (keyword ?? string.Empty).Trim() + "%"));
        }
        public void Add(int id, string name, string phone, string email) { BookService.Execute("INSERT INTO dbo.DocGia(MaDocGia,HoTen,SDT,Email) VALUES(@m,@n,@p,@e)", BookService.P("@m", id), BookService.P("@n", name), BookService.P("@p", Empty(phone)), BookService.P("@e", Empty(email))); }
        public void Update(int id, string name, string phone, string email) { BookService.Execute("UPDATE dbo.DocGia SET HoTen=@n,SDT=@p,Email=@e WHERE MaDocGia=@m", BookService.P("@m", id), BookService.P("@n", name), BookService.P("@p", Empty(phone)), BookService.P("@e", Empty(email))); }
        public void Delete(int id) { BookService.Execute("DELETE FROM dbo.DocGia WHERE MaDocGia=@m", BookService.P("@m", id)); }
        private static object Empty(string value) { return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim(); }
    }

    public sealed class LoanService
    {
        public DataTable SearchForReader(int readerId)
        {
            return BookService.Query("SELECT MaPhieu,MaDocGia,HoTen AS [Độc giả],MaSach,TenSach AS [Sách],NgayMuon AS [Ngày mượn],NgayTra AS [Ngày trả],TrangThai AS [Trạng thái] FROM dbo.PhieuMuon WHERE MaDocGia=@d ORDER BY MaPhieu DESC", BookService.P("@d", readerId));
        }
        public DataTable Search(string keyword, int field)
        {
            string[] fields = { "CONVERT(NVARCHAR(20), MaPhieu)", "CONVERT(NVARCHAR(20), MaPhieu)", "CONVERT(NVARCHAR(20), MaDocGia)", "HoTen", "CONVERT(NVARCHAR(20), MaSach)", "TenSach", "TrangThai", "CONVERT(NVARCHAR(10), NgayMuon, 23)", "CONVERT(NVARCHAR(10), NgayTra, 23)" };
            string condition = field >= 1 && field <= 8 ? fields[field] + " LIKE @k" : string.Join(" OR ", new[] { fields[0] + " LIKE @k", fields[2] + " LIKE @k", fields[3] + " LIKE @k", fields[4] + " LIKE @k", fields[5] + " LIKE @k", fields[6] + " LIKE @k", fields[7] + " LIKE @k", fields[8] + " LIKE @k" });
            return BookService.Query("SELECT MaPhieu,MaDocGia,HoTen AS [Độc giả],MaSach,TenSach AS [Sách],NgayMuon AS [Ngày mượn],NgayTra AS [Ngày trả],TrangThai AS [Trạng thái] FROM dbo.PhieuMuon WHERE " + condition + " ORDER BY MaPhieu DESC", BookService.P("@k", "%" + (keyword ?? string.Empty).Trim() + "%"));
        }
        public void Add(int receipt, int reader, string readerName, int book, string bookName, DateTime borrow, DateTime? returned, string status) { Save("INSERT INTO dbo.PhieuMuon(MaPhieu,MaDocGia,HoTen,TenSach,MaSach,NgayMuon,NgayTra,TrangThai) VALUES(@p,@d,@h,@t,@s,@m,@r,@g)", receipt, reader, readerName, book, bookName, borrow, returned, status); }
        public void Update(int receipt, int reader, string readerName, int book, string bookName, DateTime borrow, DateTime? returned, string status) { Save("UPDATE dbo.PhieuMuon SET MaDocGia=@d,HoTen=@h,TenSach=@t,MaSach=@s,NgayMuon=@m,NgayTra=@r,TrangThai=@g WHERE MaPhieu=@p", receipt, reader, readerName, book, bookName, borrow, returned, status); }
        public void Delete(int receipt) { BookService.Execute("DELETE FROM dbo.PhieuMuon WHERE MaPhieu=@p", BookService.P("@p", receipt)); }
        private static void Save(string sql, int receipt, int reader, string readerName, int book, string bookName, DateTime borrow, DateTime? returned, string status) { BookService.Execute(sql, BookService.P("@p", receipt), BookService.P("@d", reader), BookService.P("@h", readerName), BookService.P("@t", bookName), BookService.P("@s", book), BookService.P("@m", borrow), BookService.P("@r", returned.HasValue ? (object)returned.Value : DBNull.Value), BookService.P("@g", string.IsNullOrWhiteSpace(status) ? "Đang mượn" : status.Trim())); }
    }

    public sealed class LibraryStatistics
    {
        public DataTable Summary { get; set; }
        public DataTable PopularBooks { get; set; }
        public DataTable RecentLoans { get; set; }
    }

    public sealed class StatisticsService
    {
        public LibraryStatistics Get() { return new LibraryStatistics { Summary = BookService.Query("SELECT (SELECT COUNT(*) FROM dbo.Sach) AS Books,(SELECT COUNT(*) FROM dbo.DocGia) AS Readers,(SELECT COUNT(*) FROM dbo.PhieuMuon WHERE NgayTra IS NULL) AS Borrowing,(SELECT COUNT(*) FROM dbo.PhieuMuon WHERE NgayTra IS NOT NULL) AS Returned"), PopularBooks = BookService.Query("SELECT TOP 10 TenSach AS [Tên sách], COUNT(*) AS [Lượt mượn] FROM dbo.PhieuMuon GROUP BY TenSach ORDER BY COUNT(*) DESC"), RecentLoans = BookService.Query("SELECT TOP 15 MaPhieu AS [Mã phiếu], HoTen AS [Độc giả], TenSach AS [Sách], NgayMuon AS [Ngày mượn], TrangThai AS [Trạng thái] FROM dbo.PhieuMuon ORDER BY MaPhieu DESC") }; }
    }
}
