using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmPhongHoc : Form
    {
        private readonly DanhMucService _dmService;
        private List<PhongHoc> _allPhongs = new List<PhongHoc>();
        private int _selectedId = 0;

        public frmPhongHoc()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            this.Load += frmPhongHoc_Load;
        }

        private void frmPhongHoc_Load(object sender, EventArgs e)
        {
            try
            {
                cboLoaiPhong.DataSource = _dmService.LayDanhMucTheoNhom(5);
                cboLoaiPhong.DisplayMember = "TenDanhMuc";
                cboLoaiPhong.ValueMember = "MaDanhMuc";
            }
            catch { }
            NapDanhSach();
        }

        private void NapDanhSach()
        {
            try
            {
                _allPhongs = _dmService.LayDanhSachPhongHoc();
                HienThi(_allPhongs);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải phòng học: " + ex.Message); }
        }

        private void HienThi(List<PhongHoc> list)
        {
            dgvPhong.DataSource = list.Select(p => new
            {
                p.ID_PhongHoc,
                p.MaPhong,
                p.TenPhong,
                Loai = p.TenLoaiPhong,
                p.CoSo,
                SucChua = p.SucChua.HasValue ? p.SucChua.Value.ToString() : "Không giới hạn"
            }).ToList();
        }

        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            string q = txtTim.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(q)) { HienThi(_allPhongs); return; }
            HienThi(_allPhongs.Where(p => (p.MaPhong ?? "").ToLowerInvariant().Contains(q) || (p.TenPhong ?? "").ToLowerInvariant().Contains(q)).ToList());
        }

        private void dgvPhong_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhong.CurrentRow == null) return;
            _selectedId = Convert.ToInt32(dgvPhong.CurrentRow.Cells["ID_PhongHoc"].Value);
            PhongHoc ph = _allPhongs.FirstOrDefault(x => x.ID_PhongHoc == _selectedId);
            if (ph == null) return;

            txtMaPhong.Text = ph.MaPhong;
            txtTenPhong.Text = ph.TenPhong;
            if (ph.ID_LoaiPhong.HasValue) cboLoaiPhong.SelectedValue = ph.ID_LoaiPhong.Value;
            txtCoSo.Text = ph.CoSo ?? "";
            txtSucChua.Text = ph.SucChua.HasValue ? ph.SucChua.Value.ToString() : "";
        }

        private void btnMoi_Click(object sender, EventArgs e)
        {
            _selectedId = 0;
            txtMaPhong.Text = ""; txtTenPhong.Text = ""; txtCoSo.Text = ""; txtSucChua.Text = "";
            txtMaPhong.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            int scVal;
            int? sucChua = int.TryParse(txtSucChua.Text.Trim(), out scVal) ? (int?)scVal : null;
            KetQuaThaoTac res = _dmService.LuuPhongHoc(new PhongHoc
            {
                ID_PhongHoc = _selectedId,
                MaPhong = txtMaPhong.Text.Trim(),
                TenPhong = txtTenPhong.Text.Trim(),
                ID_LoaiPhong = cboLoaiPhong.SelectedValue is int ? (int?)cboLoaiPhong.SelectedValue : null,
                CoSo = txtCoSo.Text.Trim(),
                SucChua = sucChua,
                TrangThai = 1
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapDanhSach();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedId <= 0) return;
            if (MessageBox.Show("Xóa phòng học này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                KetQuaThaoTac res = _dmService.XoaPhongHoc(_selectedId);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapDanhSach();
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e) { NapDanhSach(); }
    }
}
