using System;
using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem heThongToolStripMenuItem;
        private ToolStripMenuItem dangXuatToolStripMenuItem;
        private ToolStripMenuItem qlndToolStripMenuItem;
        private ToolStripMenuItem thoatToolStripMenuItem;

        private ToolStripMenuItem nghiepVuToolStripMenuItem;
        private ToolStripMenuItem tongQuanToolStripMenuItem;
        private ToolStripMenuItem phanCongToolStripMenuItem;
        private ToolStripMenuItem xepLichToolStripMenuItem;
        private ToolStripMenuItem kiemTraToolStripMenuItem;
        private ToolStripMenuItem tinhGioToolStripMenuItem;

        private ToolStripMenuItem danhMucToolStripMenuItem;
        private ToolStripMenuItem namHocHocKyToolStripMenuItem;
        private ToolStripMenuItem quanLyDanhMucToolStripMenuItem;
        private ToolStripMenuItem giangVienToolStripMenuItem;
        private ToolStripMenuItem phongHocToolStripMenuItem;
        private ToolStripMenuItem monBaiToolStripMenuItem;
        private ToolStripMenuItem lopNhomToolStripMenuItem;
        private ToolStripMenuItem quyTacToolStripMenuItem;

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatusUser;

        private Panel panelHeader;
        private Label lblHeader;
        private Label lblSubHeader;
        private FlowLayoutPanel flpContext;
        private ComboBox cboNamHoc;
        private ComboBox cboHocKy;
        private ComboBox cboBoMon;

        private Panel panelContent;
        private Button btnTongQuan;
        private Button btnPhanCong;
        private Button btnXepLich;
        private Button btnKiemTra;
        private Button btnTinhGio;

        private Button btnDmDungChung;
        private Button btnDmGiangVien;
        private Button btnDmPhongHoc;
        private Button btnDmMonBai;
        private Button btnDmLopNhom;
        private Button btnDmQuyTac;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Button TaoNutChucNang(string text, int x, int y, Color bgColor, EventHandler onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(420, 62);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bgColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(16, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.Click += onClick;
            return btn;
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new MenuStrip();
            this.heThongToolStripMenuItem = new ToolStripMenuItem();
            this.dangXuatToolStripMenuItem = new ToolStripMenuItem();
            this.qlndToolStripMenuItem = new ToolStripMenuItem();
            this.thoatToolStripMenuItem = new ToolStripMenuItem();

            this.nghiepVuToolStripMenuItem = new ToolStripMenuItem();
            this.tongQuanToolStripMenuItem = new ToolStripMenuItem();
            this.phanCongToolStripMenuItem = new ToolStripMenuItem();
            this.xepLichToolStripMenuItem = new ToolStripMenuItem();
            this.kiemTraToolStripMenuItem = new ToolStripMenuItem();
            this.tinhGioToolStripMenuItem = new ToolStripMenuItem();

            this.danhMucToolStripMenuItem = new ToolStripMenuItem();
            this.namHocHocKyToolStripMenuItem = new ToolStripMenuItem();
            this.quanLyDanhMucToolStripMenuItem = new ToolStripMenuItem();
            this.giangVienToolStripMenuItem = new ToolStripMenuItem();
            this.phongHocToolStripMenuItem = new ToolStripMenuItem();
            this.monBaiToolStripMenuItem = new ToolStripMenuItem();
            this.lopNhomToolStripMenuItem = new ToolStripMenuItem();
            this.quyTacToolStripMenuItem = new ToolStripMenuItem();

            this.statusStrip1 = new StatusStrip();
            this.lblStatusUser = new ToolStripStatusLabel();

            this.panelHeader = new Panel();
            this.lblHeader = new Label();
            this.lblSubHeader = new Label();
            this.flpContext = new FlowLayoutPanel();
            this.cboNamHoc = new ComboBox();
            this.cboHocKy = new ComboBox();
            this.cboBoMon = new ComboBox();

            this.panelContent = new Panel();
            this.btnTongQuan = TaoNutChucNang("1. Tổng quan điều hành && Thống kê", 40, 32, Color.FromArgb(15, 118, 110), this.btnTongQuan_Click);
            this.btnPhanCong = TaoNutChucNang("2. Phân công giảng dạy", 40, 110, Color.FromArgb(13, 148, 136), this.btnPhanCong_Click);
            this.btnXepLich = TaoNutChucNang("3. Xếp Thời khóa biểu tuần (10 tiết/ngày)", 40, 188, Color.FromArgb(2, 132, 199), this.btnXepLich_Click);
            this.btnKiemTra = TaoNutChucNang("4. Kiểm tra xung đột && Xuất bản lịch", 40, 266, Color.FromArgb(217, 119, 6), this.btnKiemTra_Click);
            this.btnTinhGio = TaoNutChucNang("5. Tổng hợp Giờ chuẩn Đ12 && Xuất Excel", 40, 344, Color.FromArgb(5, 150, 105), this.btnTinhGio_Click);

            this.btnDmDungChung = TaoNutChucNang("Danh mục dùng chung (Bộ môn, Học vị...)", 492, 32, Color.FromArgb(51, 65, 85), this.quanLyDanhMucToolStripMenuItem_Click);
            this.btnDmGiangVien = TaoNutChucNang("Quản lý Giảng viên (5 tab)", 492, 110, Color.FromArgb(71, 85, 105), this.giangVienToolStripMenuItem_Click);
            this.btnDmPhongHoc = TaoNutChucNang("Quản lý Phòng học && Lab", 492, 188, Color.FromArgb(71, 85, 105), this.phongHocToolStripMenuItem_Click);
            this.btnDmMonBai = TaoNutChucNang("Quản lý Môn học && Bài học", 492, 266, Color.FromArgb(71, 85, 105), this.monBaiToolStripMenuItem_Click);
            this.btnDmLopNhom = TaoNutChucNang("Quản lý Lớp HP && Nhóm học", 492, 344, Color.FromArgb(71, 85, 105), this.lopNhomToolStripMenuItem_Click);
            this.btnDmQuyTac = TaoNutChucNang("Quy tắc quy đổi (Điều 12)", 492, 422, Color.FromArgb(71, 85, 105), this.quyTacToolStripMenuItem_Click);

            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();

            this.Font = WinFormStyleHelper.DefaultFont;
            this.ClientSize = new Size(1247, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PM_Tinhgiogiang — Trường Dược · ĐH Y Dược TP.HCM (UMP)";

            // MenuStrip
            this.menuStrip1.Font = new Font("Segoe UI", 10F);
            this.menuStrip1.Items.AddRange(new ToolStripItem[]
            {
                this.heThongToolStripMenuItem,
                this.nghiepVuToolStripMenuItem,
                this.danhMucToolStripMenuItem
            });

            this.heThongToolStripMenuItem.Text = "Hệ thống";
            this.heThongToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                this.qlndToolStripMenuItem,
                this.dangXuatToolStripMenuItem,
                this.thoatToolStripMenuItem
            });

            this.qlndToolStripMenuItem.Text = "Quản lý người dùng && Phân quyền";
            this.qlndToolStripMenuItem.Click += this.qlndToolStripMenuItem_Click;
            this.dangXuatToolStripMenuItem.Text = "Đăng xuất";
            this.dangXuatToolStripMenuItem.Click += this.dangXuatToolStripMenuItem_Click;
            this.thoatToolStripMenuItem.Text = "Thoát";
            this.thoatToolStripMenuItem.Click += this.thoatToolStripMenuItem_Click;

            this.nghiepVuToolStripMenuItem.Text = "Nghiệp vụ giảng dạy";
            this.nghiepVuToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                this.tongQuanToolStripMenuItem,
                this.phanCongToolStripMenuItem,
                this.xepLichToolStripMenuItem,
                this.kiemTraToolStripMenuItem,
                this.tinhGioToolStripMenuItem
            });

            this.tongQuanToolStripMenuItem.Text = "1. Tổng quan điều hành";
            this.tongQuanToolStripMenuItem.Click += this.btnTongQuan_Click;
            this.phanCongToolStripMenuItem.Text = "2. Phân công giảng dạy";
            this.phanCongToolStripMenuItem.Click += this.btnPhanCong_Click;
            this.xepLichToolStripMenuItem.Text = "3. Xếp thời khóa biểu";
            this.xepLichToolStripMenuItem.Click += this.btnXepLich_Click;
            this.kiemTraToolStripMenuItem.Text = "4. Kiểm tra && xuất bản";
            this.kiemTraToolStripMenuItem.Click += this.btnKiemTra_Click;
            this.tinhGioToolStripMenuItem.Text = "5. Tính giờ chuẩn && báo cáo Excel";
            this.tinhGioToolStripMenuItem.Click += this.btnTinhGio_Click;

            this.danhMucToolStripMenuItem.Text = "Danh mục nền";
            this.danhMucToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                this.namHocHocKyToolStripMenuItem,
                this.quanLyDanhMucToolStripMenuItem,
                this.giangVienToolStripMenuItem,
                this.phongHocToolStripMenuItem,
                this.monBaiToolStripMenuItem,
                this.lopNhomToolStripMenuItem,
                this.quyTacToolStripMenuItem
            });

            this.namHocHocKyToolStripMenuItem.Text = "Năm học, Học kỳ && Khung 10 Tiết";
            this.namHocHocKyToolStripMenuItem.Click += this.namHocHocKyToolStripMenuItem_Click;
            this.quanLyDanhMucToolStripMenuItem.Text = "Danh mục dùng chung (Bộ môn, Học vị...)";
            this.quanLyDanhMucToolStripMenuItem.Click += this.quanLyDanhMucToolStripMenuItem_Click;
            this.giangVienToolStripMenuItem.Text = "Giảng viên (5 tab)";
            this.giangVienToolStripMenuItem.Click += this.giangVienToolStripMenuItem_Click;
            this.phongHocToolStripMenuItem.Text = "Phòng học && Lab";
            this.phongHocToolStripMenuItem.Click += this.phongHocToolStripMenuItem_Click;
            this.monBaiToolStripMenuItem.Text = "Môn học && Bài học";
            this.monBaiToolStripMenuItem.Click += this.monBaiToolStripMenuItem_Click;
            this.lopNhomToolStripMenuItem.Text = "Lớp HP && Nhóm học";
            this.lopNhomToolStripMenuItem.Click += this.lopNhomToolStripMenuItem_Click;
            this.quyTacToolStripMenuItem.Text = "Quy tắc quy đổi (Điều 12)";
            this.quyTacToolStripMenuItem.Click += this.quyTacToolStripMenuItem_Click;

            // Header
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 128;
            this.panelHeader.BackColor = WinFormStyleHelper.AccentColor;
            this.panelHeader.Padding = new Padding(24, 14, 24, 10);

            this.lblHeader.Text = "HỆ THỐNG QUẢN LÝ GIẢNG DẠY && TÍNH GIỜ CHUẨN";
            this.lblHeader.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Dock = DockStyle.Top;
            this.lblHeader.Height = 34;

            this.lblSubHeader.Text = "Trường Dược — Đại học Y Dược TP. Hồ Chí Minh (UMP) | LINQ to SQL (.NET 4.8)";
            this.lblSubHeader.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.lblSubHeader.ForeColor = Color.FromArgb(209, 250, 229);
            this.lblSubHeader.Dock = DockStyle.Top;
            this.lblSubHeader.Height = 24;

            this.flpContext.Dock = DockStyle.Bottom;
            this.flpContext.Height = 36;
            this.flpContext.WrapContents = false;

            Label lNh = new Label { Text = "Năm học:", ForeColor = Color.White, Font = WinFormStyleHelper.BoldFont, AutoSize = true, Padding = new Padding(0, 5, 4, 0) };
            this.cboNamHoc.DropDownStyle = ComboBoxStyle.DropDownList; this.cboNamHoc.Width = 175; this.cboNamHoc.SelectedIndexChanged += this.ContextChanged;
            Label lHk = new Label { Text = "Học kỳ:", ForeColor = Color.White, Font = WinFormStyleHelper.BoldFont, AutoSize = true, Padding = new Padding(12, 5, 4, 0) };
            this.cboHocKy.DropDownStyle = ComboBoxStyle.DropDownList; this.cboHocKy.Width = 185; this.cboHocKy.SelectedIndexChanged += this.ContextChanged;
            Label lBm = new Label { Text = "Bộ môn:", ForeColor = Color.White, Font = WinFormStyleHelper.BoldFont, AutoSize = true, Padding = new Padding(12, 5, 4, 0) };
            this.cboBoMon.DropDownStyle = ComboBoxStyle.DropDownList; this.cboBoMon.Width = 220; this.cboBoMon.SelectedIndexChanged += this.ContextChanged;

            this.flpContext.Controls.AddRange(new Control[] { lNh, this.cboNamHoc, lHk, this.cboHocKy, lBm, this.cboBoMon });
            this.panelHeader.Controls.Add(this.flpContext);
            this.panelHeader.Controls.Add(this.lblSubHeader);
            this.panelHeader.Controls.Add(this.lblHeader);

            // Content
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.BackColor = WinFormStyleHelper.BackgroundColor;
            this.panelContent.AutoScroll = true;
            this.panelContent.Controls.AddRange(new Control[]
            {
                this.btnTongQuan,
                this.btnPhanCong,
                this.btnXepLich,
                this.btnKiemTra,
                this.btnTinhGio,
                this.btnDmDungChung,
                this.btnDmGiangVien,
                this.btnDmPhongHoc,
                this.btnDmMonBai,
                this.btnDmLopNhom,
                this.btnDmQuyTac
            });

            // StatusStrip
            this.lblStatusUser.Text = "Chưa đăng nhập";
            this.statusStrip1.Items.Add(this.lblStatusUser);

            this.MainMenuStrip = this.menuStrip1;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
