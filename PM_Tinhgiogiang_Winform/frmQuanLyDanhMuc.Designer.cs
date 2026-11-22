using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmQuanLyDanhMuc
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlTopFilter;
        private Label lblChonNhom;
        private ComboBox cboNhomDanhMuc;
        private Button btnLamMoi;

        private SplitContainer splitMain;
        private DataGridView dgvDanhMuc;

        private Panel pnlRight;
        private Label lblFormTitle;
        private TableLayoutPanel tlpForm;
        private TextBox txtTenDanhMuc;
        private TextBox txtVietTat;
        private TextBox txtGiaTriThamSo;
        private NumericUpDown numThuTu;
        private CheckBox chkKichHoat;
        private FlowLayoutPanel flpActions;
        private Button btnMoi;
        private Button btnLuu;
        private Button btnAn;

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
            this.pnlTopFilter = new Panel();
            this.lblChonNhom = new Label();
            this.cboNhomDanhMuc = new ComboBox();
            this.btnLamMoi = new Button();
            this.splitMain = new SplitContainer();
            this.dgvDanhMuc = new DataGridView();

            this.pnlRight = new Panel();
            this.lblFormTitle = new Label();
            this.tlpForm = new TableLayoutPanel();
            this.txtTenDanhMuc = new TextBox();
            this.txtVietTat = new TextBox();
            this.txtGiaTriThamSo = new TextBox();
            this.numThuTu = new NumericUpDown();
            this.chkKichHoat = new CheckBox();
            this.flpActions = new FlowLayoutPanel();
            this.btnMoi = new Button();
            this.btnLuu = new Button();
            this.btnAn = new Button();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.BackColor = WinFormStyleHelper.BackgroundColor;
            this.Text = "Quản lý Danh mục dùng chung (DM_NhomDanhMuc && DM_DanhMuc)";

            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Padding(12, 8, 12, 6);
            this.lblTitle.Text = "QUẢN LÝ DANH MỤC DÙNG CHUNG (BỘ MÔN, HỌC VỊ, HỌC HÀM, LOẠI PHÒNG...)";
            this.lblTitle.Font = WinFormStyleHelper.TitleFont;
            this.lblTitle.ForeColor = WinFormStyleHelper.AccentColor;
            this.lblTitle.Dock = DockStyle.Fill;
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.pnlTopFilter.Dock = DockStyle.Top;
            this.pnlTopFilter.Height = 44;
            this.pnlTopFilter.BackColor = Color.White;
            this.pnlTopFilter.Padding = new Padding(12, 8, 12, 6);
            this.pnlTopFilter.BorderStyle = BorderStyle.FixedSingle;

            this.lblChonNhom.Text = "Chọn nhóm danh mục:";
            this.lblChonNhom.Dock = DockStyle.Left;
            this.lblChonNhom.Width = 160;
            this.lblChonNhom.TextAlign = ContentAlignment.MiddleLeft;
            this.lblChonNhom.Font = WinFormStyleHelper.BoldFont;

            this.cboNhomDanhMuc.Dock = DockStyle.Left;
            this.cboNhomDanhMuc.Width = 320;
            this.cboNhomDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboNhomDanhMuc.SelectedIndexChanged += this.cboNhomDanhMuc_SelectedIndexChanged;

            this.btnLamMoi.Text = "↻ Làm mới";
            this.btnLamMoi.Dock = DockStyle.Right;
            this.btnLamMoi.Width = 100;
            this.btnLamMoi.Click += this.btnLamMoi_Click;

            this.pnlTopFilter.Controls.Add(this.cboNhomDanhMuc);
            this.pnlTopFilter.Controls.Add(this.lblChonNhom);
            this.pnlTopFilter.Controls.Add(this.btnLamMoi);

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Vertical;
            WinFormStyleHelper.ConfigureSplitContainer(this.splitMain, 0.58, 420, 380, FixedPanel.Panel2);

            WinFormStyleHelper.ConfigureGrid(this.dgvDanhMuc);
            this.dgvDanhMuc.SelectionChanged += this.dgvDanhMuc_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvDanhMuc);

            this.pnlRight.Dock = DockStyle.Fill;
            this.pnlRight.BackColor = Color.White;
            this.pnlRight.Padding = new Padding(14);
            this.pnlRight.AutoScroll = true;

            this.lblFormTitle.Dock = DockStyle.Top;
            this.lblFormTitle.Height = 32;
            this.lblFormTitle.Text = "Chi tiết mục danh mục";
            this.lblFormTitle.Font = WinFormStyleHelper.BoldFont;
            this.lblFormTitle.ForeColor = WinFormStyleHelper.AccentColor;

            this.tlpForm.Dock = DockStyle.Top;
            this.tlpForm.AutoSize = true;
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this.txtTenDanhMuc.Dock = DockStyle.Fill;
            this.txtVietTat.Dock = DockStyle.Fill;
            this.txtGiaTriThamSo.Dock = DockStyle.Fill;
            this.numThuTu.Dock = DockStyle.Fill;
            this.numThuTu.Minimum = 1;
            this.numThuTu.Maximum = 999;
            this.numThuTu.Value = 1;

            this.chkKichHoat.Text = "Đang kích hoạt (hiển thị khi chọn)";
            this.chkKichHoat.Checked = true;
            this.chkKichHoat.AutoSize = true;

            this.flpActions.Dock = DockStyle.Fill;
            this.flpActions.WrapContents = false;
            this.flpActions.AutoSize = true;
            this.flpActions.Padding = new Padding(0, 8, 0, 0);

            this.btnMoi.Text = "+ Mới";
            this.btnMoi.Width = 76;
            this.btnMoi.Height = 30;
            this.btnMoi.Click += this.btnMoi_Click;

            this.btnLuu.Text = "Lưu";
            this.btnLuu.Width = 86;
            this.btnLuu.Height = 30;
            this.btnLuu.BackColor = WinFormStyleHelper.AccentColor;
            this.btnLuu.ForeColor = Color.White;
            this.btnLuu.FlatStyle = FlatStyle.Flat;
            this.btnLuu.Click += this.btnLuu_Click;

            this.btnAn.Text = "Ẩn mục";
            this.btnAn.Width = 86;
            this.btnAn.Height = 30;
            this.btnAn.Click += this.btnAn_Click;

            this.flpActions.Controls.AddRange(new Control[] { this.btnMoi, this.btnLuu, this.btnAn });

            int r = 0;
            this.tlpForm.Controls.Add(TaoNhan("Tên danh mục *"), 0, r); this.tlpForm.Controls.Add(this.txtTenDanhMuc, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Viết tắt"), 0, r); this.tlpForm.Controls.Add(this.txtVietTat, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Giá trị tham số"), 0, r); this.tlpForm.Controls.Add(this.txtGiaTriThamSo, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Thứ tự"), 0, r); this.tlpForm.Controls.Add(this.numThuTu, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan("Trạng thái"), 0, r); this.tlpForm.Controls.Add(this.chkKichHoat, 1, r++);
            this.tlpForm.Controls.Add(TaoNhan(""), 0, r); this.tlpForm.Controls.Add(this.flpActions, 1, r++);

            this.pnlRight.Controls.Add(this.tlpForm);
            this.pnlRight.Controls.Add(this.lblFormTitle);
            this.splitMain.Panel2.Controls.Add(this.pnlRight);

            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlTopFilter);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }
    }
}
