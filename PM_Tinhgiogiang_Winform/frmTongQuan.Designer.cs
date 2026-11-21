using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmTongQuan
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnLamMoi;
        private TableLayoutPanel tblKpi;
        private Label lblKpiBuoiVal;
        private Label lblKpiXungDotVal;
        private Label lblKpiGvVal;
        private Label lblKpiNhomVal;
        private SplitContainer splitMain;
        private DataGridView dgvViecCanLam;
        private DataGridView dgvTienDoDot;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel TaoCardKpi(string caption, out Label lblValue, Color accentColor)
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            p.BorderStyle = BorderStyle.FixedSingle;
            p.Padding = new Padding(12);
            p.Margin = new Padding(4);

            Label cap = new Label();
            cap.Dock = DockStyle.Top;
            cap.Height = 22;
            cap.Text = caption;
            cap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            cap.ForeColor = Color.FromArgb(100, 116, 139);

            lblValue = new Label();
            lblValue.Dock = DockStyle.Fill;
            lblValue.Text = "0";
            lblValue.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblValue.ForeColor = accentColor;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            p.Controls.Add(lblValue);
            p.Controls.Add(cap);
            return p;
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.btnLamMoi = new Button();
            this.tblKpi = new TableLayoutPanel();
            this.splitMain = new SplitContainer();
            this.dgvViecCanLam = new DataGridView();
            this.dgvTienDoDot = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Tổng quan điều hành";

            // Header
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.lblTitle.Text = "Tổng quan xếp lịch & giờ chuẩn (3.5)";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.Dock = DockStyle.Left;
            this.lblTitle.AutoSize = true;

            this.btnLamMoi.Text = "↻ Làm mới";
            this.btnLamMoi.Dock = DockStyle.Right;
            this.btnLamMoi.Width = 100;
            this.btnLamMoi.Click += this.btnLamMoi_Click;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnLamMoi);

            // 4 KPI Cards
            this.tblKpi.Dock = DockStyle.Top;
            this.tblKpi.Height = 96;
            this.tblKpi.ColumnCount = 4;
            this.tblKpi.RowCount = 1;
            for (int i = 0; i < 4; i++) this.tblKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.tblKpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.tblKpi.Controls.Add(TaoCardKpi("BUỔI ĐÃ XẾP", out this.lblKpiBuoiVal, WinFormStyleHelper.AccentColor), 0, 0);
            this.tblKpi.Controls.Add(TaoCardKpi("CẢNH BÁO / THIẾU PHÒNG", out this.lblKpiXungDotVal, Color.Crimson), 1, 0);
            this.tblKpi.Controls.Add(TaoCardKpi("GV THIẾU / VƯỢT ĐỊNH MỨC", out this.lblKpiGvVal, Color.RoyalBlue), 2, 0);
            this.tblKpi.Controls.Add(TaoCardKpi("% NHÓM ĐÃ XẾP", out this.lblKpiNhomVal, Color.DarkCyan), 3, 0);

            // SplitContainer 2 cột
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 520;
            this.splitMain.Padding = new Padding(0, 8, 0, 0);

            Label lblLeft = new Label { Text = "Việc cần làm", Dock = DockStyle.Top, Height = 26, Font = WinFormStyleHelper.BoldFont };
            WinFormStyleHelper.ConfigureGrid(this.dgvViecCanLam);
            this.splitMain.Panel1.Controls.Add(this.dgvViecCanLam);
            this.splitMain.Panel1.Controls.Add(lblLeft);

            Label lblRight = new Label { Text = "Tiến độ đợt / Nhóm học", Dock = DockStyle.Top, Height = 26, Font = WinFormStyleHelper.BoldFont };
            WinFormStyleHelper.ConfigureGrid(this.dgvTienDoDot);
            this.splitMain.Panel2.Controls.Add(this.dgvTienDoDot);
            this.splitMain.Panel2.Controls.Add(lblRight);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.tblKpi);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
