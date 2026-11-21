using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmMonHoc_BaiHoc : Form
    {
        private readonly DanhMucService _dmService;
        private List<MonHoc> _allMons = new List<MonHoc>();
        private string _selectedMaMon = "";

        public frmMonHoc_BaiHoc()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            this.Load += frmMonHoc_BaiHoc_Load;
        }

        private void frmMonHoc_BaiHoc_Load(object sender, EventArgs e)
        {
            try
            {
                cboGVMacDinh.DataSource = _dmService.LayDanhSachGiangVien();
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
                _allMons = _dmService.LayDanhSachMonHoc();
                dgvMonHoc.DataSource = _allMons.Select(m => new
                {
                    m.MaMonHoc,
                    m.TenMonHoc,
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
            if (dgvMonHoc.CurrentRow == null) return;
            _selectedMaMon = Convert.ToString(dgvMonHoc.CurrentRow.Cells["MaMonHoc"].Value);
            NapBaiHoc();
        }

        private void NapBaiHoc()
        {
            if (string.IsNullOrEmpty(_selectedMaMon)) return;
            try
            {
                List<BaiHoc> bais = _dmService.LayDanhSachBaiHoc(_selectedMaMon);
                dgvBaiHoc.DataSource = bais.Select(b => new
                {
                    b.ID_BaiHoc,
                    b.ThuTuBai,
                    b.TenBaiHoc,
                    b.LoaiBai,
                    b.SoTiet,
                    GVMacDinh = b.TenGiangVienMacDinh
                }).ToList();
                numThuTu.Value = bais.Count + 1;
            }
            catch { }
        }

        private void btnThemBai_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaMon)) { MessageBox.Show("Vui lòng chọn môn học trước."); return; }
            KetQuaThaoTac res = _dmService.LuuBaiHoc(new BaiHoc
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
                KetQuaThaoTac res = _dmService.XoaBaiHoc(idBai);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapBaiHoc();
            }
        }
    }
}
