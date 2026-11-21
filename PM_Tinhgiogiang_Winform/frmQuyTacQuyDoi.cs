using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmQuyTacQuyDoi : Form
    {
        private readonly DanhMucService _dmService;
        private List<QuyTacQuyDoi> _allRules = new List<QuyTacQuyDoi>();
        private int _selectedId = 0;

        public frmQuyTacQuyDoi()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            this.Load += frmQuyTacQuyDoi_Load;
        }

        private void frmQuyTacQuyDoi_Load(object sender, EventArgs e)
        {
            cboLoaiHinh.Items.AddRange(new object[] { "LT", "TH_Lab", "TT_LamSang", "TT_BenhVien_Khac", "TT_XiNghiep" });
            cboLoaiHinh.SelectedIndex = 0;
            cboSimLoai.Items.AddRange(new object[] { "LT", "TH_Lab", "TT_LamSang" });
            cboSimLoai.SelectedIndex = 0;
            NapDanhSach();
            TinhMoPhong(null, EventArgs.Empty);
        }

        private void NapDanhSach()
        {
            try
            {
                _allRules = _dmService.LayDanhSachQuyTacQuyDoi();
                dgvQuyTac.DataSource = _allRules.Select(q => new
                {
                    q.ID_QuyTac,
                    q.TenQuyTac,
                    q.LoaiHinhGiangDay,
                    q.SiSoTu,
                    q.SiSoDen,
                    q.HeSoQuyDoi,
                    q.NgayHieuLucTu
                }).ToList();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải quy tắc quy đổi: " + ex.Message); }
        }

        private void dgvQuyTac_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQuyTac.CurrentRow == null) return;
            _selectedId = Convert.ToInt32(dgvQuyTac.CurrentRow.Cells["ID_QuyTac"].Value);
            QuyTacQuyDoi qt = _allRules.FirstOrDefault(x => x.ID_QuyTac == _selectedId);
            if (qt == null) return;

            txtTenQuyTac.Text = qt.TenQuyTac;
            cboLoaiHinh.Text = qt.LoaiHinhGiangDay;
            numSiSoTu.Value = qt.SiSoTu;
            numSiSoDen.Value = qt.SiSoDen;
            numHeSo.Value = (decimal)qt.HeSoQuyDoi;
            txtHieuLucTu.Text = qt.NgayHieuLucTu;
        }

        private void TinhMoPhong(object sender, EventArgs e)
        {
            string loai = cboSimLoai.Text;
            int ss = (int)numSimSiSo.Value;
            double hs = 1.0;
            if (loai == "LT") hs = ss < 40 ? 1.0 : (ss <= 100 ? 1.3 : 1.5);
            else if (loai == "TH_Lab") hs = ss < 15 ? 0.7 : (ss <= 30 ? 0.85 : 1.0);
            else if (loai == "TT_LamSang") hs = ss < 10 ? 0.6 : 0.8;
            if (chkSimNN.Checked) hs *= 1.5;

            lblSimKetQua.Text = string.Format("Kết quả mô phỏng Điều 12: Hệ số = {0:0.00} → 3 tiết = {1:0.00} giờ chuẩn", hs, 3 * hs);
        }

        private void btnMoi_Click(object sender, EventArgs e)
        {
            _selectedId = 0; txtTenQuyTac.Text = ""; numSiSoTu.Value = 0; numSiSoDen.Value = 100; numHeSo.Value = 1.0M; txtHieuLucTu.Text = "2025-01-01";
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            KetQuaThaoTac res = _dmService.LuuQuyTacQuyDoi(new QuyTacQuyDoi
            {
                ID_QuyTac = _selectedId,
                TenQuyTac = txtTenQuyTac.Text.Trim(),
                LoaiHinhGiangDay = cboLoaiHinh.Text,
                SiSoTu = (int)numSiSoTu.Value,
                SiSoDen = (int)numSiSoDen.Value,
                HeSoQuyDoi = (double)numHeSo.Value,
                NgayHieuLucTu = txtHieuLucTu.Text.Trim()
            });
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapDanhSach();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedId <= 0) return;
            if (MessageBox.Show("Xóa quy tắc này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                KetQuaThaoTac res = _dmService.XoaQuyTacQuyDoi(_selectedId);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapDanhSach();
            }
        }
    }
}
