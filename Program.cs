using System;
using System.Windows.Forms;

namespace Quanlythuvien
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK && login.Authenticated)
                    Application.Run(new MainForm(login.CurrentUser, login.CurrentReaderId));
            }
        }
    }
}
