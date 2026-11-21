using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmQuyTacQuyDoi
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitMain;
        private DataGridView dgvQuyTac;
        private ComboBox cboSimLoai, cboLoaiHinh;
        private NumericUpDown numSimSiSo, numSiSoTu, numSiSoDen, numHeSo;
        private CheckBox chkSimNN;
        private Label lblSimKetQua;
        private TextBox txtTenQuyTac, txtHieuLucTu;
        private Button btnMoi, btnLuu, btnXoa;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new SplitContainer();
            this.dgvQuyTac = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.Text = "Quy tắc quy đổi giờ chuẩn — Điều 12 (4.16)";

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 640;

            Label lblTitle = new Label { Text = "4.16. Bảng Quy tắc quy đổi (SYS_QuyTacQuyDoi)", Dock = DockStyle.Top, Height = 28, Font = WinFormStyleHelper.BoldFont };
            WinFormStyleHelper.ConfigureGrid(this.dgvQuyTac);
            this.dgvQuyTac.SelectionChanged += this.dgvQuyTac_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvQuyTac);
            this.splitMain.Panel1.Controls.Add(lblTitle);

            // Right: Simulator + Form CRUD
            Panel pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12), AutoScroll = true };
            GroupBox grpSim = new GroupBox { Text = "Bộ mô phỏng hệ số Điều 12", Dock = DockStyle.Top, Height = 140, Padding = new Padding(8) };
            TableLayoutPanel tblSim = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
            tblSim.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblSim.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            this.cboSimLoai = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.cboSimLoai.SelectedIndexChanged += this.TinhMoPhong;
            this.numSimSiSo = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 500, Value = 80 };
            this.numSimSiSo.ValueChanged += this.TinhMoPhong;
            this.chkSimNN = new CheckBox { Text = "Dạy bằng ngoại ngữ (×1.5)", AutoSize = true };
            this.chkSimNN.CheckedChanged += this.TinhMoPhong;
            this.lblSimKetQua = new Label { Dock = DockStyle.Fill, ForeColor = WinFormStyleHelper.AccentColor, Font = WinFormStyleHelper.BoldFont, AutoSize = true };

            tblSim.Controls.Add(this.cboSimLoai, 0, 0); tblSim.Controls.Add(this.numSimSiSo, 1, 0);
            tblSim.Controls.Add(this.chkSimNN, 0, 1); tblSim.SetColumnSpan(this.chkSimNN, 2);
            tblSim.Controls.Add(this.lblSimKetQua, 0, 2); tblSim.SetColumnSpan(this.lblSimKetQua, 2);
            grpSim.Controls.Add(tblSim);

            TableLayoutPanel tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(0, 12, 0, 0) };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));

            this.txtTenQuyTac = new TextBox { Dock = DockStyle.Fill };
            this.cboLoaiHinh = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.numSiSoTu = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 9999 };
            this.numSiSoDen = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 9999, Value = 9999 };
            this.numHeSo = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 2, Increment = 0.05M, Maximum = 10, Value = 1.0M };
            this.txtHieuLucTu = new TextBox { Dock = DockStyle.Fill, Text = "2025-01-01" };

            tbl.Controls.Add(new Label { Text = "Tên quy tắc *", AutoSize = true }, 0, 0); tbl.Controls.Add(this.txtTenQuyTac, 1, 0);
            tbl.Controls.Add(new Label { Text = "Loại hình", AutoSize = true }, 0, 1); tbl.Controls.Add(this.cboLoaiHinh, 1, 1);
            tbl.Controls.Add(new Label { Text = "Sĩ số từ", AutoSize = true }, 0, 2); tbl.Controls.Add(this.numSiSoTu, 1, 2);
            tbl.Controls.Add(new Label { Text = "Sĩ số đến", AutoSize = true }, 0, 3); tbl.Controls.Add(this.numSiSoDen, 1, 3);
            tbl.Controls.Add(new Label { Text = "Hệ số quy đổi", AutoSize = true }, 0, 4); tbl.Controls.Add(this.numHeSo, 1, 4);
            tbl.Controls.Add(new Label { Text = "Hiệu lực từ", AutoSize = true }, 0, 5); tbl.Controls.Add(this.txtHieuLucTu, 1, 5);

            FlowLayoutPanel flp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            this.btnMoi = new Button { Text = "+ Mới", Width = 75, Height = 30 }; this.btnMoi.Click += this.btnMoi_Click;
            this.btnLuu = new Button { Text = "Lưu", Width = 85, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnLuu.Click += this.btnLuu_Click;
            this.btnXoa = new Button { Text = "Xóa", Width = 75, Height = 30 }; this.btnXoa.Click += this.btnXoa_Click;
            flp.Controls.AddRange(new Control[] { this.btnMoi, this.btnLuu, this.btnXoa });
            tbl.Controls.Add(flp, 0, 6); tbl.SetColumnSpan(flp, 2);

            pnlRight.Controls.Add(tbl);
            pnlRight.Controls.Add(grpSim);
            this.splitMain.Panel2.Controls.Add(pnlRight);
            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
