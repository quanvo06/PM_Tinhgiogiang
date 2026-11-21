using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmTongQuan : Form
    {
        private readonly DanhMucService _dmService;
        private readonly XepLichService _xlService;
        private readonly TinhGioService _tgService;

        public frmTongQuan()
        {
            InitializeComponent();
            _dmService = new DanhMucService();
            _xlService = new XepLichService();
            _tgService = new TinhGioService();
            this.Load += frmTongQuan_Load;
        }

        private void frmTongQuan_Load(object sender, EventArgs e)
        {
            NapDuLieuTongQuan();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            NapDuLieuTongQuan();
        }

        private void NapDuLieuTongQuan()
        {
            try
            {
                int? idHK = SessionHelper.CurrentHocKyId;
                int? idNH = SessionHelper.CurrentNamHocId;

                List<NhomHoc> nhoms = _dmService.LayDanhSachNhomHoc(idHK);
                List<BuoiHocChiTiet> buois = _xlService.GetDanhSachBuoiHoc(idHK);
                List<BaoCaoGioGiangDto> baoCao = _tgService.GetTongHopGioGiang(idHK, idNH);

                int soNhomDaXep = nhoms.Count(n => buois.Any(b => b.ID_NhomHoc == n.ID_NhomHoc));
                int pctNhom = nhoms.Count > 0 ? (int)Math.Round(soNhomDaXep * 100.0 / nhoms.Count) : 0;
                int buoiThieuPhong = buois.Count(b => !b.ID_PhongHoc.HasValue);
                int gvThieu = baoCao.Count(g => g.TongGioChuanQuyDoi < g.DinhMucThucTe);
                int gvVuot = baoCao.Count(g => g.GioVuotDinhMuc > 0);

                lblKpiBuoiVal.Text = buois.Count + " buổi";
                lblKpiXungDotVal.Text = buoiThieuPhong.ToString();
                lblKpiGvVal.Text = gvThieu + " thiếu · " + gvVuot + " vượt";
                lblKpiNhomVal.Text = pctNhom + "% (" + soNhomDaXep + "/" + nhoms.Count + ")";

                // Việc cần làm
                var viecs = new List<object>();
                List<NhomHoc> nhomChuaXep = nhoms.Where(n => !buois.Any(b => b.ID_NhomHoc == n.ID_NhomHoc)).ToList();
                if (nhomChuaXep.Count > 0)
                    viecs.Add(new { MucDo = "Chờ xếp", NoiDung = nhomChuaXep.Count + " nhóm học chưa có buổi (" + string.Join(", ", nhomChuaXep.Take(3).Select(n => n.MaNhomHoc)) + ")" });
                if (buoiThieuPhong > 0)
                    viecs.Add(new { MucDo = "Cảnh báo", NoiDung = buoiThieuPhong + " buổi học chưa được gán phòng" });
                int gv0 = baoCao.Count(g => g.TongGioChuanQuyDoi == 0);
                if (gv0 > 0)
                    viecs.Add(new { MucDo = "Theo dõi", NoiDung = gv0 + " giảng viên chưa có giờ chuẩn quy đổi" });
                if (viecs.Count == 0)
                    viecs.Add(new { MucDo = "Hoàn tất", NoiDung = "Không có đầu việc tồn đọng trong học kỳ đang chọn." });
                dgvViecCanLam.DataSource = viecs;

                // Tiến độ đợt / Nhóm học
                var tienDos = nhoms.Select(n => new
                {
                    MaNhom = n.MaNhomHoc,
                    TenNhom = n.TenNhomHoc,
                    SiSo = n.TongSiSoNhom,
                    BuoiDaXep = buois.Count(b => b.ID_NhomHoc == n.ID_NhomHoc)
                }).ToList();
                dgvTienDoDot.DataSource = tienDos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu Tổng quan:\n" + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
