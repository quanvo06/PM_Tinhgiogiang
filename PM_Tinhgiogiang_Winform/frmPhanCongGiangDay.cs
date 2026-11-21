using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmPhanCongGiangDay : Form
    {
        private readonly DanhMucService _danhMucService;
        private readonly XepLichService _xepLichService;
        private readonly TinhGioService _tinhGioService;

        private List<NhomHoc> _dsNhom = new List<NhomHoc>();
        private List<BuoiHocChiTiet> _dsBuoiAll = new List<BuoiHocChiTiet>();
        private List<GiangVien> _dsGiangVien = new List<GiangVien>();
        private List<PhanCongRowItem> _dsPhanCongHienTai = new List<PhanCongRowItem>();
        private BuoiHocChiTiet _buoiDangChon;

        private class PhanCongRowItem
        {
            public int ID_GV { get; set; }
            public string MaGV { get; set; }
            public string HoTen { get; set; }
            public string VaiTro { get; set; }
            public double SoTietGiangDay { get; set; }
            public double HeSoQuyDoi { get; set; }
            public double GioChuanQuyDoi { get; set; }
        }

        public frmPhanCongGiangDay()
        {
            InitializeComponent();
            _danhMucService = new DanhMucService();
            _xepLichService = new XepLichService();
            _tinhGioService = new TinhGioService();
            this.Load += frmPhanCongGiangDay_Load;
        }

        private void frmPhanCongGiangDay_Load(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        private void TaiDuLieu()
        {
            try
            {
                int? idHk = SessionHelper.CurrentHocKyId;
                int? idNh = SessionHelper.CurrentNamHocId;

                _dsNhom = _danhMucService.LayDanhSachNhomHoc(idHk);
                _dsBuoiAll = _xepLichService.GetDanhSachBuoiHoc(idHk);
                _dsGiangVien = _danhMucService.LayDanhSachGiangVien(true);

                cboGiangVien.DataSource = null;
                cboGiangVien.DataSource = _dsGiangVien;
                cboGiangVien.DisplayMember = "HoTen";
                cboGiangVien.ValueMember = "ID_GV";

                HienThiNhomHoc();
                TaiBangTaiGio(idHk, idNh);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu phân công: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiNhomHoc()
        {
            string kw = (txtTimNhom.Text ?? "").Trim().ToLowerInvariant();
            List<NhomHoc> filtered = _dsNhom.Where(n =>
                string.IsNullOrEmpty(kw) ||
                (n.MaNhomHoc != null && n.MaNhomHoc.ToLowerInvariant().Contains(kw)) ||
                (n.TenNhomHoc != null && n.TenNhomHoc.ToLowerInvariant().Contains(kw)) ||
                (n.TenMonHoc != null && n.TenMonHoc.ToLowerInvariant().Contains(kw))).ToList();

            dgvNhomHoc.DataSource = null;
            dgvNhomHoc.DataSource = filtered.Select(n => new
            {
                n.ID_NhomHoc,
                MaNhom = n.MaNhomHoc,
                TenMon = n.TenMonHoc,
                Loai = n.LoaiNhom,
                SiSo = n.TongSiSoNhom,
                LopHP = n.DanhSachMaLopHP
            }).ToList();

            if (dgvNhomHoc.Columns["ID_NhomHoc"] != null)
                dgvNhomHoc.Columns["ID_NhomHoc"].Visible = false;
        }

        private void TaiBangTaiGio(int? idHk, int? idNh)
        {
            List<BaoCaoGioGiangDto> bc = _tinhGioService.GetTongHopGioGiang(idHk, idNh);
            dgvTaiGioGV.DataSource = null;
            dgvTaiGioGV.DataSource = bc.Select(x => new
            {
                x.MaGV,
                HoTen = x.HoTen,
                BoMon = x.TenBoMon,
                DinhMucTT = x.DinhMucThucTe,
                GioDungLop = x.GioDungLop,
                TongGioChuan = x.TongGioChuanQuyDoi,
                VuotDinhMuc = x.GioVuotDinhMuc
            }).ToList();
        }

        private void txtTimNhom_TextChanged(object sender, EventArgs e)
        {
            HienThiNhomHoc();
        }

        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        private void dgvNhomHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvNhomHoc.CurrentRow == null || dgvNhomHoc.CurrentRow.Cells["ID_NhomHoc"] == null) return;
            int idNhom = Convert.ToInt32(dgvNhomHoc.CurrentRow.Cells["ID_NhomHoc"].Value);

            List<BuoiHocChiTiet> buois = _dsBuoiAll.Where(b => b.ID_NhomHoc == idNhom)
                .OrderBy(b => b.NgayHoc).ThenBy(b => b.ChuoiTietHoc).ToList();

            dgvBuoiHoc.DataSource = null;
            dgvBuoiHoc.DataSource = buois.Select(b => new
            {
                b.ID_BuoiHoc,
                Ngay = b.NgayHoc,
                Tiet = b.ChuoiTietHoc,
                SoTiet = b.SoTietBuoi,
                Phong = b.MaPhong,
                GiangVien = b.DanhSachGiangVien
            }).ToList();

            if (dgvBuoiHoc.Columns["ID_BuoiHoc"] != null)
                dgvBuoiHoc.Columns["ID_BuoiHoc"].Visible = false;

            if (buois.Count == 0)
            {
                _buoiDangChon = null;
                lblBuoiDangChon.Text = "Nhóm học này chưa có buổi học nào được xếp lịch.";
                _dsPhanCongHienTai.Clear();
                CapNhatLuoiPhanCong();
            }
        }

        private void dgvBuoiHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBuoiHoc.CurrentRow == null || dgvBuoiHoc.CurrentRow.Cells["ID_BuoiHoc"] == null) return;
            int idBuoi = Convert.ToInt32(dgvBuoiHoc.CurrentRow.Cells["ID_BuoiHoc"].Value);
            _buoiDangChon = _dsBuoiAll.FirstOrDefault(b => b.ID_BuoiHoc == idBuoi);
            if (_buoiDangChon == null) return;

            lblBuoiDangChon.Text = "Buổi #" + _buoiDangChon.ID_BuoiHoc + " | " + _buoiDangChon.TenMonHoc + " (" + _buoiDangChon.MaNhomHoc + ") | Ngày " + _buoiDangChon.NgayHoc + " · " + _buoiDangChon.ChuoiTietHoc + " (" + _buoiDangChon.SoTietBuoi + " tiết)";
            nudSoTiet.Value = Math.Max(nudSoTiet.Minimum, Math.Min(nudSoTiet.Maximum, (decimal)_buoiDangChon.SoTietBuoi));

            List<PhanCongGiangDay> pcs = _xepLichService.LayPhanCongTheoBuoi(idBuoi);
            _dsPhanCongHienTai = new List<PhanCongRowItem>();
            foreach (PhanCongGiangDay pc in pcs)
            {
                GiangVien gv = _dsGiangVien.FirstOrDefault(g => g.ID_GV == pc.ID_GV);
                _dsPhanCongHienTai.Add(new PhanCongRowItem
                {
                    ID_GV = pc.ID_GV,
                    MaGV = gv != null ? gv.MaGV : "",
                    HoTen = gv != null ? gv.HoTen : ("GV #" + pc.ID_GV),
                    VaiTro = pc.VaiTro,
                    SoTietGiangDay = pc.SoTietGiangDay,
                    HeSoQuyDoi = pc.HeSoQuyDoi,
                    GioChuanQuyDoi = pc.GioChuanQuyDoi
                });
            }
            CapNhatLuoiPhanCong();
        }

        private void CapNhatLuoiPhanCong()
        {
            dgvPhanCong.DataSource = null;
            dgvPhanCong.DataSource = _dsPhanCongHienTai.Select(x => new
            {
                x.ID_GV,
                x.MaGV,
                GiangVien = x.HoTen,
                x.VaiTro,
                SoTiet = x.SoTietGiangDay,
                HeSo = x.HeSoQuyDoi,
                GioChuan = x.GioChuanQuyDoi
            }).ToList();

            if (dgvPhanCong.Columns["ID_GV"] != null)
                dgvPhanCong.Columns["ID_GV"].Visible = false;
        }

        private void btnThemGv_Click(object sender, EventArgs e)
        {
            if (_buoiDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn một buổi học ở bảng bên trái trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!(cboGiangVien.SelectedValue is int)) return;
            int idGv = (int)cboGiangVien.SelectedValue;
            GiangVien gv = _dsGiangVien.FirstOrDefault(g => g.ID_GV == idGv);
            if (gv == null) return;

            string vaiTro = cboVaiTro.SelectedItem != null ? cboVaiTro.SelectedItem.ToString() : "Chinh";
            double soTiet = (double)nudSoTiet.Value;
            double heSo = _buoiDangChon.HeSoQuyDoi > 0 ? _buoiDangChon.HeSoQuyDoi : 1.0;

            PhanCongRowItem existing = _dsPhanCongHienTai.FirstOrDefault(x => x.ID_GV == idGv);
            if (existing != null)
            {
                existing.VaiTro = vaiTro;
                existing.SoTietGiangDay = soTiet;
                existing.GioChuanQuyDoi = Math.Round(soTiet * heSo, 2);
            }
            else
            {
                _dsPhanCongHienTai.Add(new PhanCongRowItem
                {
                    ID_GV = gv.ID_GV,
                    MaGV = gv.MaGV,
                    HoTen = gv.HoTen,
                    VaiTro = vaiTro,
                    SoTietGiangDay = soTiet,
                    HeSoQuyDoi = heSo,
                    GioChuanQuyDoi = Math.Round(soTiet * heSo, 2)
                });
            }
            CapNhatLuoiPhanCong();
        }

        private void btnXoaGv_Click(object sender, EventArgs e)
        {
            if (dgvPhanCong.CurrentRow == null || dgvPhanCong.CurrentRow.Cells["ID_GV"] == null) return;
            int idGv = Convert.ToInt32(dgvPhanCong.CurrentRow.Cells["ID_GV"].Value);
            _dsPhanCongHienTai.RemoveAll(x => x.ID_GV == idGv);
            CapNhatLuoiPhanCong();
        }

        private void btnChiaDeuTiet_Click(object sender, EventArgs e)
        {
            if (_buoiDangChon == null || _dsPhanCongHienTai.Count == 0) return;
            double soTietMoiGv = Math.Round((double)_buoiDangChon.SoTietBuoi / _dsPhanCongHienTai.Count, 2);
            double heSo = _buoiDangChon.HeSoQuyDoi > 0 ? _buoiDangChon.HeSoQuyDoi : 1.0;

            foreach (PhanCongRowItem item in _dsPhanCongHienTai)
            {
                item.SoTietGiangDay = soTietMoiGv;
                item.GioChuanQuyDoi = Math.Round(soTietMoiGv * heSo, 2);
            }
            CapNhatLuoiPhanCong();
        }

        private void btnLuuPhanCong_Click(object sender, EventArgs e)
        {
            if (_buoiDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn buổi học cần lưu phân công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_dsPhanCongHienTai.Count == 0)
            {
                MessageBox.Show("Danh sách giảng viên phân công không được rỗng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<PhanCongGiangVienInput> inputs = _dsPhanCongHienTai.Select(x => new PhanCongGiangVienInput
            {
                ID_GV = x.ID_GV,
                VaiTro = x.VaiTro,
                SoTietGiangDay = x.SoTietGiangDay
            }).ToList();

            KetQuaThaoTac kq = _xepLichService.LuuPhanCongChoBuoi(_buoiDangChon.ID_BuoiHoc, inputs);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (kq.ThanhCong)
            {
                TaiDuLieu();
            }
        }
    }
}
