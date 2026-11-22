using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmNamHoc_HocKy_TietHoc : Form
    {
        private readonly ClassLibrary lib = new ClassLibrary();
        private List<NamHoc> _dsNH = new List<NamHoc>();
        private List<HocKy> _dsHK = new List<HocKy>();
        private List<TietHoc> _dsTiet = new List<TietHoc>();
        private int _selNhId = 0;
        private int _selHkId = 0;

        public frmNamHoc_HocKy_TietHoc()
        {
            InitializeComponent();
            this.Load += frmNamHoc_HocKy_TietHoc_Load;
        }

        private void frmNamHoc_HocKy_TietHoc_Load(object sender, EventArgs e)
        {
            NapTatCa();
        }

        private void NapTatCa()
        {
            try
            {
                _dsNH = lib.LayDanhSachNamHoc();
                dgvNamHoc.DataSource = _dsNH.Select(x => new { x.ID_NamHoc, x.MaNH, x.TenNH, x.NgayBatDau, x.NgayKetThuc }).ToList();
                if (dgvNamHoc.Columns["ID_NamHoc"] != null) dgvNamHoc.Columns["ID_NamHoc"].Visible = false;

                cboHkNamHoc.DataSource = _dsNH.ToList();
                cboHkNamHoc.DisplayMember = "TenNH";
                cboHkNamHoc.ValueMember = "ID_NamHoc";

                _dsHK = lib.LayDanhSachHocKy();
                dgvHocKy.DataSource = _dsHK.Select(x => new
                {
                    x.ID_HocKy,
                    x.MaHK,
                    x.TenHK,
                    NamHoc = (_dsNH.FirstOrDefault(n => n.ID_NamHoc == x.ID_NamHoc) ?? new NamHoc { TenNH = "" }).TenNH,
                    x.NgayBD,
                    x.NgayKT
                }).ToList();
                if (dgvHocKy.Columns["ID_HocKy"] != null) dgvHocKy.Columns["ID_HocKy"].Visible = false;

                _dsTiet = lib.LayDanhSachTietHoc();
                dgvTietHoc.DataSource = _dsTiet.Select(x => new { x.ID_TietHoc, x.SoTiet, x.BuoiTrongNgay, x.GioBatDau, x.GioKetThuc }).ToList();
                if (dgvTietHoc.Columns["ID_TietHoc"] != null) dgvTietHoc.Columns["ID_TietHoc"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu cấu hình: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvNamHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNamHoc.CurrentRow == null || dgvNamHoc.CurrentRow.Cells["ID_NamHoc"] == null) return;
            _selNhId = Convert.ToInt32(dgvNamHoc.CurrentRow.Cells["ID_NamHoc"].Value);
            NamHoc nh = _dsNH.FirstOrDefault(x => x.ID_NamHoc == _selNhId);
            if (nh == null) return;
            txtMaNH.Text = nh.MaNH;
            txtTenNH.Text = nh.TenNH;
            DateTime d;
            if (DateTime.TryParseExact(nh.NgayBatDau, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) dtpNhBD.Value = d;
            if (DateTime.TryParseExact(nh.NgayKetThuc, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) dtpNhKT.Value = d;
        }

        private void btnNhMoi_Click(object sender, EventArgs e) { _selNhId = 0; txtMaNH.Clear(); txtTenNH.Clear(); txtMaNH.Focus(); }

        private void btnNhLuu_Click(object sender, EventArgs e)
        {
            KetQuaThaoTac res = lib.LuuNamHoc(new NamHoc
            {
                ID_NamHoc = _selNhId,
                MaNH = txtMaNH.Text.Trim(),
                TenNH = txtTenNH.Text.Trim(),
                NgayBatDau = dtpNhBD.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                NgayKetThuc = dtpNhKT.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) { NapTatCa(); btnNhMoi_Click(sender, e); }
        }

        private void btnNhXoa_Click(object sender, EventArgs e)
        {
            if (_selNhId <= 0) return;
            if (MessageBox.Show("Xóa năm học đang chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            KetQuaThaoTac res = lib.XoaNamHoc(_selNhId);
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapTatCa();
        }

        private void dgvHocKy_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHocKy.CurrentRow == null || dgvHocKy.CurrentRow.Cells["ID_HocKy"] == null) return;
            _selHkId = Convert.ToInt32(dgvHocKy.CurrentRow.Cells["ID_HocKy"].Value);
            HocKy hk = _dsHK.FirstOrDefault(x => x.ID_HocKy == _selHkId);
            if (hk == null) return;
            cboHkNamHoc.SelectedValue = hk.ID_NamHoc;
            txtMaHK.Text = hk.MaHK;
            txtTenHK.Text = hk.TenHK;
            DateTime d;
            if (DateTime.TryParseExact(hk.NgayBD, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) dtpHkBD.Value = d;
            if (DateTime.TryParseExact(hk.NgayKT, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) dtpHkKT.Value = d;
        }

        private void btnHkMoi_Click(object sender, EventArgs e) { _selHkId = 0; txtMaHK.Clear(); txtTenHK.Clear(); txtMaHK.Focus(); }

        private void btnHkLuu_Click(object sender, EventArgs e)
        {
            if (!(cboHkNamHoc.SelectedValue is int)) { MessageBox.Show("Vui lòng tạo và chọn Năm học trước."); return; }
            KetQuaThaoTac res = lib.LuuHocKy(new HocKy
            {
                ID_HocKy = _selHkId,
                ID_NamHoc = (int)cboHkNamHoc.SelectedValue,
                MaHK = txtMaHK.Text.Trim(),
                TenHK = txtTenHK.Text.Trim(),
                NgayBD = dtpHkBD.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                NgayKT = dtpHkKT.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) { NapTatCa(); btnHkMoi_Click(sender, e); }
        }

        private void btnHkXoa_Click(object sender, EventArgs e)
        {
            if (_selHkId <= 0) return;
            if (MessageBox.Show("Xóa học kỳ đang chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            KetQuaThaoTac res = lib.XoaHocKy(_selHkId);
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapTatCa();
        }

        private void dgvTietHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTietHoc.CurrentRow == null || dgvTietHoc.CurrentRow.Cells["SoTiet"] == null) return;
            int so = Convert.ToInt32(dgvTietHoc.CurrentRow.Cells["SoTiet"].Value);
            TietHoc th = _dsTiet.FirstOrDefault(x => x.SoTiet == so);
            if (th == null) return;
            numSoTiet.Value = Math.Max(numSoTiet.Minimum, Math.Min(numSoTiet.Maximum, th.SoTiet));
            int idx = cboBuoiTiet.Items.IndexOf(th.BuoiTrongNgay);
            cboBuoiTiet.SelectedIndex = idx >= 0 ? idx : 0;
            txtGioBD.Text = th.GioBatDau;
            txtGioKT.Text = th.GioKetThuc;
        }

        private void btnTietLuu_Click(object sender, EventArgs e)
        {
            KetQuaThaoTac res = lib.LuuTietHoc(new TietHoc
            {
                SoTiet = (int)numSoTiet.Value,
                BuoiTrongNgay = cboBuoiTiet.Text,
                GioBatDau = txtGioBD.Text.Trim(),
                GioKetThuc = txtGioKT.Text.Trim()
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapTatCa();
        }

        private void btnKhoiTao10Tiet_Click(object sender, EventArgs e)
        {
            KetQuaThaoTac res = lib.KhoiTao10TietChuan();
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapTatCa();
        }
    }
}
