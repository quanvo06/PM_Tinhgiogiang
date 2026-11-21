using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmGiangVien : Form
    {
        private readonly DanhMucService _dmService;
        private readonly TinhGioService _tgService;
        private readonly XepLichService _xlService;
        private List<GiangVien> _allGVs = new List<GiangVien>();
        private int _selectedGVId = 0;

        public frmGiangVien()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            _tgService = new TinhGioService();
            _xlService = new XepLichService();
            this.Load += frmGiangVien_Load;
        }

        private void frmGiangVien_Load(object sender, EventArgs e)
        {
            NapDanhMucCombobox();
            NapDanhSachGV();
        }

        private void NapDanhMucCombobox()
        {
            try
            {
                cboBoMon.DataSource = _dmService.LayDanhMucTheoNhom(3);
                cboBoMon.DisplayMember = "TenDanhMuc";
                cboBoMon.ValueMember = "MaDanhMuc";

                cboHocVi.DataSource = _dmService.LayDanhMucTheoNhom(2);
                cboHocVi.DisplayMember = "TenDanhMuc";
                cboHocVi.ValueMember = "MaDanhMuc";
            }
            catch { }
        }

        private void NapDanhSachGV()
        {
            try
            {
                _allGVs = _dmService.LayDanhSachGiangVien();
                HienThiLuoi(_allGVs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách giảng viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiLuoi(List<GiangVien> list)
        {
            dgvGiangVien.DataSource = list.Select(g => new
            {
                g.ID_GV,
                g.MaGV,
                g.HoTen,
                BoMon = g.TenBoMon,
                HocVi = g.TenHocVi,
                DinhMuc = g.DinhMucGioChuan,
                g.Email
            }).ToList();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string q = txtTimKiem.Text.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(q)) { HienThiLuoi(_allGVs); return; }
            HienThiLuoi(_allGVs.Where(g => (g.HoTen ?? "").ToLowerInvariant().Contains(q) || (g.MaGV ?? "").ToLowerInvariant().Contains(q)).ToList());
        }

        private void dgvGiangVien_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGiangVien.CurrentRow == null) return;
            object idObj = dgvGiangVien.CurrentRow.Cells["ID_GV"].Value;
            if (idObj == null) return;
            _selectedGVId = Convert.ToInt32(idObj);

            GiangVien gv = _allGVs.FirstOrDefault(x => x.ID_GV == _selectedGVId);
            if (gv == null) return;

            txtMaGV.Text = gv.MaGV;
            txtHoTen.Text = gv.HoTen;
            cboBoMon.SelectedValue = gv.ID_BoMon;
            if (gv.ID_HocVi.HasValue) cboHocVi.SelectedValue = gv.ID_HocVi.Value;
            numDinhMuc.Value = (decimal)gv.DinhMucGioChuan;
            chkThinhGiang.Checked = gv.LaThinhGiang;
            txtEmail.Text = gv.Email ?? "";

            NapChiTiet5Tab(gv);
        }

        private void NapChiTiet5Tab(GiangVien gv)
        {
            try
            {
                dgvRangBuoc.DataSource = _dmService.LayRangBuocGiangVien(gv.ID_GV);
                dgvMienGiam.DataSource = _tgService.LayDanhSachMienGiam().Where(x => x.ID_GV == gv.ID_GV).ToList();

                BaoCaoGioGiangDto bc = _tgService.GetTongHopGioGiang().FirstOrDefault(x => x.MaGV == gv.MaGV);
                if (bc != null)
                {
                    lblTaiGioSummary.Text = string.Format(
                        "Định mức gốc: {0:0.##}h | Miễn giảm: {1:0.##}h | Định mức TT: {2:0.##}h\nĐứng lớp: {3:0.##}h | Đề tài: {4:0.##}h | Khảo thí: {5:0.##}h | NCKH: {6:0.##}h\nTỔNG GIỜ CHUẨN: {7:0.##}h (Vượt: {8:0.##}h)",
                        bc.DinhMucGioChuan, bc.GioMienGiam, bc.DinhMucThucTe, bc.GioDungLop, bc.GioDeTaiHoiDong, bc.GioKhaoThi, bc.GioNCKH, bc.TongGioChuanQuyDoi, bc.GioVuotDinhMuc);
                }

                dgvLichGV.DataSource = _xlService.GetDanhSachBuoiHoc()
                    .Where(b => b.DanhSachID_GV.Contains(gv.ID_GV))
                    .Select(b => new { b.NgayHoc, b.ChuoiTietHoc, b.TenMonHoc, b.MaNhomHoc, b.MaPhong, b.GioChuanQuyDoi })
                    .ToList();
            }
            catch { }
        }

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            _selectedGVId = 0;
            txtMaGV.Text = ""; txtHoTen.Text = ""; txtEmail.Text = ""; numDinhMuc.Value = 270; chkThinhGiang.Checked = false;
            txtMaGV.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            GiangVien gv = new GiangVien
            {
                ID_GV = _selectedGVId,
                MaGV = txtMaGV.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                ID_BoMon = cboBoMon.SelectedValue is int ? (int)cboBoMon.SelectedValue : 0,
                ID_HocVi = cboHocVi.SelectedValue is int ? (int?)cboHocVi.SelectedValue : null,
                DinhMucGioChuan = (double)numDinhMuc.Value,
                LaThinhGiang = chkThinhGiang.Checked,
                Email = txtEmail.Text.Trim(),
                TrangThai = 1
            };
            KetQuaThaoTac res = _dmService.LuuGiangVien(gv);
            MessageBox.Show(res.ThongBao, res.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK, res.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (res.ThanhCong) NapDanhSachGV();
        }

        private void btnNgungHD_Click(object sender, EventArgs e)
        {
            if (_selectedGVId <= 0) return;
            if (MessageBox.Show("Chuyển giảng viên sang Ngừng hoạt động?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                KetQuaThaoTac res = _dmService.NgungHoatDongGiangVien(_selectedGVId);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapDanhSachGV();
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e) { NapDanhSachGV(); }
    }
}
