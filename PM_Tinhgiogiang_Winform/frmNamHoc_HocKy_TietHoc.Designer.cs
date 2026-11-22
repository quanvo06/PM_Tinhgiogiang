using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmNamHoc_HocKy_TietHoc
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private TabControl tabMain;
        private TabPage tpNamHoc;
        private TabPage tpHocKy;
        private TabPage tpTietHoc;

        // Tab Năm học
        private SplitContainer splitNH;
        private DataGridView dgvNamHoc;
        private TextBox txtMaNH, txtTenNH;
        private DateTimePicker dtpNhBD, dtpNhKT;
        private Button btnNhMoi, btnNhLuu, btnNhXoa;

        // Tab Học kỳ
        private SplitContainer splitHK;
        private DataGridView dgvHocKy;
        private ComboBox cboHkNamHoc;
        private TextBox txtMaHK, txtTenHK;
        private DateTimePicker dtpHkBD, dtpHkKT;
        private Button btnHkMoi, btnHkLuu, btnHkXoa;

        // Tab Tiết học
        private SplitContainer splitTiet;
        private DataGridView dgvTietHoc;
        private NumericUpDown numSoTiet;
        private ComboBox cboBuoiTiet;
        private TextBox txtGioBD, txtGioKT;
        private Button btnKhoiTao10Tiet, btnTietLuu;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Label TaoNhan(string text)
        {
            return new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = WinFormStyleHelper.BoldFont };
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.tabMain = new TabControl();
            this.tpNamHoc = new TabPage("Năm học (DM_NamHoc)");
            this.tpHocKy = new TabPage("Học kỳ (DM_HocKy)");
            this.tpTietHoc = new TabPage("Khung 10 Tiết học (DM_TietHoc)");

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Cấu hình Năm học, Học kỳ && Khung 10 Tiết học";

            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "CẤU HÌNH NĂM HỌC, HỌC KỲ && KHUNG 10 TIẾT HỌC CHUẨN UMP";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.tabMain.Dock = DockStyle.Fill;

            // === TAB 1: NĂM HỌC ===
            this.splitNH = new SplitContainer { Dock = DockStyle.Fill };
            WinFormStyleHelper.ConfigureSplitContainer(this.splitNH, 0.58, 420, 360, FixedPanel.Panel2);
            this.dgvNamHoc = new DataGridView();
            WinFormStyleHelper.ConfigureGrid(this.dgvNamHoc);
            this.dgvNamHoc.SelectionChanged += this.dgvNamHoc_SelectionChanged;
            this.splitNH.Panel1.Controls.Add(this.dgvNamHoc);

            Panel pnlNhRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            TableLayoutPanel tblNH = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            tblNH.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblNH.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.txtMaNH = new TextBox { Dock = DockStyle.Fill };
            this.txtTenNH = new TextBox { Dock = DockStyle.Fill };
            this.dtpNhBD = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.dtpNhKT = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.btnNhMoi = new Button { Text = "+ Mới", Width = 76, Height = 30 }; this.btnNhMoi.Click += this.btnNhMoi_Click;
            this.btnNhLuu = new Button { Text = "Lưu", Width = 86, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnNhLuu.Click += this.btnNhLuu_Click;
            this.btnNhXoa = new Button { Text = "Xóa", Width = 76, Height = 30 }; this.btnNhXoa.Click += this.btnNhXoa_Click;
            FlowLayoutPanel flpNH = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
            flpNH.Controls.AddRange(new Control[] { this.btnNhMoi, this.btnNhLuu, this.btnNhXoa });
            tblNH.Controls.Add(TaoNhan("Mã năm học *"), 0, 0); tblNH.Controls.Add(this.txtMaNH, 1, 0);
            tblNH.Controls.Add(TaoNhan("Tên năm học *"), 0, 1); tblNH.Controls.Add(this.txtTenNH, 1, 1);
            tblNH.Controls.Add(TaoNhan("Ngày bắt đầu"), 0, 2); tblNH.Controls.Add(this.dtpNhBD, 1, 2);
            tblNH.Controls.Add(TaoNhan("Ngày kết thúc"), 0, 3); tblNH.Controls.Add(this.dtpNhKT, 1, 3);
            tblNH.Controls.Add(TaoNhan(""), 0, 4); tblNH.Controls.Add(flpNH, 1, 4);
            pnlNhRight.Controls.Add(tblNH);
            this.splitNH.Panel2.Controls.Add(pnlNhRight);
            this.tpNamHoc.Controls.Add(this.splitNH);

            // === TAB 2: HỌC KỲ ===
            this.splitHK = new SplitContainer { Dock = DockStyle.Fill };
            WinFormStyleHelper.ConfigureSplitContainer(this.splitHK, 0.58, 420, 360, FixedPanel.Panel2);
            this.dgvHocKy = new DataGridView();
            WinFormStyleHelper.ConfigureGrid(this.dgvHocKy);
            this.dgvHocKy.SelectionChanged += this.dgvHocKy_SelectionChanged;
            this.splitHK.Panel1.Controls.Add(this.dgvHocKy);

            Panel pnlHkRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            TableLayoutPanel tblHK = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            tblHK.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblHK.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.cboHkNamHoc = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.txtMaHK = new TextBox { Dock = DockStyle.Fill };
            this.txtTenHK = new TextBox { Dock = DockStyle.Fill };
            this.dtpHkBD = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.dtpHkKT = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.btnHkMoi = new Button { Text = "+ Mới", Width = 76, Height = 30 }; this.btnHkMoi.Click += this.btnHkMoi_Click;
            this.btnHkLuu = new Button { Text = "Lưu", Width = 86, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnHkLuu.Click += this.btnHkLuu_Click;
            this.btnHkXoa = new Button { Text = "Xóa", Width = 76, Height = 30 }; this.btnHkXoa.Click += this.btnHkXoa_Click;
            FlowLayoutPanel flpHK = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
            flpHK.Controls.AddRange(new Control[] { this.btnHkMoi, this.btnHkLuu, this.btnHkXoa });
            tblHK.Controls.Add(TaoNhan("Năm học *"), 0, 0); tblHK.Controls.Add(this.cboHkNamHoc, 1, 0);
            tblHK.Controls.Add(TaoNhan("Mã học kỳ *"), 0, 1); tblHK.Controls.Add(this.txtMaHK, 1, 1);
            tblHK.Controls.Add(TaoNhan("Tên học kỳ *"), 0, 2); tblHK.Controls.Add(this.txtTenHK, 1, 2);
            tblHK.Controls.Add(TaoNhan("Ngày bắt đầu"), 0, 3); tblHK.Controls.Add(this.dtpHkBD, 1, 3);
            tblHK.Controls.Add(TaoNhan("Ngày kết thúc"), 0, 4); tblHK.Controls.Add(this.dtpHkKT, 1, 4);
            tblHK.Controls.Add(TaoNhan(""), 0, 5); tblHK.Controls.Add(flpHK, 1, 5);
            pnlHkRight.Controls.Add(tblHK);
            this.splitHK.Panel2.Controls.Add(pnlHkRight);
            this.tpHocKy.Controls.Add(this.splitHK);

            // === TAB 3: KHUNG 10 TIẾT ===
            this.splitTiet = new SplitContainer { Dock = DockStyle.Fill };
            WinFormStyleHelper.ConfigureSplitContainer(this.splitTiet, 0.58, 420, 360, FixedPanel.Panel2);
            this.dgvTietHoc = new DataGridView();
            WinFormStyleHelper.ConfigureGrid(this.dgvTietHoc);
            this.dgvTietHoc.SelectionChanged += this.dgvTietHoc_SelectionChanged;
            this.splitTiet.Panel1.Controls.Add(this.dgvTietHoc);

            Panel pnlTietRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            TableLayoutPanel tblTiet = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            tblTiet.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tblTiet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.numSoTiet = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 15, Value = 1 };
            this.cboBuoiTiet = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboBuoiTiet.Items.AddRange(new object[] { "Sang", "Chieu" });
            this.cboBuoiTiet.SelectedIndex = 0;
            this.txtGioBD = new TextBox { Dock = DockStyle.Fill, Text = "07:00" };
            this.txtGioKT = new TextBox { Dock = DockStyle.Fill, Text = "07:50" };
            this.btnTietLuu = new Button { Text = "Lưu tiết", Width = 92, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnTietLuu.Click += this.btnTietLuu_Click;
            this.btnKhoiTao10Tiet = new Button { Text = "Khởi tạo 10 tiết chuẩn UMP", Width = 200, Height = 30 }; this.btnKhoiTao10Tiet.Click += this.btnKhoiTao10Tiet_Click;
            FlowLayoutPanel flpTiet = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
            flpTiet.Controls.AddRange(new Control[] { this.btnTietLuu, this.btnKhoiTao10Tiet });
            tblTiet.Controls.Add(TaoNhan("Số tiết (1-10)"), 0, 0); tblTiet.Controls.Add(this.numSoTiet, 1, 0);
            tblTiet.Controls.Add(TaoNhan("Buổi"), 0, 1); tblTiet.Controls.Add(this.cboBuoiTiet, 1, 1);
            tblTiet.Controls.Add(TaoNhan("Giờ bắt đầu"), 0, 2); tblTiet.Controls.Add(this.txtGioBD, 1, 2);
            tblTiet.Controls.Add(TaoNhan("Giờ kết thúc"), 0, 3); tblTiet.Controls.Add(this.txtGioKT, 1, 3);
            tblTiet.Controls.Add(TaoNhan(""), 0, 4); tblTiet.Controls.Add(flpTiet, 1, 4);
            pnlTietRight.Controls.Add(tblTiet);
            this.splitTiet.Panel2.Controls.Add(pnlTietRight);
            this.tpTietHoc.Controls.Add(this.splitTiet);

            this.tabMain.TabPages.AddRange(new TabPage[] { this.tpNamHoc, this.tpHocKy, this.tpTietHoc });
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
