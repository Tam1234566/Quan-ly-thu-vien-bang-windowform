using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class FrmDocGia : Form
    {
        private DataGridView grid;
        private TextBox code, name, phone, email, search;
        private ComboBox searchField;
        private readonly ReaderService service = new ReaderService();
        private readonly bool canEdit;

        public FrmDocGia(bool canEdit = true)
        {
            this.canEdit = canEdit;
            Text = "Quản lý độc giả"; Size = new Size(1050, 600); StartPosition = FormStartPosition.CenterParent;
            Build(); LoadData();
        }

        private void Build()
        {
            FlowLayoutPanel bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 75, Padding = new Padding(8) };
            code = Box(bar, "Mã", 60); name = Box(bar, "Họ tên", 170); phone = Box(bar, "SDT", 110); email = Box(bar, "Email", 170);
            if (canEdit) { bar.Controls.Add(Button("Thêm", Add)); bar.Controls.Add(Button("Sửa", Edit)); bar.Controls.Add(Button("Xóa", Delete)); } bar.Controls.Add(Button("Reset", delegate { ClearInputs(); })); bar.Controls.Add(Button("Quay lại", delegate { Close(); }));
            bar.Visible = canEdit;
            FlowLayoutPanel find = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) };
            search = Box(find, "Tìm", 220);
            searchField = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            searchField.Items.AddRange(new object[] { "Tất cả", "Mã độc giả", "Họ tên", "Số điện thoại", "Email", "Giới tính" });
            searchField.SelectedIndex = 0;
            find.Controls.Add(new Label { Text = "Theo:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            find.Controls.Add(searchField);
            search.TextChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            searchField.SelectedIndexChanged += delegate { LoadData(search.Text, searchField.SelectedIndex); };
            find.Controls.Add(Button("Tìm", delegate { LoadData(search.Text, searchField.SelectedIndex); }));
            find.Controls.Add(Button("Hiện tất cả", delegate { search.Clear(); searchField.SelectedIndex = 0; LoadData(); }));
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            grid.CellClick += Select; Controls.Add(grid); Controls.Add(find); Controls.Add(bar);
        }

        private TextBox Box(FlowLayoutPanel panel, string label, int width) { panel.Controls.Add(new Label { Text = label + ":", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); TextBox box = new TextBox { Width = width }; panel.Controls.Add(box); return box; }
        private Button Button(string text, EventHandler handler) { Button button = new Button { Text = text, Width = 75, Height = 28 }; button.Click += handler; return button; }
        private string Value(DataGridViewRow row, string column) { return Convert.ToString(row.Cells[column].Value); }
        private void Select(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex < 0) return; DataGridViewRow row = grid.Rows[e.RowIndex]; code.Text = Value(row, "MaDocGia"); name.Text = Value(row, "Họ tên"); phone.Text = Value(row, "SDT"); email.Text = Value(row, "Email"); }
        private bool Read(out int id) { return int.TryParse(code.Text, out id) && name.Text.Trim() != ""; }

        private void Add(object sender, EventArgs e) { int id; if (!Read(out id)) { MessageBox.Show("Mã và họ tên không hợp lệ."); return; } Execute(delegate { service.Add(id, name.Text.Trim(), phone.Text, email.Text); }); LoadData(); ClearInputs(); }
        private void Edit(object sender, EventArgs e) { int id; if (!Read(out id)) { MessageBox.Show("Dữ liệu độc giả không hợp lệ."); return; } Execute(delegate { service.Update(id, name.Text.Trim(), phone.Text, email.Text); }); LoadData(); }
        private void Delete(object sender, EventArgs e) { if (grid.CurrentRow == null || MessageBox.Show("Xóa độc giả đã chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return; int id; if (!int.TryParse(Convert.ToString(grid.CurrentRow.Cells["MaDocGia"].Value), out id)) return; Execute(delegate { service.Delete(id); }); LoadData(); ClearInputs(); }
        private void LoadData(string keyword = "", int field = 0)
        {
            keyword = keyword == null ? "" : keyword.Trim();
            try { grid.DataSource = service.Search(keyword, field); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); }
        }
        private void ClearInputs() { code.Clear(); name.Clear(); phone.Clear(); email.Clear(); }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmDocGia
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FrmDocGia";
            this.Load += new System.EventHandler(this.FrmDocGia_Load);
            this.ResumeLayout(false);

        }

        private void FrmDocGia_Load(object sender, EventArgs e)
        {

        }

        private void Execute(Action action) { try { action(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi dữ liệu"); } }
    }
}