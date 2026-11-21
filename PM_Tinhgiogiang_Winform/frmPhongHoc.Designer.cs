using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmPhongHoc
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitMain;
        private Panel pnlTop;
        private TextBox txtTim;
        private Button btnLamMoi;
        private DataGridView dgvPhong;
        private TextBox txtMaPhong, txtTenPhong, txtCoSo, txtSucChua;
        private ComboBox cboLoaiPhong;
        private Button btnMoi, btnLuu, btnXoa;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new SplitContainer();
            this.pnlTop = new Panel();
            this.txtTim = new TextBox();
            this.btnLamMoi = new Button();
            this.dgvPhong = new DataGridView();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.Text = "Phòng học & Lab (4.6)";

            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.SplitterDistance = 640;

            this.pnlTop.Dock = DockStyle.Top; this.pnlTop.Height = 36;
            this.txtTim.Dock = DockStyle.Left; this.txtTim.Width = 260; this.txtTim.TextChanged += this.txtTim_TextChanged;
            this.btnLamMoi.Text = "↻ Làm mới"; this.btnLamMoi.Dock = DockStyle.Right; this.btnLamMoi.Width = 95; this.btnLamMoi.Click += this.btnLamMoi_Click;
            this.pnlTop.Controls.Add(this.txtTim); this.pnlTop.Controls.Add(this.btnLamMoi);

            WinFormStyleHelper.ConfigureGrid(this.dgvPhong);
            this.dgvPhong.SelectionChanged += this.dgvPhong_SelectionChanged;
            this.splitMain.Panel1.Controls.Add(this.dgvPhong);
            this.splitMain.Panel1.Controls.Add(this.pnlTop);

            TableLayoutPanel tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(12), BackColor = Color.White };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.txtMaPhong = new TextBox { Dock = DockStyle.Fill };
            this.txtTenPhong = new TextBox { Dock = DockStyle.Fill };
            this.cboLoaiPhong = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            this.txtCoSo = new TextBox { Dock = DockStyle.Fill };
            this.txtSucChua = new TextBox { Dock = DockStyle.Fill };

            tbl.Controls.Add(new Label { Text = "Mã phòng *", AutoSize = true }, 0, 0); tbl.Controls.Add(this.txtMaPhong, 1, 0);
            tbl.Controls.Add(new Label { Text = "Tên phòng *", AutoSize = true }, 0, 1); tbl.Controls.Add(this.txtTenPhong, 1, 1);
            tbl.Controls.Add(new Label { Text = "Loại phòng", AutoSize = true }, 0, 2); tbl.Controls.Add(this.cboLoaiPhong, 1, 2);
            tbl.Controls.Add(new Label { Text = "Cơ sở", AutoSize = true }, 0, 3); tbl.Controls.Add(this.txtCoSo, 1, 3);
            tbl.Controls.Add(new Label { Text = "Sức chứa (trống = ∞)", AutoSize = true }, 0, 4); tbl.Controls.Add(this.txtSucChua, 1, 4);

            FlowLayoutPanel flp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            this.btnMoi = new Button { Text = "+ Mới", Width = 75, Height = 30 }; this.btnMoi.Click += this.btnMoi_Click;
            this.btnLuu = new Button { Text = "Lưu", Width = 85, Height = 30, BackColor = WinFormStyleHelper.AccentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat }; this.btnLuu.Click += this.btnLuu_Click;
            this.btnXoa = new Button { Text = "Xóa", Width = 75, Height = 30 }; this.btnXoa.Click += this.btnXoa_Click;
            flp.Controls.AddRange(new Control[] { this.btnMoi, this.btnLuu, this.btnXoa });
            tbl.Controls.Add(flp, 0, 5); tbl.SetColumnSpan(flp, 2);

            this.splitMain.Panel2.Controls.Add(tbl);
            this.Controls.Add(this.splitMain);
            this.ResumeLayout(false);
        }
    }
}
