using System;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class LoginForm : Form
    {
        private TextBox username, password;
        private readonly AuthService authService = new AuthService();
        private readonly ReaderService readerService = new ReaderService();
        public UserAccount CurrentUser { get; private set; }
        public int? CurrentReaderId { get; private set; }
        public bool Authenticated { get { return CurrentUser != null; } }
        public LoginForm() { Text = "Đăng nhập hệ thống"; ClientSize = new Size(460, 310); StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Build(); }
        private void Build()
        {
            Controls.Add(new Label { Text = "ĐĂNG NHẬP\nHệ thống quản lý thư viện", Dock = DockStyle.Top, Height = 85, BackColor = Color.FromArgb(31, 78, 121), ForeColor = Color.White, Font = new Font("Segoe UI", 17, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter });
            Controls.Add(new Label { Text = "Tên đăng nhập", Location = new Point(55, 105), AutoSize = true }); username = new TextBox { Location = new Point(55, 126), Width = 350 }; Controls.Add(username);
            Controls.Add(new Label { Text = "Mật khẩu", Location = new Point(55, 165), AutoSize = true }); password = new TextBox { Location = new Point(55, 186), Width = 350, UseSystemPasswordChar = true }; Controls.Add(password);
            CheckBox show = new CheckBox { Text = "Hiện mật khẩu", Location = new Point(55, 218), AutoSize = true }; show.CheckedChanged += delegate { password.UseSystemPasswordChar = !show.Checked; }; Controls.Add(show);
            Button login = Button("Đăng nhập", 55, 250, Color.SteelBlue); login.Click += Login; Button exit = Button("Thoát", 240, 250, Color.Gray); exit.Click += delegate { DialogResult = DialogResult.Cancel; Close(); }; Controls.Add(login); Controls.Add(exit); AcceptButton = login; CancelButton = exit;
        }
        private Button Button(string text, int x, int y, Color color) { return new Button { Text = text, Location = new Point(x, y), Width = 165, Height = 35, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; }
        private void Login(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text)) { MessageBox.Show("Vui lòng nhập đầy đủ thông tin."); return; }
            try
            {
                CurrentUser = authService.Authenticate(username.Text.Trim(), password.Text);
                if (CurrentUser == null) { MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng."); return; }
                if (!CurrentUser.IsAdmin && !SelectReader()) { CurrentUser = null; return; }
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi kết nối"); }
        }

        private bool SelectReader()
        {
            using (Form form = new Form { Text = "Chọn hồ sơ độc giả", ClientSize = new Size(360, 140), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                ComboBox readers = new ComboBox { Location = new Point(20, 25), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
                Button ok = new Button { Text = "Xác nhận", Location = new Point(140, 75), Width = 100, DialogResult = DialogResult.OK };
                readers.DataSource = readerService.GetAll(); readers.DisplayMember = "HoTen"; readers.ValueMember = "MaDocGia";
                form.Controls.Add(readers); form.Controls.Add(ok); form.AcceptButton = ok;
                if (form.ShowDialog(this) != DialogResult.OK || readers.SelectedValue == null) return false;
                CurrentReaderId = Convert.ToInt32(readers.SelectedValue); return true;
            }
        }
    }
}