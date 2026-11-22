using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmQuanLyNguoiDung : Form
    {
        private readonly ClassLibrary lib = new ClassLibrary();
        private List<NguoiDung> _dsNguoiDung = new List<NguoiDung>();
        private int _editingId = 0;

        public frmQuanLyNguoiDung()
        {
            InitializeComponent();
            this.Load += frmQuanLyNguoiDung_Load;
        }

        private void frmQuanLyNguoiDung_Load(object sender, EventArgs e)
        {
            NapGiangVien();
            TaiDuLieu();
        }

        private void NapGiangVien()
        {
            try
            {
                List<GiangVien> dsGV = lib.LayDanhSachGiangVien(false);
                dsGV.Insert(0, new GiangVien { ID_GV = 0, HoTen = "-- Không gắn giảng viên --" });
                cboGiangVien.DataSource = dsGV;
                cboGiangVien.DisplayMember = "HoTen";
                cboGiangVien.ValueMember = "ID_GV";
            }
            catch { }
        }

        private void TaiDuLieu()
        {
            try
            {
                _dsNguoiDung = lib.LayDanhSachNguoiDung();
                HienThiLuoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách người dùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiLuoi()
        {
            string kw = (txtTim.Text ?? "").Trim().ToLowerInvariant();
            List<NguoiDung> filtered = _dsNguoiDung.Where(x =>
                string.IsNullOrEmpty(kw) ||
                (x.TenDangNhap != null && x.TenDangNhap.ToLowerInvariant().Contains(kw)) ||
                (x.VaiTro != null && x.VaiTro.ToLowerInvariant().Contains(kw)) ||
                (x.TenGiangVien != null && x.TenGiangVien.ToLowerInvariant().Contains(kw))).ToList();

            dgvNguoiDung.DataSource = null;
            dgvNguoiDung.DataSource = filtered.Select(x => new
            {
                x.ID_NguoiDung,
                TenDangNhap = x.TenDangNhap,
                VaiTro = x.VaiTro,
                GiangVien = x.TenGiangVien ?? "-",
                TrangThai = x.TrangThai ? "Hoạt động" : "Đã khóa"
            }).ToList();

            if (dgvNguoiDung.Columns["ID_NguoiDung"] != null)
                dgvNguoiDung.Columns["ID_NguoiDung"].Visible = false;
        }

        private void dgvNguoiDung_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNguoiDung.CurrentRow == null || dgvNguoiDung.CurrentRow.Cells["ID_NguoiDung"] == null) return;
            int id = Convert.ToInt32(dgvNguoiDung.CurrentRow.Cells["ID_NguoiDung"].Value);
            NguoiDung nd = _dsNguoiDung.FirstOrDefault(x => x.ID_NguoiDung == id);
            if (nd == null) return;

            _editingId = nd.ID_NguoiDung;
            txtTenDangNhap.Text = nd.TenDangNhap;
            txtMatKhau.Text = "";
            int idx = cboVaiTro.Items.IndexOf(nd.VaiTro);
            cboVaiTro.SelectedIndex = idx >= 0 ? idx : 1;
            cboGiangVien.SelectedValue = nd.ID_GV ?? 0;
            chkKichHoat.Checked = nd.TrangThai;
        }

        private void txtTim_TextChanged(object sender, EventArgs e) { HienThiLuoi(); }
        private void btnLamMoi_Click(object sender, EventArgs e) { TaiDuLieu(); }

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            _editingId = 0;
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            cboVaiTro.SelectedIndex = 1;
            cboGiangVien.SelectedValue = 0;
            chkKichHoat.Checked = true;
            txtTenDangNhap.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            int? idGv = (cboGiangVien.SelectedValue is int && (int)cboGiangVien.SelectedValue > 0)
                ? (int?)((int)cboGiangVien.SelectedValue)
                : null;

            NguoiDung model = new NguoiDung
            {
                ID_NguoiDung = _editingId,
                TenDangNhap = txtTenDangNhap.Text.Trim(),
                MatKhauHash = txtMatKhau.Text.Trim(),
                VaiTro = cboVaiTro.SelectedItem != null ? cboVaiTro.SelectedItem.ToString() : "GiaoVu",
                ID_GV = idGv,
                TrangThai = chkKichHoat.Checked
            };

            KetQuaThaoTac kq = lib.LuuNguoiDung(model);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (kq.ThanhCong)
            {
                TaiDuLieu();
                btnThemMoi_Click(sender, e);
            }
        }

        private void btnKhoaMo_Click(object sender, EventArgs e)
        {
            if (_editingId <= 0) return;
            NguoiDung nd = _dsNguoiDung.FirstOrDefault(x => x.ID_NguoiDung == _editingId);
            if (nd == null) return;

            KetQuaThaoTac kq = lib.KhoaMoNguoiDung(_editingId, !nd.TrangThai);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (kq.ThanhCong) TaiDuLieu();
        }
    }
}
