using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmQuanLyDanhMuc : Form
    {
        private readonly ClassLibrary lib = new ClassLibrary();
        private List<DanhMuc> _dsHienTai = new List<DanhMuc>();
        private int _selectedMaDanhMuc = 0;

        public frmQuanLyDanhMuc()
        {
            InitializeComponent();
            this.Load += frmQuanLyDanhMuc_Load;
        }

        private void frmQuanLyDanhMuc_Load(object sender, EventArgs e)
        {
            NapNhomDanhMuc();
        }

        private void NapNhomDanhMuc()
        {
            try
            {
                List<NhomDanhMuc> nhoms = lib.LayDanhSachNhomDanhMuc();
                cboNhomDanhMuc.DataSource = nhoms;
                cboNhomDanhMuc.DisplayMember = "TenNhom";
                cboNhomDanhMuc.ValueMember = "MaNhom";

                NhomDanhMuc bm = nhoms.FirstOrDefault(x => x.MaNhom == ClassLibrary.NHOM_BO_MON);
                if (bm != null) cboNhomDanhMuc.SelectedValue = bm.MaNhom;

                NapDanhMucTheoNhom();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhóm danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NapDanhMucTheoNhom()
        {
            if (!(cboNhomDanhMuc.SelectedValue is int)) return;
            int maNhom = (int)cboNhomDanhMuc.SelectedValue;

            try
            {
                _dsHienTai = lib.LayDanhMucTheoNhom(maNhom, false);
                dgvDanhMuc.DataSource = null;
                dgvDanhMuc.DataSource = _dsHienTai.Select(d => new
                {
                    d.MaDanhMuc,
                    Ten = d.TenDanhMuc,
                    d.VietTat,
                    ThamSo = d.GiaTriThamSo,
                    d.ThuTu,
                    TrangThai = d.KichHoat ? "Kích hoạt" : "Đã ẩn"
                }).ToList();

                if (dgvDanhMuc.Columns["MaDanhMuc"] != null)
                    dgvDanhMuc.Columns["MaDanhMuc"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboNhomDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedMaDanhMuc = 0;
            NapDanhMucTheoNhom();
        }

        private void dgvDanhMuc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhMuc.CurrentRow == null || dgvDanhMuc.CurrentRow.Cells["MaDanhMuc"] == null) return;
            int id = Convert.ToInt32(dgvDanhMuc.CurrentRow.Cells["MaDanhMuc"].Value);
            DanhMuc dm = _dsHienTai.FirstOrDefault(x => x.MaDanhMuc == id);
            if (dm == null) return;

            _selectedMaDanhMuc = dm.MaDanhMuc;
            txtTenDanhMuc.Text = dm.TenDanhMuc;
            txtVietTat.Text = dm.VietTat ?? "";
            txtGiaTriThamSo.Text = dm.GiaTriThamSo.HasValue ? dm.GiaTriThamSo.Value.ToString("0.##", CultureInfo.InvariantCulture) : "";
            numThuTu.Value = Math.Max(numThuTu.Minimum, Math.Min(numThuTu.Maximum, dm.ThuTu));
            chkKichHoat.Checked = dm.KichHoat;
        }

        private void btnMoi_Click(object sender, EventArgs e)
        {
            _selectedMaDanhMuc = 0;
            txtTenDanhMuc.Clear();
            txtVietTat.Clear();
            txtGiaTriThamSo.Clear();
            numThuTu.Value = Math.Min(numThuTu.Maximum, _dsHienTai.Count + 1);
            chkKichHoat.Checked = true;
            txtTenDanhMuc.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!(cboNhomDanhMuc.SelectedValue is int)) return;
            int maNhom = (int)cboNhomDanhMuc.SelectedValue;

            double? thamSo = null;
            double parsed;
            if (!string.IsNullOrWhiteSpace(txtGiaTriThamSo.Text) &&
                double.TryParse(txtGiaTriThamSo.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
            {
                thamSo = parsed;
            }

            KetQuaThaoTac res = lib.LuuDanhMuc(new DanhMuc
            {
                MaDanhMuc = _selectedMaDanhMuc,
                MaNhom = maNhom,
                TenDanhMuc = txtTenDanhMuc.Text.Trim(),
                VietTat = txtVietTat.Text.Trim(),
                GiaTriThamSo = thamSo,
                ThuTu = (int)numThuTu.Value,
                KichHoat = chkKichHoat.Checked
            });

            MessageBox.Show(res.ThongBao, res.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                res.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.ThanhCong)
            {
                NapDanhMucTheoNhom();
                btnMoi_Click(sender, e);
            }
        }

        private void btnAn_Click(object sender, EventArgs e)
        {
            if (_selectedMaDanhMuc <= 0) return;
            KetQuaThaoTac res = lib.XoaHoacAnDanhMuc(_selectedMaDanhMuc);
            MessageBox.Show(res.ThongBao, res.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                res.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (res.ThanhCong) NapDanhMucTheoNhom();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            NapDanhMucTheoNhom();
        }
    }
}
