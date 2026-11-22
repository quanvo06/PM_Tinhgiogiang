using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Winform
{
    /// <summary>
    /// Form chính (Dashboard) theo chuẩn khung HNKHKTD_Winform:
    /// MenuStrip + panelHeader + panelContent các nút chức năng mở Form con bằng ShowDialog().
    /// </summary>
    public partial class frmMain : Form
    {
        private readonly ClassLibrary lib = new ClassLibrary();

        public bool IsLoggingOut { get; private set; }

        public frmMain()
        {
            InitializeComponent();
            this.Load += frmMain_Load;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (SessionHelper.LoggedInUser != null)
            {
                lblStatusUser.Text = "Đang đăng nhập: " + SessionHelper.LoggedInUser.TenDangNhap +
                                     " | Vai trò: " + SessionHelper.LoggedInUser.VaiTro +
                                     " | SQL Server: PM_Tinhgiogiang (LINQ to SQL)";
            }
            else
            {
                lblStatusUser.Text = "Chưa đăng nhập";
            }

            NapNguCanhTopBar();
            ApDungPhanQuyen();
        }

        private void NapNguCanhTopBar()
        {
            try
            {
                List<NamHoc> namHocs = lib.LayDanhSachNamHoc();
                cboNamHoc.DataSource = namHocs;
                cboNamHoc.DisplayMember = "TenNH";
                cboNamHoc.ValueMember = "ID_NamHoc";

                List<HocKy> hocKys = lib.LayDanhSachHocKy();
                cboHocKy.DataSource = hocKys;
                cboHocKy.DisplayMember = "TenHK";
                cboHocKy.ValueMember = "ID_HocKy";

                List<DanhMuc> boMons = lib.LayDanhMucTheoNhom(ClassLibrary.NHOM_BO_MON);
                boMons.Insert(0, new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "Tất cả bộ môn" });
                cboBoMon.DataSource = boMons;
                cboBoMon.DisplayMember = "TenDanhMuc";
                cboBoMon.ValueMember = "MaDanhMuc";
            }
            catch (Exception ex)
            {
                lblStatusUser.Text = "Cảnh báo kết nối CSDL: " + ex.Message;
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

        /// <summary>
        /// Kiểm soát ẩn/hiện các nút và menu theo vai trò đăng nhập (chuẩn HNKHKTD).
        /// </summary>
        private void ApDungPhanQuyen()
        {
            bool isAdmin = SessionHelper.IsAdmin();
            bool canXepLich = SessionHelper.CanXepLich();

            qlndToolStripMenuItem.Visible = isAdmin;

            btnPhanCong.Visible = canXepLich;
            phanCongToolStripMenuItem.Visible = canXepLich;

            btnKiemTra.Visible = canXepLich;
            kiemTraToolStripMenuItem.Visible = canXepLich;

            btnDmDungChung.Visible = canXepLich;
            btnDmGiangVien.Visible = canXepLich;
            btnDmPhongHoc.Visible = canXepLich;
            btnDmMonBai.Visible = canXepLich;
            btnDmLopNhom.Visible = canXepLich;
            btnDmQuyTac.Visible = canXepLich;
            danhMucToolStripMenuItem.Visible = canXepLich;

            SapXepCacNutChucNang();
        }

        private void SapXepCacNutChucNang()
        {
            int spacing = 78;

            int yCol1 = 32;
            Button[] col1 = new Button[] { btnTongQuan, btnPhanCong, btnXepLich, btnKiemTra, btnTinhGio };
            foreach (Button btn in col1)
            {
                if (btn.Visible)
                {
                    btn.Location = new Point(btn.Location.X, yCol1);
                    yCol1 += spacing;
                }
            }

            int yCol2 = 32;
            Button[] col2 = new Button[] { btnDmDungChung, btnDmGiangVien, btnDmPhongHoc, btnDmMonBai, btnDmLopNhom, btnDmQuyTac };
            foreach (Button btn in col2)
            {
                if (btn.Visible)
                {
                    btn.Location = new Point(btn.Location.X, yCol2);
                    yCol2 += spacing;
                }
            }
        }

        private void MoPopup(Form f)
        {
            f.WindowState = FormWindowState.Maximized;
            f.ShowDialog();
        }

        public void MoXepLichTheoBuoi(int idBuoiHoc)
        {
            using (frmXepLich f = new frmXepLich(idBuoiHoc))
            {
                MoPopup(f);
            }
        }

        private void btnTongQuan_Click(object sender, EventArgs e) { using (frmTongQuan f = new frmTongQuan()) MoPopup(f); }
        private void btnPhanCong_Click(object sender, EventArgs e) { using (frmPhanCongGiangDay f = new frmPhanCongGiangDay()) MoPopup(f); }
        private void btnXepLich_Click(object sender, EventArgs e) { using (frmXepLich f = new frmXepLich()) MoPopup(f); }
        private void btnKiemTra_Click(object sender, EventArgs e) { using (frmKiemTraXuatBan f = new frmKiemTraXuatBan()) MoPopup(f); }
        private void btnTinhGio_Click(object sender, EventArgs e) { using (frmTongHopGioChuan f = new frmTongHopGioChuan()) MoPopup(f); }

        private void namHocHocKyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmNamHoc_HocKy_TietHoc f = new frmNamHoc_HocKy_TietHoc()) MoPopup(f);
            NapNguCanhTopBar();
        }
        private void quanLyDanhMucToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmQuanLyDanhMuc f = new frmQuanLyDanhMuc()) MoPopup(f);
            NapNguCanhTopBar();
        }
        private void giangVienToolStripMenuItem_Click(object sender, EventArgs e) { using (frmGiangVien f = new frmGiangVien()) MoPopup(f); }
        private void phongHocToolStripMenuItem_Click(object sender, EventArgs e) { using (frmPhongHoc f = new frmPhongHoc()) MoPopup(f); }
        private void monBaiToolStripMenuItem_Click(object sender, EventArgs e) { using (frmMonHoc_BaiHoc f = new frmMonHoc_BaiHoc()) MoPopup(f); }
        private void lopNhomToolStripMenuItem_Click(object sender, EventArgs e) { using (frmLopHocPhan_NhomHoc f = new frmLopHocPhan_NhomHoc()) MoPopup(f); }
        private void quyTacToolStripMenuItem_Click(object sender, EventArgs e) { using (frmQuyTacQuyDoi f = new frmQuyTacQuyDoi()) MoPopup(f); }
        private void qlndToolStripMenuItem_Click(object sender, EventArgs e) { using (frmQuanLyNguoiDung f = new frmQuanLyNguoiDung()) MoPopup(f); }

        private void dangXuatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn muốn đăng xuất khỏi tài khoản hiện tại?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.IsLoggingOut = true;
                this.Close();
            }
        }

        private void thoatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
