using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PM_Tinhgiogiang_Lib.DataAccess;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib.Services
{
    /// <summary>
    /// Dịch vụ tra cứu quy tắc quy đổi (SYS_QuyTacQuyDoi), tổng hợp giờ chuẩn năm (4 nguồn + miễn giảm) trên SQL Server.
    /// </summary>
    public class TinhGioService
    {
        public const double HeSoNgoaiNgu = 1.5;

        private readonly DatabaseHelper _dbHelper;

        public TinhGioService(DatabaseHelper dbHelper = null)
        {
            _dbHelper = dbHelper ?? new DatabaseHelper();
        }

        public static double TraHeSoQuyDoi(
            SqlConnection conn,
            string loaiHinhGiangDay,
            int siSo,
            string ngayHoc,
            bool dayBangNgoaiNgu,
            int? idHeDaoTao = null)
        {
            const string sql = @"
                SELECT TOP 1 HeSoQuyDoi
                FROM SYS_QuyTacQuyDoi
                WHERE LoaiHinhGiangDay = @LoaiHinhGiangDay
                  AND @SiSo BETWEEN SiSoTu AND SiSoDen
                  AND NgayHieuLucTu <= @NgayHoc
                  AND (NgayHieuLucDen IS NULL OR NgayHieuLucDen >= @NgayHoc)
                  AND (ID_HeDaoTao IS NULL OR ID_HeDaoTao = @ID_HeDaoTao)
                ORDER BY CASE WHEN ID_HeDaoTao = @ID_HeDaoTao THEN 0 ELSE 1 END,
                         NgayHieuLucTu DESC,
                         ID_QuyTac DESC;";

            double heSo = 1.0;
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@LoaiHinhGiangDay", loaiHinhGiangDay);
                cmd.AddParam("@SiSo", siSo);
                cmd.AddParam("@NgayHoc", ngayHoc);
                cmd.AddParam("@ID_HeDaoTao", idHeDaoTao);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read() && !r.IsDBNull(0))
                        heSo = r.GetDoubleVal(0);
                }
            }

            if (dayBangNgoaiNgu) heSo *= HeSoNgoaiNgu;
            return Math.Round(heSo, 4);
        }

        public List<BaoCaoGioGiangDto> GetTongHopGioGiang(int? idHocKy = null, int? idNamHoc = null)
        {
            using (SqlConnection conn = _dbHelper.OpenConnection())
            {
                int? namHocHieuLuc = idNamHoc ?? LayNamHocTuHocKy(conn, idHocKy);
                List<Tuple<int, string, string, string, double>> dsGV = LayDanhSachGiangVienCoBan(conn);
                Dictionary<int, Tuple<double, double>> dungLop = LayGioDungLopTheoGV(conn, idHocKy, namHocHieuLuc);
                Dictionary<int, double> deTai = LayGioDeTaiTheoGV(conn, namHocHieuLuc);
                Dictionary<int, double> khaoThi = LayGioKhaoThiTheoGV(conn, idHocKy, namHocHieuLuc);
                Dictionary<int, double> nckh = LayGioNCKHTheoGV(conn, namHocHieuLuc);
                Dictionary<int, Tuple<double, double>> mienGiam = LayMienGiamTheoGV(conn, namHocHieuLuc);

                List<BaoCaoGioGiangDto> ketQua = new List<BaoCaoGioGiangDto>();
                foreach (Tuple<int, string, string, string, double> gv in dsGV)
                {
                    Tuple<double, double> dl;
                    dungLop.TryGetValue(gv.Item1, out dl);
                    double gioDL = dl != null ? Math.Round(dl.Item2, 2) : 0;
                    int soTiet = dl != null ? (int)Math.Round(dl.Item1) : 0;

                    double gioDT; deTai.TryGetValue(gv.Item1, out gioDT); gioDT = Math.Round(gioDT, 2);
                    double gioKT; khaoThi.TryGetValue(gv.Item1, out gioKT); gioKT = Math.Round(gioKT, 2);
                    double gioNC; nckh.TryGetValue(gv.Item1, out gioNC); gioNC = Math.Round(gioNC, 2);

                    Tuple<double, double> mg;
                    mienGiam.TryGetValue(gv.Item1, out mg);
                    double gioMG = mg != null ? Math.Round((gv.Item5 * mg.Item1 / 100.0) + mg.Item2, 2) : 0;
                    gioMG = Math.Min(gv.Item5, Math.Max(0, gioMG));

                    double dinhMucTT = Math.Max(0, Math.Round(gv.Item5 - gioMG, 2));
                    double tongGio = Math.Round(gioDL + gioDT + gioKT + gioNC, 2);
                    double gioVuot = Math.Max(0, Math.Round(tongGio - dinhMucTT, 2));

                    ketQua.Add(new BaoCaoGioGiangDto
                    {
                        MaGV = gv.Item2,
                        HoTen = gv.Item3,
                        TenBoMon = gv.Item4,
                        DinhMucGioChuan = gv.Item5,
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
        }

        public List<ChiTietBuoiDungLopDto> GetChiTietBuoiDungLopTheoMaGV(string maGV, int? idHocKy = null, int? idNamHoc = null)
        {
            List<ChiTietBuoiDungLopDto> list = new List<ChiTietBuoiDungLopDto>();
            if (string.IsNullOrWhiteSpace(maGV)) return list;

            using (SqlConnection conn = _dbHelper.OpenConnection())
            {
                int? namHocHieuLuc = idNamHoc ?? LayNamHocTuHocKy(conn, idHocKy);
                const string sql = @"
                    SELECT bh.ID_BuoiHoc, bh.NgayHoc, mh.TenMonHoc, nh.MaNhomHoc,
                           COALESCE(ph.MaPhong, '-') AS MaPhong,
                           bh.LoaiHinhGiangDay, bh.DayBangNgoaiNgu, bh.TongSiSoBuoi,
                           pc.SoTietGiangDay, pc.HeSoQuyDoi, pc.GioChuanQuyDoi
                    FROM R_PhanCongGiangDay pc
                    INNER JOIN DM_GiangVien gv ON pc.ID_GV = gv.ID_GV
                    INNER JOIN DM_BuoiHoc bh ON pc.ID_BuoiHoc = bh.ID_BuoiHoc
                    INNER JOIN DM_NhomHoc nh ON bh.ID_NhomHoc = nh.ID_NhomHoc
                    INNER JOIN DM_HocKy hk ON nh.ID_HocKy = hk.ID_HocKy
                    INNER JOIN DM_MonHoc mh ON nh.MaMonHoc = mh.MaMonHoc
                    LEFT JOIN DM_PhongHoc ph ON bh.ID_PhongHoc = ph.ID_PhongHoc
                    WHERE gv.MaGV = @MaGV
                      AND (bh.TrangThai IS NULL OR bh.TrangThai <> 'Huy')
                      AND (@ID_HocKy IS NULL OR nh.ID_HocKy = @ID_HocKy)
                      AND (@ID_NamHoc IS NULL OR hk.ID_NamHoc = @ID_NamHoc)
                    ORDER BY bh.NgayHoc ASC, bh.ID_BuoiHoc ASC;";

                using (SqlCommand cmd = conn.CreateCommand(sql))
                {
                    cmd.AddParam("@MaGV", maGV.Trim());
                    cmd.AddParam("@ID_HocKy", idHocKy);
                    cmd.AddParam("@ID_NamHoc", namHocHieuLuc);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new ChiTietBuoiDungLopDto
                            {
                                ID_BuoiHoc = r.GetInt(0),
                                NgayHoc = r.GetDateStr(1),
                                TenMonHoc = r.GetStr(2),
                                MaNhomHoc = r.GetStr(3),
                                MaPhong = r.GetStr(4),
                                LoaiHinhGiangDay = r.GetStr(5),
                                DayBangNgoaiNgu = r.GetBool(6),
                                TongSiSoBuoi = r.GetInt(7),
                                SoTietGiangDay = r.GetDoubleVal(8),
                                HeSoQuyDoi = r.GetDoubleVal(9),
                                GioChuanQuyDoi = r.GetDoubleVal(10)
                            });
                        }
                    }
                }

                if (list.Count > 0)
                {
                    Dictionary<int, List<int>> tietMap = new Dictionary<int, List<int>>();
                    using (SqlCommand cmd = conn.CreateCommand(@"
                        SELECT bt.ID_BuoiHoc, th.SoTiet
                        FROM R_BuoiHoc_Tiet bt
                        INNER JOIN DM_TietHoc th ON bt.ID_TietHoc = th.ID_TietHoc
                        ORDER BY th.SoTiet;"))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            int id = r.GetInt(0);
                            List<int> ds;
                            if (!tietMap.TryGetValue(id, out ds)) { ds = new List<int>(); tietMap[id] = ds; }
                            ds.Add(r.GetInt(1));
                        }
                    }

                    foreach (ChiTietBuoiDungLopDto item in list)
                    {
                        List<int> ds;
                        if (tietMap.TryGetValue(item.ID_BuoiHoc, out ds))
                            item.ChuoiTiet = XepLichService.DinhDangChuoiTiet(ds);
                    }
                }
            }
            return list;
        }

        public List<NckhHoatDong> LayDanhSachNCKH(int? idNamHoc = null)
        {
            const string sql = @"
                SELECT hd.ID_HoatDong, hd.ID_NamHoc, hd.TenHoatDong, hd.ID_LoaiHoatDong,
                       hd.TongGioChuanHoatDong, hd.TrangThai,
                       pc.ID_GV, pc.VaiTro, pc.TyLePhanTram, pc.GioChuanQuyDoi,
                       nh.TenNH, dm.TenDanhMuc, gv.HoTen
                FROM NCKH_HoatDong hd
                INNER JOIN R_PhanCong_NCKH pc ON hd.ID_HoatDong = pc.ID_HoatDong
                INNER JOIN DM_NamHoc nh ON hd.ID_NamHoc = nh.ID_NamHoc
                LEFT JOIN DM_DanhMuc dm ON hd.ID_LoaiHoatDong = dm.MaDanhMuc
                INNER JOIN DM_GiangVien gv ON pc.ID_GV = gv.ID_GV
                WHERE (@ID_NamHoc IS NULL OR hd.ID_NamHoc = @ID_NamHoc)
                ORDER BY hd.ID_HoatDong DESC;";

            List<NckhHoatDong> list = new List<NckhHoatDong>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_NamHoc", idNamHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new NckhHoatDong
                        {
                            ID_HoatDong = r.GetInt(0),
                            ID_NamHoc = r.GetInt(1),
                            TenHoatDong = r.GetStr(2),
                            ID_LoaiHoatDong = r.GetInt(3),
                            TongGioChuanHoatDong = r.GetDoubleVal(4),
                            TrangThai = r.GetNullableStr(5) ?? "DaHoanThanh",
                            ID_GV = r.GetInt(6),
                            VaiTro = r.GetStr(7),
                            TyLePhanTram = r.GetDoubleVal(8),
                            GioChuanQuyDoi = r.GetDoubleVal(9),
                            TenNamHoc = r.GetStr(10),
                            TenLoaiHoatDong = r.GetNullableStr(11),
                            TenGiangVien = r.GetStr(12)
                        });
                    }
                }
            }
            return list;
        }

        public List<MienGiamDinhMuc> LayDanhSachMienGiam(int? idNamHoc = null)
        {
            const string sql = @"
                SELECT mg.ID_MienGiam, mg.ID_GV, mg.ID_NamHoc, mg.TyLeGiamPhanTram, mg.SoGioGiam, mg.LyDo,
                       gv.MaGV, gv.HoTen, nh.TenNH
                FROM R_GV_MienGiamDinhMuc mg
                INNER JOIN DM_GiangVien gv ON mg.ID_GV = gv.ID_GV
                INNER JOIN DM_NamHoc nh ON mg.ID_NamHoc = nh.ID_NamHoc
                WHERE (@ID_NamHoc IS NULL OR mg.ID_NamHoc = @ID_NamHoc)
                ORDER BY mg.ID_MienGiam DESC;";

            List<MienGiamDinhMuc> list = new List<MienGiamDinhMuc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_NamHoc", idNamHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new MienGiamDinhMuc
                        {
                            ID_MienGiam = r.GetInt(0),
                            ID_GV = r.GetInt(1),
                            ID_NamHoc = r.GetInt(2),
                            TyLeGiamPhanTram = r.GetNullableDoubleVal(3) ?? 0,
                            SoGioGiam = r.GetNullableDoubleVal(4) ?? 0,
                            LyDo = r.GetStr(5),
                            MaGV = r.GetStr(6),
                            TenGiangVien = r.GetStr(7),
                            TenNamHoc = r.GetStr(8)
                        });
                    }
                }
            }
            return list;
        }

        private static int? LayNamHocTuHocKy(SqlConnection conn, int? idHocKy)
        {
            if (!idHocKy.HasValue) return null;
            using (SqlCommand cmd = conn.CreateCommand("SELECT ID_NamHoc FROM DM_HocKy WHERE ID_HocKy = @ID;"))
            {
                cmd.AddParam("@ID", idHocKy.Value);
                object val = cmd.ExecuteScalar();
                return val != null && val != DBNull.Value ? Convert.ToInt32(val) : (int?)null;
            }
        }

        private static List<Tuple<int, string, string, string, double>> LayDanhSachGiangVienCoBan(SqlConnection conn)
        {
            const string sql = @"
                SELECT gv.ID_GV, gv.MaGV, gv.HoTen, COALESCE(bm.TenDanhMuc, '-'), COALESCE(gv.DinhMucGioChuan, 270)
                FROM DM_GiangVien gv
                LEFT JOIN DM_DanhMuc bm ON gv.ID_BoMon = bm.MaDanhMuc
                WHERE gv.TrangThai = 1
                ORDER BY gv.HoTen;";

            List<Tuple<int, string, string, string, double>> res = new List<Tuple<int, string, string, string, double>>();
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                    res.Add(Tuple.Create(r.GetInt(0), r.GetStr(1), r.GetStr(2), r.GetStr(3), r.GetDoubleVal(4)));
            }
            return res;
        }

        private static Dictionary<int, Tuple<double, double>> LayGioDungLopTheoGV(SqlConnection conn, int? idHocKy, int? idNamHoc)
        {
            const string sql = @"
                SELECT pc.ID_GV, COALESCE(SUM(pc.SoTietGiangDay), 0), COALESCE(SUM(pc.GioChuanQuyDoi), 0)
                FROM R_PhanCongGiangDay pc
                INNER JOIN DM_BuoiHoc bh ON pc.ID_BuoiHoc = bh.ID_BuoiHoc
                INNER JOIN DM_NhomHoc nh ON bh.ID_NhomHoc = nh.ID_NhomHoc
                INNER JOIN DM_HocKy hk ON nh.ID_HocKy = hk.ID_HocKy
                WHERE (bh.TrangThai IS NULL OR bh.TrangThai <> 'Huy')
                  AND (@ID_HocKy IS NULL OR nh.ID_HocKy = @ID_HocKy)
                  AND (@ID_NamHoc IS NULL OR hk.ID_NamHoc = @ID_NamHoc)
                GROUP BY pc.ID_GV;";

            Dictionary<int, Tuple<double, double>> res = new Dictionary<int, Tuple<double, double>>();
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_HocKy", idHocKy);
                cmd.AddParam("@ID_NamHoc", idNamHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) res[r.GetInt(0)] = Tuple.Create(r.GetDoubleVal(1), r.GetDoubleVal(2));
                }
            }
            return res;
        }

        private static Dictionary<int, double> LayGioDeTaiTheoGV(SqlConnection conn, int? idNamHoc)
        {
            const string sql = @"
                SELECT pc.ID_GV, COALESCE(SUM(pc.GioChuanQuyDoi), 0)
                FROM R_PhanCong_DeTai pc
                INNER JOIN HD_DeTai_LuanVan dt ON pc.ID_DeTai = dt.ID_DeTai
                WHERE (@ID_NamHoc IS NULL OR dt.ID_NamHoc = @ID_NamHoc)
                GROUP BY pc.ID_GV;";
            return DocTongGio(conn, sql, idNamHoc, null);
        }

        private static Dictionary<int, double> LayGioKhaoThiTheoGV(SqlConnection conn, int? idHocKy, int? idNamHoc)
        {
            const string sql = @"
                SELECT kt.ID_GV, COALESCE(SUM(kt.GioChuanQuyDoi), 0)
                FROM KT_CongTacKhaoThi kt
                INNER JOIN DM_HocKy hk ON kt.ID_HocKy = hk.ID_HocKy
                WHERE (@ID_HocKy IS NULL OR kt.ID_HocKy = @ID_HocKy)
                  AND (@ID_NamHoc IS NULL OR hk.ID_NamHoc = @ID_NamHoc)
                GROUP BY kt.ID_GV;";
            return DocTongGio(conn, sql, idNamHoc, idHocKy);
        }

        private static Dictionary<int, double> LayGioNCKHTheoGV(SqlConnection conn, int? idNamHoc)
        {
            const string sql = @"
                SELECT pc.ID_GV, COALESCE(SUM(pc.GioChuanQuyDoi), 0)
                FROM R_PhanCong_NCKH pc
                INNER JOIN NCKH_HoatDong hd ON pc.ID_HoatDong = hd.ID_HoatDong
                WHERE (@ID_NamHoc IS NULL OR hd.ID_NamHoc = @ID_NamHoc)
                GROUP BY pc.ID_GV;";
            return DocTongGio(conn, sql, idNamHoc, null);
        }

        private static Dictionary<int, Tuple<double, double>> LayMienGiamTheoGV(SqlConnection conn, int? idNamHoc)
        {
            const string sql = @"
                SELECT ID_GV, COALESCE(SUM(COALESCE(TyLeGiamPhanTram, 0)), 0), COALESCE(SUM(COALESCE(SoGioGiam, 0)), 0)
                FROM R_GV_MienGiamDinhMuc
                WHERE (@ID_NamHoc IS NULL OR ID_NamHoc = @ID_NamHoc)
                GROUP BY ID_GV;";

            Dictionary<int, Tuple<double, double>> res = new Dictionary<int, Tuple<double, double>>();
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_NamHoc", idNamHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) res[r.GetInt(0)] = Tuple.Create(r.GetDoubleVal(1), r.GetDoubleVal(2));
                }
            }
            return res;
        }

        private static Dictionary<int, double> DocTongGio(SqlConnection conn, string sql, int? idNamHoc, int? idHocKy)
        {
            Dictionary<int, double> res = new Dictionary<int, double>();
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                if (sql.Contains("@ID_NamHoc")) cmd.AddParam("@ID_NamHoc", idNamHoc);
                if (sql.Contains("@ID_HocKy")) cmd.AddParam("@ID_HocKy", idHocKy);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) res[r.GetInt(0)] = r.GetDoubleVal(1);
                }
            }
            return res;
        }
    }
}
