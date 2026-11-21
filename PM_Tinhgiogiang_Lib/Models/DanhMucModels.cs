using System;

namespace PM_Tinhgiogiang_Lib.Models
{
    public class NhomDanhMuc
    {
        public int MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string MoTa { get; set; }
    }

    public class DanhMuc
    {
        public int MaDanhMuc { get; set; }
        public int MaNhom { get; set; }
        public string TenDanhMuc { get; set; }
        public string VietTat { get; set; }
        public double? GiaTriThamSo { get; set; }
        public int ThuTu { get; set; }
        public bool KichHoat { get; set; }

        public DanhMuc()
        {
            ThuTu = 1;
            KichHoat = true;
        }
    }

    public class NamHoc
    {
        public int ID_NamHoc { get; set; }
        public string MaNH { get; set; }
        public string TenNH { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
    }

    public class HocKy
    {
        public int ID_HocKy { get; set; }
        public int ID_NamHoc { get; set; }
        public string MaHK { get; set; }
        public string TenHK { get; set; }
        public string NgayBD { get; set; }
        public string NgayKT { get; set; }
    }

    public class TietHoc
    {
        public int ID_TietHoc { get; set; }
        public int SoTiet { get; set; }
        public string BuoiTrongNgay { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }
    }
}
