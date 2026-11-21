using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmLopHocPhan_NhomHoc : Form
    {
        private readonly DanhMucService _dmService;
        private List<LopHocPhan> _allLops = new List<LopHocPhan>();

        public frmLopHocPhan_NhomHoc()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            this.Load += frmLopHocPhan_NhomHoc_Load;
        }

        private void frmLopHocPhan_NhomHoc_Load(object sender, EventArgs e)
        {
            cboLoaiNhom.Items.AddRange(new object[] { "LT", "TH", "TT" });
            cboLoaiNhom.SelectedIndex = 0;
            NapDuLieu();
        }

        private void NapDuLieu()
        {
            try
            {
                int? idHK = SessionHelper.CurrentHocKyId;
                _allLops = _dmService.LayDanhSachLopHocPhan(idHK);

                dgvLopHP.DataSource = _allLops.Select(l => new
                {
                    l.ID_LopHocPhan,
                    l.MaLopHocPhan,
                    l.TenMonHoc,
                    l.LoaiLopHP,
                    l.SiSoDangKy,
                    l.TrangThai
                }).ToList();

                clbLopGop.Items.Clear();
                foreach (LopHocPhan l in _allLops)
                    clbLopGop.Items.Add(l.ID_LopHocPhan + " | " + l.MaLopHocPhan + " (" + l.SiSoDangKy + " SV) - " + l.TenMonHoc);

                List<NhomHoc> nhoms = _dmService.LayDanhSachNhomHoc(idHK);
                dgvNhomHoc.DataSource = nhoms.Select(n => new
                {
                    n.ID_NhomHoc,
                    n.MaNhomHoc,
                    n.TenNhomHoc,
                    n.LoaiNhom,
                    n.DanhSachMaLopHP,
                    n.TongSiSoNhom
                }).ToList();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải Lớp HP / Nhóm học: " + ex.Message); }
        }

        private void btnGopTaoNhom_Click(object sender, EventArgs e)
        {
            List<int> dsId = new List<int>();
            foreach (object item in clbLopGop.CheckedItems)
            {
                string s = item.ToString();
                int id;
                if (int.TryParse(s.Split('|')[0].Trim(), out id)) dsId.Add(id);
            }
            if (dsId.Count == 0) { MessageBox.Show("Vui lòng tick chọn ít nhất 1 Lớp học phần để gộp."); return; }

            LopHocPhan firstLop = _allLops.FirstOrDefault(x => x.ID_LopHocPhan == dsId[0]);
            if (firstLop == null) return;

            KetQuaThaoTac res = _dmService.TaoNhomHoc(new NhomHoc
            {
                MaNhomHoc = txtMaNhom.Text.Trim(),
                TenNhomHoc = txtTenNhom.Text.Trim(),
                MaMonHoc = firstLop.MaMonHoc,
                ID_HocKy = firstLop.ID_HocKy,
                LoaiNhom = cboLoaiNhom.Text
            }, dsId);
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) { txtMaNhom.Text = ""; txtTenNhom.Text = ""; NapDuLieu(); }
        }

        private void btnTachToTH_Click(object sender, EventArgs e)
        {
            if (dgvLopHP.CurrentRow == null) { MessageBox.Show("Chọn 1 Lớp học phần ở bảng bên trên để tách tổ TH."); return; }
            int idLop = Convert.ToInt32(dgvLopHP.CurrentRow.Cells["ID_LopHocPhan"].Value);
            KetQuaThaoTac res = _dmService.TachToThucHanh(idLop, (int)numSiSoToTH.Value);
            MessageBox.Show(res.ThongBao);
            if (res.ThanhCong) NapDuLieu();
        }

        private void btnXoaNhom_Click(object sender, EventArgs e)
        {
            if (dgvNhomHoc.CurrentRow == null) return;
            int idNhom = Convert.ToInt32(dgvNhomHoc.CurrentRow.Cells["ID_NhomHoc"].Value);
            if (MessageBox.Show("Xóa nhóm học đang chọn?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                KetQuaThaoTac res = _dmService.XoaNhomHoc(idNhom);
                MessageBox.Show(res.ThongBao);
                if (res.ThanhCong) NapDuLieu();
            }
        }
    }
}
