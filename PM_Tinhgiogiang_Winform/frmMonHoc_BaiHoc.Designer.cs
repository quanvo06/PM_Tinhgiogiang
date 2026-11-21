using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmMonHoc_BaiHoc
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitMain;
        private DataGridView dgvMonHoc, dgvBaiHoc;
        private TextBox txtTenBai;
        private NumericUpDown numThuTu, numSoTiet;
        private ComboBox cboLoaiBai, cboGVMacDinh;
        private Button btnThemBai, btnXoaBai;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new SplitContainer();
            this.dgvMonHoc = new DataGridView();
            this.dgvBaiHoc = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.Text = "Môn học & Đề cương Bài học (4.7 & 4.8)";

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 460;

            Label lblMon = new Label { Text = "Danh sách Môn học (DM_MonHoc)", Dock = DockStyle.Top, Height = 28, Font = WinFormStyleHelper.BoldFont };
            WinFormStyleHelper.ConfigureGrid(this.dgvMonHoc);
            this.dgvMonHoc.SelectionChanged += this.dgvMonHoc_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvMonHoc);
            this.splitMain.Panel1.Controls.Add(lblMon);

            // Right panel: Top = Bài học Grid, Bottom = Form thêm bài
            Panel pnlAddBai = new Panel { Dock = DockStyle.Bottom, Height = 128, BackColor = Color.White, Padding = new Padding(8), BorderStyle = BorderStyle.FixedSingle };
            TableLayoutPanel tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 3 };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            this.numThuTu = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 100, Value = 1 };
            this.cboLoaiBai = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.numSoTiet = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 10, Value = 3 };
            this.cboGVMacDinh = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.txtTenBai = new TextBox { Dock = DockStyle.Fill };
            this.btnThemBai = new Button { Text = "+ Thêm bài học", Width = 120, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnThemBai.Click += this.btnThemBai_Click;
            this.btnXoaBai = new Button { Text = "Xóa bài chọn", Width = 105, Height = 30 };
            this.btnXoaBai.Click += this.btnXoaBai_Click;

            tbl.Controls.Add(new Label { Text = "Thứ tự", AutoSize = true }, 0, 0); tbl.Controls.Add(this.numThuTu, 1, 0);
            tbl.Controls.Add(new Label { Text = "Loại / Số tiết", AutoSize = true }, 2, 0);
            FlowLayoutPanel flpLT = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            this.cboLoaiBai.Width = 70; this.numSoTiet.Width = 60;
            flpLT.Controls.AddRange(new Control[] { this.cboLoaiBai, this.numSoTiet });
            tbl.Controls.Add(flpLT, 3, 0);

            tbl.Controls.Add(new Label { Text = "Tên bài học *", AutoSize = true }, 0, 1); tbl.Controls.Add(this.txtTenBai, 1, 1);
            tbl.Controls.Add(new Label { Text = "GV mặc định", AutoSize = true }, 2, 1); tbl.Controls.Add(this.cboGVMacDinh, 3, 1);

            FlowLayoutPanel flpBtn = new FlowLayoutPanel { Dock = DockStyle.Fill };
            flpBtn.Controls.AddRange(new Control[] { this.btnThemBai, this.btnXoaBai });
            tbl.Controls.Add(flpBtn, 0, 2); tbl.SetColumnSpan(flpBtn, 4);
            pnlAddBai.Controls.Add(tbl);

            Label lblBai = new Label { Text = "Đề cương Bài học theo thứ tự (DM_BaiHoc)", Dock = DockStyle.Top, Height = 28, Font = WinFormStyleHelper.BoldFont };
            WinFormStyleHelper.ConfigureGrid(this.dgvBaiHoc);
            this.splitMain.Panel2.Controls.Add(this.dgvBaiHoc);
            this.splitMain.Panel2.Controls.Add(lblBai);
            this.splitMain.Panel2.Controls.Add(pnlAddBai);

            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
