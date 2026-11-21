using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Helpers;
using PM_Tinhgiogiang_Lib.Models;
using PM_Tinhgiogiang_Lib.Services;

namespace PM_Tinhgiogiang_Winform
{
    public partial class frmXepLich : Form
    {
        private const int RowHeightPx = 74;
        private const int CaDividerPx = 16;

        private readonly DanhMucService _danhMucService;
        private readonly XepLichService _xepLichService;
        private readonly int? _focusBuoiHocId;

        private List<BuoiHocChiTiet> _dsBuoiAll = new List<BuoiHocChiTiet>();
        private List<TietHoc> _dsTiet = new List<TietHoc>();
        private List<NhomHoc> _dsNhom = new List<NhomHoc>();
        private List<GiangVien> _dsGV = new List<GiangVien>();
        private List<PhongHoc> _dsPhong = new List<PhongHoc>();

        private DateTime _ngayDauTuan;
        private BuoiHocChiTiet _buoiDangChon;
        private readonly Panel[] _dayPanels = new Panel[7];
        private readonly Label[] _dayHeaderLabels = new Label[7];
        private bool _dangKhoiTao = true;

        public frmXepLich(int? focusBuoiHocId = null)
        {
            InitializeComponent();
            _danhMucService = new DanhMucService();
            _xepLichService = new XepLichService();
            _focusBuoiHocId = focusBuoiHocId;
            _ngayDauTuan = LayThuHaiCuaTuan(DateTime.Today);
            this.Load += frmXepLich_Load;
        }

