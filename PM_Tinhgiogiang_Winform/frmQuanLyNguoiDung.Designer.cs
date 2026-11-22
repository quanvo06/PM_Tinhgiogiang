using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmQuanLyNguoiDung
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private SplitContainer splitMain;
        private Panel pnlSearch;
        private Label lblTim;
        private TextBox txtTim;
        private Button btnLamMoi;
        private DataGridView dgvNguoiDung;

        private Panel pnlForm;
        private Label lblFormTitle;
        private TableLayoutPanel tlpForm;
        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;
        private ComboBox cboVaiTro;
        private ComboBox cboGiangVien;
        private CheckBox chkKichHoat;
        private FlowLayoutPanel flpActions;
        private Button btnThemMoi;
        private Button btnLuu;
        private Button btnKhoaMo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Label TaoNhan(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Font = WinFormStyleHelper.BoldFont;
            return lbl;
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.splitMain = new SplitContainer();
            this.pnlSearch = new Panel();
            this.lblTim = new Label();
            this.txtTim = new TextBox();
            this.btnLamMoi = new Button();
            this.dgvNguoiDung = new DataGridView();

            this.pnlForm = new Panel();
            this.lblFormTitle = new Label();
            this.tlpForm = new TableLayoutPanel();
            this.txtTenDangNhap = new TextBox();
            this.txtMatKhau = new TextBox();
            this.cboVaiTro = new ComboBox();
            this.cboGiangVien = new ComboBox();
            this.chkKichHoat = new CheckBox();
            this.flpActions = new FlowLayoutPanel();
            this.btnThemMoi = new Button();
            this.btnLuu = new Button();
            this.btnKhoaMo = new Button();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Bước 6 — Quản trị người dùng && Phân quyền (SYS_NguoiDung)";

            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "BƯỚC 6 — QUẢN TRỊ TÀI KHOẢN && PHÂN QUYỀN HỆ THỐNG";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Vertical;
            WinFormStyleHelper.ConfigureSplitContainer(this.splitMain, 0.60, 420, 360, FixedPanel.Panel2);

            this.pnlSearch.Dock = DockStyle.Top;
            this.pnlSearch.Height = 40;
            this.pnlSearch.BackColor = Color.White;
            this.pnlSearch.Padding = new Padding(8, 6, 8, 6);

            this.lblTim.Text = "Tìm kiếm:";
            this.lblTim.Dock = DockStyle.Left;
            this.lblTim.Width = 74;
            this.lblTim.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTim.Font = WinFormStyleHelper.BoldFont;

            this.txtTim.Dock = DockStyle.Fill;
            this.txtTim.TextChanged += this.txtTim_TextChanged;

            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.Dock = DockStyle.Right;
            this.btnLamMoi.Width = 84;
            this.btnLamMoi.Click += this.btnLamMoi_Click;

            this.pnlSearch.Controls.Add(this.txtTim);
            this.pnlSearch.Controls.Add(this.lblTim);
            this.pnlSearch.Controls.Add(this.btnLamMoi);

            WinFormStyleHelper.ConfigureGrid(this.dgvNguoiDung);
            this.dgvNguoiDung.SelectionChanged += this.dgvNguoiDung_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvNguoiDung);
            this.splitMain.Panel1.Controls.Add(this.pnlSearch);

            this.pnlForm.Dock = DockStyle.Fill;
            this.pnlForm.BackColor = Color.White;
            this.pnlForm.Padding = new Padding(14);
            this.pnlForm.AutoScroll = true;

            this.lblFormTitle.Dock = DockStyle.Top;
            this.lblFormTitle.Height = 32;
            this.lblFormTitle.Text = "Thông tin tài khoản người dùng";
            this.lblFormTitle.Font = WinFormStyleHelper.BoldFont;
            this.lblFormTitle.ForeColor = WinFormStyleHelper.AccentColor;

            this.tlpForm.Dock = DockStyle.Top;
            this.tlpForm.AutoSize = true;
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this.txtTenDangNhap.Dock = DockStyle.Fill;
            this.txtMatKhau.Dock = DockStyle.Fill;
            this.txtMatKhau.UseSystemPasswordChar = true;
            this.cboVaiTro.Dock = DockStyle.Fill;
            this.cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboVaiTro.Items.AddRange(new object[] { "Admin", "GiaoVu", "TruongBoMon", "GiangVien" });
            this.cboVaiTro.SelectedIndex = 1;

            this.cboGiangVien.Dock = DockStyle.Fill;
            this.cboGiangVien.DropDownStyle = ComboBoxStyle.DropDownList;

            this.chkKichHoat.Text = "Tài khoản đang hoạt động";
            this.chkKichHoat.Checked = true;
            this.chkKichHoat.AutoSize = true;

            this.flpActions.Dock = DockStyle.Fill;
            this.flpActions.AutoSize = true;
            this.flpActions.Padding = new Padding(0, 8, 0, 0);

            this.btnThemMoi.Text = "+ Mới";
            this.btnThemMoi.Width = 76;
            this.btnThemMoi.Height = 30;
            this.btnThemMoi.Click += this.btnThemMoi_Click;

            this.btnLuu.Text = "Lưu";
            this.btnLuu.Width = 84;
            this.btnLuu.Height = 30;
            this.btnLuu.BackColor = WinFormStyleHelper.AccentColor;
            this.btnLuu.ForeColor = Color.White;
            this.btnLuu.FlatStyle = FlatStyle.Flat;
            this.btnLuu.Click += this.btnLuu_Click;

            this.btnKhoaMo.Text = "Khóa / Mở";
            this.btnKhoaMo.Width = 96;
            this.btnKhoaMo.Height = 30;
            this.btnKhoaMo.Click += this.btnKhoaMo_Click;

            this.flpActions.Controls.AddRange(new Control[] { this.btnThemMoi, this.btnLuu, this.btnKhoaMo });

            int r = 0;
            this.tlpForm.Controls.Add(TaoNhan("Tên đăng nhập *"), 0, r); this.tlpForm.Controls.Add(this.txtTenDangNhap, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Mật khẩu"), 0, r); this.tlpForm.Controls.Add(this.txtMatKhau, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Vai trò *"), 0, r); this.tlpForm.Controls.Add(this.cboVaiTro, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Giảng viên LK"), 0, r); this.tlpForm.Controls.Add(this.cboGiangVien, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Trạng thái"), 0, r); this.tlpForm.Controls.Add(this.chkKichHoat, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan(""), 0, r); this.tlpForm.Controls.Add(this.flpActions, 1, r++);

            this.pnlForm.Controls.Add(this.tlpForm);
            this.pnlForm.Controls.Add(this.lblFormTitle);
            this.splitMain.Panel2.Controls.Add(this.pnlForm);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
