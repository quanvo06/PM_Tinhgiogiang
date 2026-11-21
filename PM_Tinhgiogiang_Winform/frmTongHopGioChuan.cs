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
    public partial class frmTongHopGioChuan : Form
    {
        private readonly TinhGioService _tinhGioService;
        private List<BaoCaoGioGiangDto> _dsBaoCao = new List<BaoCaoGioGiangDto>();
        private List<BaoCaoGioGiangDto> _dsHienThi = new List<BaoCaoGioGiangDto>();

        public frmTongHopGioChuan()
        {
            InitializeComponent();
            _tinhGioService = new TinhGioService();
            this.Load += frmTongHopGioChuan_Load;
        }

        private void frmTongHopGioChuan_Load(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        private void LayPhamVi(out int? idHk, out int? idNh, out string tenPhamVi)
        {
            idHk = null;
            idNh = null;
            if (cboPhamVi.SelectedIndex == 0)
            {
                idHk = SessionHelper.CurrentHocKyId;
                idNh = SessionHelper.CurrentNamHocId;
                tenPhamVi = "Học kỳ đang chọn";
            }
            else if (cboPhamVi.SelectedIndex == 1)
            {
                idNh = SessionHelper.CurrentNamHocId;
                tenPhamVi = "Cả năm học đang chọn";
            }
            else
            {
                tenPhamVi = "Toàn bộ dữ liệu";
            }
        }

        private void TaiDuLieu()
        {
            try
            {
                int? idHk, idNh;
                string tenPhamVi;
                LayPhamVi(out idHk, out idNh, out tenPhamVi);
                _dsBaoCao = _tinhGioService.GetTongHopGioGiang(idHk, idNh);
                HienThiBangTongHop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính tổng hợp giờ chuẩn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiBangTongHop()
        {
            string kw = (txtTimGV.Text ?? "").Trim().ToLowerInvariant();
            _dsHienThi = _dsBaoCao.Where(x =>
                string.IsNullOrEmpty(kw) ||
                (x.MaGV != null && x.MaGV.ToLowerInvariant().Contains(kw)) ||
                (x.HoTen != null && x.HoTen.ToLowerInvariant().Contains(kw)) ||
                (x.TenBoMon != null && x.TenBoMon.ToLowerInvariant().Contains(kw))).ToList();

            dgvTongHop.DataSource = null;
            dgvTongHop.DataSource = _dsHienThi.Select(x => new
            {
                x.MaGV,
                HoTen = x.HoTen,
                BoMon = x.TenBoMon,
                DinhMucGoc = x.DinhMucGioChuan,
                MienGiam = x.GioMienGiam,
                DinhMucTT = x.DinhMucThucTe,
                TietDungLop = x.TongSoTietThucTe,
                GioDungLop = x.GioDungLop,
                DeTaiHD = x.GioDeTaiHoiDong,
                KhaoThi = x.GioKhaoThi,
                NCKH = x.GioNCKH,
                TongGioChuan = x.TongGioChuanQuyDoi,
                ChenhLech = Math.Round(x.TongGioChuanQuyDoi - x.DinhMucThucTe, 2)
            }).ToList();

            DatTieuDeCotTongHop();

            double tongGio = Math.Round(_dsHienThi.Sum(x => x.TongGioChuanQuyDoi), 2);
            double tongVuot = Math.Round(_dsHienThi.Sum(x => x.GioVuotDinhMuc), 2);
            lblTongKet.Text = "GV: " + _dsHienThi.Count + "  |  Tổng giờ chuẩn: " + tongGio.ToString("0.##") + "  |  Tổng vượt ĐM: " + tongVuot.ToString("0.##");
        }

        private void DatTieuDeCotTongHop()
        {
            if (dgvTongHop.Columns["MaGV"] != null) dgvTongHop.Columns["MaGV"].HeaderText = "Mã GV";
            if (dgvTongHop.Columns["HoTen"] != null) dgvTongHop.Columns["HoTen"].HeaderText = "Họ và tên";
            if (dgvTongHop.Columns["BoMon"] != null) dgvTongHop.Columns["BoMon"].HeaderText = "Bộ môn";
            if (dgvTongHop.Columns["DinhMucGoc"] != null) dgvTongHop.Columns["DinhMucGoc"].HeaderText = "ĐM gốc";
            if (dgvTongHop.Columns["MienGiam"] != null) dgvTongHop.Columns["MienGiam"].HeaderText = "Miễn giảm";
            if (dgvTongHop.Columns["DinhMucTT"] != null) dgvTongHop.Columns["DinhMucTT"].HeaderText = "ĐM thực tế";
            if (dgvTongHop.Columns["TietDungLop"] != null) dgvTongHop.Columns["TietDungLop"].HeaderText = "Tiết ĐL";
            if (dgvTongHop.Columns["GioDungLop"] != null) dgvTongHop.Columns["GioDungLop"].HeaderText = "Giờ ĐL";
            if (dgvTongHop.Columns["DeTaiHD"] != null) dgvTongHop.Columns["DeTaiHD"].HeaderText = "Đề tài/HĐ";
            if (dgvTongHop.Columns["KhaoThi"] != null) dgvTongHop.Columns["KhaoThi"].HeaderText = "Khảo thí";
            if (dgvTongHop.Columns["NCKH"] != null) dgvTongHop.Columns["NCKH"].HeaderText = "NCKH";
            if (dgvTongHop.Columns["TongGioChuan"] != null) dgvTongHop.Columns["TongGioChuan"].HeaderText = "Tổng giờ chuẩn";
            if (dgvTongHop.Columns["ChenhLech"] != null) dgvTongHop.Columns["ChenhLech"].HeaderText = "Vượt / Thiếu";
        }

        private void dgvTongHop_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvTongHop.Columns[e.ColumnIndex].Name == "ChenhLech" && e.Value != null)
            {
                double val;
                if (double.TryParse(e.Value.ToString(), out val))
                {
                    e.CellStyle.Font = WinFormStyleHelper.BoldFont;
                    e.CellStyle.ForeColor = val >= 0 ? Color.FromArgb(5, 150, 105) : Color.FromArgb(185, 28, 28);
                }
            }
        }

        private void dgvTongHop_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTongHop.CurrentRow == null || dgvTongHop.CurrentRow.Cells["MaGV"] == null) return;
            string maGv = Convert.ToString(dgvTongHop.CurrentRow.Cells["MaGV"].Value);
            string hoTen = dgvTongHop.CurrentRow.Cells["HoTen"] != null ? Convert.ToString(dgvTongHop.CurrentRow.Cells["HoTen"].Value) : maGv;

            lblChiTietCaption.Text = "  Chi tiết các buổi đứng lớp của giảng viên: " + hoTen + " (" + maGv + ")";

            try
            {
                int? idHk, idNh;
                string tenPhamVi;
                LayPhamVi(out idHk, out idNh, out tenPhamVi);
                List<ChiTietBuoiDungLopDto> ct = _tinhGioService.GetChiTietBuoiDungLopTheoMaGV(maGv, idHk, idNh);

                dgvChiTietBuoi.DataSource = null;
                dgvChiTietBuoi.DataSource = ct.Select(x => new
                {
                    x.ID_BuoiHoc,
                    Ngay = x.NgayHoc,
                    Tiet = x.ChuoiTiet,
                    MonHoc = x.TenMonHoc,
                    Nhom = x.MaNhomHoc,
                    Phong = x.MaPhong,
                    Loai = x.LoaiHinhGiangDay,
                    NgoaiNgu = x.DayBangNgoaiNgu ? "Có (x1.5)" : "Không",
                    SiSo = x.TongSiSoBuoi,
                    SoTietGV = x.SoTietGiangDay,
                    HeSo = x.HeSoQuyDoi,
                    GioChuan = x.GioChuanQuyDoi
                }).ToList();

                if (dgvChiTietBuoi.Columns["ID_BuoiHoc"] != null)
                    dgvChiTietBuoi.Columns["ID_BuoiHoc"].Visible = false;
            }
            catch (Exception ex)
            {
                lblChiTietCaption.Text = "  Lỗi tải chi tiết buổi dạy: " + ex.Message;
            }
        }

        private void cboPhamVi_SelectedIndexChanged(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        private void txtTimGV_TextChanged(object sender, EventArgs e)
        {
            HienThiBangTongHop();
        }

        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            int? idHk, idNh;
            string tenPhamVi;
            LayPhamVi(out idHk, out idNh, out tenPhamVi);
            string fileName = "TongHop_GioChuan_UMP_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
            ExcelHelper.ExportBaoCaoGioGiangToExcel(_dsHienThi, tenPhamVi, fileName);
        }
    }
}
