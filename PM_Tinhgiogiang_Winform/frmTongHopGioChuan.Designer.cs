using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmTongHopGioChuan
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpToolbar;
        private ComboBox cboPhamVi;
        private TextBox txtTimGV;
        private Button btnTaiLai;
        private Button btnXuatExcel;
        private Label lblTongKet;

        private SplitContainer splitMain;
        private DataGridView dgvTongHop;
        private Label lblChiTietCaption;
        private DataGridView dgvChiTietBuoi;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.pnlToolbar = new Panel();
            this.flpToolbar = new FlowLayoutPanel();
            this.cboPhamVi = new ComboBox();
            this.txtTimGV = new TextBox();
            this.btnTaiLai = new Button();
            this.btnXuatExcel = new Button();
            this.lblTongKet = new Label();

            this.splitMain = new SplitContainer();
            this.dgvTongHop = new DataGridView();
            this.lblChiTietCaption = new Label();
            this.dgvChiTietBuoi = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Bước 5 — Tổng hợp Giờ chuẩn & Xuất báo cáo Excel";

            // Header
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "BƯỚC 5 — BẢNG TỔNG HỢP GIỜ CHUẨN THEO ĐIỀU 12 & XUẤT EXCEL";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // Toolbar
            this.pnlToolbar.Dock = DockStyle.Top;
            this.pnlToolbar.Height = 48;
            this.pnlToolbar.BackColor = Color.White;
            this.pnlToolbar.Padding = new Padding(8, 8, 8, 6);
            this.pnlToolbar.BorderStyle = BorderStyle.FixedSingle;

            this.flpToolbar.Dock = DockStyle.Fill;
            this.flpToolbar.WrapContents = false;

            this.cboPhamVi.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboPhamVi.Width = 185;
            this.cboPhamVi.Items.AddRange(new object[] { "Theo học kỳ đang chọn", "Cả năm học đang chọn", "Toàn bộ dữ liệu" });
            this.cboPhamVi.SelectedIndex = 0;
            this.cboPhamVi.SelectedIndexChanged += this.cboPhamVi_SelectedIndexChanged;

            this.txtTimGV.Width = 220;
            this.txtTimGV.TextChanged += this.txtTimGV_TextChanged;

            this.btnTaiLai.Text = "Tính lại giờ";
            this.btnTaiLai.Width = 105;
            this.btnTaiLai.Height = 28;
            this.btnTaiLai.Click += this.btnTaiLai_Click;

            this.btnXuatExcel.Text = "Xuất Excel (.csv)";
            this.btnXuatExcel.Width = 140;
            this.btnXuatExcel.Height = 28;
            this.btnXuatExcel.BackColor = WinFormStyleHelper.AccentColor;
            this.btnXuatExcel.ForeColor = Color.White;
            this.btnXuatExcel.FlatStyle = FlatStyle.Flat;
            this.btnXuatExcel.Font = WinFormStyleHelper.BoldFont;
            this.btnXuatExcel.Click += this.btnXuatExcel_Click;

            this.lblTongKet.AutoSize = true;
            this.lblTongKet.Padding = new Padding(12, 6, 0, 0);
            this.lblTongKet.Font = WinFormStyleHelper.BoldFont;
            this.lblTongKet.ForeColor = Color.FromArgb(15, 23, 42);
            this.lblTongKet.Text = "";

            this.flpToolbar.Controls.AddRange(new Control[]
            {
                this.cboPhamVi, this.txtTimGV, this.btnTaiLai, this.btnXuatExcel, this.lblTongKet
            });
            this.pnlToolbar.Controls.Add(this.flpToolbar);

            // Split Main (Trên: Tổng hợp GV / Dưới: Chi tiết từng buổi)
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Horizontal;
            this.splitMain.SplitterDistance = 360;

            WinFormStyleHelper.ConfigureGrid(this.dgvTongHop);
            this.dgvTongHop.SelectionChanged += this.dgvTongHop_SelectionChanged;
            this.dgvTongHop.CellFormatting += this.dgvTongHop_CellFormatting;
            this.splitMain.Panel1.Controls.Add(this.dgvTongHop);

            this.lblChiTietCaption.Dock = DockStyle.Top;
            this.lblChiTietCaption.Height = 30;
            this.lblChiTietCaption.Text = "  Chi tiết các buổi đứng lớp của giảng viên đang chọn:";
            this.lblChiTietCaption.Font = WinFormStyleHelper.BoldFont;
            this.lblChiTietCaption.TextAlign = ContentAlignment.MiddleLeft;
            this.lblChiTietCaption.BackColor = Color.FromArgb(241, 245, 249);

            WinFormStyleHelper.ConfigureGrid(this.dgvChiTietBuoi);
            this.splitMain.Panel2.Controls.Add(this.dgvChiTietBuoi);
            this.splitMain.Panel2.Controls.Add(this.lblChiTietCaption);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
