using System;
using System.Collections.Generic;

namespace PM_Tinhgiogiang_Lib.Models
{
    public class MonHoc
    {
        public string MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }
        public int ID_BoMon { get; set; }
        public int? ID_HeDaoTao { get; set; }
        public double SoTinChi { get; set; }
        public int SoTietLT { get; set; }
        public int SoTietTH { get; set; }
        public bool DayBangNgoaiNgu { get; set; }

        public string TenBoMon { get; set; }
        public string TenHeDaoTao { get; set; }
    }

    public class BaiHoc
    {
        public int ID_BaiHoc { get; set; }
        public string MaMonHoc { get; set; }
        public int ThuTuBai { get; set; }
        public string TenBaiHoc { get; set; }
        public string LoaiBai { get; set; }
        public int SoTiet { get; set; }
        public int? ID_GV_MacDinh { get; set; }

        public string TenMonHoc { get; set; }
        public string TenGiangVienMacDinh { get; set; }
    }

    public class LopHocPhan
    {
        public int ID_LopHocPhan { get; set; }
        public string MaLopHocPhan { get; set; }
        public string MaMonHoc { get; set; }
        public int ID_HocKy { get; set; }
        public string LoaiLopHP { get; set; }
        public int SiSoDangKy { get; set; }
        public string NgayBatDau { get; set; }
        public string NgayKetThuc { get; set; }
        public string TrangThai { get; set; }

        public string TenMonHoc { get; set; }
        public string TenHocKy { get; set; }

        public LopHocPhan()
        {
            LoaiLopHP = "ChinhQuy";
            TrangThai = "Draft";
        }
    }

    public class NhomHoc
    {
        public int ID_NhomHoc { get; set; }
        public string MaNhomHoc { get; set; }
        public string TenNhomHoc { get; set; }
        public string MaMonHoc { get; set; }
        public int ID_HocKy { get; set; }
        public string LoaiNhom { get; set; }
        public int TongSiSoNhom { get; set; }

        public List<int> DanhSachID_LopHP { get; set; }
        public string DanhSachMaLopHP { get; set; }
        public string TenMonHoc { get; set; }
        public string TenHocKy { get; set; }

        public NhomHoc()
        {
            LoaiNhom = "LT";
            DanhSachID_LopHP = new List<int>();
        }
    }

    public class QuyTacQuyDoi
    {
        public int ID_QuyTac { get; set; }
        public string TenQuyTac { get; set; }
        public string LoaiHinhGiangDay { get; set; }
        public int? ID_HeDaoTao { get; set; }
        public int SiSoTu { get; set; }
        public int SiSoDen { get; set; }
        public double HeSoQuyDoi { get; set; }
        public string NgayHieuLucTu { get; set; }
        public string NgayHieuLucDen { get; set; }
        public string TenHeDaoTao { get; set; }

        public QuyTacQuyDoi()
        {
            LoaiHinhGiangDay = "LT";
            SiSoDen = 9999;
            HeSoQuyDoi = 1.0;
            NgayHieuLucTu = "2025-01-01";
        }
    }
}
