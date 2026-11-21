using System;

namespace PM_Tinhgiogiang_Lib.Models
{
    public class PhongHoc
    {
        public int ID_PhongHoc { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public int? ID_LoaiPhong { get; set; }
        public string CoSo { get; set; }
        public int? SucChua { get; set; }
        public int TrangThai { get; set; }
        public string TenLoaiPhong { get; set; }

        public PhongHoc()
        {
            TrangThai = 1;
        }
    }

    public class GiangVien
    {
        public int ID_GV { get; set; }
        public string MaGV { get; set; }
        public string HoTen { get; set; }
        public int ID_BoMon { get; set; }
        public int? ID_HocHam { get; set; }
        public int? ID_HocVi { get; set; }
        public string GioiTinh { get; set; }
        public bool LaThinhGiang { get; set; }
        public string DonViCongTac { get; set; }
        public double DinhMucGioChuan { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public int TrangThai { get; set; }

        public string TenBoMon { get; set; }
        public string TenHocHam { get; set; }
        public string TenHocVi { get; set; }

        public GiangVien()
        {
            GioiTinh = "Nam";
            DinhMucGioChuan = 270;
            TrangThai = 1;
        }
    }

    public class GiangVienRangBuoc
    {
        public int ID_RangBuoc { get; set; }
        public int ID_GV { get; set; }
        public string NgayBD { get; set; }
        public string NgayKT { get; set; }
        public int? ThuTrongTuan { get; set; }
        public string BuoiTrongNgay { get; set; }
        public string LyDo { get; set; }
    }

    public class MienGiamDinhMuc
    {
        public int ID_MienGiam { get; set; }
        public int ID_GV { get; set; }
        public int ID_NamHoc { get; set; }
        public double TyLeGiamPhanTram { get; set; }
        public double SoGioGiam { get; set; }
        public string LyDo { get; set; }

        public string MaGV { get; set; }
        public string TenGiangVien { get; set; }
        public string TenNamHoc { get; set; }
    }

    public class NguoiDung
    {
        public int ID_NguoiDung { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhauHash { get; set; }
        public int? ID_GV { get; set; }
        public string VaiTro { get; set; }
        public bool TrangThai { get; set; }
    }
}
