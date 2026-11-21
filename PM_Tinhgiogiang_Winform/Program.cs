using System;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (frmDangNhap loginForm = new frmDangNhap())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new frmMain());
                }
            }
        }
    }
}
