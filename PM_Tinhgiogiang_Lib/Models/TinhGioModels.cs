using System;
using System.Collections.Generic;

namespace PM_Tinhgiogiang_Lib.Models
{
    public class NckhHoatDong
    {
        public int ID_HoatDong { get; set; }
        public int ID_NamHoc { get; set; }
        public string TenHoatDong { get; set; }
        public int ID_LoaiHoatDong { get; set; }
        public double TongGioChuanHoatDong { get; set; }
        public string TrangThai { get; set; }

        public int ID_GV { get; set; }
        public string VaiTro { get; set; }
        public double TyLePhanTram { get; set; }
        public double GioChuanQuyDoi { get; set; }

        public string TenNamHoc { get; set; }
        public string TenLoaiHoatDong { get; set; }
        public string TenGiangVien { get; set; }

        public NckhHoatDong()
        {
            TrangThai = "DaHoanThanh";
            VaiTro = "ChuNhiem";
            TyLePhanTram = 100;
        }
    }

    public class BaoCaoGioGiangDto
    {
        public string MaGV { get; set; }
        public string HoTen { get; set; }
        public string TenBoMon { get; set; }

        public double DinhMucGioChuan { get; set; }
        public double GioMienGiam { get; set; }
        public double DinhMucThucTe { get; set; }

        public int TongSoTietThucTe { get; set; }
        public double GioDungLop { get; set; }
        public double GioDeTaiHoiDong { get; set; }
        public double GioKhaoThi { get; set; }
        public double GioNCKH { get; set; }

        public double TongGioChuanQuyDoi { get; set; }
        public double GioVuotDinhMuc { get; set; }
        public string ChiTietCacBuoiDay { get; set; }
    }

    public class ChiTietGioGiangVienDto
    {
        public BaoCaoGioGiangDto TongHop { get; set; }
        public List<ChiTietBuoiDungLopDto> BuoiDungLop { get; set; }
        public List<ChiTietHoatDongGioDto> DeTaiHoiDong { get; set; }
        public List<ChiTietHoatDongGioDto> KhaoThi { get; set; }
        public List<NckhHoatDong> NCKH { get; set; }
        public List<MienGiamDinhMuc> MienGiam { get; set; }

        public ChiTietGioGiangVienDto()
        {
            TongHop = new BaoCaoGioGiangDto();
            BuoiDungLop = new List<ChiTietBuoiDungLopDto>();
            DeTaiHoiDong = new List<ChiTietHoatDongGioDto>();
            KhaoThi = new List<ChiTietHoatDongGioDto>();
            NCKH = new List<NckhHoatDong>();
            MienGiam = new List<MienGiamDinhMuc>();
        }
    }

    public class ChiTietBuoiDungLopDto
    {
        public int ID_BuoiHoc { get; set; }
        public string NgayHoc { get; set; }
        public string ChuoiTiet { get; set; }
        public string TenMonHoc { get; set; }
        public string MaNhomHoc { get; set; }
        public string MaPhong { get; set; }
        public string LoaiHinhGiangDay { get; set; }
        public bool DayBangNgoaiNgu { get; set; }
        public int TongSiSoBuoi { get; set; }
        public double SoTietGiangDay { get; set; }
        public double HeSoQuyDoi { get; set; }
        public double GioChuanQuyDoi { get; set; }
    }

    public class ChiTietHoatDongGioDto
    {
        public string TieuDe { get; set; }
        public string VaiTroHoacLoai { get; set; }
        public string NgayHoacGhiChu { get; set; }
        public double GioChuanQuyDoi { get; set; }
    }
}
