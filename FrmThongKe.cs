using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Quanlythuvien.Services;

namespace Quanlythuvien
{
    public class FrmThongKe : Form
    {
        private Label booksValue, readersValue, borrowingValue, returnedValue;
        private DataGridView popularGrid, recentGrid;
        private readonly StatisticsService service = new StatisticsService();

        public FrmThongKe()
        {
            Text = "Thống kê thư viện";
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);
            Build();
            LoadData();
        }

        private void Build()
        {
            Panel header = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(31, 78, 121) };
            Label title = new Label { Text = "THỐNG KÊ THƯ VIỆN", Dock = DockStyle.Top, Height = 52, ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            Label subtitle = new Label { Text = "Tổng quan hoạt động sách, độc giả và phiếu mượn", Dock = DockStyle.Bottom, Height = 30, ForeColor = Color.FromArgb(220, 232, 245), TextAlign = ContentAlignment.MiddleCenter };
            header.Controls.Add(subtitle); header.Controls.Add(title);

            TableLayoutPanel cards = new TableLayoutPanel { Dock = DockStyle.Top, Height = 125, Padding = new Padding(18, 15, 18, 8), ColumnCount = 4, RowCount = 1 };
            for (int i = 0; i < 4; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cards.Controls.Add(Card("ĐẦU SÁCH", Color.FromArgb(41, 128, 185), out booksValue), 0, 0);
            cards.Controls.Add(Card("ĐỘC GIẢ", Color.FromArgb(39, 144, 96), out readersValue), 1, 0);
            cards.Controls.Add(Card("ĐANG MƯỢN", Color.FromArgb(211, 117, 34), out borrowingValue), 2, 0);
            cards.Controls.Add(Card("ĐÃ TRẢ", Color.FromArgb(126, 87, 161), out returnedValue), 3, 0);

            TableLayoutPanel tables = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18, 8, 18, 10), ColumnCount = 2, RowCount = 1 };
            tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            popularGrid = Grid(); recentGrid = Grid();
            tables.Controls.Add(Section("SÁCH ĐƯỢC MƯỢN NHIỀU", popularGrid), 0, 0);
            tables.Controls.Add(Section("PHIẾU MƯỢN GẦN ĐÂY", recentGrid), 1, 0);

            FlowLayoutPanel footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            Button back = ActionButton("Quay lại", Color.Gray); back.Click += delegate { Close(); };
            Button refresh = ActionButton("Làm mới", Color.FromArgb(41, 128, 185)); refresh.Click += delegate { LoadData(); };
            footer.Controls.Add(back); footer.Controls.Add(refresh);

            Controls.Add(tables); Controls.Add(footer); Controls.Add(cards); Controls.Add(header);
        }

        private Panel Card(string caption, Color color, out Label value)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BackColor = color, Margin = new Padding(6) };
            Label name = new Label { Text = caption, Dock = DockStyle.Top, Height = 30, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            value = new Label { Text = "0", Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI", 25F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            panel.Controls.Add(value); panel.Controls.Add(name); return panel;
        }

        private Panel Section(string title, DataGridView table)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10), Margin = new Padding(6) };
            Label label = new Label { Text = title, Dock = DockStyle.Top, Height = 35, ForeColor = Color.FromArgb(31, 78, 121), Font = new Font("Segoe UI", 11F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            panel.Controls.Add(table); panel.Controls.Add(label); return panel;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmThongKe
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FrmThongKe";
            this.Load += new System.EventHandler(this.FrmThongKe_Load);
            this.ResumeLayout(false);

        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {

        }

        private DataGridView Grid()
        {
            DataGridView table = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            table.EnableHeadersVisualStyles = false; table.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 242); table.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold); table.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); return table;
        }

        private Button ActionButton(string text, Color color) { return new Button { Text = text, Width = 90, Height = 30, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; }

        private void LoadData()
        {
            try
            {
                LibraryStatistics statistics = service.Get();
                DataRow summary = statistics.Summary.Rows[0];
                booksValue.Text = Convert.ToString(summary["Books"]); readersValue.Text = Convert.ToString(summary["Readers"]); borrowingValue.Text = Convert.ToString(summary["Borrowing"]); returnedValue.Text = Convert.ToString(summary["Returned"]);
                popularGrid.DataSource = statistics.PopularBooks;
                recentGrid.DataSource = statistics.RecentLoans;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể tải thống kê", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

    }
}