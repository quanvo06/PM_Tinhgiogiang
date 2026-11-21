using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmKiemTraXuatBan : Form
    {
        private readonly XepLichService _xepLichService;
        private List<XungDotLichItem> _dsXungDot = new List<XungDotLichItem>();

        public frmKiemTraXuatBan()
        {
            InitializeComponent();
            _xepLichService = new XepLichService();
            this.Load += frmKiemTraXuatBan_Load;
        }

        private void frmKiemTraXuatBan_Load(object sender, EventArgs e)
        {
            QuetXungDot();
        }

        private void QuetXungDot()
        {
            try
            {
                _dsXungDot = _xepLichService.QuetToanBoXungDot(SessionHelper.CurrentHocKyId);
                int soChan = _dsXungDot.Count(x => x.MucDo == "Chan");
                int soCanhBao = _dsXungDot.Count(x => x.MucDo != "Chan");

                lblKpiChan.Text = soChan.ToString();
                lblKpiCanhBao.Text = soCanhBao.ToString();
                if (soChan > 0)
                {
                    lblKpiTrangThai.Text = "Bị khóa (" + soChan + " lỗi Chặn)";
                    lblKpiTrangThai.ForeColor = Color.FromArgb(220, 38, 38);
                    btnXuatBan.Enabled = false;
                }
                else
                {
                    lblKpiTrangThai.Text = "Sẵn sàng xuất bản";
                    lblKpiTrangThai.ForeColor = WinFormStyleHelper.AccentColor;
                    btnXuatBan.Enabled = true;
                }

                HienThiLuoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi quét xung đột: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiLuoi()
        {
            IEnumerable<XungDotLichItem> q = _dsXungDot;
            if (cboLocMucDo.SelectedIndex == 1) q = q.Where(x => x.MucDo == "Chan");
            else if (cboLocMucDo.SelectedIndex == 2) q = q.Where(x => x.MucDo != "Chan");

            dgvXungDot.DataSource = null;
            dgvXungDot.DataSource = q.Select(x => new
            {
                x.ID_BuoiHoc,
                MucDo = x.MucDo == "Chan" ? "CHẶN" : "Cảnh báo",
                LoaiXungDot = x.LoaiXungDot,
                NgayTiet = x.NgayTiet,
                DoiTuong1 = x.DoiTuong1,
                DoiTuong2 = x.DoiTuong2,
                MoTa = x.MoTa
            }).ToList();

            if (dgvXungDot.Columns["ID_BuoiHoc"] != null)
                dgvXungDot.Columns["ID_BuoiHoc"].HeaderText = "Mã buổi";
        }

        private void dgvXungDot_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvXungDot.Columns[e.ColumnIndex].Name == "MucDo" && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val == "CHẶN")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                    e.CellStyle.Font = WinFormStyleHelper.BoldFont;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(180, 83, 9);
                    e.CellStyle.Font = WinFormStyleHelper.BoldFont;
                }
            }
        }

        private void cboLocMucDo_SelectedIndexChanged(object sender, EventArgs e)
        {
            HienThiLuoi();
        }

        private void btnQuetLai_Click(object sender, EventArgs e)
        {
            QuetXungDot();
        }

        private void btnDiToiBuoi_Click(object sender, EventArgs e)
        {
            if (dgvXungDot.CurrentRow == null || dgvXungDot.CurrentRow.Cells["ID_BuoiHoc"] == null || dgvXungDot.CurrentRow.Cells["ID_BuoiHoc"].Value == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng xung đột để đi tới buổi học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idBuoi = Convert.ToInt32(dgvXungDot.CurrentRow.Cells["ID_BuoiHoc"].Value);
            frmMain main = this.FindForm() as frmMain;
            if (main != null)
            {
                main.MoXepLichTheoBuoi(idBuoi);
            }
            else
            {
                using (frmXepLich f = new frmXepLich(idBuoi))
                {
                    f.WindowState = FormWindowState.Maximized;
                    f.ShowDialog();
                }
                QuetXungDot();
            }
        }

        private void btnXuatBan_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xuất bản toàn bộ lịch giảng dạy Draft sang trạng thái Published?",
                "Xác nhận xuất bản", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            KetQuaThaoTac kq = _xepLichService.XuatBanLich(SessionHelper.CurrentHocKyId);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Không thể xuất bản",
                MessageBoxButtons.OK, kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            QuetXungDot();
        }
    }
}
