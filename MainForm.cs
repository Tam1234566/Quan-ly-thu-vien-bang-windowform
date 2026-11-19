using System;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class MainForm : Form
    {
        private readonly UserAccount currentUser;
        private readonly int? currentReaderId;

        public MainForm(UserAccount user, int? readerId)
        {
            currentUser = user;
            currentReaderId = readerId;
            Text = "Quản lý thư viện";
            ClientSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BuildMenu();
        }

        private void BuildMenu()
        {
            Label header = new Label
            {
                Text = "QUẢN LÝ THƯ VIỆN - " + currentUser.Role + ": " + currentUser.Username,
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(31, 78, 121),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            TableLayoutPanel menu = new TableLayoutPanel
            {
                Location = new Point(100, 145),
                Size = new Size(800, 360),
                ColumnCount = 2,
                RowCount = 3
            };
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 3; i++)
                menu.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));

            menu.Controls.Add(CreateMenuButton("QUẢN LÝ SÁCH", delegate { new FrmSach(currentUser.IsAdmin).ShowDialog(this); }, Color.SteelBlue), 0, 0);
            Button readers = CreateMenuButton("QUẢN LÝ ĐỘC GIẢ", delegate { new FrmDocGia(true).ShowDialog(this); }, Color.SeaGreen);
            readers.Visible = currentUser.IsAdmin;
            menu.Controls.Add(readers, 1, 0);
            menu.Controls.Add(CreateMenuButton("MƯỢN / TRẢ SÁCH", delegate { new FrmMuonTra(currentUser.IsAdmin, currentReaderId).ShowDialog(this); }, Color.DarkOrange), 0, 1);
            menu.Controls.Add(CreateMenuButton("THỐNG KÊ", delegate { new FrmThongKe().ShowDialog(this); }, Color.MediumPurple), 1, 1);
            menu.Controls.Add(CreateMenuButton("THOÁT", delegate { Close(); }, Color.Gray), 0, 2);
            menu.SetColumnSpan(menu.GetControlFromPosition(0, 2), 2);

            Controls.Add(menu);
            Controls.Add(header);
        }

        private Button CreateMenuButton(string text, EventHandler click, Color color)
        {
            Button button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(12),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += click;
            return button;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
