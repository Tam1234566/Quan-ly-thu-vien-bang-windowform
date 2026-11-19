namespace Quanlythuvien
{
    public class FrmDangNhap : LoginForm
    {
        public FrmDangNhap() : base() { }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmDangNhap
            // 
            this.ClientSize = new System.Drawing.Size(402, 223);
            this.Name = "FrmDangNhap";
            this.Load += new System.EventHandler(this.FrmDangNhap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void FrmDangNhap_Load(object sender, System.EventArgs e)
        {

        }
    }
}