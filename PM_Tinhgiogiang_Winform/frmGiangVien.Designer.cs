using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmGiangVien
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitMain;
        private Panel pnlToolbar;
        private TextBox txtTimKiem;
        private Button btnLamMoi;
        private DataGridView dgvGiangVien;
        private TabControl tabChiTiet;
        private TabPage tpThongTin, tpRangBuoc, tpMienGiam, tpTaiGio, tpLich;
        private TextBox txtMaGV, txtHoTen, txtEmail;
        private ComboBox cboBoMon, cboHocVi;
        private NumericUpDown numDinhMuc;
        private CheckBox chkThinhGiang;
        private Button btnThemMoi, btnLuu, btnNgungHD;
        private DataGridView dgvRangBuoc, dgvMienGiam, dgvLichGV;
        private Label lblTaiGioSummary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new SplitContainer();
            this.pnlToolbar = new Panel();
            this.txtTimKiem = new TextBox();
            this.btnLamMoi = new Button();
            this.dgvGiangVien = new DataGridView();
            this.tabChiTiet = new TabControl();
            this.tpThongTin = new TabPage("Thông tin");
            this.tpRangBuoc = new TabPage("Ràng buộc bận");
            this.tpMienGiam = new TabPage("Miễn giảm");
            this.tpTaiGio = new TabPage("Tải giờ");
            this.tpLich = new TabPage("Lịch");

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.Text = "Quản lý Giảng viên (4.4 & 4.5)";

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 620;

            // Left: Toolbar + Grid
            this.pnlToolbar.Dock = DockStyle.Top;
            this.pnlToolbar.Height = 36;
            this.txtTimKiem.Dock = DockStyle.Left;
            this.txtTimKiem.Width = 260;
            this.txtTimKiem.TextChanged += this.txtTimKiem_TextChanged;
            this.btnLamMoi.Text = "↻ Làm mới";
            this.btnLamMoi.Dock = DockStyle.Right;
            this.btnLamMoi.Width = 95;
            this.btnLamMoi.Click += this.btnLamMoi_Click;
            this.pnlToolbar.Controls.Add(this.txtTimKiem);
            this.pnlToolbar.Controls.Add(this.btnLamMoi);

            WinFormStyleHelper.ConfigureGrid(this.dgvGiangVien);
            this.dgvGiangVien.SelectionChanged += this.dgvGiangVien_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvGiangVien);
            this.splitMain.Panel1.Controls.Add(this.pnlToolbar);

            // Right: TabControl 5 tabs
            this.tabChiTiet.Dock = DockStyle.Fill;

            // Tab 1: Thông tin
            TableLayoutPanel tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 9, Padding = new Padding(8) };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.txtMaGV = new TextBox { Dock = DockStyle.Fill };
            this.txtHoTen = new TextBox { Dock = DockStyle.Fill };
            this.cboBoMon = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboHocVi = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.numDinhMuc = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 1000, Value = 270 };
            this.chkThinhGiang = new CheckBox { Text = "Giảng viên thỉnh giảng", AutoSize = true };
            this.txtEmail = new TextBox { Dock = DockStyle.Fill };

            tbl.Controls.Add(new Label { Text = "Mã GV *", AutoSize = true }, 0, 0); tbl.Controls.Add(this.txtMaGV, 1, 0);
            tbl.Controls.Add(new Label { Text = "Họ tên *", AutoSize = true }, 0, 1); tbl.Controls.Add(this.txtHoTen, 1, 1);
            tbl.Controls.Add(new Label { Text = "Bộ môn *", AutoSize = true }, 0, 2); tbl.Controls.Add(this.cboBoMon, 1, 2);
            tbl.Controls.Add(new Label { Text = "Học vị", AutoSize = true }, 0, 3); tbl.Controls.Add(this.cboHocVi, 1, 3);
            tbl.Controls.Add(new Label { Text = "Định mức", AutoSize = true }, 0, 4); tbl.Controls.Add(this.numDinhMuc, 1, 4);
            tbl.Controls.Add(new Label { Text = "Loại hình", AutoSize = true }, 0, 5); tbl.Controls.Add(this.chkThinhGiang, 1, 5);
            tbl.Controls.Add(new Label { Text = "Email", AutoSize = true }, 0, 6); tbl.Controls.Add(this.txtEmail, 1, 6);

            FlowLayoutPanel flp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            this.btnThemMoi = new Button { Text = "+ Mới", Width = 75, Height = 30 }; this.btnThemMoi.Click += this.btnThemMoi_Click;
            this.btnLuu = new Button { Text = "Lưu", Width = 85, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnLuu.Click += this.btnLuu_Click;
            this.btnNgungHD = new Button { Text = "Ngừng HĐ", Width = 95, Height = 30 }; this.btnNgungHD.Click += this.btnNgungHD_Click;
            flp.Controls.AddRange(new Control[] { this.btnThemMoi, this.btnLuu, this.btnNgungHD });
            tbl.Controls.Add(flp, 0, 7); tbl.SetColumnSpan(flp, 2);
            this.tpThongTin.Controls.Add(tbl);

            // Tab 2..5
            this.dgvRangBuoc = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvRangBuoc); this.tpRangBuoc.Controls.Add(this.dgvRangBuoc);
            this.dgvMienGiam = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvMienGiam); this.tpMienGiam.Controls.Add(this.dgvMienGiam);
            this.lblTaiGioSummary = new Label { Dock = DockStyle.Fill, Padding = new Padding(12), Text = "Chọn giảng viên để xem tải giờ." }; this.tpTaiGio.Controls.Add(this.lblTaiGioSummary);
            this.dgvLichGV = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvLichGV); this.tpLich.Controls.Add(this.dgvLichGV);

            this.tabChiTiet.TabPages.AddRange(new TabPage[] { this.tpThongTin, this.tpRangBuoc, this.tpMienGiam, this.tpTaiGio, this.tpLich });
            this.splitMain.Panel2.Controls.Add(this.tabChiTiet);
            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