        private static DateTime LayThuHaiCuaTuan(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private void frmXepLich_Load(object sender, EventArgs e)
        {
            KhoiTaoKhungLuoiTkb();
            TaiDanhMucVaDuLieu();

            if (_focusBuoiHocId.HasValue)
            {
                BuoiHocChiTiet target = _dsBuoiAll.FirstOrDefault(b => b.ID_BuoiHoc == _focusBuoiHocId.Value);
                if (target != null)
                {
                    DateTime d;
                    if (DateTime.TryParseExact(target.NgayHoc, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                    {
                        _ngayDauTuan = LayThuHaiCuaTuan(d);
                        dtpNgayTrongTuan.Value = d;
                    }
                    ChonBuoiHoc(target);
                }
            }
            else if (_dsBuoiAll.Count > 0)
            {
                // Tự nhảy đến tuần có buổi học gần nhất nếu tuần hiện tại trống
                bool coTrongTuan = _dsBuoiAll.Any(b => ThuocTuan(b.NgayHoc, _ngayDauTuan));
                if (!coTrongTuan)
                {
                    DateTime d;
                    if (DateTime.TryParseExact(_dsBuoiAll[0].NgayHoc, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                    {
                        _ngayDauTuan = LayThuHaiCuaTuan(d);
                        dtpNgayTrongTuan.Value = d;
                    }
                }
            }

            _dangKhoiTao = false;
            VeLuoiTkbTuan();
        }

        private void KhoiTaoKhungLuoiTkb()
        {
            tlpHeaderTkb.Controls.Clear();
            Label lblCaHeader = new Label();
            lblCaHeader.Text = "Ca / Tiết";
            lblCaHeader.Dock = DockStyle.Fill;
            lblCaHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblCaHeader.Font = WinFormStyleHelper.BoldFont;
            lblCaHeader.ForeColor = WinFormStyleHelper.AccentColor;
            tlpHeaderTkb.Controls.Add(lblCaHeader, 0, 0);

            for (int d = 0; d < 7; d++)
            {
                Label lbl = new Label();
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = WinFormStyleHelper.BoldFont;
                lbl.ForeColor = Color.FromArgb(15, 23, 42);
                _dayHeaderLabels[d] = lbl;
                tlpHeaderTkb.Controls.Add(lbl, d + 1, 0);
            }

            tlpGridTkb.Controls.Clear();
            Panel pnlPeriodAxis = new Panel();
            pnlPeriodAxis.Dock = DockStyle.Fill;
            pnlPeriodAxis.BackColor = Color.FromArgb(248, 250, 252);
            pnlPeriodAxis.Paint += PnlPeriodAxis_Paint;
            tlpGridTkb.Controls.Add(pnlPeriodAxis, 0, 0);

            for (int d = 0; d < 7; d++)
            {
                Panel p = new Panel();
                p.Dock = DockStyle.Fill;
                p.BackColor = Color.White;
                p.Margin = new Padding(0);
                p.Tag = d;
                p.Paint += DayPanel_Paint;
                p.SizeChanged += DayPanel_SizeChanged;
                _dayPanels[d] = p;
                tlpGridTkb.Controls.Add(p, d + 1, 0);
            }
        }

        private void PnlPeriodAxis_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240)))
            using (SolidBrush caSangBrush = new SolidBrush(Color.FromArgb(254, 249, 195)))
            using (SolidBrush caChieuBrush = new SolidBrush(Color.FromArgb(224, 242, 254)))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            using (Font fBold = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (Font fSmall = new Font("Segoe UI", 7.5F, FontStyle.Regular))
            {
                e.Graphics.FillRectangle(caSangBrush, 0, 0, p.Width, 5 * RowHeightPx);
                e.Graphics.FillRectangle(caChieuBrush, 0, 5 * RowHeightPx + CaDividerPx, p.Width, 5 * RowHeightPx);

                for (int t = 1; t <= 10; t++)
                {
                    int y = LayToaDoYTiet(t);
                    e.Graphics.DrawLine(borderPen, 0, y, p.Width, y);
                    TietHoc th = _dsTiet.FirstOrDefault(x => x.SoTiet == t);
                    string gio = th != null ? (th.GioBatDau + "\n" + th.GioKetThuc) : "";
                    e.Graphics.DrawString("Tiết " + t, fBold, textBrush, 6, y + 8);
                    if (!string.IsNullOrEmpty(gio))
                        e.Graphics.DrawString(gio, fSmall, textBrush, 6, y + 26);
                }

                using (SolidBrush divBrush = new SolidBrush(Color.FromArgb(203, 213, 225)))
                {
                    e.Graphics.FillRectangle(divBrush, 0, 5 * RowHeightPx, p.Width, CaDividerPx);
                }
                e.Graphics.DrawString("CHIỀU", fBold, textBrush, 14, 5 * RowHeightPx + 1);
            }
        }

        private void DayPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender;
            using (Pen gridPen = new Pen(Color.FromArgb(241, 245, 249)))
            using (Pen rightPen = new Pen(Color.FromArgb(226, 232, 240)))
            using (SolidBrush divBrush = new SolidBrush(Color.FromArgb(226, 232, 240)))
            {
                for (int t = 1; t <= 10; t++)
                {
                    int y = LayToaDoYTiet(t);
                    e.Graphics.DrawLine(gridPen, 0, y, p.Width, y);
                }
                e.Graphics.FillRectangle(divBrush, 0, 5 * RowHeightPx, p.Width, CaDividerPx);
                e.Graphics.DrawLine(rightPen, p.Width - 1, 0, p.Width - 1, p.Height);
            }
        }

        private void DayPanel_SizeChanged(object sender, EventArgs e)
        {
            if (!_dangKhoiTao)
            {
                Panel p = (Panel)sender;
                if (p.Tag is int) VeCacTheTrongNgay((int)p.Tag);
            }
        }

        private static int LayToaDoYTiet(int soTiet)
        {
            int idx = Math.Max(1, Math.Min(10, soTiet)) - 1;
            int y = idx * RowHeightPx;
            if (soTiet >= 6) y += CaDividerPx;
            return y;
        }

        private void TaiDanhMucVaDuLieu()
        {
            try
            {
                int? idHk = SessionHelper.CurrentHocKyId;
                _dsTiet = _danhMucService.LayDanhSachTietHoc();
                _dsNhom = _danhMucService.LayDanhSachNhomHoc(idHk);
                _dsGV = _danhMucService.LayDanhSachGiangVien(true);
                _dsPhong = _danhMucService.LayDanhSachPhongHoc();
                _dsBuoiAll = _xepLichService.GetDanhSachBuoiHoc(idHk);

                cboNhomHoc.DataSource = null;
                cboNhomHoc.DataSource = _dsNhom;
                cboNhomHoc.DisplayMember = "TenNhomHoc";
                cboNhomHoc.ValueMember = "ID_NhomHoc";

                cboPhongHoc.DataSource = null;
                cboPhongHoc.DataSource = _dsPhong;
                cboPhongHoc.DisplayMember = "MaPhong";
                cboPhongHoc.ValueMember = "ID_PhongHoc";

                cboGiangVien.DataSource = null;
                cboGiangVien.DataSource = _dsGV;
                cboGiangVien.DisplayMember = "HoTen";
                cboGiangVien.ValueMember = "ID_GV";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu TKB: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool ThuocTuan(string ngayStr, DateTime dauTuan)
        {
            DateTime d;
            if (!DateTime.TryParseExact(ngayStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return false;
            return d.Date >= dauTuan.Date && d.Date <= dauTuan.AddDays(6).Date;
        }

        private List<BuoiHocChiTiet> LayBuoiHocTheoBoLoc()
        {
            IEnumerable<BuoiHocChiTiet> q = _dsBuoiAll.Where(b => ThuocTuan(b.NgayHoc, _ngayDauTuan));
            string goc = cboGocNhin.SelectedItem != null ? cboGocNhin.SelectedItem.ToString() : "Toàn trường";
            if (goc == "Theo lớp/nhóm" && cboDoiTuongLoc.SelectedValue is int)
            {
                int id = (int)cboDoiTuongLoc.SelectedValue;
                q = q.Where(b => b.ID_NhomHoc == id);
            }
            else if (goc == "Theo giảng viên" && cboDoiTuongLoc.SelectedValue is int)
            {
                int id = (int)cboDoiTuongLoc.SelectedValue;
                q = q.Where(b => b.DanhSachID_GV.Contains(id));
            }
            else if (goc == "Theo phòng" && cboDoiTuongLoc.SelectedValue is int)
            {
                int id = (int)cboDoiTuongLoc.SelectedValue;
                q = q.Where(b => b.ID_PhongHoc.HasValue && b.ID_PhongHoc.Value == id);
            }
            return q.ToList();
        }

        private void VeLuoiTkbTuan()
        {
            string[] tenThu = new[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" };
            for (int d = 0; d < 7; d++)
            {
                DateTime ngay = _ngayDauTuan.AddDays(d);
                _dayHeaderLabels[d].Text = tenThu[d] + "\n" + ngay.ToString("dd/MM/yyyy");
                VeCacTheTrongNgay(d);
            }
            tlpGridTkb.Invalidate(true);
        }

        private void VeCacTheTrongNgay(int dayIndex)
        {
            Panel p = _dayPanels[dayIndex];
            if (p == null) return;
            p.Controls.Clear();

            string ngayStr = _ngayDauTuan.AddDays(dayIndex).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<BuoiHocChiTiet> buois = LayBuoiHocTheoBoLoc()
                .Where(b => b.NgayHoc == ngayStr && b.DanhSachSoTiet.Count > 0)
                .OrderBy(b => b.DanhSachSoTiet.Min())
                .ThenBy(b => b.ID_BuoiHoc)
                .ToList();

            if (buois.Count == 0) return;

            // Thuật toán chia cột song song cho các buổi giao tiết trong cùng ngày
            List<Tuple<BuoiHocChiTiet, int, int>> intervals = buois
                .Select(b => Tuple.Create(b, b.DanhSachSoTiet.Min(), b.DanhSachSoTiet.Max()))
                .ToList();

            int[] colIndex = new int[intervals.Count];
            int[] colTotal = new int[intervals.Count];

            // Nhóm thành các cụm giao nhau
            int idx = 0;
            while (idx < intervals.Count)
            {
                int clusterEnd = intervals[idx].Item3;
                int j = idx + 1;
                while (j < intervals.Count && intervals[j].Item2 <= clusterEnd)
                {
                    if (intervals[j].Item3 > clusterEnd) clusterEnd = intervals[j].Item3;
                    j++;
                }

                // Gán cột cho cụm [idx .. j-1]
                List<int> colEndTicks = new List<int>();
                for (int k = idx; k < j; k++)
                {
                    int assigned = -1;
                    for (int c = 0; c < colEndTicks.Count; c++)
                    {
                        if (colEndTicks[c] < intervals[k].Item2)
                        {
                            assigned = c;
                            colEndTicks[c] = intervals[k].Item3;
                            break;
                        }
                    }
                    if (assigned == -1)
                    {
                        assigned = colEndTicks.Count;
                        colEndTicks.Add(intervals[k].Item3);
                    }
                    colIndex[k] = assigned;
                }
                int totalCols = Math.Max(1, colEndTicks.Count);
                for (int k = idx; k < j; k++) colTotal[k] = totalCols;
                idx = j;
            }

            int usableWidth = Math.Max(60, p.ClientSize.Width - 6);
            for (int k = 0; k < intervals.Count; k++)
            {
                BuoiHocChiTiet b = intervals[k].Item1;
                int tStart = intervals[k].Item2;
                int tEnd = intervals[k].Item3;
                int topY = LayToaDoYTiet(tStart) + 2;
                int bottomY = LayToaDoYTiet(tEnd) + RowHeightPx - 2;
                int cardH = Math.Max(48, bottomY - topY);

                int cardW = Math.Max(40, (usableWidth / colTotal[k]) - 3);
                int leftX = 3 + colIndex[k] * (usableWidth / colTotal[k]);

                Panel card = TaoTheBuoiHocUmp(b, leftX, topY, cardW, cardH);
                p.Controls.Add(card);
            }
        }

        private Panel TaoTheBuoiHocUmp(BuoiHocChiTiet b, int x, int y, int w, int h)
        {
            Color bg = Color.FromArgb(239, 246, 255);
            Color border = Color.FromArgb(37, 99, 235);
            if (b.LoaiHinhGiangDay == "TH") { bg = Color.FromArgb(236, 253, 245); border = Color.FromArgb(5, 150, 105); }
            else if (b.LoaiHinhGiangDay == "TT") { bg = Color.FromArgb(255, 247, 237); border = Color.FromArgb(217, 119, 6); }

            Panel card = new Panel();
            card.Location = new Point(x, y);
            card.Size = new Size(w, h);
            card.BackColor = bg;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Cursor = Cursors.Hand;
            card.Tag = b;

            Panel accentBar = new Panel();
            accentBar.Dock = DockStyle.Left;
            accentBar.Width = 4;
            accentBar.BackColor = border;

            string phongCoSo = string.IsNullOrWhiteSpace(b.TenPhong)
                ? (b.MaPhong ?? "Chưa phân phòng")
                : (b.TenPhong + (string.IsNullOrWhiteSpace(b.CoSoPhong) ? "" : " - " + b.CoSoPhong));
            string ghiChuLine = !string.IsNullOrWhiteSpace(b.TenBaiHoc)
                ? ("Bài: " + b.TenBaiHoc)
                : (b.GhiChu ?? "");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(b.TenMonHoc + " [" + b.LoaiHinhGiangDay + "]");
            sb.AppendLine(!string.IsNullOrWhiteSpace(b.DanhSachMaLopHP) ? b.DanhSachMaLopHP : b.MaNhomHoc);
            sb.AppendLine("Tiết: " + (b.ChuoiTietSo ?? b.ChuoiTietHoc));
            sb.AppendLine("Giờ: " + (b.GioBatDau ?? "") + " - " + (b.GioKetThuc ?? ""));
            sb.AppendLine("Phòng: " + phongCoSo);
            sb.AppendLine("GV: " + (string.IsNullOrWhiteSpace(b.DanhSachGiangVien) ? "Chưa phân công" : b.DanhSachGiangVien));
            sb.Append("Ghi chú: " + ghiChuLine);

            Label lbl = new Label();
            lbl.Dock = DockStyle.Fill;
            lbl.Padding = new Padding(4, 3, 3, 2);
            lbl.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
            lbl.ForeColor = Color.FromArgb(15, 23, 42);
            lbl.Text = sb.ToString();
            lbl.Tag = b;
            lbl.Click += Card_Click;

            card.Click += Card_Click;
            card.Controls.Add(lbl);
            card.Controls.Add(accentBar);
            return card;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            BuoiHocChiTiet b = ctrl != null ? ctrl.Tag as BuoiHocChiTiet : null;
            if (b != null) ChonBuoiHoc(b);
        }

        private void ChonBuoiHoc(BuoiHocChiTiet b)
        {
            _buoiDangChon = b;
            tabInspector.SelectedTab = tpChiTiet;
            lblCtTieuDe.Text = b.TenMonHoc + " (" + b.MaNhomHoc + ")";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Mã buổi học: #" + b.ID_BuoiHoc + "   |   Trạng thái: " + b.TrangThai);
            sb.AppendLine("Môn học: " + b.TenMonHoc);
            sb.AppendLine("Nhóm học: " + b.MaNhomHoc + " - " + b.TenNhomHoc);
            sb.AppendLine("Lớp học phần: " + (b.DanhSachMaLopHP ?? "-"));
            sb.AppendLine("Ngày học: " + b.NgayHoc + " (" + b.BuoiTrongNgay + ")");
            sb.AppendLine("Tiết học: " + b.ChuoiTietHoc + " (" + b.GioBatDau + " - " + b.GioKetThuc + ")");
            sb.AppendLine("Loại hình: " + b.LoaiHinhGiangDay + (b.DayBangNgoaiNgu ? " · Dạy bằng ngoại ngữ (x1.5)" : ""));
            sb.AppendLine("Sĩ số buổi: " + b.TongSiSoBuoi + " SV");
            sb.AppendLine("Phòng học: " + (b.TenPhong ?? b.MaPhong ?? "Chưa gán") + (string.IsNullOrWhiteSpace(b.CoSoPhong) ? "" : " - " + b.CoSoPhong));
            sb.AppendLine("Giảng viên: " + (b.DanhSachGiangVien ?? "Chưa phân công"));
            sb.AppendLine("Bài học: " + (b.TenBaiHoc ?? "(Chưa gán bài học)"));
            sb.AppendLine("Hệ số quy đổi: " + b.HeSoQuyDoi.ToString("0.##") + "   |   Giờ chuẩn: " + b.GioChuanQuyDoi.ToString("0.##"));
            sb.AppendLine("Ghi chú: " + (b.GhiChu ?? ""));
            txtCtNoiDung.Text = sb.ToString();
        }

        private void cboGocNhin_SelectedIndexChanged(object sender, EventArgs e)
        {
            string goc = cboGocNhin.SelectedItem != null ? cboGocNhin.SelectedItem.ToString() : "Toàn trường";
            _dangKhoiTao = true;
            cboDoiTuongLoc.DataSource = null;
            if (goc == "Toàn trường")
            {
                cboDoiTuongLoc.Enabled = false;
            }
            else if (goc == "Theo lớp/nhóm")
            {
                cboDoiTuongLoc.Enabled = true;
                cboDoiTuongLoc.DataSource = _dsNhom.ToList();
                cboDoiTuongLoc.DisplayMember = "TenNhomHoc";
                cboDoiTuongLoc.ValueMember = "ID_NhomHoc";
            }
            else if (goc == "Theo giảng viên")
            {
                cboDoiTuongLoc.Enabled = true;
                cboDoiTuongLoc.DataSource = _dsGV.ToList();
                cboDoiTuongLoc.DisplayMember = "HoTen";
                cboDoiTuongLoc.ValueMember = "ID_GV";
            }
            else if (goc == "Theo phòng")
            {
                cboDoiTuongLoc.Enabled = true;
                cboDoiTuongLoc.DataSource = _dsPhong.ToList();
                cboDoiTuongLoc.DisplayMember = "MaPhong";
                cboDoiTuongLoc.ValueMember = "ID_PhongHoc";
            }
            _dangKhoiTao = false;
            VeLuoiTkbTuan();
        }

        private void cboDoiTuongLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_dangKhoiTao) VeLuoiTkbTuan();
        }

        private void btnTuanTruoc_Click(object sender, EventArgs e)
        {
            _ngayDauTuan = _ngayDauTuan.AddDays(-7);
            _dangKhoiTao = true;
            dtpNgayTrongTuan.Value = _ngayDauTuan;
            _dangKhoiTao = false;
            VeLuoiTkbTuan();
        }

        private void btnTuanSau_Click(object sender, EventArgs e)
        {
            _ngayDauTuan = _ngayDauTuan.AddDays(7);
            _dangKhoiTao = true;
            dtpNgayTrongTuan.Value = _ngayDauTuan;
            _dangKhoiTao = false;
            VeLuoiTkbTuan();
        }

        private void dtpNgayTrongTuan_ValueChanged(object sender, EventArgs e)
        {
            if (_dangKhoiTao) return;
            _ngayDauTuan = LayThuHaiCuaTuan(dtpNgayTrongTuan.Value);
            VeLuoiTkbTuan();
        }

        private void btnTaiLai_Click(object sender, EventArgs e)
        {
            TaiDanhMucVaDuLieu();
            VeLuoiTkbTuan();
        }

        private void btnMoThemNhanh_Click(object sender, EventArgs e)
        {
            tabInspector.SelectedTab = tpThemNhanh;
        }

        private void cboNhomHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cboNhomHoc.SelectedValue is int)) return;
            int idNhom = (int)cboNhomHoc.SelectedValue;
            NhomHoc nh = _dsNhom.FirstOrDefault(x => x.ID_NhomHoc == idNhom);
            if (nh != null && !string.IsNullOrWhiteSpace(nh.LoaiNhom))
            {
                int idx = cboLoaiHinh.Items.IndexOf(nh.LoaiNhom);
                if (idx >= 0) cboLoaiHinh.SelectedIndex = idx;
            }
            BaiHoc baiTiep = _xepLichService.LayBaiHocTiepTheoChuaXep(idNhom);
            if (baiTiep != null && baiTiep.ID_GV_MacDinh.HasValue)
            {
                cboGiangVien.SelectedValue = baiTiep.ID_GV_MacDinh.Value;
            }
        }

        private void btnLuuXepLich_Click(object sender, EventArgs e)
        {
            if (!(cboNhomHoc.SelectedValue is int))
            {
                MessageBox.Show("Vui lòng chọn Nhóm học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<int> dsSoTiet = chkTietHoc.Where(c => c.Checked).Select(c => (int)c.Tag).ToList();
            if (dsSoTiet.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một Tiết học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<int> dsIdTiet = _dsTiet.Where(t => dsSoTiet.Contains(t.SoTiet)).Select(t => t.ID_TietHoc).ToList();
            if (dsIdTiet.Count == 0) dsIdTiet = dsSoTiet;

            List<int> dsThu = chkThuTrongTuan.Where(c => c.Checked).Select(c => (int)c.Tag).ToList();
            int buocNhay = cboChuKyLap.SelectedIndex == 1 ? 2 : 1;

            List<PhanCongGiangVienInput> dsPC = new List<PhanCongGiangVienInput>();
            if (cboGiangVien.SelectedValue is int)
            {
                dsPC.Add(new PhanCongGiangVienInput
                {
                    ID_GV = (int)cboGiangVien.SelectedValue,
                    VaiTro = "Chinh",
                    SoTietGiangDay = dsSoTiet.Count
                });
            }

            XepLichLapLaiRequest req = new XepLichLapLaiRequest
            {
                ID_NhomHoc = (int)cboNhomHoc.SelectedValue,
                NgayBatDau = dtpNgayBD.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DanhSachThuTrongTuan = dsThu,
                SoTuanLap = (int)nudSoTuanLap.Value,
                BuocNhayTuan = buocNhay,
                DanhSachID_Tiet = dsIdTiet,
                ID_PhongHoc = cboPhongHoc.SelectedValue is int ? (int?)((int)cboPhongHoc.SelectedValue) : null,
                LoaiHinhGiangDay = cboLoaiHinh.SelectedItem != null ? cboLoaiHinh.SelectedItem.ToString() : "LT",
                DayBangNgoaiNgu = chkNgoaiNgu.Checked,
                DanhSachPhanCong = dsPC,
                TuDongGanBaiHoc = chkTuDongBaiHoc.Checked,
                GhiChu = txtGhiChu.Text
            };

            KetQuaXepLichLapLai kq = _xepLichService.XepLichNhanhLapLai(req);
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(kq.ThongBaoTongQuat);
            if (kq.ChiTietBiTrung.Count > 0)
            {
                msg.AppendLine("\nCác buổi bị trùng / bỏ qua:");
                foreach (string s in kq.ChiTietBiTrung.Take(6)) msg.AppendLine("- " + s);
            }

            MessageBox.Show(msg.ToString(), kq.ThanhCong ? "Kết quả xếp lịch" : "Cảnh báo xung đột",
                MessageBoxButtons.OK, kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (kq.ThanhCong)
            {
                _ngayDauTuan = LayThuHaiCuaTuan(dtpNgayBD.Value);
                _dangKhoiTao = true;
                dtpNgayTrongTuan.Value = dtpNgayBD.Value;
                _dangKhoiTao = false;
                TaiDanhMucVaDuLieu();
                VeLuoiTkbTuan();
            }
        }

        private void btnSapXepBaiHoc_Click(object sender, EventArgs e)
        {
            if (_buoiDangChon == null) return;
            KetQuaThaoTac kq = _xepLichService.SapXepLaiThuTuBaiHocTheoThoiGian(_buoiDangChon.ID_NhomHoc);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (kq.ThanhCong)
            {
                TaiDanhMucVaDuLieu();
                VeLuoiTkbTuan();
            }
        }

        private void btnXoaBuoi_Click(object sender, EventArgs e)
        {
            if (_buoiDangChon == null) return;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa buổi học #" + _buoiDangChon.ID_BuoiHoc + " (" + _buoiDangChon.TenMonHoc + " - " + _buoiDangChon.NgayHoc + ")?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            KetQuaThaoTac kq = _xepLichService.XoaBuoiHoc(_buoiDangChon.ID_BuoiHoc);
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (kq.ThanhCong)
            {
                _buoiDangChon = null;
                txtCtNoiDung.Clear();
                lblCtTieuDe.Text = "Đã xóa buổi học.";
                TaiDanhMucVaDuLieu();
                VeLuoiTkbTuan();
            }
        }
    }
}
