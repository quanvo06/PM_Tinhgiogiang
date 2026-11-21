using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTopBar;
        private FlowLayoutPanel flpContext;
        private Label lblAppTitle;
        private ComboBox cboNamHoc;
        private ComboBox cboHocKy;
        private ComboBox cboDot;
        private ComboBox cboBoMon;
        private Panel pnlSidebar;
        private FlowLayoutPanel flpSidebarMenu;
        private Panel pnlWorkspace;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusDb;
        private ToolStripStatusLabel lblStatusUser;

        private Button btnNavTongQuan;
        private Button btnNavKhaiBao;
        private Button btnNavPhanCong;
        private Button btnNavXepLich;
        private Button btnNavKiemTra;
        private Button btnNavTinhGio;
        private Button btnNavQuanTri;
        private Button btnNavGiangVien;
        private Button btnNavPhongHoc;
        private Button btnNavMonBai;
        private Button btnNavLopNhom;
        private Button btnNavQuyTac;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Button TaoNutSidebar(string text, System.EventHandler onClick, bool enabled = true)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = 216;
            btn.Height = 34;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(12, 0, 0, 0);
            btn.Font = WinFormStyleHelper.DefaultFont;
            btn.ForeColor = enabled ? Color.FromArgb(51, 65, 85) : Color.Gray;
            btn.Enabled = enabled;
            btn.Margin = new Padding(4, 1, 4, 1);
            if (onClick != null) btn.Click += onClick;
            return btn;
        }

        private Label TaoNhomCaption(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Width = 216;
            lbl.Height = 24;
            lbl.Padding = new Padding(12, 8, 0, 0);
            lbl.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(100, 116, 139);
            return lbl;
        }

        private void InitializeComponent()
        {
            this.pnlTopBar = new Panel();
            this.flpContext = new FlowLayoutPanel();
            this.lblAppTitle = new Label();
            this.cboNamHoc = new ComboBox();
            this.cboHocKy = new ComboBox();
            this.cboDot = new ComboBox();
            this.cboBoMon = new ComboBox();
            this.pnlSidebar = new Panel();
            this.flpSidebarMenu = new FlowLayoutPanel();
            this.pnlWorkspace = new Panel();
            this.statusStrip = new StatusStrip();
            this.lblStatusDb = new ToolStripStatusLabel();
            this.lblStatusUser = new ToolStripStatusLabel();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.ClientSize = new Size(1366, 768);
            this.MinimumSize = new Size(1024, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "PM_Tinhgiogiang — Trường Dược · ĐH Y Dược TP.HCM (UMP)";

            // TOP BAR & CONTEXT BAR
            this.pnlTopBar.Dock = DockStyle.Top;
            this.pnlTopBar.Height = 48;
            this.pnlTopBar.BackColor = Color.White;
            this.pnlTopBar.Padding = new Padding(12, 8, 12, 6);
            this.pnlTopBar.BorderStyle = BorderStyle.FixedSingle;

            this.flpContext.Dock = DockStyle.Fill;
            this.flpContext.WrapContents = false;

            this.lblAppTitle.Text = "UMP · Trường Dược   |";
            this.lblAppTitle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.lblAppTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Padding = new Padding(0, 5, 8, 0);

            this.cboNamHoc.DropDownStyle = ComboBoxStyle.DropDownList; this.cboNamHoc.Width = 170; this.cboNamHoc.SelectedIndexChanged += this.ContextChanged;
            this.cboHocKy.DropDownStyle = ComboBoxStyle.DropDownList; this.cboHocKy.Width = 175; this.cboHocKy.SelectedIndexChanged += this.ContextChanged;
            this.cboDot.DropDownStyle = ComboBoxStyle.DropDownList; this.cboDot.Width = 145;
            this.cboBoMon.DropDownStyle = ComboBoxStyle.DropDownList; this.cboBoMon.Width = 210; this.cboBoMon.SelectedIndexChanged += this.ContextChanged;

            this.flpContext.Controls.Add(this.lblAppTitle);
            this.flpContext.Controls.Add(this.cboNamHoc);
            this.flpContext.Controls.Add(this.cboHocKy);
            this.flpContext.Controls.Add(this.cboDot);
            this.flpContext.Controls.Add(this.cboBoMon);
            this.pnlTopBar.Controls.Add(this.flpContext);

            // SIDEBAR TRÁI
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = 232;
            this.pnlSidebar.BackColor = WinFormStyleHelper.SidebarColor;
            this.pnlSidebar.BorderStyle = BorderStyle.FixedSingle;

            this.flpSidebarMenu.Dock = DockStyle.Fill;
            this.flpSidebarMenu.FlowDirection = FlowDirection.TopDown;
            this.flpSidebarMenu.WrapContents = false;
            this.flpSidebarMenu.AutoScroll = true;

            this.btnNavTongQuan = TaoNutSidebar("▦  Tổng quan", this.btnNavTongQuan_Click);
            this.btnNavKhaiBao = TaoNutSidebar("1. Khai báo dữ liệu", this.btnNavGiangVien_Click);
            this.btnNavPhanCong = TaoNutSidebar("2. Phân công giảng dạy", this.btnNavPhanCong_Click);
            this.btnNavXepLich = TaoNutSidebar("3. Xếp thời khóa biểu", this.btnNavXepLich_Click);
            this.btnNavKiemTra = TaoNutSidebar("4. Kiểm tra & xuất bản", this.btnNavKiemTra_Click);
            this.btnNavTinhGio = TaoNutSidebar("5. Tính giờ & báo cáo", this.btnNavTinhGio_Click);
            this.btnNavQuanTri = TaoNutSidebar("6. Quản trị hệ thống (P1)", null, false);

            this.btnNavGiangVien = TaoNutSidebar("•  Giảng viên (5 tab)", this.btnNavGiangVien_Click);
            this.btnNavPhongHoc = TaoNutSidebar("•  Phòng học & Lab", this.btnNavPhongHoc_Click);
            this.btnNavMonBai = TaoNutSidebar("•  Môn học & Bài học", this.btnNavMonBai_Click);
            this.btnNavLopNhom = TaoNutSidebar("•  Lớp HP & Nhóm học", this.btnNavLopNhom_Click);
            this.btnNavQuyTac = TaoNutSidebar("•  Quy tắc quy đổi (Đ12)", this.btnNavQuyTac_Click);

            this.flpSidebarMenu.Controls.Add(this.btnNavTongQuan);
            this.flpSidebarMenu.Controls.Add(TaoNhomCaption("QUY TRÌNH 6 BƯỚC"));
            this.flpSidebarMenu.Controls.Add(this.btnNavKhaiBao);
            this.flpSidebarMenu.Controls.Add(this.btnNavPhanCong);
            this.flpSidebarMenu.Controls.Add(this.btnNavXepLich);
            this.flpSidebarMenu.Controls.Add(this.btnNavKiemTra);
            this.flpSidebarMenu.Controls.Add(this.btnNavTinhGio);
            this.flpSidebarMenu.Controls.Add(this.btnNavQuanTri);
            this.flpSidebarMenu.Controls.Add(TaoNhomCaption("DANH MỤC NỀN (P0)"));
            this.flpSidebarMenu.Controls.Add(this.btnNavGiangVien);
            this.flpSidebarMenu.Controls.Add(this.btnNavPhongHoc);
            this.flpSidebarMenu.Controls.Add(this.btnNavMonBai);
            this.flpSidebarMenu.Controls.Add(this.btnNavLopNhom);
            this.flpSidebarMenu.Controls.Add(this.btnNavQuyTac);
            this.pnlSidebar.Controls.Add(this.flpSidebarMenu);

            // WORKSPACE
            this.pnlWorkspace.Dock = DockStyle.Fill;
            this.pnlWorkspace.BackColor = WinFormStyleHelper.BackgroundColor;
            this.pnlWorkspace.Padding = new Padding(12);

            // STATUS BAR
            this.lblStatusDb.Text = "● Sẵn sàng";
            this.lblStatusDb.Spring = true;
            this.lblStatusDb.TextAlign = ContentAlignment.MiddleLeft;
            this.lblStatusUser.Text = "Trường Dược - UMP";
            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.lblStatusDb, this.lblStatusUser });

            this.Controls.Add(this.pnlWorkspace);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlTopBar);
            this.Controls.Add(this.statusStrip);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
