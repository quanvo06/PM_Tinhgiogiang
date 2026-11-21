using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmLopHocPhan_NhomHoc
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitMain;
        private SplitContainer splitLeft;
        private DataGridView dgvLopHP, dgvNhomHoc;
        private CheckedListBox clbLopGop;
        private TextBox txtMaNhom, txtTenNhom;
        private ComboBox cboLoaiNhom;
        private NumericUpDown numSiSoToTH;
        private Button btnGopTaoNhom, btnTachToTH, btnXoaNhom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new SplitContainer();
            this.splitLeft = new SplitContainer();
            this.dgvLopHP = new DataGridView();
            this.dgvNhomHoc = new DataGridView();
            this.clbLopGop = new CheckedListBox();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.Text = "Lớp học phần & Trình tạo Nhóm học (4.11 & 4.13)";

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 680;

            // Left: Split vertical Top=LopHP, Bottom=NhomHoc
            this.splitLeft.Dock = DockStyle.Fill;
            this.splitLeft.Orientation = Orientation.Horizontal;
            this.splitLeft.SplitterDistance = 280;

            Panel pnlLopBar = new Panel { Dock = DockStyle.Top, Height = 34 };
            Label lblLop = new Label { Text = "4.11. Lớp học phần", Dock = DockStyle.Left, AutoSize = true, Font = WinFormStyleHelper.BoldFont, Padding = new Padding(0, 6, 0, 0) };
            FlowLayoutPanel flpTach = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true, WrapContents = false };
            this.numSiSoToTH = new NumericUpDown { Minimum = 15, Maximum = 25, Value = 20, Width = 55 };
            this.btnTachToTH = new Button { Text = "Tách tổ TH (15-25 SV)", AutoSize = true, Height = 28 };
            this.btnTachToTH.Click += this.btnTachToTH_Click;
            flpTach.Controls.AddRange(new Control[] { new Label { Text = "SV/tổ:", AutoSize = true, Padding = new Padding(0, 5, 0, 0) }, this.numSiSoToTH, this.btnTachToTH });
            pnlLopBar.Controls.Add(lblLop); pnlLopBar.Controls.Add(flpTach);

            WinFormStyleHelper.ConfigureGrid(this.dgvLopHP);
            this.splitLeft.Panel1.Controls.Add(this.dgvLopHP);
            this.splitLeft.Panel1.Controls.Add(pnlLopBar);

            Panel pnlNhomBar = new Panel { Dock = DockStyle.Top, Height = 34 };
            Label lblNhom = new Label { Text = "4.13. Danh sách Nhóm học (DM_NhomHoc)", Dock = DockStyle.Left, AutoSize = true, Font = WinFormStyleHelper.BoldFont, Padding = new Padding(0, 6, 0, 0) };
            this.btnXoaNhom = new Button { Text = "Xóa nhóm chọn", Dock = DockStyle.Right, Width = 120 };
            this.btnXoaNhom.Click += this.btnXoaNhom_Click;
            pnlNhomBar.Controls.Add(lblNhom); pnlNhomBar.Controls.Add(this.btnXoaNhom);

            WinFormStyleHelper.ConfigureGrid(this.dgvNhomHoc);
            this.splitLeft.Panel2.Controls.Add(this.dgvNhomHoc);
            this.splitLeft.Panel2.Controls.Add(pnlNhomBar);
            this.splitMain.Panel1.Controls.Add(this.splitLeft);

            // Right: Gộp lớp LT / Tạo Nhóm học
            TableLayoutPanel tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, Padding = new Padding(12), BackColor = Color.White };
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.txtMaNhom = new TextBox { Dock = DockStyle.Fill };
            this.txtTenNhom = new TextBox { Dock = DockStyle.Fill };
            this.cboLoaiNhom = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.clbLopGop.Dock = DockStyle.Fill;
            this.clbLopGop.CheckOnClick = true;
            this.btnGopTaoNhom = new Button { Text = "✓ Gộp lớp & Tạo Nhóm học", Dock = DockStyle.Fill, Height = 36, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            this.btnGopTaoNhom.Click += this.btnGopTaoNhom_Click;

            tbl.Controls.Add(new Label { Text = "Gộp Lớp HP thành Nhóm học mới", Font = WinFormStyleHelper.BoldFont, AutoSize = true }, 0, 0);
            tbl.Controls.Add(new Label { Text = "Mã nhóm học * (VD: LT_02)", AutoSize = true }, 0, 1);
            tbl.Controls.Add(this.txtMaNhom, 0, 2);
            tbl.Controls.Add(new Label { Text = "Tên nhóm học * & Loại nhóm (LT/TH/TT)", AutoSize = true }, 0, 3);
            TableLayoutPanel tblTenLoai = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Height = 30 };
            tblTenLoai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tblTenLoai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tblTenLoai.Controls.Add(this.txtTenNhom, 0, 0); tblTenLoai.Controls.Add(this.cboLoaiNhom, 1, 0);
            tbl.Controls.Add(tblTenLoai, 0, 4);
            tbl.Controls.Add(new Label { Text = "Tick chọn các Lớp HP gộp vào nhóm:", AutoSize = true }, 0, 5);
            tbl.Controls.Add(this.clbLopGop, 0, 6);
            tbl.Controls.Add(this.btnGopTaoNhom, 0, 7);

            this.splitMain.Panel2.Controls.Add(tbl);
            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
