using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmMonHoc_BaiHoc : Form
    {
        private readonly ClassLibrary lib = new ClassLibrary();
        private List<MonHoc> _allMons = new List<MonHoc>();
        private string _selectedMaMon = "";

        public frmMonHoc_BaiHoc()
        {
            InitializeComponent();
            this.Load += frmMonHoc_BaiHoc_Load;
        }

        private void frmMonHoc_BaiHoc_Load(object sender, EventArgs e)
        {
            try
            {
                cboMonBoMon.DataSource = lib.LayDanhMucTheoNhom(ClassLibrary.NHOM_BO_MON);
                cboMonBoMon.DisplayMember = "TenDanhMuc";
                cboMonBoMon.ValueMember = "MaDanhMuc";

                List<DanhMuc> heDTs = lib.LayDanhMucTheoNhom(ClassLibrary.NHOM_HE_DAO_TAO);
                heDTs.Insert(0, new DanhMuc { MaDanhMuc = 0, TenDanhMuc = "-- Mặc định --" });
                cboMonHeDT.DataSource = heDTs;
                cboMonHeDT.DisplayMember = "TenDanhMuc";
                cboMonHeDT.ValueMember = "MaDanhMuc";

                cboGVMacDinh.DataSource = lib.LayDanhSachGiangVien();
                cboGVMacDinh.DisplayMember = "HoTen";
                cboGVMacDinh.ValueMember = "ID_GV";
                cboLoaiBai.Items.AddRange(new object[] { "LT", "TH", "TT" });
                cboLoaiBai.SelectedIndex = 0;
            }
            catch { }
            NapMonHoc();
        }

        private void NapMonHoc()
        {
            try
            {
                _allMons = lib.LayDanhSachMonHoc();
                dgvMonHoc.DataSource = _allMons.Select(m => new
                {
                    m.MaMonHoc,
                    m.TenMonHoc,
                    BoMon = m.TenBoMon,
                    TC = m.SoTinChi,
                    LT = m.SoTietLT,
                    TH = m.SoTietTH,
                    NgoaiNgu = m.DayBangNgoaiNgu ? "Có" : "Không"
                }).ToList();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải môn học: " + ex.Message); }
        }

        private void dgvMonHoc_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null || dgvMonHoc.CurrentRow.Cells["MaMonHoc"] == null) return;
            _selectedMaMon = Convert.ToString(dgvMonHoc.CurrentRow.Cells["MaMonHoc"].Value);

            MonHoc mh = _allMons.FirstOrDefault(x => x.MaMonHoc == _selectedMaMon);
            if (mh != null)
            {
                txtMaMon.Text = mh.MaMonHoc;
                txtTenMon.Text = mh.TenMonHoc;
                cboMonBoMon.SelectedValue = mh.ID_BoMon;
                cboMonHeDT.SelectedValue = mh.ID_HeDaoTao ?? 0;
                numSoTinChi.Value = Math.Max(numSoTinChi.Minimum, Math.Min(numSoTinChi.Maximum, (decimal)mh.SoTinChi));
                numTietLT.Value = Math.Max(numTietLT.Minimum, Math.Min(numTietLT.Maximum, mh.SoTietLT));
                numTietTH.Value = Math.Max(numTietTH.Minimum, Math.Min(numTietTH.Maximum, mh.SoTietTH));
                chkMonNgoaiNgu.Checked = mh.DayBangNgoaiNgu;
            }

            NapBaiHoc();
        }

        private void btnMonMoi_Click(object sender, EventArgs e)
        {
            _selectedMaMon = "";
            txtMaMon.Clear();
            txtTenMon.Clear();
            numSoTinChi.Value = 2M;
            numTietLT.Value = 30;
            numTietTH.Value = 0;
            chkMonNgoaiNgu.Checked = false;
            txtMaMon.Focus();
        }

        private void btnMonLuu_Click(object sender, EventArgs e)
        {
            if (!(cboMonBoMon.SelectedValue is int) || (int)cboMonBoMon.SelectedValue <= 0)
            {
                MessageBox.Show("Chưa có Bộ môn. Vui lòng thêm Bộ môn trong 'Danh mục dùng chung' trước.");
                return;
            }
            int? idHe = (cboMonHeDT.SelectedValue is int && (int)cboMonHeDT.SelectedValue > 0)
                ? (int?)((int)cboMonHeDT.SelectedValue) : null;

            KetQuaThaoTac res = lib.LuuMonHoc(new MonHoc
            {
                MaMonHoc = txtMaMon.Text.Trim(),
                TenMonHoc = txtTenMon.Text.Trim(),
                ID_BoMon = (int)cboMonBoMon.SelectedValue,
                ID_HeDaoTao = idHe,
                SoTinChi = (double)numSoTinChi.Value,
                SoTietLT = (int)numTietLT.Value,
                SoTietTH = (int)numTietTH.Value,
                DayBangNgoaiNgu = chkMonNgoaiNgu.Checked
            });
            MessageBox.Show(res.ThongBao, res.ThanhCong ? "Thành công" : "Lỗi");
            if (res.ThanhCong) NapMonHoc();
        }

        private void btnMonXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaMon)) return;
            if (MessageBox.Show("Xóa môn học '" + _selectedMaMon + "'?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            KetQuaThaoTac res = lib.XoaMonHoc(_selectedMaMon);
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) { btnMonMoi_Click(sender, e); NapMonHoc(); }
        }

        private void NapBaiHoc()
        {
            if (string.IsNullOrEmpty(_selectedMaMon)) return;
            try
            {
                List<BaiHoc> bais = lib.LayDanhSachBaiHoc(_selectedMaMon);
                dgvBaiHoc.DataSource = bais.Select(b => new
                {
                    b.ID_BaiHoc,
                    b.ThuTuBai,
                    b.TenBaiHoc,
                    b.LoaiBai,
                    b.SoTiet,
                    GVMacDinh = b.TenGiangVienMacDinh
                }).ToList();
                numThuTu.Value = Math.Min(numThuTu.Maximum, bais.Count + 1);
            }
            catch { }
        }

        private void btnThemBai_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaMon)) { MessageBox.Show("Vui lòng chọn môn học trước."); return; }
            KetQuaThaoTac res = lib.LuuBaiHoc(new BaiHoc
            {
                MaMonHoc = _selectedMaMon,
                ThuTuBai = (int)numThuTu.Value,
                TenBaiHoc = txtTenBai.Text.Trim(),
                LoaiBai = cboLoaiBai.Text,
                SoTiet = (int)numSoTiet.Value,
                ID_GV_MacDinh = cboGVMacDinh.SelectedValue is int ? (int?)cboGVMacDinh.SelectedValue : null
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) { txtTenBai.Text = ""; NapBaiHoc(); }
        }

        private void btnXoaBai_Click(object sender, EventArgs e)
        {
            if (dgvBaiHoc.CurrentRow == null) return;
            int idBai = Convert.ToInt32(dgvBaiHoc.CurrentRow.Cells["ID_BaiHoc"].Value);
            if (MessageBox.Show("Xóa bài học này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                KetQuaThaoTac res = lib.XoaBaiHoc(idBai);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapBaiHoc();
            }
        }
    }
}
