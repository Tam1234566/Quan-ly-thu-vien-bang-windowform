using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class FrmSach : Form
    {
        private DataGridView grid;
        private TextBox code, title, author, category, year, publisher, search;
        private ComboBox searchField;
        private readonly BookService service = new BookService();
        private readonly bool canEdit;

        public FrmSach(bool canEdit = true)
        {
            this.canEdit = canEdit;
            Text = "Quản lý sách";
            Size = new Size(1100, 620);
            StartPosition = FormStartPosition.CenterParent;
            Build();
            LoadData();
        }

        private void Build()
        {
            FlowLayoutPanel bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 85, Padding = new Padding(8) };
            code = Box(bar, "Mã", 70); title = Box(bar, "Tên sách", 150); author = Box(bar, "Tác giả", 110);
            category = Box(bar, "Thể loại", 100); year = Box(bar, "Năm XB", 70); publisher = Box(bar, "Nhà XB", 130);
            if (canEdit) { bar.Controls.Add(Button("Thêm", Add)); bar.Controls.Add(Button("Sửa", Edit)); bar.Controls.Add(Button("Xóa", Delete)); }
            bar.Controls.Add(Button("Reset", delegate { ClearInputs(); })); bar.Controls.Add(Button("Quay lại", delegate { Close(); }));
            bar.Visible = canEdit;
            FlowLayoutPanel find = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) };
            search = Box(find, "Tìm kiếm", 220);
            searchField = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            searchField.Items.AddRange(new object[] { "Tất cả", "Mã sách", "Tên sách", "Tác giả", "Thể loại", "Năm xuất bản", "Nhà xuất bản" });
            searchField.SelectedIndex = 0;
            find.Controls.Add(new Label { Text = "Theo:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            find.Controls.Add(searchField);
            search.TextChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            searchField.SelectedIndexChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            find.Controls.Add(Button("Tìm", delegate { LoadData(search.Text, searchField.SelectedIndex); }));
            find.Controls.Add(Button("Hiện tất cả", delegate { search.Clear(); searchField.SelectedIndex = 0; LoadData(); }));
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            grid.CellClick += SelectBook;
            Controls.Add(grid); Controls.Add(find); Controls.Add(bar);
        }

        private TextBox Box(FlowLayoutPanel panel, string label, int width)
        {
            panel.Controls.Add(new Label { Text = label + ":", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            TextBox box = new TextBox { Width = width }; panel.Controls.Add(box); return box;
        }

        private Button Button(string text, EventHandler handler)
        {
            Button button = new Button { Text = text, Width = 75, Height = 28 }; button.Click += handler; return button;
        }

        private void SelectBook(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = grid.Rows[e.RowIndex];
            code.Text = Value(row, "MaSach"); title.Text = Value(row, "Tên sách"); author.Text = Value(row, "Tác giả");
            category.Text = Value(row, "Thể loại"); year.Text = Value(row, "Năm XB"); publisher.Text = Value(row, "Nhà XB");
        }

        private string Value(DataGridViewRow row, string column) { return Convert.ToString(row.Cells[column].Value); }
        private bool Read(out int id, out int publicationYear)
        {
            id = 0;
            publicationYear = 0;
            bool validId = int.TryParse(code.Text, out id);
            bool validYear = int.TryParse(year.Text, out publicationYear);
            return validId && validYear && publicationYear >= 1900 && title.Text.Trim() != "";
        }

        private void Add(object sender, EventArgs e)
        {
            int id, publicationYear; if (!Read(out id, out publicationYear)) { MessageBox.Show("Mã, tên và năm xuất bản không hợp lệ."); return; }
            Execute(delegate { service.Add(id, title.Text.Trim(), author.Text, category.Text, publicationYear, publisher.Text); });
            LoadData(); ClearInputs();
        }

        private void Edit(object sender, EventArgs e)
        {
            int id, publicationYear; if (!Read(out id, out publicationYear)) { MessageBox.Show("Dữ liệu sách không hợp lệ."); return; }
            Execute(delegate { service.Update(id, title.Text.Trim(), author.Text, category.Text, publicationYear, publisher.Text); });
            LoadData();
        }

        private void Delete(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || MessageBox.Show("Xóa sách đã chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int id; if (!int.TryParse(Convert.ToString(grid.CurrentRow.Cells["MaSach"].Value), out id)) return;
            Execute(delegate { service.Delete(id); }); LoadData(); ClearInputs();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmSach
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FrmSach";
            this.Load += new System.EventHandler(this.FrmSach_Load);
            this.ResumeLayout(false);

        }

        private void FrmSach_Load(object sender, EventArgs e)
        {

        }

        private void LoadData(string keyword = "", int field = 0)
        {
            try { grid.DataSource = service.Search(keyword, field); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); }
        }

        private void ClearInputs()
        {
            code.Clear(); title.Clear(); author.Clear(); category.Clear(); year.Clear(); publisher.Clear();
        }

        private void Execute(Action action) { try { action(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); } }
    }
}