using System;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmDangNhap : Form
    {
        private readonly DanhMucService _danhMucService;

        public frmDangNhap()
        {
            InitializeComponent();
            _danhMucService = new DanhMucService();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string user = txtTenDangNhap.Text.Trim();
            string pass = txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                lblThongBao.Text = "Vui lòng nhập Tên đăng nhập và Mật khẩu.";
                return;
            }

            try
            {
                NguoiDung nd = _danhMucService.DangNhap(user, pass);
                if (nd == null)
                {
                    lblThongBao.Text = "Thông tin đăng nhập không chính xác hoặc tài khoản bị khóa.";
                    return;
                }

                SessionHelper.LoggedInUser = nd;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblThongBao.Text = "Lỗi kết nối SQL Server: " + ex.Message;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
