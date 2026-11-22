using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ trung tâm của dự án PM_Tinhgiogiang (LINQ to SQL — chuẩn khung HNKHKTD_Lib).
    /// Toàn bộ truy vấn CSDL qua PM_Tinhgiogiang_DBDataContext, xếp lịch 10 tiết, kiểm tra xung đột
    /// và tính giờ chuẩn Điều 12 được tập trung tại đây để WinForms sử dụng thống nhất.
    /// </summary>
    public class ClassLibrary
    {
        private readonly PM_Tinhgiogiang_DBDataContext db = new PM_Tinhgiogiang_DBDataContext();

        public const double HeSoNgoaiNgu = 1.5;

        #region HẰNG SỐ NHÓM DANH MỤC (9 NHÓM CHÍNH)
        public const int NHOM_HOC_HAM = 1;
        public const int NHOM_HOC_VI = 2;
        public const int NHOM_BO_MON = 3;
        public const int NHOM_HE_DAO_TAO = 4;
        public const int NHOM_LOAI_PHONG = 5;
        public const int NHOM_LOAI_NCKH = 6;
        public const int NHOM_CAP_DAO_TAO = 7;
        public const int NHOM_NHIEM_VU_DETAI = 8;
        public const int NHOM_CONG_VIEC_KHAO_THI = 9;
        #endregion

        #region 1. NGƯỜI DÙNG & ĐĂNG NHẬP
        public NguoiDung DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;
            string u = tenDangNhap.Trim();

            // Nếu bảng SYS_NguoiDung chưa có tài khoản admin, tự động khởi tạo mặc định (admin / 123456)
            if (string.Equals(u, "admin", StringComparison.OrdinalIgnoreCase)
                && !db.SYS_NguoiDungs.Any(x => x.TenDangNhap == "admin"))
            {
                db.SYS_NguoiDungs.InsertOnSubmit(new SYS_NguoiDung
                {
                    TenDangNhap = "admin",
                    MatKhauHash = "123456",
                    VaiTro = "Admin",
                    TrangThai = true
                });
                db.SubmitChanges();
            }

            SYS_NguoiDung row = db.SYS_NguoiDungs
                .FirstOrDefault(x => x.TenDangNhap == u && (x.TrangThai == null || x.TrangThai == true));
            if (row == null) return null;
            if (!string.Equals(row.MatKhauHash, matKhau, StringComparison.Ordinal)) return null;

            return new NguoiDung
            {
                ID_NguoiDung = row.ID_NguoiDung,
                TenDangNhap = row.TenDangNhap,
                MatKhauHash = row.MatKhauHash,
                ID_GV = row.ID_GV,
                VaiTro = row.VaiTro,
                TrangThai = row.TrangThai ?? true
            };
        }

        public List<NguoiDung> LayDanhSachNguoiDung()
        {
            var query = from nd in db.SYS_NguoiDungs
                        join gv in db.DM_GiangViens on nd.ID_GV equals gv.ID_GV into gj
                        from g in gj.DefaultIfEmpty()
                        orderby nd.ID_NguoiDung
                        select new NguoiDung
                        {
                            ID_NguoiDung = nd.ID_NguoiDung,
                            TenDangNhap = nd.TenDangNhap,
                            MatKhauHash = nd.MatKhauHash,
                            ID_GV = nd.ID_GV,
                            VaiTro = nd.VaiTro ?? "GiaoVu",
                            TrangThai = nd.TrangThai ?? true,
                            TenGiangVien = g != null ? g.HoTen : null
                        };
            return query.ToList();
        }

        public KetQuaThaoTac LuuNguoiDung(NguoiDung nd)
        {
            if (nd == null || string.IsNullOrWhiteSpace(nd.TenDangNhap))
                return KetQuaThaoTac.Loi("Tên đăng nhập là bắt buộc.");
            if (nd.ID_NguoiDung <= 0 && string.IsNullOrWhiteSpace(nd.MatKhauHash))
                return KetQuaThaoTac.Loi("Vui lòng nhập mật khẩu cho tài khoản mới.");

            int? idGv = (nd.ID_GV.HasValue && nd.ID_GV.Value > 0) ? nd.ID_GV : null;
            string user = nd.TenDangNhap.Trim();

            try
            {
                if (nd.ID_NguoiDung > 0)
                {
                    SYS_NguoiDung entity = db.SYS_NguoiDungs.FirstOrDefault(x => x.ID_NguoiDung == nd.ID_NguoiDung);
                    if (entity == null) return KetQuaThaoTac.Loi("Không tìm thấy tài khoản.");
                    entity.TenDangNhap = user;
                    if (!string.IsNullOrWhiteSpace(nd.MatKhauHash))
                        entity.MatKhauHash = nd.MatKhauHash.Trim();
                    entity.ID_GV = idGv;
                    entity.VaiTro = string.IsNullOrWhiteSpace(nd.VaiTro) ? "GiaoVu" : nd.VaiTro.Trim();
                    entity.TrangThai = nd.TrangThai;
                    db.SubmitChanges();
                    return KetQuaThaoTac.Ok("Đã cập nhật tài khoản '" + entity.TenDangNhap + "'.");
                }
                else
                {
                    if (db.SYS_NguoiDungs.Any(x => x.TenDangNhap == user))
                        return KetQuaThaoTac.Loi("Tên đăng nhập '" + user + "' đã tồn tại.");
                    SYS_NguoiDung entity = new SYS_NguoiDung
                    {
                        TenDangNhap = user,
                        MatKhauHash = nd.MatKhauHash.Trim(),
                        ID_GV = idGv,
                        VaiTro = string.IsNullOrWhiteSpace(nd.VaiTro) ? "GiaoVu" : nd.VaiTro.Trim(),
                        TrangThai = nd.TrangThai
                    };
                    db.SYS_NguoiDungs.InsertOnSubmit(entity);
                    db.SubmitChanges();
                    return KetQuaThaoTac.Ok("Đã tạo tài khoản '" + entity.TenDangNhap + "'.");
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu người dùng: " + ex.Message); }
        }

        public KetQuaThaoTac KhoaMoNguoiDung(int idNguoiDung, bool trangThaiMoi)
        {
            try
            {
                SYS_NguoiDung entity = db.SYS_NguoiDungs.FirstOrDefault(x => x.ID_NguoiDung == idNguoiDung);
                if (entity == null) return KetQuaThaoTac.Loi("Không tìm thấy tài khoản.");
                entity.TrangThai = trangThaiMoi;
                db.SubmitChanges();
                return KetQuaThaoTac.Ok(trangThaiMoi ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi cập nhật trạng thái tài khoản: " + ex.Message); }
        }
        #endregion

        #region 2. DANH MỤC NỀN TẢNG (NĂM HỌC, HỌC KỲ, TIẾT, PHÒNG, GIẢNG VIÊN, MÔN, QUY TẮC)
        public List<NhomDanhMuc> LayDanhSachNhomDanhMuc()
        {
            if (!db.DM_NhomDanhMucs.Any())
            {
                string[] tenNhoms = new[]
                {
                    "Học hàm",
                    "Học vị",
                    "Bộ môn",
                    "Hệ đào tạo",
                    "Loại phòng học",
                    "Loại hoạt động NCKH",
                    "Cấp đào tạo",
                    "Nhiệm vụ đề tài / hội đồng",
                    "Công việc khảo thí"
                };
                foreach (string ten in tenNhoms)
                {
                    db.DM_NhomDanhMucs.InsertOnSubmit(new DM_NhomDanhMuc { TenNhom = ten });
                }
                db.SubmitChanges();
            }

            return db.DM_NhomDanhMucs
                .OrderBy(n => n.MaNhom)
                .Select(n => new NhomDanhMuc
                {
                    MaNhom = n.MaNhom,
                    TenNhom = n.TenNhom,
                    MoTa = n.MoTa
                }).ToList();
        }

        public KetQuaThaoTac LuuDanhMuc(DanhMuc dm)
        {
            if (dm == null || dm.MaNhom <= 0 || string.IsNullOrWhiteSpace(dm.TenDanhMuc))
                return KetQuaThaoTac.Loi("Nhóm danh mục và Tên danh mục là bắt buộc.");
            try
            {
                if (dm.MaDanhMuc > 0)
                {
                    DM_DanhMuc e = db.DM_DanhMucs.FirstOrDefault(x => x.MaDanhMuc == dm.MaDanhMuc);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy danh mục.");
                    e.MaNhom = dm.MaNhom;
                    e.TenDanhMuc = dm.TenDanhMuc.Trim();
                    e.VietTat = string.IsNullOrWhiteSpace(dm.VietTat) ? null : dm.VietTat.Trim();
                    e.GiaTriThamSo = dm.GiaTriThamSo.HasValue ? (decimal?)dm.GiaTriThamSo.Value : null;
                    e.ThuTu = dm.ThuTu;
                    e.KichHoat = dm.KichHoat;
                    db.SubmitChanges();
                }
                else
                {
                    DM_DanhMuc e = new DM_DanhMuc
                    {
                        MaNhom = dm.MaNhom,
                        TenDanhMuc = dm.TenDanhMuc.Trim(),
                        VietTat = string.IsNullOrWhiteSpace(dm.VietTat) ? null : dm.VietTat.Trim(),
                        GiaTriThamSo = dm.GiaTriThamSo.HasValue ? (decimal?)dm.GiaTriThamSo.Value : null,
                        ThuTu = dm.ThuTu > 0 ? dm.ThuTu : 1,
                        KichHoat = dm.KichHoat
                    };
                    db.DM_DanhMucs.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
                return KetQuaThaoTac.Ok("Đã lưu danh mục '" + dm.TenDanhMuc.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu danh mục: " + ex.Message); }
        }

        public KetQuaThaoTac XoaHoacAnDanhMuc(int maDanhMuc)
        {
            try
            {
                DM_DanhMuc e = db.DM_DanhMucs.FirstOrDefault(x => x.MaDanhMuc == maDanhMuc);
                if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy danh mục.");
                e.KichHoat = false;
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã ẩn danh mục '" + e.TenDanhMuc + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi ẩn danh mục: " + ex.Message); }
        }

        public List<DanhMuc> LayDanhMucTheoNhom(int maNhom, bool chiLayKichHoat = true)
        {
            return db.DM_DanhMucs
                .Where(d => d.MaNhom == maNhom && (!chiLayKichHoat || d.KichHoat == true))
                .OrderBy(d => d.ThuTu ?? 1)
                .ThenBy(d => d.TenDanhMuc)
                .Select(d => new DanhMuc
                {
                    MaDanhMuc = d.MaDanhMuc,
                    MaNhom = d.MaNhom,
                    TenDanhMuc = d.TenDanhMuc,
                    VietTat = d.VietTat,
                    GiaTriThamSo = d.GiaTriThamSo.HasValue ? (double?)d.GiaTriThamSo.Value : null,
                    ThuTu = d.ThuTu ?? 1,
                    KichHoat = d.KichHoat ?? true
                }).ToList();
        }

        public List<NamHoc> LayDanhSachNamHoc()
        {
            return db.DM_NamHocs.OrderByDescending(n => n.NgayBatDau).ToList()
                .Select(n => new NamHoc
                {
                    ID_NamHoc = n.ID_NamHoc,
                    MaNH = n.MaNH,
                    TenNH = n.TenNH,
                    NgayBatDau = n.NgayBatDau.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    NgayKetThuc = n.NgayKetThuc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                }).ToList();
        }

        public List<HocKy> LayDanhSachHocKy()
        {
            return db.DM_HocKys.OrderByDescending(h => h.NgayBD).ToList()
                .Select(h => new HocKy
                {
                    ID_HocKy = h.ID_HocKy,
                    ID_NamHoc = h.ID_NamHoc,
                    MaHK = h.MaHK,
                    TenHK = h.TenHK,
                    NgayBD = h.NgayBD.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    NgayKT = h.NgayKT.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                }).ToList();
        }

        public List<TietHoc> LayDanhSachTietHoc()
        {
            return db.DM_TietHocs.OrderBy(t => t.SoTiet).ToList()
                .Select(t => new TietHoc
                {
                    ID_TietHoc = t.ID_TietHoc,
                    SoTiet = t.SoTiet,
                    BuoiTrongNgay = t.BuoiTrongNgay,
                    GioBatDau = t.GioBatDau.ToString(@"hh\:mm"),
                    GioKetThuc = t.GioKetThuc.ToString(@"hh\:mm")
                }).ToList();
        }

        public KetQuaThaoTac LuuNamHoc(NamHoc nh)
        {
            if (nh == null || string.IsNullOrWhiteSpace(nh.MaNH) || string.IsNullOrWhiteSpace(nh.TenNH))
                return KetQuaThaoTac.Loi("Mã năm học và Tên năm học là bắt buộc.");
            DateTime bd, kt;
            if (!DateTime.TryParseExact((nh.NgayBatDau ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out bd) ||
                !DateTime.TryParseExact((nh.NgayKetThuc ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out kt))
                return KetQuaThaoTac.Loi("Ngày bắt đầu / kết thúc phải đúng định dạng yyyy-MM-dd.");
            try
            {
                if (nh.ID_NamHoc > 0)
                {
                    DM_NamHoc e = db.DM_NamHocs.FirstOrDefault(x => x.ID_NamHoc == nh.ID_NamHoc);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy năm học.");
                    e.MaNH = nh.MaNH.Trim();
                    e.TenNH = nh.TenNH.Trim();
                    e.NgayBatDau = bd.Date;
                    e.NgayKetThuc = kt.Date;
                }
                else
                {
                    db.DM_NamHocs.InsertOnSubmit(new DM_NamHoc
                    {
                        MaNH = nh.MaNH.Trim(),
                        TenNH = nh.TenNH.Trim(),
                        NgayBatDau = bd.Date,
                        NgayKetThuc = kt.Date
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã lưu năm học '" + nh.TenNH.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu năm học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaNamHoc(int idNamHoc)
        {
            try
            {
                DM_NamHoc e = db.DM_NamHocs.FirstOrDefault(x => x.ID_NamHoc == idNamHoc);
                if (e != null) { db.DM_NamHocs.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa năm học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Không thể xóa năm học (đã có học kỳ hoặc dữ liệu liên quan): " + ex.Message); }
        }

        public KetQuaThaoTac LuuHocKy(HocKy hk)
        {
            if (hk == null || hk.ID_NamHoc <= 0 || string.IsNullOrWhiteSpace(hk.MaHK) || string.IsNullOrWhiteSpace(hk.TenHK))
                return KetQuaThaoTac.Loi("Năm học, Mã học kỳ và Tên học kỳ là bắt buộc.");
            DateTime bd, kt;
            if (!DateTime.TryParseExact((hk.NgayBD ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out bd) ||
                !DateTime.TryParseExact((hk.NgayKT ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out kt))
                return KetQuaThaoTac.Loi("Ngày BĐ / KT phải đúng định dạng yyyy-MM-dd.");
            try
            {
                if (hk.ID_HocKy > 0)
                {
                    DM_HocKy e = db.DM_HocKys.FirstOrDefault(x => x.ID_HocKy == hk.ID_HocKy);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy học kỳ.");
                    e.ID_NamHoc = hk.ID_NamHoc;
                    e.MaHK = hk.MaHK.Trim();
                    e.TenHK = hk.TenHK.Trim();
                    e.NgayBD = bd.Date;
                    e.NgayKT = kt.Date;
                }
                else
                {
                    db.DM_HocKys.InsertOnSubmit(new DM_HocKy
                    {
                        ID_NamHoc = hk.ID_NamHoc,
                        MaHK = hk.MaHK.Trim(),
                        TenHK = hk.TenHK.Trim(),
                        NgayBD = bd.Date,
                        NgayKT = kt.Date
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã lưu học kỳ '" + hk.TenHK.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu học kỳ: " + ex.Message); }
        }

        public KetQuaThaoTac XoaHocKy(int idHocKy)
        {
            try
            {
                DM_HocKy e = db.DM_HocKys.FirstOrDefault(x => x.ID_HocKy == idHocKy);
                if (e != null) { db.DM_HocKys.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa học kỳ.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Không thể xóa học kỳ (đã có lớp/nhóm học): " + ex.Message); }
        }

        public KetQuaThaoTac KhoiTao10TietChuan()
        {
            try
            {
                if (db.DM_TietHocs.Any())
                    return KetQuaThaoTac.Ok("Khung tiết học đã có sẵn (" + db.DM_TietHocs.Count() + " tiết).");

                var chuan10 = new[]
                {
                    Tuple.Create(1, "Sang", new TimeSpan(7, 0, 0), new TimeSpan(7, 50, 0)),
                    Tuple.Create(2, "Sang", new TimeSpan(7, 50, 0), new TimeSpan(8, 40, 0)),
                    Tuple.Create(3, "Sang", new TimeSpan(8, 40, 0), new TimeSpan(9, 30, 0)),
                    Tuple.Create(4, "Sang", new TimeSpan(9, 45, 0), new TimeSpan(10, 35, 0)),
                    Tuple.Create(5, "Sang", new TimeSpan(10, 35, 0), new TimeSpan(11, 25, 0)),
                    Tuple.Create(6, "Chieu", new TimeSpan(13, 0, 0), new TimeSpan(13, 50, 0)),
                    Tuple.Create(7, "Chieu", new TimeSpan(13, 50, 0), new TimeSpan(14, 40, 0)),
                    Tuple.Create(8, "Chieu", new TimeSpan(14, 40, 0), new TimeSpan(15, 30, 0)),
                    Tuple.Create(9, "Chieu", new TimeSpan(15, 45, 0), new TimeSpan(16, 35, 0)),
                    Tuple.Create(10, "Chieu", new TimeSpan(16, 35, 0), new TimeSpan(17, 25, 0))
                };
                foreach (var t in chuan10)
                {
                    db.DM_TietHocs.InsertOnSubmit(new DM_TietHoc
                    {
                        SoTiet = t.Item1,
                        BuoiTrongNgay = t.Item2,
                        GioBatDau = t.Item3,
                        GioKetThuc = t.Item4
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã khởi tạo 10 tiết chuẩn UMP (Sáng 1–5, Chiều 6–10).");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi khởi tạo tiết học: " + ex.Message); }
        }

        public KetQuaThaoTac LuuTietHoc(TietHoc t)
        {
            if (t == null || t.SoTiet < 1 || t.SoTiet > 15)
                return KetQuaThaoTac.Loi("Số tiết không hợp lệ.");
            TimeSpan gbd, gkt;
            if (!TimeSpan.TryParse(t.GioBatDau, out gbd) || !TimeSpan.TryParse(t.GioKetThuc, out gkt))
                return KetQuaThaoTac.Loi("Giờ bắt đầu / kết thúc phải đúng định dạng HH:mm (ví dụ 07:00).");
            try
            {
                DM_TietHoc e = t.ID_TietHoc > 0
                    ? db.DM_TietHocs.FirstOrDefault(x => x.ID_TietHoc == t.ID_TietHoc)
                    : db.DM_TietHocs.FirstOrDefault(x => x.SoTiet == t.SoTiet);
                if (e != null)
                {
                    e.BuoiTrongNgay = string.IsNullOrWhiteSpace(t.BuoiTrongNgay) ? (t.SoTiet <= 5 ? "Sang" : "Chieu") : t.BuoiTrongNgay;
                    e.GioBatDau = gbd;
                    e.GioKetThuc = gkt;
                }
                else
                {
                    db.DM_TietHocs.InsertOnSubmit(new DM_TietHoc
                    {
                        SoTiet = t.SoTiet,
                        BuoiTrongNgay = string.IsNullOrWhiteSpace(t.BuoiTrongNgay) ? (t.SoTiet <= 5 ? "Sang" : "Chieu") : t.BuoiTrongNgay,
                        GioBatDau = gbd,
                        GioKetThuc = gkt
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã lưu Tiết " + t.SoTiet + ".");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu tiết học: " + ex.Message); }
        }

        public List<PhongHoc> LayDanhSachPhongHoc()
        {
            var q = from ph in db.DM_PhongHocs
                    join dm in db.DM_DanhMucs on ph.ID_LoaiPhong equals dm.MaDanhMuc into dj
                    from d in dj.DefaultIfEmpty()
                    where ph.TrangThai == null || ph.TrangThai == true
                    orderby ph.MaPhong
                    select new PhongHoc
                    {
                        ID_PhongHoc = ph.ID_PhongHoc,
                        MaPhong = ph.MaPhong,
                        TenPhong = ph.TenPhong,
                        ID_LoaiPhong = ph.ID_LoaiPhong,
                        CoSo = ph.CoSo,
                        SucChua = ph.SucChua,
                        TrangThai = (ph.TrangThai ?? true) ? 1 : 0,
                        TenLoaiPhong = d != null ? d.TenDanhMuc : null
                    };
            return q.ToList();
        }

        public KetQuaThaoTac LuuPhongHoc(PhongHoc ph)
        {
            if (ph == null || string.IsNullOrWhiteSpace(ph.MaPhong) || string.IsNullOrWhiteSpace(ph.TenPhong))
                return KetQuaThaoTac.Loi("Mã phòng và Tên phòng là bắt buộc.");
            try
            {
                if (ph.ID_PhongHoc > 0)
                {
                    DM_PhongHoc e = db.DM_PhongHocs.FirstOrDefault(x => x.ID_PhongHoc == ph.ID_PhongHoc);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy phòng học.");
                    e.MaPhong = ph.MaPhong.Trim();
                    e.TenPhong = ph.TenPhong.Trim();
                    e.ID_LoaiPhong = ph.ID_LoaiPhong;
                    e.CoSo = ph.CoSo;
                    e.SucChua = ph.SucChua;
                    db.SubmitChanges();
                }
                else
                {
                    DM_PhongHoc e = new DM_PhongHoc
                    {
                        MaPhong = ph.MaPhong.Trim(),
                        TenPhong = ph.TenPhong.Trim(),
                        ID_LoaiPhong = ph.ID_LoaiPhong,
                        CoSo = ph.CoSo,
                        SucChua = ph.SucChua,
                        TrangThai = true
                    };
                    db.DM_PhongHocs.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
                return KetQuaThaoTac.Ok("Đã lưu phòng học '" + ph.MaPhong.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu phòng học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaPhongHoc(int idPhongHoc)
        {
            try
            {
                DM_PhongHoc e = db.DM_PhongHocs.FirstOrDefault(x => x.ID_PhongHoc == idPhongHoc);
                if (e != null) { e.TrangThai = false; db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã ngừng sử dụng phòng học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa phòng học: " + ex.Message); }
        }

        public List<GiangVien> LayDanhSachGiangVien(bool chiLayDangHoatDong = true)
        {
            var q = from gv in db.DM_GiangViens
                    join bm in db.DM_DanhMucs on gv.ID_BoMon equals bm.MaDanhMuc into bmj
                    from b in bmj.DefaultIfEmpty()
                    join hh in db.DM_DanhMucs on gv.ID_HocHam equals hh.MaDanhMuc into hhj
                    from h1 in hhj.DefaultIfEmpty()
                    join hv in db.DM_DanhMucs on gv.ID_HocVi equals hv.MaDanhMuc into hvj
                    from h2 in hvj.DefaultIfEmpty()
                    where !chiLayDangHoatDong || gv.TrangThai == null || gv.TrangThai == true
                    orderby gv.HoTen
                    select new GiangVien
                    {
                        ID_GV = gv.ID_GV,
                        MaGV = gv.MaGV,
                        HoTen = gv.HoTen,
                        ID_BoMon = gv.ID_BoMon,
                        ID_HocHam = gv.ID_HocHam,
                        ID_HocVi = gv.ID_HocVi,
                        GioiTinh = gv.GioiTinh,
                        LaThinhGiang = gv.LaThinhGiang ?? false,
                        DonViCongTac = gv.DonViCongTac,
                        DinhMucGioChuan = gv.DinhMucGioChuan.HasValue ? (double)gv.DinhMucGioChuan.Value : 270.0,
                        Email = gv.Email,
                        SoDienThoai = gv.SoDienThoai,
                        TrangThai = (gv.TrangThai ?? true) ? 1 : 0,
                        TenBoMon = b != null ? b.TenDanhMuc : null,
                        TenHocHam = h1 != null ? h1.TenDanhMuc : null,
                        TenHocVi = h2 != null ? h2.TenDanhMuc : null
                    };
            return q.ToList();
        }

        public KetQuaThaoTac LuuGiangVien(GiangVien gv)
        {
            if (gv == null || string.IsNullOrWhiteSpace(gv.MaGV) || string.IsNullOrWhiteSpace(gv.HoTen))
                return KetQuaThaoTac.Loi("Mã GV và Họ tên là bắt buộc.");
            if (gv.ID_BoMon <= 0)
                return KetQuaThaoTac.Loi("Vui lòng chọn Bộ môn cho giảng viên.");
            int? idHocHam = (gv.ID_HocHam.HasValue && gv.ID_HocHam.Value > 0) ? gv.ID_HocHam : null;
            int? idHocVi = (gv.ID_HocVi.HasValue && gv.ID_HocVi.Value > 0) ? gv.ID_HocVi : null;
            try
            {
                if (gv.ID_GV > 0)
                {
                    DM_GiangVien e = db.DM_GiangViens.FirstOrDefault(x => x.ID_GV == gv.ID_GV);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy giảng viên.");
                    e.MaGV = gv.MaGV.Trim();
                    e.HoTen = gv.HoTen.Trim();
                    e.ID_BoMon = gv.ID_BoMon;
                    e.ID_HocHam = idHocHam;
                    e.ID_HocVi = idHocVi;
                    e.GioiTinh = gv.GioiTinh;
                    e.LaThinhGiang = gv.LaThinhGiang;
                    e.DonViCongTac = gv.DonViCongTac;
                    e.DinhMucGioChuan = (decimal)gv.DinhMucGioChuan;
                    e.Email = gv.Email;
                    e.SoDienThoai = gv.SoDienThoai;
                    e.TrangThai = gv.TrangThai != 0;
                    db.SubmitChanges();
                }
                else
                {
                    DM_GiangVien e = new DM_GiangVien
                    {
                        MaGV = gv.MaGV.Trim(),
                        HoTen = gv.HoTen.Trim(),
                        ID_BoMon = gv.ID_BoMon,
                        ID_HocHam = idHocHam,
                        ID_HocVi = idHocVi,
                        GioiTinh = gv.GioiTinh,
                        LaThinhGiang = gv.LaThinhGiang,
                        DonViCongTac = gv.DonViCongTac,
                        DinhMucGioChuan = (decimal)gv.DinhMucGioChuan,
                        Email = gv.Email,
                        SoDienThoai = gv.SoDienThoai,
                        TrangThai = true
                    };
                    db.DM_GiangViens.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
                return KetQuaThaoTac.Ok("Đã lưu giảng viên '" + gv.HoTen.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu giảng viên: " + ex.Message); }
        }

        public KetQuaThaoTac NgungHoatDongGiangVien(int idGV)
        {
            try
            {
                DM_GiangVien e = db.DM_GiangViens.FirstOrDefault(x => x.ID_GV == idGV);
                if (e != null) { e.TrangThai = false; db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã chuyển giảng viên sang trạng thái Ngừng hoạt động.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi ngừng hoạt động GV: " + ex.Message); }
        }

        public List<GiangVienRangBuoc> LayRangBuocGiangVien(int idGV)
        {
            return db.R_GV_RangBuocs.Where(r => r.ID_GV == idGV).OrderBy(r => r.NgayBD).ToList()
                .Select(r => new GiangVienRangBuoc
                {
                    ID_RangBuoc = r.ID_RangBuoc,
                    ID_GV = r.ID_GV,
                    NgayBD = r.NgayBD.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    NgayKT = r.NgayKT.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ThuTrongTuan = r.ThuTrongTuan,
                    BuoiTrongNgay = r.BuoiTrongNgay,
                    LyDo = r.LyDo
                }).ToList();
        }

        public KetQuaThaoTac ThemRangBuocGiangVien(GiangVienRangBuoc rb)
        {
            if (rb == null || rb.ID_GV <= 0) return KetQuaThaoTac.Loi("Vui lòng chọn Giảng viên.");
            DateTime bd, kt;
            if (!DateTime.TryParseExact((rb.NgayBD ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out bd) ||
                !DateTime.TryParseExact((rb.NgayKT ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out kt))
                return KetQuaThaoTac.Loi("Ngày BĐ / KT phải đúng định dạng yyyy-MM-dd.");
            try
            {
                db.R_GV_RangBuocs.InsertOnSubmit(new R_GV_RangBuoc
                {
                    ID_GV = rb.ID_GV,
                    NgayBD = bd.Date,
                    NgayKT = kt.Date,
                    ThuTrongTuan = rb.ThuTrongTuan,
                    BuoiTrongNgay = string.IsNullOrWhiteSpace(rb.BuoiTrongNgay) ? null : rb.BuoiTrongNgay.Trim(),
                    LyDo = string.IsNullOrWhiteSpace(rb.LyDo) ? "Bận" : rb.LyDo.Trim()
                });
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã thêm ràng buộc bận cho giảng viên.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi thêm ràng buộc: " + ex.Message); }
        }

        public KetQuaThaoTac XoaRangBuocGiangVien(int idRangBuoc)
        {
            try
            {
                R_GV_RangBuoc e = db.R_GV_RangBuocs.FirstOrDefault(x => x.ID_RangBuoc == idRangBuoc);
                if (e != null) { db.R_GV_RangBuocs.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa ràng buộc bận.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa ràng buộc: " + ex.Message); }
        }

        public KetQuaThaoTac ThemMienGiamGiangVien(MienGiamDinhMuc mg)
        {
            if (mg == null || mg.ID_GV <= 0 || mg.ID_NamHoc <= 0)
                return KetQuaThaoTac.Loi("Vui lòng chọn Giảng viên và Năm học.");
            if (string.IsNullOrWhiteSpace(mg.LyDo))
                return KetQuaThaoTac.Loi("Vui lòng nhập lý do miễn giảm.");
            try
            {
                db.R_GV_MienGiamDinhMucs.InsertOnSubmit(new R_GV_MienGiamDinhMuc
                {
                    ID_GV = mg.ID_GV,
                    ID_NamHoc = mg.ID_NamHoc,
                    TyLeGiamPhanTram = (decimal)mg.TyLeGiamPhanTram,
                    SoGioGiam = (decimal)mg.SoGioGiam,
                    LyDo = mg.LyDo.Trim()
                });
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã thêm miễn giảm định mức.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi thêm miễn giảm: " + ex.Message); }
        }

        public KetQuaThaoTac XoaMienGiamGiangVien(int idMienGiam)
        {
            try
            {
                R_GV_MienGiamDinhMuc e = db.R_GV_MienGiamDinhMucs.FirstOrDefault(x => x.ID_MienGiam == idMienGiam);
                if (e != null) { db.R_GV_MienGiamDinhMucs.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa miễn giảm.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa miễn giảm: " + ex.Message); }
        }

        public List<MienGiamDinhMuc> LayDanhSachMienGiam(int? idNamHoc = null)
        {
            var q = from mg in db.R_GV_MienGiamDinhMucs
                    join gv in db.DM_GiangViens on mg.ID_GV equals gv.ID_GV
                    join nh in db.DM_NamHocs on mg.ID_NamHoc equals nh.ID_NamHoc
                    where !idNamHoc.HasValue || mg.ID_NamHoc == idNamHoc.Value
                    orderby mg.ID_MienGiam descending
                    select new MienGiamDinhMuc
                    {
                        ID_MienGiam = mg.ID_MienGiam,
                        ID_GV = mg.ID_GV,
                        ID_NamHoc = mg.ID_NamHoc,
                        TyLeGiamPhanTram = mg.TyLeGiamPhanTram.HasValue ? (double)mg.TyLeGiamPhanTram.Value : 0,
                        SoGioGiam = mg.SoGioGiam.HasValue ? (double)mg.SoGioGiam.Value : 0,
                        LyDo = mg.LyDo,
                        MaGV = gv.MaGV,
                        TenGiangVien = gv.HoTen,
                        TenNamHoc = nh.TenNH
                    };
            return q.ToList();
        }

        public List<MonHoc> LayDanhSachMonHoc()
        {
            var q = from mh in db.DM_MonHocs
                    join bm in db.DM_DanhMucs on mh.ID_BoMon equals bm.MaDanhMuc into bmj
                    from b in bmj.DefaultIfEmpty()
                    join hdt in db.DM_DanhMucs on mh.ID_HeDaoTao equals hdt.MaDanhMuc into hdtj
                    from h in hdtj.DefaultIfEmpty()
                    orderby mh.TenMonHoc
                    select new MonHoc
                    {
                        MaMonHoc = mh.MaMonHoc,
                        TenMonHoc = mh.TenMonHoc,
                        ID_BoMon = mh.ID_BoMon,
                        ID_HeDaoTao = mh.ID_HeDaoTao,
                        SoTinChi = (double)mh.SoTinChi,
                        SoTietLT = mh.SoTietLT ?? 0,
                        SoTietTH = mh.SoTietTH ?? 0,
                        DayBangNgoaiNgu = mh.DayBangNgoaiNgu ?? false,
                        TenBoMon = b != null ? b.TenDanhMuc : null,
                        TenHeDaoTao = h != null ? h.TenDanhMuc : null
                    };
            return q.ToList();
        }

        public KetQuaThaoTac LuuMonHoc(MonHoc mh)
        {
            if (mh == null || string.IsNullOrWhiteSpace(mh.MaMonHoc) || string.IsNullOrWhiteSpace(mh.TenMonHoc))
                return KetQuaThaoTac.Loi("Mã môn học và Tên môn học là bắt buộc.");
            if (mh.ID_BoMon <= 0)
                return KetQuaThaoTac.Loi("Vui lòng chọn Bộ môn phụ trách cho môn học.");
            int? idHe = (mh.ID_HeDaoTao.HasValue && mh.ID_HeDaoTao.Value > 0) ? mh.ID_HeDaoTao : null;
            string ma = mh.MaMonHoc.Trim();
            try
            {
                DM_MonHoc e = db.DM_MonHocs.FirstOrDefault(x => x.MaMonHoc == ma);
                if (e != null)
                {
                    e.TenMonHoc = mh.TenMonHoc.Trim();
                    e.ID_BoMon = mh.ID_BoMon;
                    e.ID_HeDaoTao = idHe;
                    e.SoTinChi = (decimal)mh.SoTinChi;
                    e.SoTietLT = mh.SoTietLT;
                    e.SoTietTH = mh.SoTietTH;
                    e.DayBangNgoaiNgu = mh.DayBangNgoaiNgu;
                }
                else
                {
                    db.DM_MonHocs.InsertOnSubmit(new DM_MonHoc
                    {
                        MaMonHoc = ma,
                        TenMonHoc = mh.TenMonHoc.Trim(),
                        ID_BoMon = mh.ID_BoMon,
                        ID_HeDaoTao = idHe,
                        SoTinChi = (decimal)mh.SoTinChi,
                        SoTietLT = mh.SoTietLT,
                        SoTietTH = mh.SoTietTH,
                        DayBangNgoaiNgu = mh.DayBangNgoaiNgu
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã lưu môn học '" + mh.TenMonHoc.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu môn học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaMonHoc(string maMonHoc)
        {
            try
            {
                DM_MonHoc e = db.DM_MonHocs.FirstOrDefault(x => x.MaMonHoc == maMonHoc);
                if (e != null) { db.DM_MonHocs.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa môn học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Không thể xóa môn học (đã có bài học hoặc lớp học phần): " + ex.Message); }
        }

        public List<BaiHoc> LayDanhSachBaiHoc(string maMonHoc = null)
        {
            var q = from bh in db.DM_BaiHocs
                    join gv in db.DM_GiangViens on bh.ID_GV_MacDinh equals gv.ID_GV into gvj
                    from g in gvj.DefaultIfEmpty()
                    where string.IsNullOrEmpty(maMonHoc) || bh.MaMonHoc == maMonHoc
                    orderby bh.MaMonHoc, bh.ThuTuBai
                    select new BaiHoc
                    {
                        ID_BaiHoc = bh.ID_BaiHoc,
                        MaMonHoc = bh.MaMonHoc,
                        ThuTuBai = bh.ThuTuBai,
                        TenBaiHoc = bh.TenBaiHoc,
                        LoaiBai = bh.LoaiBai,
                        SoTiet = bh.SoTiet,
                        ID_GV_MacDinh = bh.ID_GV_MacDinh,
                        TenGiangVienMacDinh = g != null ? g.HoTen : null
                    };
            return q.ToList();
        }

        public KetQuaThaoTac LuuBaiHoc(BaiHoc b)
        {
            if (b == null || string.IsNullOrWhiteSpace(b.MaMonHoc) || string.IsNullOrWhiteSpace(b.TenBaiHoc))
                return KetQuaThaoTac.Loi("Mã môn và Tên bài học là bắt buộc.");
            int? idGvMacDinh = (b.ID_GV_MacDinh.HasValue && b.ID_GV_MacDinh.Value > 0) ? b.ID_GV_MacDinh : null;
            try
            {
                if (b.ID_BaiHoc > 0)
                {
                    DM_BaiHoc e = db.DM_BaiHocs.FirstOrDefault(x => x.ID_BaiHoc == b.ID_BaiHoc);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy bài học.");
                    e.ThuTuBai = b.ThuTuBai;
                    e.TenBaiHoc = b.TenBaiHoc.Trim();
                    e.LoaiBai = b.LoaiBai;
                    e.SoTiet = b.SoTiet;
                    e.ID_GV_MacDinh = idGvMacDinh;
                    db.SubmitChanges();
                }
                else
                {
                    DM_BaiHoc e = new DM_BaiHoc
                    {
                        MaMonHoc = b.MaMonHoc.Trim(),
                        ThuTuBai = b.ThuTuBai,
                        TenBaiHoc = b.TenBaiHoc.Trim(),
                        LoaiBai = string.IsNullOrWhiteSpace(b.LoaiBai) ? "LT" : b.LoaiBai.Trim(),
                        SoTiet = b.SoTiet,
                        ID_GV_MacDinh = idGvMacDinh
                    };
                    db.DM_BaiHocs.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
                return KetQuaThaoTac.Ok("Đã lưu bài học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu bài học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaBaiHoc(int idBaiHoc)
        {
            try
            {
                foreach (DM_BuoiHoc bh in db.DM_BuoiHocs.Where(x => x.ID_BaiHoc == idBaiHoc))
                    bh.ID_BaiHoc = null;
                DM_BaiHoc target = db.DM_BaiHocs.FirstOrDefault(x => x.ID_BaiHoc == idBaiHoc);
                if (target != null) db.DM_BaiHocs.DeleteOnSubmit(target);
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã xóa bài học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa bài học: " + ex.Message); }
        }

        public List<QuyTacQuyDoi> LayDanhSachQuyTacQuyDoi()
        {
            return db.SYS_QuyTacQuyDois.OrderBy(q => q.LoaiHinhGiangDay).ThenBy(q => q.SiSoTu).ToList()
                .Select(q => new QuyTacQuyDoi
                {
                    ID_QuyTac = q.ID_QuyTac,
                    TenQuyTac = q.TenQuyTac,
                    LoaiHinhGiangDay = q.LoaiHinhGiangDay,
                    ID_HeDaoTao = q.ID_HeDaoTao,
                    SiSoTu = q.SiSoTu,
                    SiSoDen = q.SiSoDen,
                    HeSoQuyDoi = (double)q.HeSoQuyDoi,
                    NgayHieuLucTu = q.NgayHieuLucTu.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    NgayHieuLucDen = q.NgayHieuLucDen.HasValue ? q.NgayHieuLucDen.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null
                }).ToList();
        }

        public KetQuaThaoTac LuuQuyTacQuyDoi(QuyTacQuyDoi qt)
        {
            if (qt == null || string.IsNullOrWhiteSpace(qt.TenQuyTac))
                return KetQuaThaoTac.Loi("Tên quy tắc là bắt buộc.");
            DateTime hieuLucTu;
            if (!DateTime.TryParseExact((qt.NgayHieuLucTu ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out hieuLucTu))
                return KetQuaThaoTac.Loi("Ngày hiệu lực từ không hợp lệ (yyyy-MM-dd).");

            try
            {
                if (qt.ID_QuyTac > 0)
                {
                    SYS_QuyTacQuyDoi e = db.SYS_QuyTacQuyDois.FirstOrDefault(x => x.ID_QuyTac == qt.ID_QuyTac);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy quy tắc.");
                    e.TenQuyTac = qt.TenQuyTac.Trim();
                    e.LoaiHinhGiangDay = qt.LoaiHinhGiangDay;
                    e.SiSoTu = qt.SiSoTu;
                    e.SiSoDen = qt.SiSoDen;
                    e.HeSoQuyDoi = (decimal)qt.HeSoQuyDoi;
                    e.NgayHieuLucTu = hieuLucTu;
                    db.SubmitChanges();
                }
                else
                {
                    SYS_QuyTacQuyDoi e = new SYS_QuyTacQuyDoi
                    {
                        TenQuyTac = qt.TenQuyTac.Trim(),
                        LoaiHinhGiangDay = qt.LoaiHinhGiangDay,
                        SiSoTu = qt.SiSoTu,
                        SiSoDen = qt.SiSoDen,
                        HeSoQuyDoi = (decimal)qt.HeSoQuyDoi,
                        NgayHieuLucTu = hieuLucTu
                    };
                    db.SYS_QuyTacQuyDois.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
                return KetQuaThaoTac.Ok("Đã lưu quy tắc quy đổi.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu quy tắc: " + ex.Message); }
        }

        public KetQuaThaoTac XoaQuyTacQuyDoi(int idQuyTac)
        {
            try
            {
                SYS_QuyTacQuyDoi e = db.SYS_QuyTacQuyDois.FirstOrDefault(x => x.ID_QuyTac == idQuyTac);
                if (e != null) { db.SYS_QuyTacQuyDois.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa quy tắc quy đổi.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa quy tắc: " + ex.Message); }
        }
        #endregion

        #region 3. LỚP HỌC PHẦN & NHÓM HỌC
        public List<Models.LopHocPhan> LayDanhSachLopHocPhan(int? idHocKy = null)
        {
            var q = (from lhp in db.LopHocPhans
                     join mh in db.DM_MonHocs on lhp.MaMonHoc equals mh.MaMonHoc
                     join hk in db.DM_HocKys on lhp.ID_HocKy equals hk.ID_HocKy
                     where !idHocKy.HasValue || lhp.ID_HocKy == idHocKy.Value
                     orderby lhp.MaLopHocPhan
                     select new { lhp, mh.TenMonHoc, hk.TenHK }).ToList();

            return q.Select(x => new Models.LopHocPhan
            {
                ID_LopHocPhan = x.lhp.ID_LopHocPhan,
                MaLopHocPhan = x.lhp.MaLopHocPhan,
                MaMonHoc = x.lhp.MaMonHoc,
                ID_HocKy = x.lhp.ID_HocKy,
                LoaiLopHP = x.lhp.LoaiLopHP,
                SiSoDangKy = x.lhp.SiSoDangKy,
                NgayBatDau = x.lhp.NgayBatDau.HasValue ? x.lhp.NgayBatDau.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                NgayKetThuc = x.lhp.NgayKetThuc.HasValue ? x.lhp.NgayKetThuc.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                TrangThai = x.lhp.TrangThai,
                TenMonHoc = x.TenMonHoc,
                TenHocKy = x.TenHK
            }).ToList();
        }

        public KetQuaThaoTac LuuLopHocPhan(Models.LopHocPhan lhp)
        {
            if (lhp == null || string.IsNullOrWhiteSpace(lhp.MaLopHocPhan) || string.IsNullOrWhiteSpace(lhp.MaMonHoc) || lhp.ID_HocKy <= 0)
                return KetQuaThaoTac.Loi("Mã lớp học phần, Môn học và Học kỳ là bắt buộc.");
            try
            {
                if (lhp.ID_LopHocPhan > 0)
                {
                    LopHocPhan e = db.LopHocPhans.FirstOrDefault(x => x.ID_LopHocPhan == lhp.ID_LopHocPhan);
                    if (e == null) return KetQuaThaoTac.Loi("Không tìm thấy lớp học phần.");
                    e.MaLopHocPhan = lhp.MaLopHocPhan.Trim();
                    e.MaMonHoc = lhp.MaMonHoc.Trim();
                    e.ID_HocKy = lhp.ID_HocKy;
                    e.LoaiLopHP = string.IsNullOrWhiteSpace(lhp.LoaiLopHP) ? "LT" : lhp.LoaiLopHP.Trim();
                    e.SiSoDangKy = lhp.SiSoDangKy;
                    e.TrangThai = string.IsNullOrWhiteSpace(lhp.TrangThai) ? "Mo" : lhp.TrangThai.Trim();
                }
                else
                {
                    db.LopHocPhans.InsertOnSubmit(new LopHocPhan
                    {
                        MaLopHocPhan = lhp.MaLopHocPhan.Trim(),
                        MaMonHoc = lhp.MaMonHoc.Trim(),
                        ID_HocKy = lhp.ID_HocKy,
                        LoaiLopHP = string.IsNullOrWhiteSpace(lhp.LoaiLopHP) ? "LT" : lhp.LoaiLopHP.Trim(),
                        SiSoDangKy = lhp.SiSoDangKy,
                        TrangThai = string.IsNullOrWhiteSpace(lhp.TrangThai) ? "Mo" : lhp.TrangThai.Trim()
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã lưu lớp học phần '" + lhp.MaLopHocPhan.Trim() + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu lớp học phần: " + ex.Message); }
        }

        public KetQuaThaoTac XoaLopHocPhan(int idLopHocPhan)
        {
            try
            {
                LopHocPhan e = db.LopHocPhans.FirstOrDefault(x => x.ID_LopHocPhan == idLopHocPhan);
                if (e != null) { db.LopHocPhans.DeleteOnSubmit(e); db.SubmitChanges(); }
                return KetQuaThaoTac.Ok("Đã xóa lớp học phần.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Không thể xóa lớp học phần (đã gắn vào nhóm học): " + ex.Message); }
        }

        public List<NhomHoc> LayDanhSachNhomHoc(int? idHocKy = null)
        {
            var list = (from nh in db.DM_NhomHocs
                        join mh in db.DM_MonHocs on nh.MaMonHoc equals mh.MaMonHoc
                        join hk in db.DM_HocKys on nh.ID_HocKy equals hk.ID_HocKy
                        where !idHocKy.HasValue || nh.ID_HocKy == idHocKy.Value
                        orderby nh.ID_NhomHoc descending
                        select new NhomHoc
                        {
                            ID_NhomHoc = nh.ID_NhomHoc,
                            MaNhomHoc = nh.MaNhomHoc,
                            TenNhomHoc = nh.TenNhomHoc,
                            MaMonHoc = nh.MaMonHoc,
                            ID_HocKy = nh.ID_HocKy,
                            LoaiNhom = nh.LoaiNhom,
                            TongSiSoNhom = nh.TongSiSoNhom,
                            TenMonHoc = mh.TenMonHoc,
                            TenHocKy = hk.TenHK
                        }).ToList();

            var lopMap = (from nl in db.R_NhomHoc_LopHPs
                          join lhp in db.LopHocPhans on nl.ID_LopHocPhan equals lhp.ID_LopHocPhan
                          orderby lhp.MaLopHocPhan
                          select new { nl.ID_NhomHoc, lhp.ID_LopHocPhan, lhp.MaLopHocPhan }).ToList()
                          .GroupBy(x => x.ID_NhomHoc)
                          .ToDictionary(g => g.Key, g => g.ToList());

            foreach (NhomHoc nh in list)
            {
                if (lopMap.ContainsKey(nh.ID_NhomHoc))
                {
                    var items = lopMap[nh.ID_NhomHoc];
                    nh.DanhSachID_LopHP = items.Select(i => i.ID_LopHocPhan).ToList();
                    nh.DanhSachMaLopHP = string.Join(", ", items.Select(i => i.MaLopHocPhan));
                }
            }
            return list;
        }

        public KetQuaThaoTac TaoNhomHoc(NhomHoc nhom, List<int> danhSachID_LopHP)
        {
            if (nhom == null || string.IsNullOrWhiteSpace(nhom.MaNhomHoc) || string.IsNullOrWhiteSpace(nhom.TenNhomHoc))
                return KetQuaThaoTac.Loi("Mã nhóm học và Tên nhóm học là bắt buộc.");
            List<int> dsLop = danhSachID_LopHP != null ? danhSachID_LopHP.Distinct().ToList() : new List<int>();
            if (dsLop.Count == 0) return KetQuaThaoTac.Loi("Vui lòng chọn ít nhất một Lớp học phần.");

            try
            {
                int tongSiSo = nhom.TongSiSoNhom > 0
                    ? nhom.TongSiSoNhom
                    : db.LopHocPhans.Where(l => dsLop.Contains(l.ID_LopHocPhan)).Sum(l => l.SiSoDangKy);

                DM_NhomHoc entity = new DM_NhomHoc
                {
                    MaNhomHoc = nhom.MaNhomHoc.Trim(),
                    TenNhomHoc = nhom.TenNhomHoc.Trim(),
                    MaMonHoc = nhom.MaMonHoc.Trim(),
                    ID_HocKy = nhom.ID_HocKy,
                    LoaiNhom = string.IsNullOrWhiteSpace(nhom.LoaiNhom) ? "LT" : nhom.LoaiNhom.Trim(),
                    TongSiSoNhom = tongSiSo
                };
                db.DM_NhomHocs.InsertOnSubmit(entity);
                db.SubmitChanges();

                foreach (int idLop in dsLop)
                {
                    db.R_NhomHoc_LopHPs.InsertOnSubmit(new R_NhomHoc_LopHP
                    {
                        ID_NhomHoc = entity.ID_NhomHoc,
                        ID_LopHocPhan = idLop
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã tạo Nhóm học '" + entity.MaNhomHoc + "' (Tổng sĩ số: " + tongSiSo + ").", null, entity.ID_NhomHoc);
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi tạo Nhóm học: " + ex.Message); }
        }

        public KetQuaThaoTac TachToThucHanh(int idLopHocPhan, int siSoMoiTo = 20)
        {
            if (idLopHocPhan <= 0) return KetQuaThaoTac.Loi("Vui lòng chọn Lớp học phần để tách tổ.");
            if (siSoMoiTo < 10) siSoMoiTo = 15;
            if (siSoMoiTo > 30) siSoMoiTo = 25;

            try
            {
                LopHocPhan lhp = db.LopHocPhans.FirstOrDefault(x => x.ID_LopHocPhan == idLopHocPhan);
                if (lhp == null) return KetQuaThaoTac.Loi("Không tìm thấy Lớp học phần.");
                DM_MonHoc mh = db.DM_MonHocs.FirstOrDefault(x => x.MaMonHoc == lhp.MaMonHoc);
                string tenMon = mh != null ? mh.TenMonHoc : lhp.MaMonHoc;

                int soTo = Math.Max(1, (int)Math.Ceiling((double)lhp.SiSoDangKy / siSoMoiTo));
                int phanDu = lhp.SiSoDangKy;
                for (int i = 1; i <= soTo; i++)
                {
                    int siSoTo = (i < soTo) ? Math.Min(siSoMoiTo, phanDu) : Math.Max(1, phanDu);
                    phanDu -= siSoTo;
                    TaoNhomHoc(new NhomHoc
                    {
                        MaNhomHoc = lhp.MaLopHocPhan + "_TH" + i.ToString("00"),
                        TenNhomHoc = tenMon + " - Tổ TH " + i + " (" + lhp.MaLopHocPhan + ")",
                        MaMonHoc = lhp.MaMonHoc,
                        ID_HocKy = lhp.ID_HocKy,
                        LoaiNhom = "TH",
                        TongSiSoNhom = siSoTo
                    }, new List<int> { idLopHocPhan });
                }
                return KetQuaThaoTac.Ok("Đã tách lớp '" + lhp.MaLopHocPhan + "' (" + lhp.SiSoDangKy + " SV) thành " + soTo + " tổ Thực hành.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi tách tổ TH: " + ex.Message); }
        }

        public KetQuaThaoTac XoaNhomHoc(int idNhomHoc)
        {
            try
            {
                var links = db.R_NhomHoc_LopHPs.Where(x => x.ID_NhomHoc == idNhomHoc).ToList();
                db.R_NhomHoc_LopHPs.DeleteAllOnSubmit(links);
                DM_NhomHoc nh = db.DM_NhomHocs.FirstOrDefault(x => x.ID_NhomHoc == idNhomHoc);
                if (nh != null) db.DM_NhomHocs.DeleteOnSubmit(nh);
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã xóa Nhóm học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Không thể xóa Nhóm học (có thể đã có buổi học xếp lịch): " + ex.Message); }
        }
        #endregion

        #region 4. XẾP LỊCH & KIỂM TRA XUNG ĐỘT
        public List<BuoiHocChiTiet> GetDanhSachBuoiHoc(int? idHocKy = null)
        {
            var rows = (from bh in db.DM_BuoiHocs
                        join nh in db.DM_NhomHocs on bh.ID_NhomHoc equals nh.ID_NhomHoc
                        join mh in db.DM_MonHocs on nh.MaMonHoc equals mh.MaMonHoc
                        join bai in db.DM_BaiHocs on bh.ID_BaiHoc equals bai.ID_BaiHoc into baij
                        from b in baij.DefaultIfEmpty()
                        join ph in db.DM_PhongHocs on bh.ID_PhongHoc equals ph.ID_PhongHoc into phj
                        from p in phj.DefaultIfEmpty()
                        where !idHocKy.HasValue || nh.ID_HocKy == idHocKy.Value
                        orderby bh.NgayHoc descending, bh.ID_BuoiHoc descending
                        select new
                        {
                            bh,
                            nh.MaNhomHoc,
                            nh.TenNhomHoc,
                            mh.TenMonHoc,
                            TenBaiHoc = b != null ? b.TenBaiHoc : null,
                            MaPhong = p != null ? p.MaPhong : null,
                            TenPhong = p != null ? p.TenPhong : null,
                            CoSoPhong = p != null ? p.CoSo : null
                        }).ToList();

            if (rows.Count == 0) return new List<BuoiHocChiTiet>();

            var lopTheoNhom = (from nl in db.R_NhomHoc_LopHPs
                               join lhp in db.LopHocPhans on nl.ID_LopHocPhan equals lhp.ID_LopHocPhan
                               orderby lhp.MaLopHocPhan
                               select new { nl.ID_NhomHoc, lhp.MaLopHocPhan }).ToList()
                               .GroupBy(x => x.ID_NhomHoc)
                               .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(i => i.MaLopHocPhan)));

            var tietTheoBuoi = (from bt in db.R_BuoiHoc_Tiets
                                join th in db.DM_TietHocs on bt.ID_TietHoc equals th.ID_TietHoc
                                orderby th.SoTiet
                                select new { bt.ID_BuoiHoc, bt.ID_TietHoc, th.SoTiet, th.GioBatDau, th.GioKetThuc }).ToList()
                                .GroupBy(x => x.ID_BuoiHoc)
                                .ToDictionary(g => g.Key, g => g.ToList());

            var gvTheoBuoi = (from pc in db.R_PhanCongGiangDays
                              join gv in db.DM_GiangViens on pc.ID_GV equals gv.ID_GV
                              select new { pc.ID_BuoiHoc, pc.ID_GV, gv.HoTen, pc.VaiTro, pc.SoTietGiangDay, pc.HeSoQuyDoi, pc.GioChuanQuyDoi }).ToList()
                              .GroupBy(x => x.ID_BuoiHoc)
                              .ToDictionary(g => g.Key, g => g.OrderBy(x => x.VaiTro == "Chinh" ? 0 : 1).ThenBy(x => x.HoTen).ToList());

            List<BuoiHocChiTiet> ketQua = new List<BuoiHocChiTiet>();
            foreach (var r in rows)
            {
                BuoiHocChiTiet dto = new BuoiHocChiTiet
                {
                    ID_BuoiHoc = r.bh.ID_BuoiHoc,
                    ID_NhomHoc = r.bh.ID_NhomHoc,
                    MaNhomHoc = r.MaNhomHoc,
                    TenNhomHoc = r.TenNhomHoc,
                    TenMonHoc = r.TenMonHoc,
                    NgayHoc = r.bh.NgayHoc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    BuoiTrongNgay = r.bh.BuoiTrongNgay,
                    SoTietBuoi = r.bh.SoTietBuoi,
                    LoaiHinhGiangDay = r.bh.LoaiHinhGiangDay,
                    DayBangNgoaiNgu = r.bh.DayBangNgoaiNgu ?? false,
                    TongSiSoBuoi = r.bh.TongSiSoBuoi,
                    TrangThai = r.bh.TrangThai ?? "Draft",
                    GhiChu = r.bh.GhiChu,
                    ID_BaiHoc = r.bh.ID_BaiHoc,
                    TenBaiHoc = r.TenBaiHoc,
                    ID_PhongHoc = r.bh.ID_PhongHoc,
                    MaPhong = r.MaPhong,
                    TenPhong = r.TenPhong,
                    CoSoPhong = r.CoSoPhong
                };

                string maLops;
                if (lopTheoNhom.TryGetValue(dto.ID_NhomHoc, out maLops)) dto.DanhSachMaLopHP = maLops;

                if (tietTheoBuoi.ContainsKey(dto.ID_BuoiHoc))
                {
                    var tiets = tietTheoBuoi[dto.ID_BuoiHoc];
                    dto.DanhSachID_Tiet = tiets.Select(t => t.ID_TietHoc).ToList();
                    dto.DanhSachSoTiet = tiets.Select(t => t.SoTiet).ToList();
                    dto.ChuoiTietHoc = DinhDangChuoiTiet(dto.DanhSachSoTiet);
                    int minT = dto.DanhSachSoTiet.Min();
                    int maxT = dto.DanhSachSoTiet.Max();
                    dto.ChuoiTietSo = minT == maxT ? minT.ToString() : (minT + " - " + maxT);
                    dto.GioBatDau = tiets[0].GioBatDau.ToString(@"hh\:mm");
                    dto.GioKetThuc = tiets[tiets.Count - 1].GioKetThuc.ToString(@"hh\:mm");
                }

                if (gvTheoBuoi.ContainsKey(dto.ID_BuoiHoc))
                {
                    var pcs = gvTheoBuoi[dto.ID_BuoiHoc];
                    dto.DanhSachGiangVien = string.Join(", ", pcs.Select(x => x.HoTen + " (" + ((double)x.SoTietGiangDay).ToString("0.##") + "t)"));
                    dto.DanhSachID_GV = pcs.Select(x => x.ID_GV).ToList();
                    dto.HeSoQuyDoi = (double)pcs[0].HeSoQuyDoi;
                    dto.GioChuanQuyDoi = Math.Round((double)pcs.Sum(x => x.GioChuanQuyDoi), 2);
                }
                ketQua.Add(dto);
            }
            return ketQua;
        }

        public BaiHoc LayBaiHocTiepTheoChuaXep(int idNhomHoc)
        {
            DM_NhomHoc nh = db.DM_NhomHocs.FirstOrDefault(x => x.ID_NhomHoc == idNhomHoc);
            if (nh == null) return null;

            List<int> daXepIds = db.DM_BuoiHocs
                .Where(b => b.ID_NhomHoc == idNhomHoc && b.ID_BaiHoc.HasValue && (b.TrangThai == null || b.TrangThai != "Huy"))
                .Select(b => b.ID_BaiHoc.Value).Distinct().ToList();

            var bai = (from bh in db.DM_BaiHocs
                       join gv in db.DM_GiangViens on bh.ID_GV_MacDinh equals gv.ID_GV into gvj
                       from g in gvj.DefaultIfEmpty()
                       where bh.MaMonHoc == nh.MaMonHoc && !daXepIds.Contains(bh.ID_BaiHoc)
                       orderby bh.ThuTuBai, bh.ID_BaiHoc
                       select new BaiHoc
                       {
                           ID_BaiHoc = bh.ID_BaiHoc,
                           MaMonHoc = bh.MaMonHoc,
                           ThuTuBai = bh.ThuTuBai,
                           TenBaiHoc = bh.TenBaiHoc,
                           LoaiBai = bh.LoaiBai,
                           SoTiet = bh.SoTiet,
                           ID_GV_MacDinh = bh.ID_GV_MacDinh,
                           TenGiangVienMacDinh = g != null ? g.HoTen : null
                       }).FirstOrDefault();
            return bai;
        }

        public KetQuaThaoTac ThemBuoiHoc(BuoiHoc model, List<int> danhSachID_Tiet, List<PhanCongGiangVienInput> danhSachPhanCong)
        {
            if (model == null || model.ID_NhomHoc <= 0)
                return KetQuaThaoTac.Loi("Vui lòng chọn Nhóm học để xếp lịch.");
            DateTime ngayHocDate;
            if (!DateTime.TryParseExact(model.NgayHoc != null ? model.NgayHoc.Trim() : "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayHocDate))
                return KetQuaThaoTac.Loi("Ngày học không hợp lệ (yyyy-MM-dd).");

            List<int> dsTiet = danhSachID_Tiet != null ? danhSachID_Tiet.Distinct().ToList() : new List<int>();
            List<PhanCongGiangVienInput> dsPC = danhSachPhanCong != null
                ? danhSachPhanCong.Where(x => x.ID_GV > 0).GroupBy(x => x.ID_GV).Select(g => g.First()).ToList()
                : new List<PhanCongGiangVienInput>();

            if (dsTiet.Count == 0) return KetQuaThaoTac.Loi("Vui lòng chọn ít nhất một Tiết học.");
            if (dsPC.Count == 0) return KetQuaThaoTac.Loi("Vui lòng chọn ít nhất một Giảng viên.");

            try
            {
                DM_NhomHoc nhom = db.DM_NhomHocs.FirstOrDefault(x => x.ID_NhomHoc == model.ID_NhomHoc);
                if (nhom == null) return KetQuaThaoTac.Loi("Không tìm thấy Nhóm học.");
                DM_MonHoc mon = db.DM_MonHocs.FirstOrDefault(x => x.MaMonHoc == nhom.MaMonHoc);
                List<DM_TietHoc> tiets = db.DM_TietHocs.Where(t => dsTiet.Contains(t.ID_TietHoc)).OrderBy(t => t.SoTiet).ToList();
                if (tiets.Count == 0) return KetQuaThaoTac.Loi("Không tìm thấy thông tin tiết học.");

                int soTietBuoi = model.SoTietBuoi > 0 ? model.SoTietBuoi : tiets.Count;
                string buoiTrongNgay = string.IsNullOrWhiteSpace(model.BuoiTrongNgay) ? tiets[0].BuoiTrongNgay : model.BuoiTrongNgay.Trim();
                int tongSiSo = model.TongSiSoBuoi > 0 ? model.TongSiSoBuoi : nhom.TongSiSoNhom;
                bool dayNgoaiNgu = model.DayBangNgoaiNgu || (mon != null && (mon.DayBangNgoaiNgu ?? false));
                List<int> dsGV = dsPC.Select(x => x.ID_GV).ToList();

                int? idPhong = (model.ID_PhongHoc.HasValue && model.ID_PhongHoc.Value > 0) ? model.ID_PhongHoc : null;
                int? idBai = (model.ID_BaiHoc.HasValue && model.ID_BaiHoc.Value > 0) ? model.ID_BaiHoc : null;

                string loi = KiemTraXungDot(0, ngayHocDate, buoiTrongNgay, dsTiet, dsGV, idPhong, tongSiSo);
                if (loi != null) return KetQuaThaoTac.Loi(loi);

                double heSo = TraHeSoQuyDoi(model.LoaiHinhGiangDay.Trim(), tongSiSo, ngayHocDate, dayNgoaiNgu, mon != null ? mon.ID_HeDaoTao : null);

                DM_BuoiHoc buoiEntity = new DM_BuoiHoc
                {
                    ID_NhomHoc = model.ID_NhomHoc,
                    ID_BaiHoc = idBai,
                    NgayHoc = ngayHocDate.Date,
                    BuoiTrongNgay = buoiTrongNgay,
                    ID_PhongHoc = idPhong,
                    SoTietBuoi = soTietBuoi,
                    LoaiHinhGiangDay = model.LoaiHinhGiangDay.Trim(),
                    DayBangNgoaiNgu = dayNgoaiNgu,
                    TongSiSoBuoi = tongSiSo,
                    TrangThai = string.IsNullOrWhiteSpace(model.TrangThai) ? "Draft" : model.TrangThai.Trim(),
                    ID_BuoiHocGoc = model.ID_BuoiHocGoc,
                    GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim()
                };
                db.DM_BuoiHocs.InsertOnSubmit(buoiEntity);
                db.SubmitChanges();

                foreach (int idTiet in dsTiet)
                {
                    db.R_BuoiHoc_Tiets.InsertOnSubmit(new R_BuoiHoc_Tiet
                    {
                        ID_BuoiHoc = buoiEntity.ID_BuoiHoc,
                        ID_TietHoc = idTiet
                    });
                }

                for (int i = 0; i < dsPC.Count; i++)
                {
                    PhanCongGiangVienInput pc = dsPC[i];
                    double soTietGV = pc.SoTietGiangDay > 0 ? pc.SoTietGiangDay : soTietBuoi;
                    double gioChuan = Math.Round(soTietGV * heSo, 2);
                    db.R_PhanCongGiangDays.InsertOnSubmit(new R_PhanCongGiangDay
                    {
                        ID_BuoiHoc = buoiEntity.ID_BuoiHoc,
                        ID_GV = pc.ID_GV,
                        VaiTro = string.IsNullOrWhiteSpace(pc.VaiTro) ? (i == 0 ? "Chinh" : "Phu") : pc.VaiTro,
                        SoTietGiangDay = (decimal)soTietGV,
                        HeSoQuyDoi = (decimal)heSo,
                        GioChuanQuyDoi = (decimal)gioChuan,
                        DaXacNhanHoanThanh = false
                    });
                }
                db.SubmitChanges();

                return KetQuaThaoTac.Ok("Xếp lịch thành công ngày " + ngayHocDate.ToString("yyyy-MM-dd") + " (" + DinhDangChuoiTiet(tiets.Select(t => t.SoTiet)) + ").", buoiEntity.ID_BuoiHoc);
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xếp lịch: " + ex.Message); }
        }

        public KetQuaXepLichLapLai XepLichNhanhLapLai(XepLichLapLaiRequest req)
        {
            KetQuaXepLichLapLai kq = new KetQuaXepLichLapLai();
            if (req == null || req.ID_NhomHoc <= 0) { kq.ThongBaoTongQuat = "Vui lòng chọn Nhóm học."; return kq; }

            DateTime ngayBD;
            if (!DateTime.TryParseExact(req.NgayBatDau != null ? req.NgayBatDau.Trim() : "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayBD))
            {
                kq.ThongBaoTongQuat = "Ngày bắt đầu không hợp lệ.";
                return kq;
            }

            List<DateTime> ngayMucTieu = new List<DateTime>();
            if (req.DanhSachNgayChon != null && req.DanhSachNgayChon.Count > 0)
            {
                foreach (string s in req.DanhSachNgayChon)
                {
                    DateTime d;
                    if (DateTime.TryParseExact(s != null ? s.Trim() : "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                        ngayMucTieu.Add(d.Date);
                }
                ngayMucTieu = ngayMucTieu.Distinct().OrderBy(d => d).ToList();
            }
            else
            {
                List<int> dsThu = req.DanhSachThuTrongTuan != null ? req.DanhSachThuTrongTuan.Where(t => t >= 2 && t <= 8).Distinct().OrderBy(t => t).ToList() : new List<int>();
                if (dsThu.Count == 0) dsThu.Add(ngayBD.DayOfWeek == DayOfWeek.Sunday ? 8 : (int)ngayBD.DayOfWeek + 1);
                int soTuan = Math.Max(1, Math.Min(20, req.SoTuanLap));
                int buocNhay = Math.Max(1, Math.Min(8, req.BuocNhayTuan));

                for (int k = 0; k < soTuan; k++)
                {
                    int baseOffset = k * buocNhay * 7;
                    for (int off = 0; off < 7; off++)
                    {
                        DateTime d = ngayBD.AddDays(baseOffset + off);
                        int thuVN = d.DayOfWeek == DayOfWeek.Sunday ? 8 : (int)d.DayOfWeek + 1;
                        if (dsThu.Contains(thuVN)) ngayMucTieu.Add(d);
                    }
                }
            }

            foreach (DateTime ngay in ngayMucTieu)
            {
                string ngayStr = ngay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                BaiHoc baiTiep = req.TuDongGanBaiHoc ? LayBaiHocTiepTheoChuaXep(req.ID_NhomHoc) : null;
                List<PhanCongGiangVienInput> dsPC = req.DanhSachPhanCong.Count > 0
                    ? req.DanhSachPhanCong
                    : (baiTiep != null && baiTiep.ID_GV_MacDinh.HasValue
                        ? new List<PhanCongGiangVienInput> { new PhanCongGiangVienInput { ID_GV = baiTiep.ID_GV_MacDinh.Value, VaiTro = "Chinh" } }
                        : new List<PhanCongGiangVienInput>());

                KetQuaThaoTac res = ThemBuoiHoc(new BuoiHoc
                {
                    ID_NhomHoc = req.ID_NhomHoc,
                    ID_BaiHoc = baiTiep != null ? (int?)baiTiep.ID_BaiHoc : null,
                    NgayHoc = ngayStr,
                    ID_PhongHoc = req.ID_PhongHoc,
                    LoaiHinhGiangDay = req.LoaiHinhGiangDay,
                    DayBangNgoaiNgu = req.DayBangNgoaiNgu,
                    TrangThai = "Draft",
                    GhiChu = req.GhiChu
                }, req.DanhSachID_Tiet, dsPC);

                if (res.ThanhCong) { kq.SoBuoiThanhCong++; kq.ChiTietThanhCong.Add(ngayStr + ": Hợp lệ"); }
                else { kq.SoBuoiBiTrung++; kq.ChiTietBiTrung.Add(ngayStr + ": " + res.ThongBao); }
            }

            if (req.TuDongGanBaiHoc && kq.SoBuoiThanhCong > 0)
                SapXepLaiThuTuBaiHocTheoThoiGian(req.ID_NhomHoc);

            kq.ThanhCong = kq.SoBuoiThanhCong > 0;
            kq.ThongBaoTongQuat = "Xếp lặp lại: " + kq.SoBuoiThanhCong + "/" + ngayMucTieu.Count + " buổi hợp lệ.";
            return kq;
        }

        public KetQuaThaoTac SapXepLaiThuTuBaiHocTheoThoiGian(int idNhomHoc)
        {
            try
            {
                DM_NhomHoc nh = db.DM_NhomHocs.FirstOrDefault(x => x.ID_NhomHoc == idNhomHoc);
                if (nh == null) return KetQuaThaoTac.Loi("Không tìm thấy nhóm học.");

                var tietMap = (from bt in db.R_BuoiHoc_Tiets
                               join th in db.DM_TietHocs on bt.ID_TietHoc equals th.ID_TietHoc
                               select new { bt.ID_BuoiHoc, th.SoTiet }).ToList()
                               .GroupBy(x => x.ID_BuoiHoc)
                               .ToDictionary(g => g.Key, g => g.Min(i => i.SoTiet));

                List<DM_BuoiHoc> buois = db.DM_BuoiHocs
                    .Where(b => b.ID_NhomHoc == idNhomHoc && (b.TrangThai == null || b.TrangThai != "Huy"))
                    .ToList()
                    .OrderBy(b => b.NgayHoc)
                    .ThenBy(b => tietMap.ContainsKey(b.ID_BuoiHoc) ? tietMap[b.ID_BuoiHoc] : 99)
                    .ThenBy(b => b.ID_BuoiHoc)
                    .ToList();

                List<int> dsIdBai = db.DM_BaiHocs
                    .Where(b => b.MaMonHoc == nh.MaMonHoc)
                    .OrderBy(b => b.ThuTuBai).ThenBy(b => b.ID_BaiHoc)
                    .Select(b => b.ID_BaiHoc).ToList();

                for (int i = 0; i < buois.Count; i++)
                    buois[i].ID_BaiHoc = i < dsIdBai.Count ? (int?)dsIdBai[i] : null;
                db.SubmitChanges();

                return KetQuaThaoTac.Ok("Đã sắp xếp lại thứ tự bài học theo dòng thời gian.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi sắp xếp bài học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaBuoiHoc(int idBuoiHoc)
        {
            try
            {
                foreach (DM_BuoiHoc child in db.DM_BuoiHocs.Where(b => b.ID_BuoiHocGoc == idBuoiHoc))
                    child.ID_BuoiHocGoc = null;
                db.R_PhanCongGiangDays.DeleteAllOnSubmit(db.R_PhanCongGiangDays.Where(p => p.ID_BuoiHoc == idBuoiHoc));
                db.R_BuoiHoc_Tiets.DeleteAllOnSubmit(db.R_BuoiHoc_Tiets.Where(t => t.ID_BuoiHoc == idBuoiHoc));
                DM_BuoiHoc target = db.DM_BuoiHocs.FirstOrDefault(b => b.ID_BuoiHoc == idBuoiHoc);
                if (target != null) db.DM_BuoiHocs.DeleteOnSubmit(target);
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã xóa buổi học #" + idBuoiHoc + ".", idBuoiHoc);
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa buổi học: " + ex.Message); }
        }

        public List<PhanCongGiangDay> LayPhanCongTheoBuoi(int idBuoiHoc)
        {
            return db.R_PhanCongGiangDays.Where(p => p.ID_BuoiHoc == idBuoiHoc).ToList()
                .OrderBy(p => p.VaiTro == "Chinh" ? 0 : 1).ThenBy(p => p.ID_PhanCong)
                .Select(p => new PhanCongGiangDay
                {
                    ID_PhanCong = p.ID_PhanCong,
                    ID_BuoiHoc = p.ID_BuoiHoc,
                    ID_GV = p.ID_GV,
                    VaiTro = p.VaiTro ?? "Chinh",
                    SoTietGiangDay = (double)p.SoTietGiangDay,
                    HeSoQuyDoi = (double)p.HeSoQuyDoi,
                    GioChuanQuyDoi = (double)p.GioChuanQuyDoi,
                    DaXacNhanHoanThanh = p.DaXacNhanHoanThanh ?? false
                }).ToList();
        }

        public KetQuaThaoTac LuuPhanCongChoBuoi(int idBuoiHoc, List<PhanCongGiangVienInput> danhSachPhanCong)
        {
            if (idBuoiHoc <= 0) return KetQuaThaoTac.Loi("Vui lòng chọn buổi học.");
            List<PhanCongGiangVienInput> dsPC = danhSachPhanCong != null
                ? danhSachPhanCong.Where(x => x.ID_GV > 0).GroupBy(x => x.ID_GV).Select(g => g.First()).ToList()
                : new List<PhanCongGiangVienInput>();
            if (dsPC.Count == 0) return KetQuaThaoTac.Loi("Phải có ít nhất 1 giảng viên được phân công.");

            try
            {
                DM_BuoiHoc bh = db.DM_BuoiHocs.FirstOrDefault(b => b.ID_BuoiHoc == idBuoiHoc);
                if (bh == null) return KetQuaThaoTac.Loi("Không tìm thấy buổi học #" + idBuoiHoc + ".");
                DM_NhomHoc nh = db.DM_NhomHocs.FirstOrDefault(n => n.ID_NhomHoc == bh.ID_NhomHoc);
                DM_MonHoc mh = nh != null ? db.DM_MonHocs.FirstOrDefault(m => m.MaMonHoc == nh.MaMonHoc) : null;

                double heSo = TraHeSoQuyDoi(bh.LoaiHinhGiangDay, bh.TongSiSoBuoi, bh.NgayHoc, bh.DayBangNgoaiNgu ?? false, mh != null ? mh.ID_HeDaoTao : null);

                db.R_PhanCongGiangDays.DeleteAllOnSubmit(db.R_PhanCongGiangDays.Where(p => p.ID_BuoiHoc == idBuoiHoc));
                for (int i = 0; i < dsPC.Count; i++)
                {
                    PhanCongGiangVienInput pc = dsPC[i];
                    double st = pc.SoTietGiangDay > 0 ? pc.SoTietGiangDay : bh.SoTietBuoi;
                    double gc = Math.Round(st * heSo, 2);
                    db.R_PhanCongGiangDays.InsertOnSubmit(new R_PhanCongGiangDay
                    {
                        ID_BuoiHoc = idBuoiHoc,
                        ID_GV = pc.ID_GV,
                        VaiTro = string.IsNullOrWhiteSpace(pc.VaiTro) ? (i == 0 ? "Chinh" : "Phu") : pc.VaiTro,
                        SoTietGiangDay = (decimal)st,
                        HeSoQuyDoi = (decimal)heSo,
                        GioChuanQuyDoi = (decimal)gc,
                        DaXacNhanHoanThanh = false
                    });
                }
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã cập nhật phân công giảng dạy cho buổi #" + idBuoiHoc + ".", idBuoiHoc);
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu phân công: " + ex.Message); }
        }

        public List<XungDotLichItem> QuetToanBoXungDot(int? idHocKy = null)
        {
            List<XungDotLichItem> ketQua = new List<XungDotLichItem>();
            List<BuoiHocChiTiet> dsBuoi = GetDanhSachBuoiHoc(idHocKy)
                .Where(b => !string.Equals(b.TrangThai, "Huy", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (dsBuoi.Count == 0) return ketQua;

            for (int i = 0; i < dsBuoi.Count; i++)
            {
                BuoiHocChiTiet a = dsBuoi[i];
                for (int j = i + 1; j < dsBuoi.Count; j++)
                {
                    BuoiHocChiTiet b = dsBuoi[j];
                    if (a.NgayHoc != b.NgayHoc) continue;
                    List<int> tietTrung = a.DanhSachSoTiet.Intersect(b.DanhSachSoTiet).ToList();
                    if (tietTrung.Count == 0) continue;

                    string ngayTiet = a.NgayHoc + " (" + DinhDangChuoiTiet(tietTrung) + ")";
                    if (a.DanhSachID_GV.Intersect(b.DanhSachID_GV).Any())
                    {
                        ketQua.Add(new XungDotLichItem
                        {
                            MucDo = "Chan",
                            LoaiXungDot = "Trùng giảng viên",
                            ID_BuoiHoc = a.ID_BuoiHoc,
                            NgayTiet = ngayTiet,
                            DoiTuong1 = a.MaNhomHoc + " - " + a.TenMonHoc,
                            DoiTuong2 = b.MaNhomHoc + " - " + b.TenMonHoc,
                            MoTa = "Cùng giảng viên dạy 2 buổi giao tiết (" + a.DanhSachGiangVien + ")."
                        });
                    }
                    if (a.ID_PhongHoc.HasValue && b.ID_PhongHoc.HasValue && a.ID_PhongHoc.Value == b.ID_PhongHoc.Value)
                    {
                        ketQua.Add(new XungDotLichItem
                        {
                            MucDo = "Chan",
                            LoaiXungDot = "Trùng phòng học",
                            ID_BuoiHoc = a.ID_BuoiHoc,
                            NgayTiet = ngayTiet,
                            DoiTuong1 = a.MaPhong + " (" + a.MaNhomHoc + ")",
                            DoiTuong2 = b.MaPhong + " (" + b.MaNhomHoc + ")",
                            MoTa = "Phòng " + a.MaPhong + " bị xếp trùng lịch 2 nhóm học."
                        });
                    }
                    if (a.ID_NhomHoc == b.ID_NhomHoc)
                    {
                        ketQua.Add(new XungDotLichItem
                        {
                            MucDo = "Chan",
                            LoaiXungDot = "Trùng nhóm học",
                            ID_BuoiHoc = a.ID_BuoiHoc,
                            NgayTiet = ngayTiet,
                            DoiTuong1 = a.MaNhomHoc + " (#" + a.ID_BuoiHoc + ")",
                            DoiTuong2 = b.MaNhomHoc + " (#" + b.ID_BuoiHoc + ")",
                            MoTa = "Nhóm " + a.MaNhomHoc + " có 2 buổi học trùng thời gian."
                        });
                    }
                }
            }

            Dictionary<int, int> sucChuaMap = db.DM_PhongHocs.ToDictionary(p => p.ID_PhongHoc, p => p.SucChua ?? 0);
            List<R_GV_RangBuoc> rangBuocs = db.R_GV_RangBuocs.ToList();

            foreach (BuoiHocChiTiet b in dsBuoi)
            {
                string ngayTiet = b.NgayHoc + " (" + (b.ChuoiTietHoc ?? "") + ")";
                if (!b.ID_PhongHoc.HasValue)
                {
                    ketQua.Add(new XungDotLichItem
                    {
                        MucDo = "CanhBao",
                        LoaiXungDot = "Chưa gán phòng",
                        ID_BuoiHoc = b.ID_BuoiHoc,
                        NgayTiet = ngayTiet,
                        DoiTuong1 = b.MaNhomHoc + " - " + b.TenMonHoc,
                        DoiTuong2 = "-",
                        MoTa = "Buổi học chưa chọn phòng học / phòng Lab."
                    });
                }
                else
                {
                    int sucChua;
                    if (sucChuaMap.TryGetValue(b.ID_PhongHoc.Value, out sucChua) && sucChua > 0 && b.TongSiSoBuoi > sucChua)
                    {
                        ketQua.Add(new XungDotLichItem
                        {
                            MucDo = "Chan",
                            LoaiXungDot = "Vượt sức chứa",
                            ID_BuoiHoc = b.ID_BuoiHoc,
                            NgayTiet = ngayTiet,
                            DoiTuong1 = b.MaPhong + " (" + sucChua + " chỗ)",
                            DoiTuong2 = b.MaNhomHoc + " (" + b.TongSiSoBuoi + " SV)",
                            MoTa = "Sĩ số buổi học (" + b.TongSiSoBuoi + ") vượt quá sức chứa phòng " + b.MaPhong + " (" + sucChua + ")."
                        });
                    }
                }

                if (b.DanhSachID_GV.Count == 0)
                {
                    ketQua.Add(new XungDotLichItem
                    {
                        MucDo = "CanhBao",
                        LoaiXungDot = "Chưa phân công GV",
                        ID_BuoiHoc = b.ID_BuoiHoc,
                        NgayTiet = ngayTiet,
                        DoiTuong1 = b.MaNhomHoc + " - " + b.TenMonHoc,
                        DoiTuong2 = "-",
                        MoTa = "Buổi học chưa có giảng viên giảng dạy."
                    });
                }

                DateTime ngayDt;
                if (DateTime.TryParseExact(b.NgayHoc, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayDt))
                {
                    int thuVN = ngayDt.DayOfWeek == DayOfWeek.Sunday ? 8 : (int)ngayDt.DayOfWeek + 1;
                    foreach (int idGv in b.DanhSachID_GV)
                    {
                        foreach (R_GV_RangBuoc rb in rangBuocs)
                        {
                            if (rb.ID_GV != idGv) continue;
                            if (ngayDt.Date < rb.NgayBD.Date || ngayDt.Date > rb.NgayKT.Date) continue;
                            if (rb.ThuTrongTuan.HasValue && rb.ThuTrongTuan.Value != thuVN) continue;
                            if (!string.IsNullOrWhiteSpace(rb.BuoiTrongNgay) && !string.Equals(rb.BuoiTrongNgay, b.BuoiTrongNgay, StringComparison.OrdinalIgnoreCase)) continue;

                            ketQua.Add(new XungDotLichItem
                            {
                                MucDo = "CanhBao",
                                LoaiXungDot = "GV có lịch bận",
                                ID_BuoiHoc = b.ID_BuoiHoc,
                                NgayTiet = ngayTiet,
                                DoiTuong1 = b.DanhSachGiangVien,
                                DoiTuong2 = b.MaNhomHoc,
                                MoTa = "Trùng ràng buộc bận của giảng viên: " + (rb.LyDo ?? "Bận")
                            });
                        }
                    }
                }
            }

            return ketQua.OrderBy(x => x.MucDo == "Chan" ? 0 : 1).ThenBy(x => x.NgayTiet).ToList();
        }

        public KetQuaThaoTac XuatBanLich(int? idHocKy = null)
        {
            List<XungDotLichItem> xungDots = QuetToanBoXungDot(idHocKy);
            int soLoiChan = xungDots.Count(x => x.MucDo == "Chan");
            if (soLoiChan > 0)
                return KetQuaThaoTac.Loi("Không thể xuất bản vì còn " + soLoiChan + " lỗi mức Chặn.");

            try
            {
                var buois = (from bh in db.DM_BuoiHocs
                             join nh in db.DM_NhomHocs on bh.ID_NhomHoc equals nh.ID_NhomHoc
                             where (bh.TrangThai == null || bh.TrangThai == "Draft")
                                && (!idHocKy.HasValue || nh.ID_HocKy == idHocKy.Value)
                             select bh).ToList();

                foreach (DM_BuoiHoc bh in buois) bh.TrangThai = "Published";
                db.SubmitChanges();
                return KetQuaThaoTac.Ok("Đã xuất bản lịch giảng dạy thành công (" + buois.Count + " buổi chuyển sang Published).");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xuất bản lịch: " + ex.Message); }
        }

        private string KiemTraXungDot(int idBuoiBoQua, DateTime ngayDt, string buoiTrongNgay, List<int> dsTiet, List<int> dsGV, int? idPhong, int tongSiSo)
        {
            var trungGv = (from bh in db.DM_BuoiHocs
                           join bt in db.R_BuoiHoc_Tiets on bh.ID_BuoiHoc equals bt.ID_BuoiHoc
                           join th in db.DM_TietHocs on bt.ID_TietHoc equals th.ID_TietHoc
                           join pc in db.R_PhanCongGiangDays on bh.ID_BuoiHoc equals pc.ID_BuoiHoc
                           join gv in db.DM_GiangViens on pc.ID_GV equals gv.ID_GV
                           where bh.ID_BuoiHoc != idBuoiBoQua && bh.NgayHoc == ngayDt.Date
                              && (bh.TrangThai == null || bh.TrangThai != "Huy")
                              && dsTiet.Contains(bt.ID_TietHoc) && dsGV.Contains(pc.ID_GV)
                           select new { gv.HoTen, th.SoTiet }).FirstOrDefault();
            if (trungGv != null)
                return "Trùng lịch giảng viên " + trungGv.HoTen + " tại Tiết " + trungGv.SoTiet + " ngày " + ngayDt.ToString("yyyy-MM-dd") + ".";

            if (idPhong.HasValue)
            {
                var trungPhong = (from bh in db.DM_BuoiHocs
                                  join bt in db.R_BuoiHoc_Tiets on bh.ID_BuoiHoc equals bt.ID_BuoiHoc
                                  join th in db.DM_TietHocs on bt.ID_TietHoc equals th.ID_TietHoc
                                  join ph in db.DM_PhongHocs on bh.ID_PhongHoc equals ph.ID_PhongHoc
                                  where bh.ID_BuoiHoc != idBuoiBoQua && bh.NgayHoc == ngayDt.Date
                                     && (bh.TrangThai == null || bh.TrangThai != "Huy")
                                     && bh.ID_PhongHoc == idPhong.Value && dsTiet.Contains(bt.ID_TietHoc)
                                  select new { ph.MaPhong, th.SoTiet }).FirstOrDefault();
                if (trungPhong != null)
                    return "Trùng phòng " + trungPhong.MaPhong + " tại Tiết " + trungPhong.SoTiet + " ngày " + ngayDt.ToString("yyyy-MM-dd") + ".";

                DM_PhongHoc phong = db.DM_PhongHocs.FirstOrDefault(p => p.ID_PhongHoc == idPhong.Value);
                if (phong != null && phong.SucChua.HasValue && tongSiSo > phong.SucChua.Value)
                    return "Vượt sức chứa phòng " + phong.MaPhong + " (" + phong.SucChua.Value + " chỗ < " + tongSiSo + " SV).";
            }

            int thuVN = ngayDt.DayOfWeek == DayOfWeek.Sunday ? 8 : (int)ngayDt.DayOfWeek + 1;
            var rbList = (from rb in db.R_GV_RangBuocs
                          join gv in db.DM_GiangViens on rb.ID_GV equals gv.ID_GV
                          where dsGV.Contains(rb.ID_GV) && ngayDt.Date >= rb.NgayBD && ngayDt.Date <= rb.NgayKT
                          select new { gv.HoTen, rb.ThuTrongTuan, rb.BuoiTrongNgay, rb.LyDo }).ToList();
            foreach (var r in rbList)
            {
                if (r.ThuTrongTuan.HasValue && r.ThuTrongTuan.Value != thuVN) continue;
                if (!string.IsNullOrWhiteSpace(r.BuoiTrongNgay) && !string.Equals(r.BuoiTrongNgay, buoiTrongNgay, StringComparison.OrdinalIgnoreCase)) continue;
                return "Giảng viên " + r.HoTen + " có lịch bận ngày " + ngayDt.ToString("yyyy-MM-dd") + ": " + r.LyDo;
            }
            return null;
        }

        public static string DinhDangChuoiTiet(IEnumerable<int> danhSachSoTiet)
        {
            List<int> sorted = danhSachSoTiet.Distinct().OrderBy(t => t).ToList();
            if (sorted.Count == 0) return string.Empty;
            List<string> nhom = new List<string>();
            int start = sorted[0], prev = sorted[0];
            for (int i = 1; i < sorted.Count; i++)
            {
                if (sorted[i] == prev + 1) prev = sorted[i];
                else
                {
                    nhom.Add(start == prev ? start.ToString() : (start + "-" + prev));
                    start = prev = sorted[i];
                }
            }
            nhom.Add(start == prev ? start.ToString() : (start + "-" + prev));
            return "Tiết " + string.Join(", ", nhom);
        }
        #endregion

        #region 5. TÍNH GIỜ CHUẨN ĐIỀU 12 & BÁO CÁO
        public double TraHeSoQuyDoi(string loaiHinhGiangDay, int siSo, DateTime ngayHoc, bool dayBangNgoaiNgu, int? idHeDaoTao = null)
        {
            SYS_QuyTacQuyDoi rule = db.SYS_QuyTacQuyDois
                .Where(q => q.LoaiHinhGiangDay == loaiHinhGiangDay
                         && siSo >= q.SiSoTu && siSo <= q.SiSoDen
                         && q.NgayHieuLucTu <= ngayHoc.Date
                         && (q.NgayHieuLucDen == null || q.NgayHieuLucDen >= ngayHoc.Date)
                         && (q.ID_HeDaoTao == null || q.ID_HeDaoTao == idHeDaoTao))
                .ToList()
                .OrderBy(q => q.ID_HeDaoTao == idHeDaoTao ? 0 : 1)
                .ThenByDescending(q => q.NgayHieuLucTu)
                .ThenByDescending(q => q.ID_QuyTac)
                .FirstOrDefault();

            double heSo = rule != null ? (double)rule.HeSoQuyDoi : 1.0;
            if (dayBangNgoaiNgu) heSo *= HeSoNgoaiNgu;
            return Math.Round(heSo, 4);
        }

        public List<BaoCaoGioGiangDto> GetTongHopGioGiang(int? idHocKy = null, int? idNamHoc = null)
        {
            int? namHocHieuLuc = idNamHoc;
            if (!namHocHieuLuc.HasValue && idHocKy.HasValue)
            {
                DM_HocKy hk = db.DM_HocKys.FirstOrDefault(x => x.ID_HocKy == idHocKy.Value);
                if (hk != null) namHocHieuLuc = hk.ID_NamHoc;
            }

            var dsGV = (from gv in db.DM_GiangViens
                        join bm in db.DM_DanhMucs on gv.ID_BoMon equals bm.MaDanhMuc into bmj
                        from b in bmj.DefaultIfEmpty()
                        where gv.TrangThai == null || gv.TrangThai == true
                        orderby gv.HoTen
                        select new
                        {
                            gv.ID_GV,
                            gv.MaGV,
                            gv.HoTen,
                            TenBoMon = b != null ? b.TenDanhMuc : "-",
                            DinhMuc = gv.DinhMucGioChuan.HasValue ? (double)gv.DinhMucGioChuan.Value : 270.0
                        }).ToList();

            var dungLopMap = (from pc in db.R_PhanCongGiangDays
                              join bh in db.DM_BuoiHocs on pc.ID_BuoiHoc equals bh.ID_BuoiHoc
                              join nh in db.DM_NhomHocs on bh.ID_NhomHoc equals nh.ID_NhomHoc
                              join hk in db.DM_HocKys on nh.ID_HocKy equals hk.ID_HocKy
                              where (bh.TrangThai == null || bh.TrangThai != "Huy")
                                 && (!idHocKy.HasValue || nh.ID_HocKy == idHocKy.Value)
                                 && (!namHocHieuLuc.HasValue || hk.ID_NamHoc == namHocHieuLuc.Value)
                              select new { pc.ID_GV, pc.SoTietGiangDay, pc.GioChuanQuyDoi }).ToList()
                              .GroupBy(x => x.ID_GV)
                              .ToDictionary(g => g.Key, g => Tuple.Create((double)g.Sum(i => i.SoTietGiangDay), (double)g.Sum(i => i.GioChuanQuyDoi)));

            var deTaiMap = (from pc in db.R_PhanCong_DeTais
                            join dt in db.HD_DeTai_LuanVans on pc.ID_DeTai equals dt.ID_DeTai
                            where !namHocHieuLuc.HasValue || dt.ID_NamHoc == namHocHieuLuc.Value
                            select new { pc.ID_GV, pc.GioChuanQuyDoi }).ToList()
                            .GroupBy(x => x.ID_GV)
                            .ToDictionary(g => g.Key, g => (double)g.Sum(i => i.GioChuanQuyDoi));

            var khaoThiMap = (from kt in db.KT_CongTacKhaoThis
                              join hk in db.DM_HocKys on kt.ID_HocKy equals hk.ID_HocKy
                              where (!idHocKy.HasValue || kt.ID_HocKy == idHocKy.Value)
                                 && (!namHocHieuLuc.HasValue || hk.ID_NamHoc == namHocHieuLuc.Value)
                              select new { kt.ID_GV, kt.GioChuanQuyDoi }).ToList()
                              .GroupBy(x => x.ID_GV)
                              .ToDictionary(g => g.Key, g => (double)g.Sum(i => i.GioChuanQuyDoi));

            var nckhMap = (from pc in db.R_PhanCong_NCKHs
                           join hd in db.NCKH_HoatDongs on pc.ID_HoatDong equals hd.ID_HoatDong
                           where !namHocHieuLuc.HasValue || hd.ID_NamHoc == namHocHieuLuc.Value
                           select new { pc.ID_GV, pc.GioChuanQuyDoi }).ToList()
                           .GroupBy(x => x.ID_GV)
                           .ToDictionary(g => g.Key, g => (double)g.Sum(i => i.GioChuanQuyDoi));

            var mienGiamMap = db.R_GV_MienGiamDinhMucs
                .Where(m => !namHocHieuLuc.HasValue || m.ID_NamHoc == namHocHieuLuc.Value)
                .ToList()
                .GroupBy(m => m.ID_GV)
                .ToDictionary(g => g.Key, g => Tuple.Create(
                    (double)g.Sum(i => i.TyLeGiamPhanTram ?? 0m),
                    (double)g.Sum(i => i.SoGioGiam ?? 0m)));

            List<BaoCaoGioGiangDto> ketQua = new List<BaoCaoGioGiangDto>();
            foreach (var gv in dsGV)
            {
                Tuple<double, double> dl;
                dungLopMap.TryGetValue(gv.ID_GV, out dl);
                double gioDL = dl != null ? Math.Round(dl.Item2, 2) : 0;
                int soTiet = dl != null ? (int)Math.Round(dl.Item1) : 0;

                double gioDT; deTaiMap.TryGetValue(gv.ID_GV, out gioDT); gioDT = Math.Round(gioDT, 2);
                double gioKT; khaoThiMap.TryGetValue(gv.ID_GV, out gioKT); gioKT = Math.Round(gioKT, 2);
                double gioNC; nckhMap.TryGetValue(gv.ID_GV, out gioNC); gioNC = Math.Round(gioNC, 2);

                Tuple<double, double> mg;
                mienGiamMap.TryGetValue(gv.ID_GV, out mg);
                double gioMG = mg != null ? Math.Round((gv.DinhMuc * mg.Item1 / 100.0) + mg.Item2, 2) : 0;
                gioMG = Math.Min(gv.DinhMuc, Math.Max(0, gioMG));

                double dinhMucTT = Math.Max(0, Math.Round(gv.DinhMuc - gioMG, 2));
                double tongGio = Math.Round(gioDL + gioDT + gioKT + gioNC, 2);
                double gioVuot = Math.Max(0, Math.Round(tongGio - dinhMucTT, 2));

                ketQua.Add(new BaoCaoGioGiangDto
                {
                    MaGV = gv.MaGV,
                    HoTen = gv.HoTen,
                    TenBoMon = gv.TenBoMon,
                    DinhMucGioChuan = gv.DinhMuc,
                    GioMienGiam = gioMG,
                    DinhMucThucTe = dinhMucTT,
                    TongSoTietThucTe = soTiet,
                    GioDungLop = gioDL,
                    GioDeTaiHoiDong = gioDT,
                    GioKhaoThi = gioKT,
                    GioNCKH = gioNC,
                    TongGioChuanQuyDoi = tongGio,
                    GioVuotDinhMuc = gioVuot
                });
            }
            return ketQua;
        }

        public List<ChiTietBuoiDungLopDto> GetChiTietBuoiDungLopTheoMaGV(string maGV, int? idHocKy = null, int? idNamHoc = null)
        {
            if (string.IsNullOrWhiteSpace(maGV)) return new List<ChiTietBuoiDungLopDto>();
            int? namHocHieuLuc = idNamHoc;
            if (!namHocHieuLuc.HasValue && idHocKy.HasValue)
            {
                DM_HocKy hk = db.DM_HocKys.FirstOrDefault(x => x.ID_HocKy == idHocKy.Value);
                if (hk != null) namHocHieuLuc = hk.ID_NamHoc;
            }

            string ma = maGV.Trim();
            var rows = (from pc in db.R_PhanCongGiangDays
                        join gv in db.DM_GiangViens on pc.ID_GV equals gv.ID_GV
                        join bh in db.DM_BuoiHocs on pc.ID_BuoiHoc equals bh.ID_BuoiHoc
                        join nh in db.DM_NhomHocs on bh.ID_NhomHoc equals nh.ID_NhomHoc
                        join hk in db.DM_HocKys on nh.ID_HocKy equals hk.ID_HocKy
                        join mh in db.DM_MonHocs on nh.MaMonHoc equals mh.MaMonHoc
                        join ph in db.DM_PhongHocs on bh.ID_PhongHoc equals ph.ID_PhongHoc into phj
                        from p in phj.DefaultIfEmpty()
                        where gv.MaGV == ma
                           && (bh.TrangThai == null || bh.TrangThai != "Huy")
                           && (!idHocKy.HasValue || nh.ID_HocKy == idHocKy.Value)
                           && (!namHocHieuLuc.HasValue || hk.ID_NamHoc == namHocHieuLuc.Value)
                        orderby bh.NgayHoc, bh.ID_BuoiHoc
                        select new
                        {
                            bh.ID_BuoiHoc,
                            bh.NgayHoc,
                            mh.TenMonHoc,
                            nh.MaNhomHoc,
                            MaPhong = p != null ? p.MaPhong : "-",
                            bh.LoaiHinhGiangDay,
                            bh.DayBangNgoaiNgu,
                            bh.TongSiSoBuoi,
                            pc.SoTietGiangDay,
                            pc.HeSoQuyDoi,
                            pc.GioChuanQuyDoi
                        }).ToList();

            var tietMap = (from bt in db.R_BuoiHoc_Tiets
                           join th in db.DM_TietHocs on bt.ID_TietHoc equals th.ID_TietHoc
                           orderby th.SoTiet
                           select new { bt.ID_BuoiHoc, th.SoTiet }).ToList()
                           .GroupBy(x => x.ID_BuoiHoc)
                           .ToDictionary(g => g.Key, g => DinhDangChuoiTiet(g.Select(i => i.SoTiet)));

            return rows.Select(x => new ChiTietBuoiDungLopDto
            {
                ID_BuoiHoc = x.ID_BuoiHoc,
                NgayHoc = x.NgayHoc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ChuoiTiet = tietMap.ContainsKey(x.ID_BuoiHoc) ? tietMap[x.ID_BuoiHoc] : "",
                TenMonHoc = x.TenMonHoc,
                MaNhomHoc = x.MaNhomHoc,
                MaPhong = x.MaPhong,
                LoaiHinhGiangDay = x.LoaiHinhGiangDay,
                DayBangNgoaiNgu = x.DayBangNgoaiNgu ?? false,
                TongSiSoBuoi = x.TongSiSoBuoi,
                SoTietGiangDay = (double)x.SoTietGiangDay,
                HeSoQuyDoi = (double)x.HeSoQuyDoi,
                GioChuanQuyDoi = (double)x.GioChuanQuyDoi
            }).ToList();
        }
        #endregion
    }
}
