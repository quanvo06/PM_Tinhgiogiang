using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    /// <summary>
    /// Chuẩn hóa giao diện WinForms (Segoe UI 9.5pt, DataGridView Fill, FullRowSelect, bảng màu sáng UMP).
    /// </summary>
    internal static class WinFormStyleHelper
    {
        public static readonly Color AccentColor = Color.FromArgb(11, 107, 87);
        public static readonly Color SurfaceColor = Color.White;
        public static readonly Color BackgroundColor = Color.FromArgb(241, 245, 249);
        public static readonly Color SidebarColor = Color.FromArgb(248, 250, 252);
        public static readonly Font DefaultFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font BoldFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font TitleFont = new Font("Segoe UI", 12F, FontStyle.Bold);

        public static void ConfigureGrid(DataGridView dgv)
        {
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 242, 246);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgv.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            dgv.ColumnHeadersHeight = 32;
            dgv.RowTemplate.Height = 28;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(209, 250, 229);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(6, 78, 59);
            dgv.Font = DefaultFont;
        }
    }
}
