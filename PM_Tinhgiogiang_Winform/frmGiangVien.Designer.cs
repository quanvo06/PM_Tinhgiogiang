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
        private DateTimePicker dtpRbBD, dtpRbKT;
        private ComboBox cboRbThu, cboRbBuoi, cboMgNamHoc;
        private TextBox txtRbLyDo, txtMgLyDo;
        private NumericUpDown numMgTyLe, numMgSoGio;
        private Button btnThemRB, btnXoaRB, btnThemMG, btnXoaMG;
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

            // Tab 2: Ràng buộc bận (Grid + Form Thêm/Xóa)
            this.dgvRangBuoc = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvRangBuoc);
            Panel pnlRbAdd = new Panel { Dock = DockStyle.Bottom, Height = 112, BackColor = Color.White, Padding = new Padding(6), BorderStyle = BorderStyle.FixedSingle };
            TableLayoutPanel tblRb = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3 };
            tblRb.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F)); tblRb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblRb.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F)); tblRb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.dtpRbBD = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.dtpRbKT = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.cboRbThu = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboRbThu.Items.AddRange(new object[] { "Tất cả thứ", "2", "3", "4", "5", "6", "7", "8" }); this.cboRbThu.SelectedIndex = 0;
            this.cboRbBuoi = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboRbBuoi.Items.AddRange(new object[] { "Cả ngày", "Sang", "Chieu" }); this.cboRbBuoi.SelectedIndex = 0;
            this.txtRbLyDo = new TextBox { Dock = DockStyle.Fill };
            this.btnThemRB = new Button { Text = "+ Thêm bận", Width = 92, Height = 26, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnThemRB.Click += this.btnThemRB_Click;
            this.btnXoaRB = new Button { Text = "Xóa chọn", Width = 80, Height = 26 }; this.btnXoaRB.Click += this.btnXoaRB_Click;
            tblRb.Controls.Add(new Label { Text = "Từ ngày", AutoSize = true }, 0, 0); tblRb.Controls.Add(this.dtpRbBD, 1, 0);
            tblRb.Controls.Add(new Label { Text = "Đến ngày", AutoSize = true }, 2, 0); tblRb.Controls.Add(this.dtpRbKT, 3, 0);
            tblRb.Controls.Add(new Label { Text = "Thứ", AutoSize = true }, 0, 1); tblRb.Controls.Add(this.cboRbThu, 1, 1);
            tblRb.Controls.Add(new Label { Text = "Buổi", AutoSize = true }, 2, 1); tblRb.Controls.Add(this.cboRbBuoi, 3, 1);
            tblRb.Controls.Add(new Label { Text = "Lý do", AutoSize = true }, 0, 2); tblRb.Controls.Add(this.txtRbLyDo, 1, 2);
            FlowLayoutPanel flpRb = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            flpRb.Controls.AddRange(new Control[] { this.btnThemRB, this.btnXoaRB });
            tblRb.Controls.Add(flpRb, 2, 2); tblRb.SetColumnSpan(flpRb, 2);
            pnlRbAdd.Controls.Add(tblRb);
            this.tpRangBuoc.Controls.Add(this.dgvRangBuoc);
            this.tpRangBuoc.Controls.Add(pnlRbAdd);

            // Tab 3: Miễn giảm định mức (Grid + Form Thêm/Xóa)
            this.dgvMienGiam = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvMienGiam);
            Panel pnlMgAdd = new Panel { Dock = DockStyle.Bottom, Height = 112, BackColor = Color.White, Padding = new Padding(6), BorderStyle = BorderStyle.FixedSingle };
            TableLayoutPanel tblMg = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3 };
            tblMg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76F)); tblMg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblMg.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76F)); tblMg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.cboMgNamHoc = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.numMgTyLe = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Maximum = 100, Value = 0 };
            this.numMgSoGio = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Maximum = 500, Value = 0 };
            this.txtMgLyDo = new TextBox { Dock = DockStyle.Fill };
            this.btnThemMG = new Button { Text = "+ Thêm MG", Width = 92, Height = 26, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnThemMG.Click += this.btnThemMG_Click;
            this.btnXoaMG = new Button { Text = "Xóa chọn", Width = 80, Height = 26 }; this.btnXoaMG.Click += this.btnXoaMG_Click;
            tblMg.Controls.Add(new Label { Text = "Năm học", AutoSize = true }, 0, 0); tblMg.Controls.Add(this.cboMgNamHoc, 1, 0);
            tblMg.Controls.Add(new Label { Text = "Giảm (%)", AutoSize = true }, 2, 0); tblMg.Controls.Add(this.numMgTyLe, 3, 0);
            tblMg.Controls.Add(new Label { Text = "Giảm (giờ)", AutoSize = true }, 0, 1); tblMg.Controls.Add(this.numMgSoGio, 1, 1);
            tblMg.Controls.Add(new Label { Text = "Lý do *", AutoSize = true }, 2, 1); tblMg.Controls.Add(this.txtMgLyDo, 3, 1);
            FlowLayoutPanel flpMg = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            flpMg.Controls.AddRange(new Control[] { this.btnThemMG, this.btnXoaMG });
            tblMg.Controls.Add(flpMg, 0, 2); tblMg.SetColumnSpan(flpMg, 4);
            pnlMgAdd.Controls.Add(tblMg);
            this.tpMienGiam.Controls.Add(this.dgvMienGiam);
            this.tpMienGiam.Controls.Add(pnlMgAdd);

            this.lblTaiGioSummary = new Label { Dock = DockStyle.Fill, Padding = new Padding(12), Text = "Chọn giảng viên để xem tải giờ." }; this.tpTaiGio.Controls.Add(this.lblTaiGioSummary);
            this.dgvLichGV = new DataGridView(); WinFormStyleHelper.ConfigureGrid(this.dgvLichGV); this.tpLich.Controls.Add(this.dgvLichGV);

            this.tabChiTiet.TabPages.AddRange(new TabPage[] { this.tpThongTin, this.tpRangBuoc, this.tpMienGiam, this.tpTaiGio, this.tpLich });
            this.splitMain.Panel2.Controls.Add(this.tabChiTiet);
            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
