using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class FrmMuonTra : Form
    {
        private DataGridView grid;
        private TextBox receiptCode, readerCode, readerName, bookCode, bookName, status, search;
        private DateTimePicker borrowDate, returnDate;
        private ComboBox searchField;
        private readonly LoanService service = new LoanService();
        private readonly bool canEdit;
        private readonly int? readerId;

        public FrmMuonTra(bool canEdit = true, int? readerId = null)
        {
            this.canEdit = canEdit;
            this.readerId = readerId;
            Text = "Quản lý phiếu mượn - trả"; Size = new Size(1150, 650); StartPosition = FormStartPosition.CenterParent;
            Build(); LoadData();
        }

        private void Build()
        {
            FlowLayoutPanel bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 115, Padding = new Padding(8) };
            receiptCode = Box(bar, "Mã phiếu", 65); readerCode = Box(bar, "Mã độc giả", 70); readerName = Box(bar, "Họ tên", 140);
            bookCode = Box(bar, "Mã sách", 65); bookName = Box(bar, "Tên sách", 140); status = Box(bar, "Trạng thái", 100);
            borrowDate = DateBox(bar, "Ngày mượn"); returnDate = DateBox(bar, "Ngày trả");
            if (canEdit) { bar.Controls.Add(Button("Thêm", Add)); bar.Controls.Add(Button("Sửa", Edit)); bar.Controls.Add(Button("Trả sách", ReturnBook)); bar.Controls.Add(Button("Xóa", Delete)); }
            bar.Controls.Add(Button("Reset", delegate { ClearInputs(); })); bar.Controls.Add(Button("Quay lại", delegate { Close(); }));
            bar.Visible = canEdit;
            FlowLayoutPanel find = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) };
            search = Box(find, "Tìm kiếm", 220);
            searchField = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            searchField.Items.AddRange(new object[] { "Tất cả", "Mã phiếu", "Mã độc giả", "Tên độc giả", "Mã sách", "Tên sách", "Trạng thái", "Ngày mượn", "Ngày trả" });
            searchField.SelectedIndex = 0;
            find.Controls.Add(new Label { Text = "Theo:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            find.Controls.Add(searchField);
            search.TextChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            searchField.SelectedIndexChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            find.Controls.Add(Button("Tìm", delegate { LoadData(search.Text, searchField.SelectedIndex); }));
            find.Controls.Add(Button("Hiện tất cả", delegate { search.Clear(); searchField.SelectedIndex = 0; LoadData(); }));
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            grid.CellClick += SelectLoan; Controls.Add(grid); Controls.Add(find); Controls.Add(bar);
        }

        private TextBox Box(FlowLayoutPanel panel, string label, int width) { panel.Controls.Add(new Label { Text = label + ":", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); TextBox box = new TextBox { Width = width }; panel.Controls.Add(box); return box; }
        private DateTimePicker DateBox(FlowLayoutPanel panel, string label) { panel.Controls.Add(new Label { Text = label + ":", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); DateTimePicker picker = new DateTimePicker { Width = 105, Format = DateTimePickerFormat.Short, ShowCheckBox = true }; picker.Checked = false; panel.Controls.Add(picker); return picker; }
        private Button Button(string text, EventHandler handler) { Button button = new Button { Text = text, Width = 75, Height = 28 }; button.Click += handler; return button; }
        private string Value(DataGridViewRow row, string column) { return Convert.ToString(row.Cells[column].Value); }
        private DateTime? DateValue(DateTimePicker picker) { return picker.Checked ? (DateTime?)picker.Value.Date : null; }
        private void SelectLoan(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; DataGridViewRow row = grid.Rows[e.RowIndex];
            receiptCode.Text = Value(row, "MaPhieu"); readerCode.Text = Value(row, "MaDocGia"); readerName.Text = Value(row, "Độc giả"); bookCode.Text = Value(row, "MaSach"); bookName.Text = Value(row, "Sách"); status.Text = Value(row, "Trạng thái");
            DateTime date; borrowDate.Checked = DateTime.TryParse(Value(row, "Ngày mượn"), out date); if (borrowDate.Checked) borrowDate.Value = date;
            returnDate.Checked = DateTime.TryParse(Value(row, "Ngày trả"), out date); if (returnDate.Checked) returnDate.Value = date;
        }

        private bool Read(out int receipt, out int reader, out int book)
        {
            receipt = 0;
            reader = 0;
            book = 0;
            bool validReceipt = int.TryParse(receiptCode.Text, out receipt);
            bool validReader = int.TryParse(readerCode.Text, out reader);
            bool validBook = int.TryParse(bookCode.Text, out book);
            return validReceipt && validReader && validBook && readerName.Text.Trim() != "" && bookName.Text.Trim() != "" && borrowDate.Checked;
        }

        private void Add(object sender, EventArgs e)
        {
            int receipt, reader, book; if (!Read(out receipt, out reader, out book)) { MessageBox.Show("Vui lòng nhập đầy đủ mã phiếu, độc giả, sách và ngày mượn."); return; }
            Execute(delegate { service.Add(receipt, reader, readerName.Text.Trim(), book, bookName.Text.Trim(), borrowDate.Value.Date, DateValue(returnDate), status.Text); });
            LoadData(); ClearInputs();
        }

        private void Edit(object sender, EventArgs e)
        {
            int receipt, reader, book; if (!Read(out receipt, out reader, out book)) { MessageBox.Show("Dữ liệu phiếu mượn không hợp lệ."); return; }
            Execute(delegate { service.Update(receipt, reader, readerName.Text.Trim(), book, bookName.Text.Trim(), borrowDate.Value.Date, DateValue(returnDate), status.Text); });
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmMuonTra
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FrmMuonTra";
            this.Load += new System.EventHandler(this.FrmMuonTra_Load);
            this.ResumeLayout(false);

        }

        private void FrmMuonTra_Load(object sender, EventArgs e)
        {

        }

        private void ReturnBook(object sender, EventArgs e) { returnDate.Checked = true; returnDate.Value = DateTime.Today; status.Text = "Đã trả"; Edit(sender, e); }
        private void Delete(object sender, EventArgs e) { if (grid.CurrentRow == null || MessageBox.Show("Xóa phiếu đã chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return; int id; if (!int.TryParse(Convert.ToString(grid.CurrentRow.Cells["MaPhieu"].Value), out id)) return; Execute(delegate { service.Delete(id); }); LoadData(); ClearInputs(); }
        private void LoadData(string keyword = "", int field = 0)
        {
            keyword = keyword == null ? "" : keyword.Trim();
            try { grid.DataSource = readerId.HasValue ? service.SearchForReader(readerId.Value) : service.Search(keyword, field); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); }
        }
        private void ClearInputs() { receiptCode.Clear(); readerCode.Clear(); readerName.Clear(); bookCode.Clear(); bookName.Clear(); status.Text = "Đang mượn"; borrowDate.Checked = false; returnDate.Checked = false; }
        private void Execute(Action action) { try { action(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); } }
    }
}