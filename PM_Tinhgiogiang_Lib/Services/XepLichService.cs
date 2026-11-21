using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using PM_Tinhgiogiang_Lib.DataAccess;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib.Services
{
    /// <summary>
    /// Dịch vụ Xếp lịch giảng dạy (10 tiết/ngày), kiểm tra xung đột, xếp lịch lặp lại cách tuần,
    /// nhân bản nhiều tuần và sắp xếp lại bài học theo dòng thời gian trên SQL Server.
    /// </summary>
    public class XepLichService
    {
        private readonly DatabaseHelper _dbHelper;

        public XepLichService(DatabaseHelper dbHelper = null)
        {
            _dbHelper = dbHelper ?? new DatabaseHelper();
        }

        public List<BuoiHocChiTiet> GetDanhSachBuoiHoc(int? idHocKy = null)
        {
            const string sql = @"
                SELECT bh.ID_BuoiHoc, bh.ID_NhomHoc, nh.MaNhomHoc, nh.TenNhomHoc,
                       mh.TenMonHoc, bh.NgayHoc, bh.BuoiTrongNgay, bh.SoTietBuoi,
                       bh.LoaiHinhGiangDay, bh.DayBangNgoaiNgu, bh.TongSiSoBuoi,
                       bh.TrangThai, bh.GhiChu,
                       bh.ID_BaiHoc, bai.TenBaiHoc,
                       bh.ID_PhongHoc, ph.MaPhong, ph.TenPhong, ph.CoSo
                FROM DM_BuoiHoc bh
                INNER JOIN DM_NhomHoc nh ON bh.ID_NhomHoc = nh.ID_NhomHoc
                INNER JOIN DM_MonHoc mh ON nh.MaMonHoc = mh.MaMonHoc
                LEFT JOIN DM_BaiHoc bai ON bh.ID_BaiHoc = bai.ID_BaiHoc
                LEFT JOIN DM_PhongHoc ph ON bh.ID_PhongHoc = ph.ID_PhongHoc
                WHERE (@ID_HocKy IS NULL OR nh.ID_HocKy = @ID_HocKy)
                ORDER BY bh.NgayHoc DESC, bh.ID_BuoiHoc DESC;";

            using (SqlConnection conn = _dbHelper.OpenConnection())
            {
                List<BuoiHocChiTiet> danhSach = new List<BuoiHocChiTiet>();
                using (SqlCommand cmd = conn.CreateCommand(sql))
                {
                    cmd.AddParam("@ID_HocKy", idHocKy);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            danhSach.Add(new BuoiHocChiTiet
                            {
                                ID_BuoiHoc = r.GetInt(0),
                                ID_NhomHoc = r.GetInt(1),
                                MaNhomHoc = r.GetStr(2),
                                TenNhomHoc = r.GetStr(3),
                                TenMonHoc = r.GetStr(4),
                                NgayHoc = r.GetDateStr(5),
                                BuoiTrongNgay = r.GetStr(6),
                                SoTietBuoi = r.GetInt(7),
                                LoaiHinhGiangDay = r.GetStr(8),
                                DayBangNgoaiNgu = r.GetBool(9),
                                TongSiSoBuoi = r.GetInt(10),
                                TrangThai = r.GetNullableStr(11) ?? "Draft",
                                GhiChu = r.GetNullableStr(12),
                                ID_BaiHoc = r.GetNullableInt(13),
                                TenBaiHoc = r.GetNullableStr(14),
                                ID_PhongHoc = r.GetNullableInt(15),
                                MaPhong = r.GetNullableStr(16),
                                TenPhong = r.GetNullableStr(17),
                                CoSoPhong = r.GetNullableStr(18)
                            });
                        }
                    }
                }

                if (danhSach.Count == 0) return danhSach;

                Dictionary<int, string> lopTheoNhom = LayMaLopTheoNhomHoc(conn);
                Dictionary<int, List<Tuple<int, int, string, string>>> tietTheoBuoi = LayThongTinTietTheoBuoi(conn);
                Dictionary<int, Tuple<string, List<int>, double, double>> gvTheoBuoi = LayThongTinGiangVienTheoBuoi(conn);

                foreach (BuoiHocChiTiet item in danhSach)
                {
                    string maLops;
                    if (lopTheoNhom.TryGetValue(item.ID_NhomHoc, out maLops))
                        item.DanhSachMaLopHP = maLops;

                    List<Tuple<int, int, string, string>> tiets;
                    if (tietTheoBuoi.TryGetValue(item.ID_BuoiHoc, out tiets) && tiets.Count > 0)
                    {
                        item.DanhSachID_Tiet = tiets.Select(t => t.Item1).ToList();
                        item.DanhSachSoTiet = tiets.Select(t => t.Item2).ToList();
                        item.ChuoiTietHoc = DinhDangChuoiTiet(item.DanhSachSoTiet);
                        int minT = item.DanhSachSoTiet.Min();
                        int maxT = item.DanhSachSoTiet.Max();
                        item.ChuoiTietSo = minT == maxT ? minT.ToString() : (minT + " - " + maxT);
                        item.GioBatDau = tiets[0].Item3;
                        item.GioKetThuc = tiets[tiets.Count - 1].Item4;
                    }

                    Tuple<string, List<int>, double, double> gvInfo;
                    if (gvTheoBuoi.TryGetValue(item.ID_BuoiHoc, out gvInfo))
                    {
                        item.DanhSachGiangVien = gvInfo.Item1;
                        item.DanhSachID_GV = gvInfo.Item2;
                        item.HeSoQuyDoi = gvInfo.Item3;
                        item.GioChuanQuyDoi = gvInfo.Item4;
                    }
                }

                return danhSach;
            }
        }

        public BaiHoc LayBaiHocTiepTheoChuaXep(int idNhomHoc)
        {
            if (idNhomHoc <= 0) return null;
            const string sql = @"
                SELECT TOP 1 bh.ID_BaiHoc, bh.MaMonHoc, bh.ThuTuBai, bh.TenBaiHoc,
                       bh.LoaiBai, bh.SoTiet, bh.ID_GV_MacDinh, gv.HoTen
                FROM DM_NhomHoc nh
                INNER JOIN DM_BaiHoc bh ON nh.MaMonHoc = bh.MaMonHoc
                LEFT JOIN DM_GiangVien gv ON bh.ID_GV_MacDinh = gv.ID_GV
                WHERE nh.ID_NhomHoc = @ID_NhomHoc
                  AND bh.ID_BaiHoc NOT IN (
                      SELECT DISTINCT ID_BaiHoc FROM DM_BuoiHoc
                      WHERE ID_NhomHoc = @ID_NhomHoc AND ID_BaiHoc IS NOT NULL AND (TrangThai IS NULL OR TrangThai <> 'Huy')
                  )
                ORDER BY bh.ThuTuBai ASC, bh.ID_BaiHoc ASC;";

            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID_NhomHoc", idNhomHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new BaiHoc
                    {
                        ID_BaiHoc = r.GetInt(0),
                        MaMonHoc = r.GetStr(1),
                        ThuTuBai = r.GetInt(2),
                        TenBaiHoc = r.GetStr(3),
                        LoaiBai = r.GetStr(4),
                        SoTiet = r.GetInt(5),
                        ID_GV_MacDinh = r.GetNullableInt(6),
                        TenGiangVienMacDinh = r.GetNullableStr(7)
                    };
                }
            }
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
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    Tuple<string, int, int?, bool> nhomInfo = LayThongTinNhomHoc(conn, model.ID_NhomHoc);
                    if (nhomInfo == null) return KetQuaThaoTac.Loi("Không tìm thấy Nhóm học #" + model.ID_NhomHoc + ".");

                    List<TietHoc> tiets = LayThongTinTietTheoID(conn, dsTiet);
                    int soTietBuoi = model.SoTietBuoi > 0 ? model.SoTietBuoi : tiets.Count;
                    string buoiTrongNgay = string.IsNullOrWhiteSpace(model.BuoiTrongNgay) ? tiets[0].BuoiTrongNgay : model.BuoiTrongNgay.Trim();
                    int tongSiSo = model.TongSiSoBuoi > 0 ? model.TongSiSoBuoi : nhomInfo.Item2;
                    bool dayNgoaiNgu = model.DayBangNgoaiNgu || nhomInfo.Item4;
                    string ngayStr = model.NgayHoc.Trim();
                    List<int> dsGV = dsPC.Select(x => x.ID_GV).ToList();

                    string loi = KiemTraXungDot(conn, 0, ngayStr, ngayHocDate, buoiTrongNgay, dsTiet, dsGV, model.ID_PhongHoc, model.ID_NhomHoc, tongSiSo);
                    if (loi != null) return KetQuaThaoTac.Loi(loi);

                    double heSo = TinhGioService.TraHeSoQuyDoi(conn, model.LoaiHinhGiangDay.Trim(), tongSiSo, ngayStr, dayNgoaiNgu, nhomInfo.Item3);

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            const string insertBuoiSql = @"
                                INSERT INTO DM_BuoiHoc
                                    (ID_NhomHoc, ID_BaiHoc, NgayHoc, BuoiTrongNgay, ID_PhongHoc, SoTietBuoi,
                                     LoaiHinhGiangDay, DayBangNgoaiNgu, TongSiSoBuoi, TrangThai, ID_BuoiHocGoc, GhiChu)
                                VALUES
                                    (@ID_NhomHoc, @ID_BaiHoc, @NgayHoc, @BuoiTrongNgay, @ID_PhongHoc, @SoTietBuoi,
                                     @LoaiHinhGiangDay, @DayBangNgoaiNgu, @TongSiSoBuoi, @TrangThai, @ID_BuoiHocGoc, @GhiChu);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int idBuoiHoc;
                            using (SqlCommand cmd = conn.CreateCommand(insertBuoiSql, tran))
                            {
                                cmd.AddParam("@ID_NhomHoc", model.ID_NhomHoc);
                                cmd.AddParam("@ID_BaiHoc", model.ID_BaiHoc);
                                cmd.AddParam("@NgayHoc", ngayStr);
                                cmd.AddParam("@BuoiTrongNgay", buoiTrongNgay);
                                cmd.AddParam("@ID_PhongHoc", model.ID_PhongHoc);
                                cmd.AddParam("@SoTietBuoi", soTietBuoi);
                                cmd.AddParam("@LoaiHinhGiangDay", model.LoaiHinhGiangDay.Trim());
                                cmd.AddParam("@DayBangNgoaiNgu", dayNgoaiNgu ? 1 : 0);
                                cmd.AddParam("@TongSiSoBuoi", tongSiSo);
                                cmd.AddParam("@TrangThai", string.IsNullOrWhiteSpace(model.TrangThai) ? "Draft" : model.TrangThai.Trim());
                                cmd.AddParam("@ID_BuoiHocGoc", model.ID_BuoiHocGoc);
                                cmd.AddParam("@GhiChu", string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim());
                                idBuoiHoc = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (int idTiet in dsTiet)
                            {
                                using (SqlCommand cmd = conn.CreateCommand("INSERT INTO R_BuoiHoc_Tiet (ID_BuoiHoc, ID_TietHoc) VALUES (@B, @T);", tran))
                                {
                                    cmd.AddParam("@B", idBuoiHoc);
                                    cmd.AddParam("@T", idTiet);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            for (int i = 0; i < dsPC.Count; i++)
                            {
                                PhanCongGiangVienInput pc = dsPC[i];
                                double soTietGV = pc.SoTietGiangDay > 0 ? pc.SoTietGiangDay : soTietBuoi;
                                double gioChuan = Math.Round(soTietGV * heSo, 2);

                                using (SqlCommand cmd = conn.CreateCommand(@"
                                    INSERT INTO R_PhanCongGiangDay (ID_BuoiHoc, ID_GV, VaiTro, SoTietGiangDay, HeSoQuyDoi, GioChuanQuyDoi, DaXacNhanHoanThanh)
                                    VALUES (@B, @GV, @VT, @ST, @HS, @GC, 0);", tran))
                                {
                                    cmd.AddParam("@B", idBuoiHoc);
                                    cmd.AddParam("@GV", pc.ID_GV);
                                    cmd.AddParam("@VT", string.IsNullOrWhiteSpace(pc.VaiTro) ? (i == 0 ? "Chinh" : "Phu") : pc.VaiTro);
                                    cmd.AddParam("@ST", soTietGV);
                                    cmd.AddParam("@HS", heSo);
                                    cmd.AddParam("@GC", gioChuan);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                            return KetQuaThaoTac.Ok("Xếp lịch thành công ngày " + ngayStr + " (" + DinhDangChuoiTiet(tiets.Select(t => t.SoTiet)) + ").", idBuoiHoc);
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xếp lịch: " + ex.Message); }
        }

        public KetQuaXepLichLapLai XepLichNhanhLapLai(XepLichLapLaiRequest req)
        {
            KetQuaXepLichLapLai kq = new KetQuaXepLichLapLai();
            if (req == null || req.ID_NhomHoc <= 0)
            {
                kq.ThongBaoTongQuat = "Vui lòng chọn Nhóm học.";
                return kq;
            }

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

                if (res.ThanhCong)
                {
                    kq.SoBuoiThanhCong++;
                    kq.ChiTietThanhCong.Add(ngayStr + ": Hợp lệ");
                }
                else
                {
                    kq.SoBuoiBiTrung++;
                    kq.ChiTietBiTrung.Add(ngayStr + ": " + res.ThongBao);
                }
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
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string maMon;
                    using (SqlCommand c = conn.CreateCommand("SELECT MaMonHoc FROM DM_NhomHoc WHERE ID_NhomHoc = @ID;"))
                    {
                        c.AddParam("@ID", idNhomHoc);
                        object val = c.ExecuteScalar();
                        maMon = val != null ? val.ToString() : null;
                    }
                    if (string.IsNullOrWhiteSpace(maMon)) return KetQuaThaoTac.Loi("Không tìm thấy nhóm học.");

                    List<int> dsIdBuoi = new List<int>();
                    using (SqlCommand c = conn.CreateCommand(@"
                        SELECT bh.ID_BuoiHoc, bh.NgayHoc, COALESCE(MIN(th.SoTiet), 99) AS TietBD
                        FROM DM_BuoiHoc bh
                        LEFT JOIN R_BuoiHoc_Tiet bt ON bh.ID_BuoiHoc = bt.ID_BuoiHoc
                        LEFT JOIN DM_TietHoc th ON bt.ID_TietHoc = th.ID_TietHoc
                        WHERE bh.ID_NhomHoc = @ID AND (bh.TrangThai IS NULL OR bh.TrangThai <> 'Huy')
                        GROUP BY bh.ID_BuoiHoc, bh.NgayHoc
                        ORDER BY bh.NgayHoc ASC, TietBD ASC, bh.ID_BuoiHoc ASC;"))
                    {
                        c.AddParam("@ID", idNhomHoc);
                        using (SqlDataReader r = c.ExecuteReader())
                        {
                            while (r.Read()) dsIdBuoi.Add(r.GetInt(0));
                        }
                    }

                    List<int> dsIdBai = new List<int>();
                    using (SqlCommand c = conn.CreateCommand("SELECT ID_BaiHoc FROM DM_BaiHoc WHERE MaMonHoc = @M ORDER BY ThuTuBai ASC, ID_BaiHoc ASC;"))
                    {
                        c.AddParam("@M", maMon);
                        using (SqlDataReader r = c.ExecuteReader())
                        {
                            while (r.Read()) dsIdBai.Add(r.GetInt(0));
                        }
                    }

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < dsIdBuoi.Count; i++)
                            {
                                int? idBai = i < dsIdBai.Count ? (int?)dsIdBai[i] : null;
                                using (SqlCommand u = conn.CreateCommand("UPDATE DM_BuoiHoc SET ID_BaiHoc = @Bai WHERE ID_BuoiHoc = @Buoi;", tran))
                                {
                                    u.AddParam("@Bai", idBai);
                                    u.AddParam("@Buoi", dsIdBuoi[i]);
                                    u.ExecuteNonQuery();
                                }
                            }
                            tran.Commit();
                            return KetQuaThaoTac.Ok("Đã sắp xếp lại thứ tự bài học theo dòng thời gian.");
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi sắp xếp bài học: " + ex.Message); }
        }

        public KetQuaThaoTac XoaBuoiHoc(int idBuoiHoc)
        {
            try
            {
                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        string[] sqls = new[]
                        {
                            "UPDATE DM_BuoiHoc SET ID_BuoiHocGoc = NULL WHERE ID_BuoiHocGoc = @ID;",
                            "DELETE FROM R_PhanCongGiangDay WHERE ID_BuoiHoc = @ID;",
                            "DELETE FROM R_BuoiHoc_Tiet WHERE ID_BuoiHoc = @ID;",
                            "DELETE FROM DM_BuoiHoc WHERE ID_BuoiHoc = @ID;"
                        };
                        foreach (string s in sqls)
                        {
                            using (SqlCommand cmd = conn.CreateCommand(s, tran))
                            {
                                cmd.AddParam("@ID", idBuoiHoc);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return KetQuaThaoTac.Ok("Đã xóa buổi học #" + idBuoiHoc + ".", idBuoiHoc);
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xóa buổi học: " + ex.Message); }
        }

        public List<PhanCongGiangDay> LayPhanCongTheoBuoi(int idBuoiHoc)
        {
            List<PhanCongGiangDay> list = new List<PhanCongGiangDay>();
            const string sql = @"
                SELECT ID_PhanCong, ID_BuoiHoc, ID_GV, VaiTro, SoTietGiangDay, HeSoQuyDoi, GioChuanQuyDoi, DaXacNhanHoanThanh
                FROM R_PhanCongGiangDay
                WHERE ID_BuoiHoc = @ID
                ORDER BY CASE WHEN VaiTro = 'Chinh' THEN 0 ELSE 1 END, ID_PhanCong;";

            using (SqlConnection conn = _dbHelper.OpenConnection())
            using (SqlCommand cmd = conn.CreateCommand(sql))
            {
                cmd.AddParam("@ID", idBuoiHoc);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new PhanCongGiangDay
                        {
                            ID_PhanCong = r.GetInt(0),
                            ID_BuoiHoc = r.GetInt(1),
                            ID_GV = r.GetInt(2),
                            VaiTro = r.GetNullableStr(3) ?? "Chinh",
                            SoTietGiangDay = r.GetDoubleVal(4),
                            HeSoQuyDoi = r.GetDoubleVal(5),
                            GioChuanQuyDoi = r.GetDoubleVal(6),
                            DaXacNhanHoanThanh = r.GetBool(7)
                        });
                    }
                }
            }
            return list;
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
                using (SqlConnection conn = _dbHelper.OpenConnection())
                {
                    string ngayHoc = null;
                    string loaiHinh = "LT";
                    int tongSiSo = 0;
                    int soTietBuoi = 0;
                    bool ngoaiNgu = false;
                    int? idHeDaoTao = null;

                    using (SqlCommand c = conn.CreateCommand(@"
                        SELECT bh.NgayHoc, bh.LoaiHinhGiangDay, bh.TongSiSoBuoi, bh.SoTietBuoi, bh.DayBangNgoaiNgu, mh.ID_HeDaoTao
                        FROM DM_BuoiHoc bh
                        INNER JOIN DM_NhomHoc nh ON bh.ID_NhomHoc = nh.ID_NhomHoc
                        INNER JOIN DM_MonHoc mh ON nh.MaMonHoc = mh.MaMonHoc
                        WHERE bh.ID_BuoiHoc = @ID;"))
                    {
                        c.AddParam("@ID", idBuoiHoc);
                        using (SqlDataReader r = c.ExecuteReader())
                        {
                            if (!r.Read()) return KetQuaThaoTac.Loi("Không tìm thấy buổi học #" + idBuoiHoc + ".");
                            ngayHoc = r.GetDateStr(0);
                            loaiHinh = r.GetStr(1);
                            tongSiSo = r.GetInt(2);
                            soTietBuoi = r.GetInt(3);
                            ngoaiNgu = r.GetBool(4);
                            idHeDaoTao = r.GetNullableInt(5);
                        }
                    }

                    double heSo = TinhGioService.TraHeSoQuyDoi(conn, loaiHinh, tongSiSo, ngayHoc, ngoaiNgu, idHeDaoTao);

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand del = conn.CreateCommand("DELETE FROM R_PhanCongGiangDay WHERE ID_BuoiHoc = @ID;", tran))
                            {
                                del.AddParam("@ID", idBuoiHoc);
                                del.ExecuteNonQuery();
                            }

                            for (int i = 0; i < dsPC.Count; i++)
                            {
                                PhanCongGiangVienInput pc = dsPC[i];
                                double st = pc.SoTietGiangDay > 0 ? pc.SoTietGiangDay : soTietBuoi;
                                double gc = Math.Round(st * heSo, 2);
                                using (SqlCommand ins = conn.CreateCommand(@"
                                    INSERT INTO R_PhanCongGiangDay (ID_BuoiHoc, ID_GV, VaiTro, SoTietGiangDay, HeSoQuyDoi, GioChuanQuyDoi, DaXacNhanHoanThanh)
                                    VALUES (@B, @GV, @VT, @ST, @HS, @GC, 0);", tran))
                                {
                                    ins.AddParam("@B", idBuoiHoc);
                                    ins.AddParam("@GV", pc.ID_GV);
                                    ins.AddParam("@VT", string.IsNullOrWhiteSpace(pc.VaiTro) ? (i == 0 ? "Chinh" : "Phu") : pc.VaiTro);
                                    ins.AddParam("@ST", st);
                                    ins.AddParam("@HS", heSo);
                                    ins.AddParam("@GC", gc);
                                    ins.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                            return KetQuaThaoTac.Ok("Đã cập nhật phân công giảng dạy cho buổi #" + idBuoiHoc + ".", idBuoiHoc);
                        }
                        catch { tran.Rollback(); throw; }
                    }
                }
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

            // 1. Trùng giảng viên / Trùng phòng / Trùng nhóm trong cùng ngày + tiết
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

                    // Trùng giảng viên
                    List<int> gvTrung = a.DanhSachID_GV.Intersect(b.DanhSachID_GV).ToList();
                    if (gvTrung.Count > 0)
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

                    // Trùng phòng
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

                    // Trùng nhóm học
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

            // 2. Vượt sức chứa phòng + Giảng viên bận + Cảnh báo thiếu phòng/GV
            using (SqlConnection conn = _dbHelper.OpenConnection())
            {
                Dictionary<int, int> sucChuaMap = new Dictionary<int, int>();
                using (SqlCommand c = conn.CreateCommand("SELECT ID_PhongHoc, COALESCE(SucChua, 0) FROM DM_PhongHoc;"))
                using (SqlDataReader r = c.ExecuteReader())
                {
                    while (r.Read()) sucChuaMap[r.GetInt(0)] = r.GetInt(1);
                }

                List<Tuple<int, string, string, int?, string, string>> rangBuocs = new List<Tuple<int, string, string, int?, string, string>>();
                using (SqlCommand c = conn.CreateCommand("SELECT ID_GV, NgayBD, NgayKT, ThuTrongTuan, BuoiTrongNgay, LyDo FROM R_GV_RangBuoc;"))
                using (SqlDataReader r = c.ExecuteReader())
                {
                    while (r.Read())
                    {
                        rangBuocs.Add(Tuple.Create(
                            r.GetInt(0), r.GetDateStr(1), r.GetDateStr(2),
                            r.GetNullableInt(3), r.GetNullableStr(4), r.GetNullableStr(5)));
                    }
                }

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
                            foreach (Tuple<int, string, string, int?, string, string> rb in rangBuocs)
                            {
                                if (rb.Item1 != idGv) continue;
                                if (string.Compare(b.NgayHoc, rb.Item2, StringComparison.Ordinal) < 0 ||
                                    string.Compare(b.NgayHoc, rb.Item3, StringComparison.Ordinal) > 0) continue;
                                if (rb.Item4.HasValue && rb.Item4.Value != thuVN) continue;
                                if (!string.IsNullOrWhiteSpace(rb.Item5) && !string.Equals(rb.Item5, b.BuoiTrongNgay, StringComparison.OrdinalIgnoreCase)) continue;

                                ketQua.Add(new XungDotLichItem
                                {
                                    MucDo = "CanhBao",
                                    LoaiXungDot = "GV có lịch bận",
                                    ID_BuoiHoc = b.ID_BuoiHoc,
                                    NgayTiet = ngayTiet,
                                    DoiTuong1 = b.DanhSachGiangVien,
                                    DoiTuong2 = b.MaNhomHoc,
                                    MoTa = "Trùng ràng buộc bận của giảng viên: " + (rb.Item6 ?? "Bận")
                                });
                            }
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
                const string sql = @"
                    UPDATE bh
                    SET bh.TrangThai = 'Published'
                    FROM DM_BuoiHoc bh
                    INNER JOIN DM_NhomHoc nh ON bh.ID_NhomHoc = nh.ID_NhomHoc
                    WHERE (bh.TrangThai IS NULL OR bh.TrangThai = 'Draft')
                      AND (@ID_HocKy IS NULL OR nh.ID_HocKy = @ID_HocKy);";

                using (SqlConnection conn = _dbHelper.OpenConnection())
                using (SqlCommand cmd = conn.CreateCommand(sql))
                {
                    cmd.AddParam("@ID_HocKy", idHocKy);
                    int affected = cmd.ExecuteNonQuery();
                    return KetQuaThaoTac.Ok("Đã xuất bản lịch giảng dạy thành công (" + affected + " buổi chuyển sang Published).");
                }
            }
            catch (Exception ex) { return KetQuaThaoTac.Loi("Lỗi xuất bản lịch: " + ex.Message); }
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

        private static string KiemTraXungDot(
            SqlConnection conn, int idBuoiBoQua, string ngayStr, DateTime ngayDt, string buoiTrongNgay,
            List<int> dsTiet, List<int> dsGV, int? idPhong, int idNhom, int tongSiSo)
        {
            string tPh = string.Join(", ", Enumerable.Range(0, dsTiet.Count).Select(i => "@t" + i));
            string gPh = string.Join(", ", Enumerable.Range(0, dsGV.Count).Select(i => "@g" + i));

            // 1. Trùng GV
            using (SqlCommand cmd = conn.CreateCommand(@"
                SELECT TOP 1 gv.HoTen, th.SoTiet FROM DM_BuoiHoc bh
                INNER JOIN R_BuoiHoc_Tiet bt ON bh.ID_BuoiHoc = bt.ID_BuoiHoc
                INNER JOIN DM_TietHoc th ON bt.ID_TietHoc = th.ID_TietHoc
                INNER JOIN R_PhanCongGiangDay pc ON bh.ID_BuoiHoc = pc.ID_BuoiHoc
                INNER JOIN DM_GiangVien gv ON pc.ID_GV = gv.ID_GV
                WHERE bh.ID_BuoiHoc <> @BQ AND bh.NgayHoc = @Ngay AND (bh.TrangThai IS NULL OR bh.TrangThai <> 'Huy')
                  AND bt.ID_TietHoc IN (" + tPh + ") AND pc.ID_GV IN (" + gPh + ");"))
            {
                cmd.AddParam("@BQ", idBuoiBoQua); cmd.AddParam("@Ngay", ngayStr);
                for (int i = 0; i < dsTiet.Count; i++) cmd.AddParam("@t" + i, dsTiet[i]);
                for (int i = 0; i < dsGV.Count; i++) cmd.AddParam("@g" + i, dsGV[i]);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read()) return "Trùng lịch giảng viên " + r.GetStr(0) + " tại Tiết " + r.GetInt(1) + " ngày " + ngayStr + ".";
                }
            }

            // 2. Trùng phòng & Sức chứa
            if (idPhong.HasValue)
            {
                using (SqlCommand cmd = conn.CreateCommand(@"
                    SELECT TOP 1 ph.MaPhong, th.SoTiet FROM DM_BuoiHoc bh
                    INNER JOIN R_BuoiHoc_Tiet bt ON bh.ID_BuoiHoc = bt.ID_BuoiHoc
                    INNER JOIN DM_TietHoc th ON bt.ID_TietHoc = th.ID_TietHoc
                    INNER JOIN DM_PhongHoc ph ON bh.ID_PhongHoc = ph.ID_PhongHoc
                    WHERE bh.ID_BuoiHoc <> @BQ AND bh.NgayHoc = @Ngay AND (bh.TrangThai IS NULL OR bh.TrangThai <> 'Huy')
                      AND bh.ID_PhongHoc = @P AND bt.ID_TietHoc IN (" + tPh + ");"))
                {
                    cmd.AddParam("@BQ", idBuoiBoQua); cmd.AddParam("@Ngay", ngayStr); cmd.AddParam("@P", idPhong.Value);
                    for (int i = 0; i < dsTiet.Count; i++) cmd.AddParam("@t" + i, dsTiet[i]);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read()) return "Trùng phòng " + r.GetStr(0) + " tại Tiết " + r.GetInt(1) + " ngày " + ngayStr + ".";
                    }
                }

                using (SqlCommand cmd = conn.CreateCommand("SELECT MaPhong, SucChua FROM DM_PhongHoc WHERE ID_PhongHoc = @P;"))
                {
                    cmd.AddParam("@P", idPhong.Value);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read() && !r.IsDBNull(1) && tongSiSo > r.GetInt(1))
                            return "Vượt sức chứa phòng " + r.GetStr(0) + " (" + r.GetInt(1) + " chỗ < " + tongSiSo + " SV).";
                    }
                }
            }

            // 3. Ràng buộc bận GV
            int thuVN = ngayDt.DayOfWeek == DayOfWeek.Sunday ? 8 : (int)ngayDt.DayOfWeek + 1;
            using (SqlCommand cmd = conn.CreateCommand(@"
                SELECT TOP 1 gv.HoTen, rb.LyDo FROM R_GV_RangBuoc rb
                INNER JOIN DM_GiangVien gv ON rb.ID_GV = gv.ID_GV
                WHERE rb.ID_GV IN (" + gPh + @") AND @Ngay BETWEEN rb.NgayBD AND rb.NgayKT
                  AND (rb.ThuTrongTuan IS NULL OR rb.ThuTrongTuan = @Thu)
                  AND (rb.BuoiTrongNgay IS NULL OR rb.BuoiTrongNgay = '' OR rb.BuoiTrongNgay = @Buoi);"))
            {
                cmd.AddParam("@Ngay", ngayStr); cmd.AddParam("@Thu", thuVN); cmd.AddParam("@Buoi", buoiTrongNgay);
                for (int i = 0; i < dsGV.Count; i++) cmd.AddParam("@g" + i, dsGV[i]);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read()) return "Giảng viên " + r.GetStr(0) + " có lịch bận ngày " + ngayStr + ": " + r.GetNullableStr(1);
                }
            }

            return null;
        }

        private static Tuple<string, int, int?, bool> LayThongTinNhomHoc(SqlConnection conn, int idNhom)
        {
            using (SqlCommand cmd = conn.CreateCommand(@"
                SELECT nh.MaNhomHoc, nh.TongSiSoNhom, mh.ID_HeDaoTao, mh.DayBangNgoaiNgu
                FROM DM_NhomHoc nh INNER JOIN DM_MonHoc mh ON nh.MaMonHoc = mh.MaMonHoc
                WHERE nh.ID_NhomHoc = @ID;"))
            {
                cmd.AddParam("@ID", idNhom);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return Tuple.Create(r.GetStr(0), r.GetInt(1), r.GetNullableInt(2), r.GetBool(3));
                }
            }
        }

        private static List<TietHoc> LayThongTinTietTheoID(SqlConnection conn, List<int> dsTiet)
        {
            string ph = string.Join(", ", Enumerable.Range(0, dsTiet.Count).Select(i => "@t" + i));
            List<TietHoc> res = new List<TietHoc>();
            using (SqlCommand cmd = conn.CreateCommand("SELECT ID_TietHoc, SoTiet, BuoiTrongNgay, GioBatDau, GioKetThuc FROM DM_TietHoc WHERE ID_TietHoc IN (" + ph + ") ORDER BY SoTiet;"))
            {
                for (int i = 0; i < dsTiet.Count; i++) cmd.AddParam("@t" + i, dsTiet[i]);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        res.Add(new TietHoc { ID_TietHoc = r.GetInt(0), SoTiet = r.GetInt(1), BuoiTrongNgay = r.GetStr(2), GioBatDau = r.GetTimeStr(3), GioKetThuc = r.GetTimeStr(4) });
                }
            }
            return res;
        }

        private static Dictionary<int, string> LayMaLopTheoNhomHoc(SqlConnection conn)
        {
            Dictionary<int, List<string>> map = new Dictionary<int, List<string>>();
            using (SqlCommand cmd = conn.CreateCommand("SELECT nl.ID_NhomHoc, lhp.MaLopHocPhan FROM R_NhomHoc_LopHP nl INNER JOIN LopHocPhan lhp ON nl.ID_LopHocPhan = lhp.ID_LopHocPhan ORDER BY lhp.MaLopHocPhan;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int id = r.GetInt(0);
                    List<string> l;
                    if (!map.TryGetValue(id, out l)) { l = new List<string>(); map[id] = l; }
                    l.Add(r.GetStr(1));
                }
            }
            return map.ToDictionary(kv => kv.Key, kv => string.Join(", ", kv.Value));
        }

        private static Dictionary<int, List<Tuple<int, int, string, string>>> LayThongTinTietTheoBuoi(SqlConnection conn)
        {
            Dictionary<int, List<Tuple<int, int, string, string>>> res = new Dictionary<int, List<Tuple<int, int, string, string>>>();
            using (SqlCommand cmd = conn.CreateCommand("SELECT bt.ID_BuoiHoc, bt.ID_TietHoc, th.SoTiet, th.GioBatDau, th.GioKetThuc FROM R_BuoiHoc_Tiet bt INNER JOIN DM_TietHoc th ON bt.ID_TietHoc = th.ID_TietHoc ORDER BY th.SoTiet;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int id = r.GetInt(0);
                    List<Tuple<int, int, string, string>> l;
                    if (!res.TryGetValue(id, out l)) { l = new List<Tuple<int, int, string, string>>(); res[id] = l; }
                    l.Add(Tuple.Create(r.GetInt(1), r.GetInt(2), r.GetTimeStr(3), r.GetTimeStr(4)));
                }
            }
            return res;
        }

        private static Dictionary<int, Tuple<string, List<int>, double, double>> LayThongTinGiangVienTheoBuoi(SqlConnection conn)
        {
            Dictionary<int, Tuple<List<string>, List<int>, double, double>> map = new Dictionary<int, Tuple<List<string>, List<int>, double, double>>();
            using (SqlCommand cmd = conn.CreateCommand("SELECT pc.ID_BuoiHoc, pc.ID_GV, gv.HoTen, pc.SoTietGiangDay, pc.HeSoQuyDoi, pc.GioChuanQuyDoi FROM R_PhanCongGiangDay pc INNER JOIN DM_GiangVien gv ON pc.ID_GV = gv.ID_GV ORDER BY CASE WHEN pc.VaiTro='Chinh' THEN 0 ELSE 1 END, gv.HoTen;"))
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int id = r.GetInt(0);
                    Tuple<List<string>, List<int>, double, double> e;
                    if (!map.TryGetValue(id, out e))
                        e = Tuple.Create(new List<string>(), new List<int>(), r.GetDoubleVal(4), 0.0);
                    e.Item1.Add(r.GetStr(2) + " (" + r.GetDoubleVal(3).ToString("0.##") + "t)");
                    e.Item2.Add(r.GetInt(1));
                    map[id] = Tuple.Create(e.Item1, e.Item2, e.Item3, e.Item4 + r.GetDoubleVal(5));
                }
            }
            return map.ToDictionary(kv => kv.Key, kv => Tuple.Create(string.Join(", ", kv.Value.Item1), kv.Value.Item2, kv.Value.Item3, Math.Round(kv.Value.Item4, 2)));
        }
    }
}
