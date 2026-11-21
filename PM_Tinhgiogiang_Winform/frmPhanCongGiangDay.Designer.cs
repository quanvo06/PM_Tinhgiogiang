using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmPhanCongGiangDay
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private SplitContainer splitMain;
        private SplitContainer splitLeft;
        private SplitContainer splitRight;

        // Trái trên: Nhóm học
        private Panel pnlNhomTop;
        private TextBox txtTimNhom;
        private Button btnTaiLai;
        private DataGridView dgvNhomHoc;

        // Trái dưới: Buổi học thuộc nhóm
        private Label lblBuoiCaption;
        private DataGridView dgvBuoiHoc;

        // Phải trên: Phân công giảng viên cho buổi
        private Panel pnlPcHeader;
        private Label lblBuoiDangChon;
        private FlowLayoutPanel flpAddGv;
        private ComboBox cboGiangVien;
        private ComboBox cboVaiTro;
        private NumericUpDown nudSoTiet;
        private Button btnThemGv;
        private Button btnXoaGv;
        private Button btnChiaDeuTiet;
        private Button btnLuuPhanCong;
        private DataGridView dgvPhanCong;

        // Phải dưới: Tải giờ hiện tại của giảng viên
        private Label lblTaiGioCaption;
        private DataGridView dgvTaiGioGV;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.splitMain = new SplitContainer();
            this.splitLeft = new SplitContainer();
            this.splitRight = new SplitContainer();

            this.pnlNhomTop = new Panel();
            this.txtTimNhom = new TextBox();
            this.btnTaiLai = new Button();
            this.dgvNhomHoc = new DataGridView();

            this.lblBuoiCaption = new Label();
            this.dgvBuoiHoc = new DataGridView();

            this.pnlPcHeader = new Panel();
            this.lblBuoiDangChon = new Label();
            this.flpAddGv = new FlowLayoutPanel();
            this.cboGiangVien = new ComboBox();
            this.cboVaiTro = new ComboBox();
            this.nudSoTiet = new NumericUpDown();
            this.btnThemGv = new Button();
            this.btnXoaGv = new Button();
            this.btnChiaDeuTiet = new Button();
            this.btnLuuPhanCong = new Button();
            this.dgvPhanCong = new DataGridView();

            this.lblTaiGioCaption = new Label();
            this.dgvTaiGioGV = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Bước 2 — Phân công giảng dạy";

            // Header
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "BƯỚC 2 — PHÂN CÔNG GIẢNG DẠY & CÂN ĐỐI TẢI GIỜ GIẢNG VIÊN";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // Split Main
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Vertical;
            this.splitMain.SplitterDistance = 460;

            // Split Left (Nhóm học trên / Buổi học dưới)
            this.splitLeft.Dock = DockStyle.Fill;
            this.splitLeft.Orientation = Orientation.Horizontal;
            this.splitLeft.SplitterDistance = 290;

            this.pnlNhomTop.Dock = DockStyle.Top;
            this.pnlNhomTop.Height = 38;
            this.pnlNhomTop.BackColor = Color.White;
            this.pnlNhomTop.Padding = new Padding(6);
            this.txtTimNhom.Dock = DockStyle.Fill;
            this.txtTimNhom.TextChanged += this.txtTimNhom_TextChanged;
            this.btnTaiLai.Text = "Tải lại";
            this.btnTaiLai.Dock = DockStyle.Right;
            this.btnTaiLai.Width = 76;
            this.btnTaiLai.Click += this.btnTaiLai_Click;
            this.pnlNhomTop.Controls.Add(this.txtTimNhom);
            this.pnlNhomTop.Controls.Add(this.btnTaiLai);

            WinFormStyleHelper.ConfigureGrid(this.dgvNhomHoc);
            this.dgvNhomHoc.SelectionChanged += this.dgvNhomHoc_SelectionChanged;

            this.splitLeft.Panel1.Controls.Add(this.dgvNhomHoc);
            this.splitLeft.Panel1.Controls.Add(this.pnlNhomTop);

            this.lblBuoiCaption.Dock = DockStyle.Top;
            this.lblBuoiCaption.Height = 28;
            this.lblBuoiCaption.Text = "  Các buổi học thuộc nhóm đã chọn (click chọn buổi để phân công):";
            this.lblBuoiCaption.Font = WinFormStyleHelper.BoldFont;
            this.lblBuoiCaption.TextAlign = ContentAlignment.MiddleLeft;
            this.lblBuoiCaption.BackColor = Color.FromArgb(241, 245, 249);

            WinFormStyleHelper.ConfigureGrid(this.dgvBuoiHoc);
            this.dgvBuoiHoc.SelectionChanged += this.dgvBuoiHoc_SelectionChanged;

            this.splitLeft.Panel2.Controls.Add(this.dgvBuoiHoc);
            this.splitLeft.Panel2.Controls.Add(this.lblBuoiCaption);
            this.splitMain.Panel1.Controls.Add(this.splitLeft);

            // Split Right (Phân công trên / Tải giờ GV dưới)
            this.splitRight.Dock = DockStyle.Fill;
            this.splitRight.Orientation = Orientation.Horizontal;
            this.splitRight.SplitterDistance = 320;

            this.pnlPcHeader.Dock = DockStyle.Top;
            this.pnlPcHeader.Height = 82;
            this.pnlPcHeader.BackColor = Color.White;
            this.pnlPcHeader.Padding = new Padding(8, 6, 8, 6);

            this.lblBuoiDangChon.Dock = DockStyle.Top;
            this.lblBuoiDangChon.Height = 24;
            this.lblBuoiDangChon.Text = "Chưa chọn buổi học";
            this.lblBuoiDangChon.Font = WinFormStyleHelper.BoldFont;
            this.lblBuoiDangChon.ForeColor = WinFormStyleHelper.AccentColor;

            this.flpAddGv.Dock = DockStyle.Fill;
            this.flpAddGv.WrapContents = false;
            this.flpAddGv.Padding = new Padding(0, 6, 0, 0);

            this.cboGiangVien.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboGiangVien.Width = 200;

            this.cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboVaiTro.Width = 105;
            this.cboVaiTro.Items.AddRange(new object[] { "Chinh", "Phu", "DongGiang", "KTV" });
            this.cboVaiTro.SelectedIndex = 0;

            this.nudSoTiet.DecimalPlaces = 1;
            this.nudSoTiet.Minimum = 0.5M;
            this.nudSoTiet.Maximum = 15M;
            this.nudSoTiet.Increment = 0.5M;
            this.nudSoTiet.Value = 3M;
            this.nudSoTiet.Width = 65;

            this.btnThemGv.Text = "+ Thêm GV";
            this.btnThemGv.Width = 88;
            this.btnThemGv.Height = 28;
            this.btnThemGv.Click += this.btnThemGv_Click;

            this.btnXoaGv.Text = "Xóa dòng";
            this.btnXoaGv.Width = 82;
            this.btnXoaGv.Height = 28;
            this.btnXoaGv.Click += this.btnXoaGv_Click;

            this.btnChiaDeuTiet.Text = "Chia đều tiết";
            this.btnChiaDeuTiet.Width = 102;
            this.btnChiaDeuTiet.Height = 28;
            this.btnChiaDeuTiet.Click += this.btnChiaDeuTiet_Click;

            this.btnLuuPhanCong.Text = "Lưu phân công";
            this.btnLuuPhanCong.Width = 116;
            this.btnLuuPhanCong.Height = 28;
            this.btnLuuPhanCong.BackColor = WinFormStyleHelper.AccentColor;
            this.btnLuuPhanCong.ForeColor = Color.White;
            this.btnLuuPhanCong.FlatStyle = FlatStyle.Flat;
            this.btnLuuPhanCong.Click += this.btnLuuPhanCong_Click;

            this.flpAddGv.Controls.AddRange(new Control[]
            {
                this.cboGiangVien, this.cboVaiTro, this.nudSoTiet,
                this.btnThemGv, this.btnXoaGv, this.btnChiaDeuTiet, this.btnLuuPhanCong
            });
            this.pnlPcHeader.Controls.Add(this.flpAddGv);
            this.pnlPcHeader.Controls.Add(this.lblBuoiDangChon);

            WinFormStyleHelper.ConfigureGrid(this.dgvPhanCong);
            this.splitRight.Panel1.Controls.Add(this.dgvPhanCong);
            this.splitRight.Panel1.Controls.Add(this.pnlPcHeader);

            this.lblTaiGioCaption.Dock = DockStyle.Top;
            this.lblTaiGioCaption.Height = 28;
            this.lblTaiGioCaption.Text = "  Tải giờ hiện tại của giảng viên trong học kỳ / năm học (Định mức & Giờ chuẩn):";
            this.lblTaiGioCaption.Font = WinFormStyleHelper.BoldFont;
            this.lblTaiGioCaption.TextAlign = ContentAlignment.MiddleLeft;
            this.lblTaiGioCaption.BackColor = Color.FromArgb(241, 245, 249);

            WinFormStyleHelper.ConfigureGrid(this.dgvTaiGioGV);
            this.splitRight.Panel2.Controls.Add(this.dgvTaiGioGV);
            this.splitRight.Panel2.Controls.Add(this.lblTaiGioCaption);

            this.splitMain.Panel2.Controls.Add(this.splitRight);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
