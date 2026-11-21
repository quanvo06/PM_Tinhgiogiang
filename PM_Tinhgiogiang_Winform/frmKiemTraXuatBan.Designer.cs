using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmKiemTraXuatBan
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private TableLayoutPanel tlpKpi;
        private Label lblKpiChan;
        private Label lblKpiCanhBao;
        private Label lblKpiTrangThai;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpToolbar;
        private ComboBox cboLocMucDo;
        private Button btnQuetLai;
        private Button btnDiToiBuoi;
        private Button btnXuatBan;
        private DataGridView dgvXungDot;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private Panel TaoTheKpi(string caption, out Label valueLabel, Color accent)
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            p.BorderStyle = BorderStyle.FixedSingle;
            p.Margin = new Padding(6);
            p.Padding = new Padding(12, 8, 12, 8);

            Label lblCap = new Label();
            lblCap.Text = caption;
            lblCap.Dock = DockStyle.Top;
            lblCap.Height = 20;
            lblCap.ForeColor = Color.FromArgb(100, 116, 139);
            lblCap.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            valueLabel = new Label();
            valueLabel.Text = "0";
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            valueLabel.ForeColor = accent;
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

            p.Controls.Add(valueLabel);
            p.Controls.Add(lblCap);
            return p;
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Panel();
            this.lblTitle = new Label();
            this.tlpKpi = new TableLayoutPanel();
            this.pnlToolbar = new Panel();
            this.flpToolbar = new FlowLayoutPanel();
            this.cboLocMucDo = new ComboBox();
            this.btnQuetLai = new Button();
            this.btnDiToiBuoi = new Button();
            this.btnXuatBan = new Button();
            this.dgvXungDot = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Bước 4 — Kiểm tra xung đột & Xuất bản lịch";

            // Header
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "BƯỚC 4 — TRUNG TÂM KIỂM TRA XUNG ĐỘT & XUẤT BẢN LỊCH GIẢNG DẠY";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            // KPI row
            this.tlpKpi.Dock = DockStyle.Top;
            this.tlpKpi.Height = 84;
            this.tlpKpi.ColumnCount = 3;
            this.tlpKpi.RowCount = 1;
            this.tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            this.tlpKpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.tlpKpi.Controls.Add(TaoTheKpi("LỖI MỨC CHẶN (BẮT BUỘC SỬA)", out this.lblKpiChan, Color.FromArgb(220, 38, 38)), 0, 0);
            this.tlpKpi.Controls.Add(TaoTheKpi("CẢNH BÁO (NÊN KIỂM TRA)", out this.lblKpiCanhBao, Color.FromArgb(217, 119, 6)), 1, 0);
            this.tlpKpi.Controls.Add(TaoTheKpi("ĐIỀU KIỆN XUẤT BẢN", out this.lblKpiTrangThai, WinFormStyleHelper.AccentColor), 2, 0);

            // Toolbar
            this.pnlToolbar.Dock = DockStyle.Top;
            this.pnlToolbar.Height = 46;
            this.pnlToolbar.BackColor = Color.White;
            this.pnlToolbar.Padding = new Padding(8, 7, 8, 6);
            this.pnlToolbar.BorderStyle = BorderStyle.FixedSingle;

            this.flpToolbar.Dock = DockStyle.Fill;
            this.flpToolbar.WrapContents = false;

            this.cboLocMucDo.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLocMucDo.Width = 170;
            this.cboLocMucDo.Items.AddRange(new object[] { "Tất cả xung đột", "Chỉ lỗi Chặn", "Chỉ Cảnh báo" });
            this.cboLocMucDo.SelectedIndex = 0;
            this.cboLocMucDo.SelectedIndexChanged += this.cboLocMucDo_SelectedIndexChanged;

            this.btnQuetLai.Text = "Quét lại xung đột";
            this.btnQuetLai.Width = 135;
            this.btnQuetLai.Height = 28;
            this.btnQuetLai.Click += this.btnQuetLai_Click;

            this.btnDiToiBuoi.Text = "Đi tới buổi học →";
            this.btnDiToiBuoi.Width = 135;
            this.btnDiToiBuoi.Height = 28;
            this.btnDiToiBuoi.Click += this.btnDiToiBuoi_Click;

            this.btnXuatBan.Text = "✓ Xuất bản lịch chính thức";
            this.btnXuatBan.Width = 195;
            this.btnXuatBan.Height = 28;
            this.btnXuatBan.BackColor = WinFormStyleHelper.AccentColor;
            this.btnXuatBan.ForeColor = Color.White;
            this.btnXuatBan.FlatStyle = FlatStyle.Flat;
            this.btnXuatBan.Font = WinFormStyleHelper.BoldFont;
            this.btnXuatBan.Click += this.btnXuatBan_Click;

            this.flpToolbar.Controls.AddRange(new Control[]
            {
                this.cboLocMucDo, this.btnQuetLai, this.btnDiToiBuoi, this.btnXuatBan
            });
            this.pnlToolbar.Controls.Add(this.flpToolbar);

            // Grid
            WinFormStyleHelper.ConfigureGrid(this.dgvXungDot);
            this.dgvXungDot.CellFormatting += this.dgvXungDot_CellFormatting;

            this.Controls.Add(this.dgvXungDot);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.tlpKpi);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
