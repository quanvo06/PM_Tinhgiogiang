using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmMain : Form
    {
        private readonly DanhMucService _danhMucService;
        private Form _activeChildForm;
        private Button _activeNavButton;

        public frmMain()
        {
            InitializeComponent();
            _danhMucService = new DanhMucService();
            this.Load += frmMain_Load;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            NapNguCanhTopBar();
            MoFormCon(new frmTongQuan(), btnNavTongQuan);
        }

        private void NapNguCanhTopBar()
        {
            try
            {
                List<NamHoc> namHocs = _danhMucService.LayDanhSachNamHoc();
                cboNamHoc.DataSource = namHocs;
                cboNamHoc.DisplayMember = "TenNH";
                cboNamHoc.ValueMember = "ID_NamHoc";

                List<HocKy> hocKys = _danhMucService.LayDanhSachHocKy();
                cboHocKy.DataSource = hocKys;
                cboHocKy.DisplayMember = "TenHK";
                cboHocKy.ValueMember = "ID_HocKy";

                List<DanhMuc> boMons = _danhMucService.LayDanhMucTheoNhom(3);
                boMons.Insert(0, new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "Tất cả bộ môn" });
                cboBoMon.DataSource = boMons;
                cboBoMon.DisplayMember = "TenDanhMuc";
                cboBoMon.ValueMember = "MaDanhMuc";

                cboDot.Items.Clear();
                cboDot.Items.Add("Đợt 1 (Tất cả tuần)");
                cboDot.SelectedIndex = 0;

                string userName = SessionHelper.LoggedInUser != null ? SessionHelper.LoggedInUser.TenDangNhap : "Giáo vụ";
                lblStatusUser.Text = "Người dùng: " + userName + " | SQL Server: PM_Tinhgiogiang";
            }
            catch (Exception ex)
            {
                lblStatusDb.Text = "Cảnh báo kết nối: " + ex.Message;
            }
        }

        private void ContextChanged(object sender, EventArgs e)
        {
            if (cboNamHoc.SelectedValue is int) SessionHelper.CurrentNamHocId = (int)cboNamHoc.SelectedValue;
            if (cboHocKy.SelectedValue is int) SessionHelper.CurrentHocKyId = (int)cboHocKy.SelectedValue;
            if (cboBoMon.SelectedValue is int)
            {
                int bm = (int)cboBoMon.SelectedValue;
                SessionHelper.CurrentBoMonId = bm > 0 ? (int?)bm : null;
            }
        }

        public void MoFormCon(Form childForm, Button navButton = null)
        {
            if (_activeChildForm != null)
            {
                _activeChildForm.Close();
                _activeChildForm.Dispose();
            }

            _activeChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlWorkspace.Controls.Clear();
            pnlWorkspace.Controls.Add(childForm);
            childForm.Show();

            if (navButton != null)
            {
                if (_activeNavButton != null)
                {
                    _activeNavButton.BackColor = Color.Transparent;
                    _activeNavButton.ForeColor = Color.FromArgb(51, 65, 85);
                    _activeNavButton.Font = WinFormStyleHelper.DefaultFont;
                }
                _activeNavButton = navButton;
                _activeNavButton.BackColor = Color.FromArgb(236, 253, 245);
                _activeNavButton.ForeColor = WinFormStyleHelper.AccentColor;
                _activeNavButton.Font = WinFormStyleHelper.BoldFont;
            }

            lblStatusDb.Text = "● Đang mở: " + childForm.Text;
        }

        private void btnNavTongQuan_Click(object sender, EventArgs e) { MoFormCon(new frmTongQuan(), btnNavTongQuan); }
        private void btnNavPhanCong_Click(object sender, EventArgs e) { MoFormCon(new frmPhanCongGiangDay(), btnNavPhanCong); }
        private void btnNavXepLich_Click(object sender, EventArgs e) { MoFormCon(new frmXepLich(), btnNavXepLich); }
        private void btnNavKiemTra_Click(object sender, EventArgs e) { MoFormCon(new frmKiemTraXuatBan(), btnNavKiemTra); }
        private void btnNavTinhGio_Click(object sender, EventArgs e) { MoFormCon(new frmTongHopGioChuan(), btnNavTinhGio); }

        private void btnNavGiangVien_Click(object sender, EventArgs e) { MoFormCon(new frmGiangVien(), btnNavGiangVien); }
        private void btnNavPhongHoc_Click(object sender, EventArgs e) { MoFormCon(new frmPhongHoc(), btnNavPhongHoc); }
        private void btnNavMonBai_Click(object sender, EventArgs e) { MoFormCon(new frmMonHoc_BaiHoc(), btnNavMonBai); }
        private void btnNavLopNhom_Click(object sender, EventArgs e) { MoFormCon(new frmLopHocPhan_NhomHoc(), btnNavLopNhom); }
        private void btnNavQuyTac_Click(object sender, EventArgs e) { MoFormCon(new frmQuyTacQuyDoi(), btnNavQuyTac); }

        public void MoXepLichTheoBuoi(int idBuoiHoc)
        {
            MoFormCon(new frmXepLich(idBuoiHoc), btnNavXepLich);
        }
    }
}
