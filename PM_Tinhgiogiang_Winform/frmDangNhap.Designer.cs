using System.Drawing;
using System.Windows.Forms;

namespace PM_Tinhgiogiang_Winform
{
    partial class frmDangNhap
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel tblRoot;
        private Panel pnlLeft;
        private Panel pnlRight;
        private Label lblBrand;
        private Label lblSub;
        private TableLayoutPanel tblForm;
        private Label lblTitle;
        private Label lblUser;
        private TextBox txtTenDangNhap;
        private Label lblPass;
        private TextBox txtMatKhau;
        private Label lblThongBao;
        private FlowLayoutPanel flpButtons;
        private Button btnDangNhap;
        private Button btnThoat;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tblRoot = new TableLayoutPanel();
            this.pnlLeft = new Panel();
            this.pnlRight = new Panel();
            this.lblBrand = new Label();
            this.lblSub = new Label();
            this.tblForm = new TableLayoutPanel();
            this.lblTitle = new Label();
            this.lblUser = new Label();
            this.txtTenDangNhap = new TextBox();
            this.lblPass = new Label();
            this.txtMatKhau = new TextBox();
            this.lblThongBao = new Label();
            this.flpButtons = new FlowLayoutPanel();
            this.btnDangNhap = new Button();
            this.btnThoat = new Button();

            this.SuspendLayout();
            this.Font = WinFormStyleHelper.DefaultFont;
            this.ClientSize = new Size(760, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập — PM_Tinhgiogiang (UMP)";

            this.tblRoot.Dock = DockStyle.Fill;
            this.tblRoot.ColumnCount = 2;
            this.tblRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            this.tblRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            this.tblRoot.RowCount = 1;
            this.tblRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.pnlLeft.Dock = DockStyle.Fill;
            this.pnlLeft.BackColor = WinFormStyleHelper.AccentColor;
            this.pnlLeft.Padding = new Padding(24);

            this.lblBrand.Dock = DockStyle.Top;
            this.lblBrand.ForeColor = Color.White;
            this.lblBrand.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblBrand.Height = 70;
            this.lblBrand.Text = "TRƯỜNG DƯỢC\nĐH Y DƯỢC TP.HCM";

            this.lblSub.Dock = DockStyle.Fill;
            this.lblSub.ForeColor = Color.FromArgb(209, 250, 229);
            this.lblSub.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.lblSub.Text = "\nPhần mềm Xếp lịch Giảng dạy &\nTính giờ chuẩn theo Điều 12.\n\n• Kiểm tra xung đột trực tiếp\n• Chuẩn hóa 10 tiết/ngày\n• Tổng hợp giờ & Xuất Excel";
            this.pnlLeft.Controls.Add(this.lblSub);
            this.pnlLeft.Controls.Add(this.lblBrand);

            this.pnlRight.Dock = DockStyle.Fill;
            this.pnlRight.BackColor = Color.White;
            this.pnlRight.Padding = new Padding(32, 36, 32, 24);

            this.tblForm.Dock = DockStyle.Fill;
            this.tblForm.ColumnCount = 1;
            this.tblForm.RowCount = 7;

            this.lblTitle.Text = "Đăng nhập hệ thống";
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Margin = new Padding(0, 0, 0, 16);

            this.lblUser.Text = "Tên đăng nhập *";
            this.lblUser.AutoSize = true;
            this.txtTenDangNhap.Dock = DockStyle.Fill;
            this.txtTenDangNhap.Margin = new Padding(0, 4, 0, 12);

            this.lblPass.Text = "Mật khẩu *";
            this.lblPass.AutoSize = true;
            this.txtMatKhau.Dock = DockStyle.Fill;
            this.txtMatKhau.UseSystemPasswordChar = true;
            this.txtMatKhau.Margin = new Padding(0, 4, 0, 10);

            this.lblThongBao.ForeColor = Color.Crimson;
            this.lblThongBao.AutoSize = true;
            this.lblThongBao.MaximumSize = new Size(340, 0);
            this.lblThongBao.Margin = new Padding(0, 4, 0, 12);

            this.flpButtons.Dock = DockStyle.Fill;
            this.flpButtons.FlowDirection = FlowDirection.RightToLeft;

            this.btnDangNhap.Text = "Đăng nhập";
            this.btnDangNhap.BackColor = WinFormStyleHelper.AccentColor;
            this.btnDangNhap.ForeColor = Color.White;
            this.btnDangNhap.FlatStyle = FlatStyle.Flat;
            this.btnDangNhap.Width = 120;
            this.btnDangNhap.Height = 34;
            this.btnDangNhap.Click += new System.EventHandler(this.btnDangNhap_Click);

            this.btnThoat.Text = "Thoát";
            this.btnThoat.Width = 90;
            this.btnThoat.Height = 34;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);

            this.flpButtons.Controls.Add(this.btnDangNhap);
            this.flpButtons.Controls.Add(this.btnThoat);

            this.tblForm.Controls.Add(this.lblTitle, 0, 0);
            this.tblForm.Controls.Add(this.lblUser, 0, 1);
            this.tblForm.Controls.Add(this.txtTenDangNhap, 0, 2);
            this.tblForm.Controls.Add(this.lblPass, 0, 3);
            this.tblForm.Controls.Add(this.txtMatKhau, 0, 4);
            this.tblForm.Controls.Add(this.lblThongBao, 0, 5);
            this.tblForm.Controls.Add(this.flpButtons, 0, 6);

            this.pnlRight.Controls.Add(this.tblForm);
            this.tblRoot.Controls.Add(this.pnlLeft, 0, 0);
            this.tblRoot.Controls.Add(this.pnlRight, 1, 0);
            this.Controls.Add(this.tblRoot);

            this.AcceptButton = this.btnDangNhap;
            this.CancelButton = this.btnThoat;
            this.ResumeLayout(false);
        }
    }
}
