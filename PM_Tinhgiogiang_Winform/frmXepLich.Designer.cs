using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmXepLich
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpToolbar;
        private Label lblGocNhin;
        private ComboBox cboGocNhin;
        private ComboBox cboDoiTuongLoc;
        private Button btnTuanTruoc;
        private DateTimePicker dtpNgayTrongTuan;
        private Button btnTuanSau;
        private Button btnTaiLai;
        private Button btnMoThemNhanh;

        private SplitContainer splitMain;

        // Trái: Lưới TKB 7 ngày x 10 tiết
        private TableLayoutPanel tlpHeaderTkb;
        private Panel pnlScrollTkb;
        private TableLayoutPanel tlpGridTkb;

        // Phải: Inspector 2 tab (Chi tiết buổi / Thêm & Xếp lặp lại)
        private TabControl tabInspector;
        private TabPage tpChiTiet;
        private TabPage tpThemNhanh;

        // Tab Chi tiết buổi
        private Panel pnlChiTietBody;
        private Label lblCtTieuDe;
        private TextBox txtCtNoiDung;
        private FlowLayoutPanel flpCtActions;
        private Button btnSapXepBaiHoc;
        private Button btnXoaBuoi;

        // Tab Thêm buổi / Xếp lịch nhanh lặp lại
        private TableLayoutPanel tlpThemNhanh;
        private ComboBox cboNhomHoc;
        private ComboBox cboLoaiHinh;
        private ComboBox cboPhongHoc;
        private ComboBox cboGiangVien;
        private DateTimePicker dtpNgayBD;
        private FlowLayoutPanel flpChonThu;
        private CheckBox[] chkThuTrongTuan;
        private ComboBox cboChuKyLap;
        private NumericUpDown nudSoTuanLap;
        private FlowLayoutPanel flpChonTiet;
        private CheckBox[] chkTietHoc;
        private CheckBox chkNgoaiNgu;
        private CheckBox chkTuDongBaiHoc;
        private TextBox txtGhiChu;
        private Button btnLuuXepLich;

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
            this.pnlToolbar = new Panel();
            this.flpToolbar = new FlowLayoutPanel();
            this.lblGocNhin = new Label();
            this.cboGocNhin = new ComboBox();
            this.cboDoiTuongLoc = new ComboBox();
            this.btnTuanTruoc = new Button();
            this.dtpNgayTrongTuan = new DateTimePicker();
            this.btnTuanSau = new Button();
            this.btnTaiLai = new Button();
            this.btnMoThemNhanh = new Button();

            this.splitMain = new SplitContainer();
            this.tlpHeaderTkb = new TableLayoutPanel();
            this.pnlScrollTkb = new Panel();
            this.tlpGridTkb = new TableLayoutPanel();

            this.tabInspector = new TabControl();
            this.tpChiTiet = new TabPage();
            this.tpThemNhanh = new TabPage();

            this.pnlChiTietBody = new Panel();
            this.lblCtTieuDe = new Label();
            this.txtCtNoiDung = new TextBox();
            this.flpCtActions = new FlowLayoutPanel();
            this.btnSapXepBaiHoc = new Button();
            this.btnXoaBuoi = new Button();

            this.tlpThemNhanh = new TableLayoutPanel();
            this.cboNhomHoc = new ComboBox();
            this.cboLoaiHinh = new ComboBox();
            this.cboPhongHoc = new ComboBox();
            this.cboGiangVien = new ComboBox();
            this.dtpNgayBD = new DateTimePicker();
            this.flpChonThu = new FlowLayoutPanel();
            this.chkThuTrongTuan = new CheckBox[7];
            this.cboChuKyLap = new ComboBox();
            this.nudSoTuanLap = new NumericUpDown();
            this.flpChonTiet = new FlowLayoutPanel();
            this.chkTietHoc = new CheckBox[10];
            this.chkNgoaiNgu = new CheckBox();
            this.chkTuDongBaiHoc = new CheckBox();
            this.txtGhiChu = new TextBox();
            this.btnLuuXepLich = new Button();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Bước 3 — Lưới Thời khóa biểu tuần (10 tiết/ngày)";

            // TOOLBAR
            this.pnlToolbar.Dock = DockStyle.Top;
            this.pnlToolbar.Height = 46;
            this.pnlToolbar.BackColor = Color.White;
            this.pnlToolbar.Padding = new Padding(8, 7, 8, 6);
            this.pnlToolbar.BorderStyle = BorderStyle.FixedSingle;

            this.flpToolbar.Dock = DockStyle.Fill;
            this.flpToolbar.WrapContents = false;

            this.lblGocNhin.Text = "Góc nhìn:";
            this.lblGocNhin.AutoSize = true;
            this.lblGocNhin.Padding = new Padding(0, 6, 4, 0);
            this.lblGocNhin.Font = WinFormStyleHelper.BoldFont;

            this.cboGocNhin.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboGocNhin.Width = 145;
            this.cboGocNhin.Items.AddRange(new object[] { "Toàn trường", "Theo lớp/nhóm", "Theo giảng viên", "Theo phòng" });
            this.cboGocNhin.SelectedIndex = 0;
            this.cboGocNhin.SelectedIndexChanged += this.cboGocNhin_SelectedIndexChanged;

            this.cboDoiTuongLoc.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboDoiTuongLoc.Width = 210;
            this.cboDoiTuongLoc.Enabled = false;
            this.cboDoiTuongLoc.SelectedIndexChanged += this.cboDoiTuongLoc_SelectedIndexChanged;

            this.btnTuanTruoc.Text = "◀ Tuần trước";
            this.btnTuanTruoc.Width = 98;
            this.btnTuanTruoc.Height = 28;
            this.btnTuanTruoc.Click += this.btnTuanTruoc_Click;

            this.dtpNgayTrongTuan.Format = DateTimePickerFormat.Custom;
            this.dtpNgayTrongTuan.CustomFormat = "dd/MM/yyyy";
            this.dtpNgayTrongTuan.Width = 112;
            this.dtpNgayTrongTuan.ValueChanged += this.dtpNgayTrongTuan_ValueChanged;

            this.btnTuanSau.Text = "Tuần sau ▶";
            this.btnTuanSau.Width = 92;
            this.btnTuanSau.Height = 28;
            this.btnTuanSau.Click += this.btnTuanSau_Click;

            this.btnTaiLai.Text = "Tải lại";
            this.btnTaiLai.Width = 70;
            this.btnTaiLai.Height = 28;
            this.btnTaiLai.Click += this.btnTaiLai_Click;

            this.btnMoThemNhanh.Text = "+ Thêm / Xếp lặp lại";
            this.btnMoThemNhanh.Width = 148;
            this.btnMoThemNhanh.Height = 28;
            this.btnMoThemNhanh.BackColor = WinFormStyleHelper.AccentColor;
            this.btnMoThemNhanh.ForeColor = Color.White;
            this.btnMoThemNhanh.FlatStyle = FlatStyle.Flat;
            this.btnMoThemNhanh.Click += this.btnMoThemNhanh_Click;

            this.flpToolbar.Controls.AddRange(new Control[]
            {
                this.lblGocNhin, this.cboGocNhin, this.cboDoiTuongLoc,
                this.btnTuanTruoc, this.dtpNgayTrongTuan, this.btnTuanSau,
                this.btnTaiLai, this.btnMoThemNhanh
            });
            this.pnlToolbar.Controls.Add(this.flpToolbar);

            // SPLIT MAIN
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Vertical;
            this.splitMain.SplitterDistance = 760;

            // HEADER TKB (8 cột: Ca/Tiết + Thứ 2..CN)
            this.tlpHeaderTkb.Dock = DockStyle.Top;
            this.tlpHeaderTkb.Height = 42;
            this.tlpHeaderTkb.ColumnCount = 8;
            this.tlpHeaderTkb.RowCount = 1;
            this.tlpHeaderTkb.BackColor = Color.FromArgb(238, 242, 246);
            this.tlpHeaderTkb.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74F));
            for (int c = 0; c < 7; c++)
                this.tlpHeaderTkb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857F));
            this.tlpHeaderTkb.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // SCROLL PANEL + GRID TKB
            this.pnlScrollTkb.Dock = DockStyle.Fill;
            this.pnlScrollTkb.AutoScroll = true;
            this.pnlScrollTkb.BackColor = Color.White;

            this.tlpGridTkb.Dock = DockStyle.Top;
            this.tlpGridTkb.Height = 760; // 10 tiết x 74px + 20px phân cách Sáng/Chiều
            this.tlpGridTkb.ColumnCount = 8;
            this.tlpGridTkb.RowCount = 1;
            this.tlpGridTkb.Margin = new Padding(0);
            this.tlpGridTkb.Padding = new Padding(0);
            this.tlpGridTkb.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74F));
            for (int c = 0; c < 7; c++)
                this.tlpGridTkb.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857F));
            this.tlpGridTkb.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.pnlScrollTkb.Controls.Add(this.tlpGridTkb);
            this.splitMain.Panel1.Controls.Add(this.pnlScrollTkb);
            this.splitMain.Panel1.Controls.Add(this.tlpHeaderTkb);

            // INSPECTOR PHẢI
            this.tabInspector.Dock = DockStyle.Fill;
            this.tpChiTiet.Text = "Chi tiết buổi";
            this.tpChiTiet.BackColor = Color.White;
            this.tpChiTiet.Padding = new Padding(10);

            this.tpThemNhanh.Text = "Thêm / Lặp lại";
            this.tpThemNhanh.BackColor = Color.White;
            this.tpThemNhanh.AutoScroll = true;
            this.tpThemNhanh.Padding = new Padding(10);

            // Tab 1: Chi tiết buổi
            this.pnlChiTietBody.Dock = DockStyle.Fill;
            this.lblCtTieuDe.Dock = DockStyle.Top;
            this.lblCtTieuDe.Height = 32;
            this.lblCtTieuDe.Text = "Click một thẻ buổi học trên lưới để xem chi tiết";
            this.lblCtTieuDe.Font = WinFormStyleHelper.BoldFont;
            this.lblCtTieuDe.ForeColor = WinFormStyleHelper.AccentColor;

            this.txtCtNoiDung.Dock = DockStyle.Fill;
            this.txtCtNoiDung.Multiline = true;
            this.txtCtNoiDung.ReadOnly = true;
            this.txtCtNoiDung.ScrollBars = ScrollBars.Vertical;
            this.txtCtNoiDung.BackColor = Color.FromArgb(248, 250, 252);
            this.txtCtNoiDung.Font = WinFormStyleHelper.DefaultFont;

            this.flpCtActions.Dock = DockStyle.Bottom;
            this.flpCtActions.Height = 42;
            this.flpCtActions.Padding = new Padding(0, 6, 0, 0);

            this.btnSapXepBaiHoc.Text = "Sắp xếp lại bài học";
            this.btnSapXepBaiHoc.Width = 148;
            this.btnSapXepBaiHoc.Height = 30;
            this.btnSapXepBaiHoc.Click += this.btnSapXepBaiHoc_Click;

            this.btnXoaBuoi.Text = "Xóa buổi này";
            this.btnXoaBuoi.Width = 112;
            this.btnXoaBuoi.Height = 30;
            this.btnXoaBuoi.BackColor = Color.FromArgb(220, 38, 38);
            this.btnXoaBuoi.ForeColor = Color.White;
            this.btnXoaBuoi.FlatStyle = FlatStyle.Flat;
            this.btnXoaBuoi.Click += this.btnXoaBuoi_Click;

            this.flpCtActions.Controls.Add(this.btnSapXepBaiHoc);
            this.flpCtActions.Controls.Add(this.btnXoaBuoi);

            this.pnlChiTietBody.Controls.Add(this.txtCtNoiDung);
            this.pnlChiTietBody.Controls.Add(this.lblCtTieuDe);
            this.pnlChiTietBody.Controls.Add(this.flpCtActions);
            this.tpChiTiet.Controls.Add(this.pnlChiTietBody);

            // Tab 2: Thêm buổi / Xếp lịch lặp lại
            this.tlpThemNhanh.Dock = DockStyle.Top;
            this.tlpThemNhanh.AutoSize = true;
            this.tlpThemNhanh.ColumnCount = 2;
            this.tlpThemNhanh.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            this.tlpThemNhanh.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this.cboNhomHoc.Dock = DockStyle.Fill; this.cboNhomHoc.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboNhomHoc.SelectedIndexChanged += this.cboNhomHoc_SelectedIndexChanged;
            this.cboLoaiHinh.Dock = DockStyle.Fill; this.cboLoaiHinh.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLoaiHinh.Items.AddRange(new object[] { "LT", "TH", "TT" });
            this.cboLoaiHinh.SelectedIndex = 0;
            this.cboPhongHoc.Dock = DockStyle.Fill; this.cboPhongHoc.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboGiangVien.Dock = DockStyle.Fill; this.cboGiangVien.DropDownStyle = ComboBoxStyle.DropDownList;

            this.dtpNgayBD.Dock = DockStyle.Fill;
            this.dtpNgayBD.Format = DateTimePickerFormat.Custom;
            this.dtpNgayBD.CustomFormat = "dd/MM/yyyy";

            this.flpChonThu.Dock = DockStyle.Fill;
            this.flpChonThu.AutoSize = true;
            string[] nhanThu = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
            for (int i = 0; i < 7; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Text = nhanThu[i];
                cb.Tag = i + 2; // 2..8
                cb.AutoSize = true;
                cb.Margin = new Padding(2, 2, 6, 2);
                this.chkThuTrongTuan[i] = cb;
                this.flpChonThu.Controls.Add(cb);
            }

            this.cboChuKyLap.Dock = DockStyle.Fill;
            this.cboChuKyLap.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboChuKyLap.Items.AddRange(new object[] { "Hằng tuần (1 tuần/lần)", "Cách 1 tuần (2 tuần/lần)" });
            this.cboChuKyLap.SelectedIndex = 0;

            this.nudSoTuanLap.Dock = DockStyle.Fill;
            this.nudSoTuanLap.Minimum = 1;
            this.nudSoTuanLap.Maximum = 20;
            this.nudSoTuanLap.Value = 1;

            this.flpChonTiet.Dock = DockStyle.Fill;
            this.flpChonTiet.AutoSize = true;
            for (int i = 0; i < 10; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Text = (i < 5 ? "S-T" : "C-T") + (i + 1);
                cb.Tag = i + 1;
                cb.AutoSize = true;
                cb.Margin = new Padding(2, 2, 6, 2);
                if (i < 3) cb.Checked = true;
                this.chkTietHoc[i] = cb;
                this.flpChonTiet.Controls.Add(cb);
            }

            this.chkNgoaiNgu.Text = "Dạy bằng ngoại ngữ (x1.5)";
            this.chkNgoaiNgu.AutoSize = true;
            this.chkTuDongBaiHoc.Text = "Tự động rải bài học theo dòng thời gian";
            this.chkTuDongBaiHoc.AutoSize = true;
            this.chkTuDongBaiHoc.Checked = true;

            this.txtGhiChu.Dock = DockStyle.Fill;
            this.btnLuuXepLich.Text = "Xếp lịch & Kiểm tra trùng";
            this.btnLuuXepLich.Dock = DockStyle.Fill;
            this.btnLuuXepLich.Height = 34;
            this.btnLuuXepLich.BackColor = WinFormStyleHelper.AccentColor;
            this.btnLuuXepLich.ForeColor = Color.White;
            this.btnLuuXepLich.FlatStyle = FlatStyle.Flat;
            this.btnLuuXepLich.Font = WinFormStyleHelper.BoldFont;
            this.btnLuuXepLich.Click += this.btnLuuXepLich_Click;

            int r = 0;
            this.tlpThemNhanh.Controls.Add(TaoNhan("Nhóm học:"), 0, r); this.tlpThemNhanh.Controls.Add(this.cboNhomHoc, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Loại hình:"), 0, r); this.tlpThemNhanh.Controls.Add(this.cboLoaiHinh, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Phòng học:"), 0, r); this.tlpThemNhanh.Controls.Add(this.cboPhongHoc, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Giảng viên:"), 0, r); this.tlpThemNhanh.Controls.Add(this.cboGiangVien, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Ngày bắt đầu:"), 0, r); this.tlpThemNhanh.Controls.Add(this.dtpNgayBD, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Thứ lặp lại:"), 0, r); this.tlpThemNhanh.Controls.Add(this.flpChonThu, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Chu kỳ lặp:"), 0, r); this.tlpThemNhanh.Controls.Add(this.cboChuKyLap, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Số lần lặp:"), 0, r); this.tlpThemNhanh.Controls.Add(this.nudSoTuanLap, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Chọn tiết:"), 0, r); this.tlpThemNhanh.Controls.Add(this.flpChonTiet, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Tùy chọn:"), 0, r); this.tlpThemNhanh.Controls.Add(this.chkNgoaiNgu, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan(""), 0, r); this.tlpThemNhanh.Controls.Add(this.chkTuDongBaiHoc, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan("Ghi chú:"), 0, r); this.tlpThemNhanh.Controls.Add(this.txtGhiChu, 1, r++);
            this.tlpThemNhanh.Controls.Add(TaoNhan(""), 0, r); this.tlpThemNhanh.Controls.Add(this.btnLuuXepLich, 1, r++);

            this.tpThemNhanh.Controls.Add(this.tlpThemNhanh);

            this.tabInspector.TabPages.Add(this.tpChiTiet);
            this.tabInspector.TabPages.Add(this.tpThemNhanh);
            this.splitMain.Panel2.Controls.Add(this.tabInspector);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlToolbar);
            this.ResumeLayout(false);
        }
    }
}
