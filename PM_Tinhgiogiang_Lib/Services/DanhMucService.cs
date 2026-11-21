using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PM_Tinhgiogiang_Lib.DataAccess;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib.Services
{
    /// <summary>
    /// Dịch vụ truy vấn & quản lý 9 nhóm danh mục dùng chung, dữ liệu nền tảng và Nhóm học (SQL Server).
    /// </summary>
    public class DanhMucService
    {
        private readonly DatabaseHelper _dbHelper;

        public DanhMucService(DatabaseHelper dbHelper = null)
        {
            _dbHelper = dbHelper ?? new DatabaseHelper();
        }

        public List<NhomDanhMuc> LayDanhSachNhomDanhMuc()
        {
            const string sql = @"SELECT MaNhom, TenNhom, MoTa FROM DM_NhomDanhMuc ORDER BY MaNhom;";
            List<NhomDanhMuc> ketQua = new List<NhomDanhMuc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    ketQua.Add(new NhomDanhMuc
                    {
                        MaNhom = r.GetInt(0),
                        TenNhom = r.GetStr(1),
                        MoTa = r.GetNullableStr(2)
                    });
                }
            }
            return ketQua;
        }

        public List<DanhMuc> LayDanhMucTheoNhom(int maNhom, bool chiLayKichHoat = true)
        {
            const string sql = @"
                SELECT MaDanhMuc, MaNhom, TenDanhMuc, VietTat, GiaTriThamSo, ThuTu, KichHoat
                FROM DM_DanhMuc
                WHERE MaNhom = @MaNhom
                  AND (@ChiLayKichHoat = 0 OR KichHoat = 1)
                ORDER BY ThuTu, TenDanhMuc;";

            List<DanhMuc> ketQua = new List<DanhMuc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@MaNhom", maNhom);
                cmd.AddParam("@ChiLayKichHoat", chiLayKichHoat ? 1 : 0);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ketQua.Add(new DanhMuc
                        {
                            MaDanhMuc = r.GetInt(0),
                            MaNhom = r.GetInt(1),
                            TenDanhMuc = r.GetStr(2),
                            VietTat = r.GetNullableStr(3),
                            GiaTriThamSo = r.GetNullableDoubleVal(4),
                            ThuTu = r.GetNullableInt(5) ?? 1,
                            KichHoat = r.GetBool(6, true)
                        });
                    }
                }
            }
            return ketQua;
        }

        public List<NamHoc> LayDanhSachNamHoc()
        {
            const string sql = @"SELECT ID_NamHoc, MaNH, TenNH, NgayBatDau, NgayKetThuc FROM DM_NamHoc ORDER BY NgayBatDau DESC;";
            List<NamHoc> ketQua = new List<NamHoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    ketQua.Add(new NamHoc
                    {
                        ID_NamHoc = r.GetInt(0),
                        MaNH = r.GetStr(1),
                        TenNH = r.GetStr(2),
                        NgayBatDau = r.GetDateStr(3),
                        NgayKetThuc = r.GetDateStr(4)
                    });
                }
            }
            return ketQua;
        }

        public List<HocKy> LayDanhSachHocKy()
        {
            const string sql = @"SELECT ID_HocKy, ID_NamHoc, MaHK, TenHK, NgayBD, NgayKT FROM DM_HocKy ORDER BY NgayBD DESC;";
            List<HocKy> ketQua = new List<HocKy>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    ketQua.Add(new HocKy
                    {
                        ID_HocKy = r.GetInt(0),
                        ID_NamHoc = r.GetInt(1),
                        MaHK = r.GetStr(2),
                        TenHK = r.GetStr(3),
                        NgayBD = r.GetDateStr(4),
                        NgayKT = r.GetDateStr(5)
                    });
                }
            }
            return ketQua;
        }

        public List<TietHoc> LayDanhSachTietHoc()
        {
            const string sql = @"SELECT ID_TietHoc, SoTiet, BuoiTrongNgay, GioBatDau, GioKetThuc FROM DM_TietHoc ORDER BY SoTiet;";
            List<TietHoc> ketQua = new List<TietHoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    ketQua.Add(new TietHoc
                    {
                        ID_TietHoc = r.GetInt(0),
                        SoTiet = r.GetInt(1),
                        BuoiTrongNgay = r.GetStr(2),
                        GioBatDau = r.GetTimeStr(3),
                        GioKetThuc = r.GetTimeStr(4)
                    });
                }
            }
            return ketQua;
        }

        public List<GiangVien> LayDanhSachGiangVien(bool chiLayDangHoatDong = true)
        {
            const string sql = @"
                SELECT gv.ID_GV, gv.MaGV, gv.HoTen, gv.ID_BoMon, gv.ID_HocHam, gv.ID_HocVi,
                       gv.GioiTinh, gv.LaThinhGiang, gv.DonViCongTac, gv.DinhMucGioChuan,
                       gv.Email, gv.SoDienThoai, gv.TrangThai,
                       bm.TenDanhMuc, hh.TenDanhMuc, hv.TenDanhMuc
                FROM DM_GiangVien gv
                LEFT JOIN DM_DanhMuc bm ON gv.ID_BoMon = bm.MaDanhMuc
                LEFT JOIN DM_DanhMuc hh ON gv.ID_HocHam = hh.MaDanhMuc
                LEFT JOIN DM_DanhMuc hv ON gv.ID_HocVi = hv.MaDanhMuc
                WHERE (@ChiLayHoatDong = 0 OR gv.TrangThai = 1)
                ORDER BY gv.HoTen;";

            List<GiangVien> ketQua = new List<GiangVien>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ChiLayHoatDong", chiLayDangHoatDong ? 1 : 0);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ketQua.Add(new GiangVien
                        {
                            ID_GV = r.GetInt(0),
                            MaGV = r.GetStr(1),
                            HoTen = r.GetStr(2),
                            ID_BoMon = r.GetInt(3),
                            ID_HocHam = r.GetNullableInt(4),
                            ID_HocVi = r.GetNullableInt(5),
                            GioiTinh = r.GetNullableStr(6) ?? "Nam",
                            LaThinhGiang = r.GetBool(7),
                            DonViCongTac = r.GetNullableStr(8),
                            DinhMucGioChuan = r.GetNullableDoubleVal(9) ?? 270,
                            Email = r.GetNullableStr(10),
                            SoDienThoai = r.GetNullableStr(11),
                            TrangThai = r.GetBool(12, true) ? 1 : 0,
                            TenBoMon = r.GetNullableStr(13),
                            TenHocHam = r.GetNullableStr(14),
                            TenHocVi = r.GetNullableStr(15)
                        });
                    }
                }
            }
            return ketQua;
        }

        public List<PhongHoc> LayDanhSachPhongHoc(bool chiLayDangHoatDong = true)
        {
            const string sql = @"
                SELECT ph.ID_PhongHoc, ph.MaPhong, ph.TenPhong, ph.ID_LoaiPhong,
                       ph.CoSo, ph.SucChua, ph.TrangThai, lp.TenDanhMuc
                FROM DM_PhongHoc ph
                LEFT JOIN DM_DanhMuc lp ON ph.ID_LoaiPhong = lp.MaDanhMuc
                WHERE (@ChiLayHoatDong = 0 OR ph.TrangThai = 1)
                ORDER BY ph.MaPhong;";

            List<PhongHoc> ketQua = new List<PhongHoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ChiLayHoatDong", chiLayDangHoatDong ? 1 : 0);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ketQua.Add(new PhongHoc
                        {
                            ID_PhongHoc = r.GetInt(0),
                            MaPhong = r.GetStr(1),
                            TenPhong = r.GetStr(2),
                            ID_LoaiPhong = r.GetNullableInt(3),
                            CoSo = r.GetNullableStr(4),
                            SucChua = r.GetNullableInt(5),
                            TrangThai = r.GetBool(6, true) ? 1 : 0,
                            TenLoaiPhong = r.GetNullableStr(7)
                        });
                    }
                }
            }
            return ketQua;
        }

        public List<MonHoc> LayDanhSachMonHoc()
        {
            const string sql = @"
                SELECT mh.MaMonHoc, mh.TenMonHoc, mh.ID_BoMon, mh.ID_HeDaoTao,
                       mh.SoTinChi, mh.SoTietLT, mh.SoTietTH, mh.DayBangNgoaiNgu,
                       bm.TenDanhMuc, hdt.TenDanhMuc
                FROM DM_MonHoc mh
                LEFT JOIN DM_DanhMuc bm ON mh.ID_BoMon = bm.MaDanhMuc
                LEFT JOIN DM_DanhMuc hdt ON mh.ID_HeDaoTao = hdt.MaDanhMuc
                ORDER BY mh.MaMonHoc;";

            List<MonHoc> ketQua = new List<MonHoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    ketQua.Add(new MonHoc
                    {
                        MaMonHoc = r.GetStr(0),
                        TenMonHoc = r.GetStr(1),
                        ID_BoMon = r.GetInt(2),
                        ID_HeDaoTao = r.GetNullableInt(3),
                        SoTinChi = r.GetDoubleVal(4),
                        SoTietLT = r.GetNullableInt(5) ?? 0,
                        SoTietTH = r.GetNullableInt(6) ?? 0,
                        DayBangNgoaiNgu = r.GetBool(7),
                        TenBoMon = r.GetNullableStr(8),
                        TenHeDaoTao = r.GetNullableStr(9)
                    });
                }
            }
            return ketQua;
        }

        public List<BaiHoc> LayDanhSachBaiHoc(string maMonHoc = null)
        {
            const string sql = @"
                SELECT bh.ID_BaiHoc, bh.MaMonHoc, bh.ThuTuBai, bh.TenBaiHoc,
                       bh.LoaiBai, bh.SoTiet, bh.ID_GV_MacDinh, mh.TenMonHoc, gv.HoTen
                FROM DM_BaiHoc bh
                INNER JOIN DM_MonHoc mh ON bh.MaMonHoc = mh.MaMonHoc
                LEFT JOIN DM_GiangVien gv ON bh.ID_GV_MacDinh = gv.ID_GV
                WHERE (@MaMonHoc IS NULL OR bh.MaMonHoc = @MaMonHoc)
                ORDER BY bh.MaMonHoc, bh.ThuTuBai;";

            List<BaiHoc> ketQua = new List<BaiHoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@MaMonHoc", string.IsNullOrWhiteSpace(maMonHoc) ? null : maMonHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ketQua.Add(new BaiHoc
                        {
                            ID_BaiHoc = r.GetInt(0),
                            MaMonHoc = r.GetStr(1),
                            ThuTuBai = r.GetInt(2),
                            TenBaiHoc = r.GetStr(3),
                            LoaiBai = r.GetStr(4),
                            SoTiet = r.GetInt(5),
                            ID_GV_MacDinh = r.GetNullableInt(6),
                            TenMonHoc = r.GetNullableStr(7),
                            TenGiangVienMacDinh = r.GetNullableStr(8)
                        });
                    }
                }
            }
            return ketQua;
        }

        public List<LopHocPhan> LayDanhSachLopHocPhan(int? idHocKy = null)
        {
            const string sql = @"
                SELECT lhp.ID_LopHocPhan, lhp.MaLopHocPhan, lhp.MaMonHoc, lhp.ID_HocKy,
                       lhp.LoaiLopHP, lhp.SiSoDangKy, lhp.NgayBatDau, lhp.NgayKetThuc, lhp.TrangThai,
                       mh.TenMonHoc, hk.TenHK
                FROM LopHocPhan lhp
                INNER JOIN DM_MonHoc mh ON lhp.MaMonHoc = mh.MaMonHoc
                LEFT JOIN DM_HocKy hk ON lhp.ID_HocKy = hk.ID_HocKy
                WHERE (@ID_HocKy IS NULL OR lhp.ID_HocKy = @ID_HocKy)
                ORDER BY lhp.ID_HocKy DESC, lhp.MaLopHocPhan;";

            List<LopHocPhan> ketQua = new List<LopHocPhan>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_HocKy", idHocKy);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ketQua.Add(new LopHocPhan
                        {
                            ID_LopHocPhan = r.GetInt(0),
                            MaLopHocPhan = r.GetStr(1),
                            MaMonHoc = r.GetStr(2),
                            ID_HocKy = r.GetInt(3),
                            LoaiLopHP = r.GetNullableStr(4) ?? "ChinhQuy",
                            SiSoDangKy = r.GetInt(5),
                            NgayBatDau = r.GetNullableDateStr(6),
                            NgayKetThuc = r.GetNullableDateStr(7),
                            TrangThai = r.GetNullableStr(8) ?? "Draft",
                            TenMonHoc = r.GetStr(9),
                            TenHocKy = r.GetNullableStr(10)
                        });
                    }
                }
            }
            return ketQua;
        }

        public List<NhomHoc> LayDanhSachNhomHoc(int? idHocKy = null, string maMonHoc = null)
        {
            const string sql = @"
                SELECT nh.ID_NhomHoc, nh.MaNhomHoc, nh.TenNhomHoc, nh.MaMonHoc, nh.ID_HocKy,
                       nh.LoaiNhom, nh.TongSiSoNhom, mh.TenMonHoc, hk.TenHK
                FROM DM_NhomHoc nh
                INNER JOIN DM_MonHoc mh ON nh.MaMonHoc = mh.MaMonHoc
                LEFT JOIN DM_HocKy hk ON nh.ID_HocKy = hk.ID_HocKy
                WHERE (@ID_HocKy IS NULL OR nh.ID_HocKy = @ID_HocKy)
                  AND (@MaMonHoc IS NULL OR nh.MaMonHoc = @MaMonHoc)
                ORDER BY nh.ID_HocKy DESC, nh.MaNhomHoc;";

            using (SqlConnection conn = _dbHelper.OpenConnection())
            {
                List<NhomHoc> ketQua = new List<NhomHoc>();
                using (SqlCommand cmd = conn.CreateCommand(sql))
                {
                    cmd.AddParam("@ID_HocKy", idHocKy);
                    cmd.AddParam("@MaMonHoc", string.IsNullOrWhiteSpace(maMonHoc) ? null : maMonHoc);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            ketQua.Add(new NhomHoc
                            {
                                ID_NhomHoc = r.GetInt(0),
                                MaNhomHoc = r.GetStr(1),
                                TenNhomHoc = r.GetStr(2),
                                MaMonHoc = r.GetStr(3),
                                ID_HocKy = r.GetInt(4),
                                LoaiNhom = r.GetStr(5),
                                TongSiSoNhom = r.GetInt(6),
                                TenMonHoc = r.GetNullableStr(7),
                                TenHocKy = r.GetNullableStr(8)
                            });
                        }
                    }
                }

                if (ketQua.Count > 0)
                {
                    Dictionary<int, List<Tuple<int, string>>> lopMap = LayLopHPTheoNhomHoc(conn);
                    foreach (NhomHoc nhom in ketQua)
                    {
                        List<Tuple<int, string>> lops;
                        if (lopMap.TryGetValue(nhom.ID_NhomHoc, out lops))
                        {
                            nhom.DanhSachID_LopHP = lops.Select(x => x.Item1).ToList();
                            nhom.DanhSachMaLopHP = string.Join(", ", lops.Select(x => x.Item2));
                        }
                    }
                }
                return ketQua;
            }
        }

        public KetQuaThaoTac TaoNhomHoc(NhomHoc nhomHoc, List<int> danhSachID_LopHP)
        {
            if (nhomHoc == null || string.IsNullOrWhiteSpace(nhomHoc.MaNhomHoc) || string.IsNullOrWhiteSpace(nhomHoc.TenNhomHoc))
                return KetQuaThaoTac.Loi("Mã nhóm học và Tên nhóm học không được để trống.");

            List<int> dsLop = danhSachID_LopHP != null ? danhSachID_LopHP.Distinct().ToList() : new List<int>();
            if (dsLop.Count == 0)
                return KetQuaThaoTac.Loi("Vui lòng chọn ít nhất một Lớp học phần.");

            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    int tongSiSo = TinhTongSiSoLopHP(conn, dsLop);
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            const string insertSql = @"
                                INSERT INTO DM_NhomHoc (MaNhomHoc, TenNhomHoc, MaMonHoc, ID_HocKy, LoaiNhom, TongSiSoNhom)
                                VALUES (@Ma, @Ten, @Mon, @HK, @Loai, @SiSo);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int idNhomHoc;
                            using (SqlCommand cmd = conn.CreateCommand(insertSql, tran))
                            {
                                cmd.AddParam("@Ma", nhomHoc.MaNhomHoc.Trim());
                                cmd.AddParam("@Ten", nhomHoc.TenNhomHoc.Trim());
                                cmd.AddParam("@Mon", nhomHoc.MaMonHoc.Trim());
                                cmd.AddParam("@HK", nhomHoc.ID_HocKy);
                                cmd.AddParam("@Loai", string.IsNullOrWhiteSpace(nhomHoc.LoaiNhom) ? "LT" : nhomHoc.LoaiNhom.Trim());
                                cmd.AddParam("@SiSo", tongSiSo);
                                idNhomHoc = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (int idLop in dsLop)
                            {
                                using (SqlCommand cmd = conn.CreateCommand("INSERT INTO R_NhomHoc_LopHP (ID_NhomHoc, ID_LopHocPhan) VALUES (@N, @L);", tran))
                                {
                                    cmd.AddParam("@N", idNhomHoc);
                                    cmd.AddParam("@L", idLop);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                            return KetQuaThaoTac.Ok("Tạo nhóm học thành công (Tổng sĩ số: " + tongSiSo + ").", null, idNhomHoc);
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi tạo nhóm học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaNhomHoc(int idNhomHoc)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    using (SqlCommand chk = conn.CreateCommand("SELECT COUNT(1) FROM DM_BuoiHoc WHERE ID_NhomHoc = @ID;"))
                    {
                        chk.AddParam("@ID", idNhomHoc);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                            return KetQuaThaoTac.Loi("Không thể xóa nhóm học vì đã có buổi học được xếp.");
                    }

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand c1 = conn.CreateCommand("DELETE FROM R_NhomHoc_LopHP WHERE ID_NhomHoc = @ID;", tran))
                            {
                                c1.AddParam("@ID", idNhomHoc);
                                c1.ExecuteNonQuery();
                            }
                            using (SqlCommand c2 = conn.CreateCommand("DELETE FROM DM_NhomHoc WHERE ID_NhomHoc = @ID;", tran))
                            {
                                c2.AddParam("@ID", idNhomHoc);
                                c2.ExecuteNonQuery();
                            }
                            tran.Commit();
                            return KetQuaThaoTac.Ok("Đã xóa nhóm học.", null, idNhomHoc);
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa nhóm học: " + ex.Message); }
        }

        public List<QuyTacQuyDoi> LayDanhSachQuyTacQuyDoi()
        {
            const string sql = @"
                SELECT q.ID_QuyTac, q.TenQuyTac, q.LoaiHinhGiangDay, q.ID_HeDaoTao,
                       q.SiSoTu, q.SiSoDen, q.HeSoQuyDoi, q.NgayHieuLucTu, q.NgayHieuLucDen, dm.TenDanhMuc
                FROM SYS_QuyTacQuyDoi q
                LEFT JOIN DM_DanhMuc dm ON q.ID_HeDaoTao = dm.MaDanhMuc
                ORDER BY q.LoaiHinhGiangDay, q.SiSoTu;";

            List<QuyTacQuyDoi> list = new List<QuyTacQuyDoi>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    list.Add(new QuyTacQuyDoi
                    {
                        ID_QuyTac = r.GetInt(0),
                        TenQuyTac = r.GetStr(1),
                        LoaiHinhGiangDay = r.GetStr(2),
                        ID_HeDaoTao = r.GetNullableInt(3),
                        SiSoTu = r.GetInt(4),
                        SiSoDen = r.GetInt(5),
                        HeSoQuyDoi = r.GetDoubleVal(6),
                        NgayHieuLucTu = r.GetDateStr(7),
                        NgayHieuLucDen = r.GetNullableDateStr(8),
                        TenHeDaoTao = r.GetNullableStr(9)
                    });
                }
            }
            return list;
        }

        public List<GiangVienRangBuoc> LayRangBuocGiangVien(int idGV)
        {
            List<GiangVienRangBuoc> list = new List<GiangVienRangBuoc>();
            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand("SELECT ID_RangBuoc, ID_GV, NgayBD, NgayKT, ThuTrongTuan, BuoiTrongNgay, LyDo FROM R_GV_RangBuoc WHERE ID_GV = @ID ORDER BY NgayBD DESC;"))
            {
                cmd.AddParam("@ID", idGV);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new GiangVienRangBuoc
                        {
                            ID_RangBuoc = r.GetInt(0),
                            ID_GV = r.GetInt(1),
                            NgayBD = r.GetDateStr(2),
                            NgayKT = r.GetDateStr(3),
                            ThuTrongTuan = r.GetNullableInt(4),
                            BuoiTrongNgay = r.GetNullableStr(5),
                            LyDo = r.GetNullableStr(6)
                        });
                    }
                }
            }
            return list;
        }

        public KetQuaThaoTac LuuGiangVien(GiangVien gv)
        {
            if (gv == null || string.IsNullOrWhiteSpace(gv.MaGV) || string.IsNullOrWhiteSpace(gv.HoTen) || gv.ID_BoMon <= 0)
                return KetQuaThaoTac.Loi("Mã GV, Họ tên và Bộ môn là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string sql = gv.ID_GV > 0
                        ? @"UPDATE DM_GiangVien SET MaGV=@Ma, HoTen=@Ten, ID_BoMon=@BM, ID_HocHam=@HH, ID_HocVi=@HV, GioiTinh=@GT, LaThinhGiang=@TG, DonViCongTac=@DV, DinhMucGioChuan=@DM, Email=@Email, SoDienThoai=@SDT, TrangThai=@TT WHERE ID_GV=@ID;"
                        : @"INSERT INTO DM_GiangVien (MaGV, HoTen, ID_BoMon, ID_HocHam, ID_HocVi, GioiTinh, LaThinhGiang, DonViCongTac, DinhMucGioChuan, Email, SoDienThoai, TrangThai) VALUES (@Ma, @Ten, @BM, @HH, @HV, @GT, @TG, @DV, @DM, @Email, @SDT, @TT);";
                    using (SqlCommand cmd = conn.CreateCommand(sql))
                    {
                        if (gv.ID_GV > 0) cmd.AddParam("@ID", gv.ID_GV);
                        cmd.AddParam("@Ma", gv.MaGV.Trim());
                        cmd.AddParam("@Ten", gv.HoTen.Trim());
                        cmd.AddParam("@BM", gv.ID_BoMon);
                        cmd.AddParam("@HH", gv.ID_HocHam);
                        cmd.AddParam("@HV", gv.ID_HocVi);
                        cmd.AddParam("@GT", gv.GioiTinh);
                        cmd.AddParam("@TG", gv.LaThinhGiang ? 1 : 0);
                        cmd.AddParam("@DV", gv.DonViCongTac);
                        cmd.AddParam("@DM", gv.DinhMucGioChuan);
                        cmd.AddParam("@Email", gv.Email);
                        cmd.AddParam("@SDT", gv.SoDienThoai);
                        cmd.AddParam("@TT", gv.TrangThai == 0 ? 0 : 1);
                        cmd.ExecuteNonQuery();
                    }
                    return KetQuaThaoTac.Ok("Đã lưu giảng viên '" + gv.HoTen + "'.");
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu giảng viên: " + ex.Message); }
        }

        public KetQuaThaoTac LuuPhongHoc(PhongHoc ph)
        {
            if (ph == null || string.IsNullOrWhiteSpace(ph.MaPhong) || string.IsNullOrWhiteSpace(ph.TenPhong))
                return KetQuaThaoTac.Loi("Mã phòng và Tên phòng là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string sql = ph.ID_PhongHoc > 0
                        ? @"UPDATE DM_PhongHoc SET MaPhong=@Ma, TenPhong=@Ten, ID_LoaiPhong=@Loai, CoSo=@CoSo, SucChua=@SC, TrangThai=@TT WHERE ID_PhongHoc=@ID;"
                        : @"INSERT INTO DM_PhongHoc (MaPhong, TenPhong, ID_LoaiPhong, CoSo, SucChua, TrangThai) VALUES (@Ma, @Ten, @Loai, @CoSo, @SC, @TT);";
                    using (SqlCommand cmd = conn.CreateCommand(sql))
                    {
                        if (ph.ID_PhongHoc > 0) cmd.AddParam("@ID", ph.ID_PhongHoc);
                        cmd.AddParam("@Ma", ph.MaPhong.Trim());
                        cmd.AddParam("@Ten", ph.TenPhong.Trim());
                        cmd.AddParam("@Loai", ph.ID_LoaiPhong);
                        cmd.AddParam("@CoSo", ph.CoSo);
                        cmd.AddParam("@SC", ph.SucChua);
                        cmd.AddParam("@TT", ph.TrangThai == 0 ? 0 : 1);
                        cmd.ExecuteNonQuery();
                    }
                    return KetQuaThaoTac.Ok("Đã lưu phòng học '" + ph.TenPhong + "'.");
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu phòng học: " + ex.Message); }
        }

        public KetQuaThaoTac TachToThucHanh(int idLopHocPhan, int siSoMoiTo = 20)
        {
            if (siSoMoiTo < 15 || siSoMoiTo > 25) siSoMoiTo = 20;
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string maLop; string maMon; int idHK; int siSo;
                    using (SqlCommand cmd = conn.CreateCommand("SELECT MaLopHocPhan, MaMonHoc, ID_HocKy, SiSoDangKy FROM LopHocPhan WHERE ID_LopHocPhan = @ID;"))
                    {
                        cmd.AddParam("@ID", idLopHocPhan);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return KetQuaThaoTac.Loi("Không tìm thấy lớp học phần.");
                            maLop = r.GetStr(0); maMon = r.GetStr(1); idHK = r.GetInt(2); siSo = r.GetInt(3);
                        }
                    }

                    int soTo = Math.Max(1, (int)Math.Ceiling(siSo / (double)siSoMoiTo));
                    int siSoCoBan = siSo / soTo;
                    int du = siSo % soTo;

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 1; i <= soTo; i++)
                            {
                                int siSoTo = siSoCoBan + (i <= du ? 1 : 0);
                                const string sqlIns = @"
                                    INSERT INTO DM_NhomHoc (MaNhomHoc, TenNhomHoc, MaMonHoc, ID_HocKy, LoaiNhom, TongSiSoNhom)
                                    VALUES (@Ma, @Ten, @Mon, @HK, 'TH', @SiSo);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                                int idNhom;
                                using (SqlCommand insCmd = conn.CreateCommand(sqlIns, tran))
                                {
                                    insCmd.AddParam("@Ma", "TH_" + maLop + "_" + i);
                                    insCmd.AddParam("@Ten", "Tổ TH " + i + " (" + maLop + ")");
                                    insCmd.AddParam("@Mon", maMon);
                                    insCmd.AddParam("@HK", idHK);
                                    insCmd.AddParam("@SiSo", siSoTo);
                                    idNhom = Convert.ToInt32(insCmd.ExecuteScalar());
                                }
                                using (SqlCommand linkCmd = conn.CreateCommand("INSERT INTO R_NhomHoc_LopHP (ID_NhomHoc, ID_LopHocPhan) VALUES (@N, @L);", tran))
                                {
                                    linkCmd.AddParam("@N", idNhom);
                                    linkCmd.AddParam("@L", idLopHocPhan);
                                    linkCmd.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                            return KetQuaThaoTac.Ok("Đã tách lớp " + maLop + " thành " + soTo + " tổ Thực hành.");
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi tách tổ thực hành: " + ex.Message); }
        }

        public NguoiDung DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;
            const string sql = @"
                SELECT TOP 1 ID_NguoiDung, TenDangNhap, MatKhauHash, ID_GV, VaiTro, TrangThai
                FROM SYS_NguoiDung
                WHERE TenDangNhap = @User AND (TrangThai IS NULL OR TrangThai = 1);";

            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@User", tenDangNhap.Trim());
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    string hash = r.GetStr(2);
                    // Hỗ trợ kiểm tra trực tiếp hoặc hash (phục vụ môi trường nội bộ UMP)
                    if (!string.Equals(hash, matKhau, StringComparison.Ordinal))
                        return null;

                    return new NguoiDung
                    {
                        ID_NguoiDung = r.GetInt(0),
                        TenDangNhap = r.GetStr(1),
                        MatKhauHash = hash,
                        ID_GV = r.GetNullableInt(3),
                        VaiTro = r.GetStr(4),
                        TrangThai = r.GetBool(5, true)
                    };
                }
            }
        }

        public KetQuaThaoTac NgungHoatDongGiangVien(int idGV)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("UPDATE DM_GiangVien SET TrangThai = 0 WHERE ID_GV = @ID;"))
                {
                    cmd.AddParam("@ID", idGV);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã chuyển giảng viên sang trạng thái Ngừng hoạt động.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi ngừng hoạt động GV: " + ex.Message); }
        }

        public KetQuaThaoTac ThemRangBuocGiangVien(GiangVienRangBuoc rb)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("INSERT INTO R_GV_RangBuoc (ID_GV, NgayBD, NgayKT, ThuTrongTuan, BuoiTrongNgay, LyDo) VALUES (@GV, @BD, @KT, @Thu, @Buoi, @LD);"))
                {
                    cmd.AddParam("@GV", rb.ID_GV);
                    cmd.AddParam("@BD", rb.NgayBD);
                    cmd.AddParam("@KT", rb.NgayKT);
                    cmd.AddParam("@Thu", rb.ThuTrongTuan);
                    cmd.AddParam("@Buoi", string.IsNullOrWhiteSpace(rb.BuoiTrongNgay) ? null : rb.BuoiTrongNgay);
                    cmd.AddParam("@LD", rb.LyDo);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã thêm lịch bận giảng viên.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi thêm lịch bận: " + ex.Message); }
        }

        public KetQuaThaoTac XoaRangBuocGiangVien(int idRangBuoc)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("DELETE FROM R_GV_RangBuoc WHERE ID_RangBuoc = @ID;"))
                {
                    cmd.AddParam("@ID", idRangBuoc);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã xóa lịch bận.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa lịch bận: " + ex.Message); }
        }

        public KetQuaThaoTac XoaPhongHoc(int idPhong)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    using (SqlCommand chk = conn.CreateCommand("SELECT COUNT(1) FROM DM_BuoiHoc WHERE ID_PhongHoc = @ID;"))
                    {
                        chk.AddParam("@ID", idPhong);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                            return KetQuaThaoTac.Loi("Không thể xóa vì phòng đang được xếp lịch trong DM_BuoiHoc.");
                    }
                    using (SqlCommand cmd = conn.CreateCommand("DELETE FROM DM_PhongHoc WHERE ID_PhongHoc = @ID;"))
                    {
                        cmd.AddParam("@ID", idPhong);
                        cmd.ExecuteNonQuery();
                    }
                }
                return KetQuaThaoTac.Ok("Đã xóa phòng học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa phòng học: " + ex.Message); }
        }

        public KetQuaThaoTac LuuMonHoc(MonHoc mh, bool isNew)
        {
            if (mh == null || string.IsNullOrWhiteSpace(mh.MaMonHoc) || string.IsNullOrWhiteSpace(mh.TenMonHoc))
                return KetQuaThaoTac.Loi("Mã môn và Tên môn là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string sql = isNew
                        ? "INSERT INTO DM_MonHoc (MaMonHoc, TenMonHoc, ID_BoMon, ID_HeDaoTao, SoTinChi, SoTietLT, SoTietTH, DayBangNgoaiNgu) VALUES (@Ma, @Ten, @BM, @HDT, @TC, @LT, @TH, @NN);"
                        : "UPDATE DM_MonHoc SET TenMonHoc=@Ten, ID_BoMon=@BM, ID_HeDaoTao=@HDT, SoTinChi=@TC, SoTietLT=@LT, SoTietTH=@TH, DayBangNgoaiNgu=@NN WHERE MaMonHoc=@Ma;";
                    using (SqlCommand cmd = conn.CreateCommand(sql))
                    {
                        cmd.AddParam("@Ma", mh.MaMonHoc.Trim());
                        cmd.AddParam("@Ten", mh.TenMonHoc.Trim());
                        cmd.AddParam("@BM", mh.ID_BoMon);
                        cmd.AddParam("@HDT", mh.ID_HeDaoTao);
                        cmd.AddParam("@TC", mh.SoTinChi);
                        cmd.AddParam("@LT", mh.SoTietLT);
                        cmd.AddParam("@TH", mh.SoTietTH);
                        cmd.AddParam("@NN", mh.DayBangNgoaiNgu ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
                return KetQuaThaoTac.Ok("Đã lưu môn học '" + mh.TenMonHoc + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu môn học: " + ex.Message); }
        }

        public KetQuaThaoTac LuuBaiHoc(BaiHoc bh)
        {
            if (bh == null || string.IsNullOrWhiteSpace(bh.MaMonHoc) || string.IsNullOrWhiteSpace(bh.TenBaiHoc))
                return KetQuaThaoTac.Loi("Môn học và Tên bài học là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("INSERT INTO DM_BaiHoc (MaMonHoc, ThuTuBai, TenBaiHoc, LoaiBai, SoTiet, ID_GV_MacDinh) VALUES (@Mon, @TT, @Ten, @Loai, @ST, @GV);"))
                {
                    cmd.AddParam("@Mon", bh.MaMonHoc.Trim());
                    cmd.AddParam("@TT", bh.ThuTuBai);
                    cmd.AddParam("@Ten", bh.TenBaiHoc.Trim());
                    cmd.AddParam("@Loai", bh.LoaiBai);
                    cmd.AddParam("@ST", bh.SoTiet);
                    cmd.AddParam("@GV", bh.ID_GV_MacDinh);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã thêm bài học '" + bh.TenBaiHoc + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi thêm bài học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaBaiHoc(int idBaiHoc)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("DELETE FROM DM_BaiHoc WHERE ID_BaiHoc = @ID;"))
                {
                    cmd.AddParam("@ID", idBaiHoc);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã xóa bài học.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa bài học: " + ex.Message); }
        }

        public KetQuaThaoTac LuuLopHocPhan(LopHocPhan lhp)
        {
            if (lhp == null || string.IsNullOrWhiteSpace(lhp.MaLopHocPhan) || string.IsNullOrWhiteSpace(lhp.MaMonHoc))
                return KetQuaThaoTac.Loi("Mã lớp HP và Môn học là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("INSERT INTO LopHocPhan (MaLopHocPhan, MaMonHoc, ID_HocKy, LoaiLopHP, SiSoDangKy, TrangThai) VALUES (@Ma, @Mon, @HK, @Loai, @SS, 'Published');"))
                {
                    cmd.AddParam("@Ma", lhp.MaLopHocPhan.Trim());
                    cmd.AddParam("@Mon", lhp.MaMonHoc);
                    cmd.AddParam("@HK", lhp.ID_HocKy);
                    cmd.AddParam("@Loai", string.IsNullOrWhiteSpace(lhp.LoaiLopHP) ? "ChinhQuy" : lhp.LoaiLopHP);
                    cmd.AddParam("@SS", lhp.SiSoDangKy);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã thêm lớp học phần '" + lhp.MaLopHocPhan + "'.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi thêm lớp HP: " + ex.Message); }
        }

        public KetQuaThaoTac LuuQuyTacQuyDoi(QuyTacQuyDoi qt)
        {
            if (qt == null || string.IsNullOrWhiteSpace(qt.TenQuyTac))
                return KetQuaThaoTac.Loi("Tên quy tắc là bắt buộc.");
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string sql = qt.ID_QuyTac > 0
                        ? "UPDATE SYS_QuyTacQuyDoi SET TenQuyTac=@Ten, LoaiHinhGiangDay=@Loai, SiSoTu=@Tu, SiSoDen=@Den, HeSoQuyDoi=@HS, NgayHieuLucTu=@HLTu WHERE ID_QuyTac=@ID;"
                        : "INSERT INTO SYS_QuyTacQuyDoi (TenQuyTac, LoaiHinhGiangDay, SiSoTu, SiSoDen, HeSoQuyDoi, NgayHieuLucTu) VALUES (@Ten, @Loai, @Tu, @Den, @HS, @HLTu);";
                    using (SqlCommand cmd = conn.CreateCommand(sql))
                    {
                        if (qt.ID_QuyTac > 0) cmd.AddParam("@ID", qt.ID_QuyTac);
                        cmd.AddParam("@Ten", qt.TenQuyTac.Trim());
                        cmd.AddParam("@Loai", qt.LoaiHinhGiangDay);
                        cmd.AddParam("@Tu", qt.SiSoTu);
                        cmd.AddParam("@Den", qt.SiSoDen);
                        cmd.AddParam("@HS", qt.HeSoQuyDoi);
                        cmd.AddParam("@HLTu", qt.NgayHieuLucTu);
                        cmd.ExecuteNonQuery();
                    }
                }
                return KetQuaThaoTac.Ok("Đã lưu quy tắc quy đổi.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi lưu quy tắc: " + ex.Message); }
        }

        public KetQuaThaoTac XoaQuyTacQuyDoi(int idQuyTac)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand("DELETE FROM SYS_QuyTacQuyDoi WHERE ID_QuyTac = @ID;"))
                {
                    cmd.AddParam("@ID", idQuyTac);
                    cmd.ExecuteNonQuery();
                }
                return KetQuaThaoTac.Ok("Đã xóa quy tắc quy đổi.");
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa quy tắc: " + ex.Message); }
        }

        private static int TinhTongSiSoLopHP(SqlConnection conn, List<int> dsLop)
        {
            string placeholders = string.Join(", ", Enumerable.Range(0, dsLop.Count).Select(i => "@l" + i));
            using (SqlCommand cmd = conn.CreateCommand("SELECT COALESCE(SUM(SiSoDangKy), 0) FROM LopHocPhan WHERE ID_LopHocPhan IN (" + placeholders + ");"))
            {
                for (int i = 0; i < dsLop.Count; i++) cmd.AddParam("@l" + i, dsLop[i]);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static Dictionary<int, List<Tuple<int, string>>> LayLopHPTheoNhomHoc(SqlConnection conn)
        {
            const string sql = @"
                SELECT nl.ID_NhomHoc, lhp.ID_LopHocPhan, lhp.MaLopHocPhan
                FROM R_NhomHoc_LopHP nl
                INNER JOIN LopHocPhan lhp ON nl.ID_LopHocPhan = lhp.ID_LopHocPhan
                ORDER BY lhp.MaLopHocPhan;";

            Dictionary<int, List<Tuple<int, string>>> result = new Dictionary<int, List<Tuple<int, string>>>();
            using (SqlCommand cmd = conn.CreateCommand(sql))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int idNhom = r.GetInt(0);
                    List<Tuple<int, string>> list;
                    if (!result.TryGetValue(idNhom, out list))
                    {
                        list = new List<Tuple<int, string>>();
                        result[idNhom] = list;
                    }
                    list.Add(Tuple.Create(r.GetInt(1), r.GetStr(2)));
                }
            }
            return result;
        }
    }
}
