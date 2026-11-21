using System;
using System.Collections.Generic;

namespace PM_Tinhgiogiang_Lib.Models
{
    public class BuoiHoc
    {
        public int ID_BuoiHoc { get; set; }
        public int ID_NhomHoc { get; set; }
        public int? ID_BaiHoc { get; set; }
        public string NgayHoc { get; set; }
        public string BuoiTrongNgay { get; set; }
        public int? ID_PhongHoc { get; set; }
        public int SoTietBuoi { get; set; }
        public string LoaiHinhGiangDay { get; set; }
        public bool DayBangNgoaiNgu { get; set; }
        public int TongSiSoBuoi { get; set; }
        public string TrangThai { get; set; }
        public int? ID_BuoiHocGoc { get; set; }
        public string GhiChu { get; set; }

        public BuoiHoc()
        {
            TrangThai = "Draft";
        }
    }

    public class PhanCongGiangDay
    {
        public int ID_PhanCong { get; set; }
        public int ID_BuoiHoc { get; set; }
        public int ID_GV { get; set; }
        public string VaiTro { get; set; }
        public double SoTietGiangDay { get; set; }
        public double HeSoQuyDoi { get; set; }
        public double GioChuanQuyDoi { get; set; }
        public bool DaXacNhanHoanThanh { get; set; }

        public PhanCongGiangDay()
        {
            VaiTro = "Chinh";
        }
    }

    public class PhanCongGiangVienInput
    {
        public int ID_GV { get; set; }
        public double SoTietGiangDay { get; set; }
        public string VaiTro { get; set; }

        public PhanCongGiangVienInput()
        {
            VaiTro = "Chinh";
        }
    }

    public class BuoiHocChiTiet
    {
        public int ID_BuoiHoc { get; set; }
        public int ID_NhomHoc { get; set; }
        public string MaNhomHoc { get; set; }
        public string TenNhomHoc { get; set; }
        public string NgayHoc { get; set; }
        public string BuoiTrongNgay { get; set; }
        public int SoTietBuoi { get; set; }
        public string LoaiHinhGiangDay { get; set; }
        public bool DayBangNgoaiNgu { get; set; }
        public int TongSiSoBuoi { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        public string TenMonHoc { get; set; }
        public string DanhSachMaLopHP { get; set; }
        public int? ID_BaiHoc { get; set; }
        public string TenBaiHoc { get; set; }

        public string ChuoiTietHoc { get; set; }
        public string ChuoiTietSo { get; set; }
        public string GioBatDau { get; set; }
        public string GioKetThuc { get; set; }
        public int? ID_PhongHoc { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public string CoSoPhong { get; set; }

        public string DanhSachGiangVien { get; set; }
        public double HeSoQuyDoi { get; set; }
        public double GioChuanQuyDoi { get; set; }

        public List<int> DanhSachID_Tiet { get; set; }
        public List<int> DanhSachSoTiet { get; set; }
        public List<int> DanhSachID_GV { get; set; }

        public BuoiHocChiTiet()
        {
            TrangThai = "Draft";
            DanhSachID_Tiet = new List<int>();
            DanhSachSoTiet = new List<int>();
            DanhSachID_GV = new List<int>();
        }
    }

    public class KetQuaThaoTac
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }
        public int? ID_BuoiHoc { get; set; }
        public int? ID_NhomHoc { get; set; }

        public static KetQuaThaoTac Ok(string thongBao, int? idBuoiHoc = null, int? idNhomHoc = null)
        {
            return new KetQuaThaoTac
            {
                ThanhCong = true,
                ThongBao = thongBao,
                ID_BuoiHoc = idBuoiHoc,
                ID_NhomHoc = idNhomHoc
            };
        }

        public static KetQuaThaoTac Loi(string thongBao)
        {
            return new KetQuaThaoTac
            {
                ThanhCong = false,
                ThongBao = thongBao
            };
        }
    }

    public class XungDotLichItem
    {
        public string MucDo { get; set; }
        public string LoaiXungDot { get; set; }
        public int? ID_BuoiHoc { get; set; }
        public string NgayTiet { get; set; }
        public string DoiTuong1 { get; set; }
        public string DoiTuong2 { get; set; }
        public string MoTa { get; set; }

        public XungDotLichItem()
        {
            MucDo = "Chan";
        }
    }

    public class XepLichLapLaiRequest
    {
        public int ID_NhomHoc { get; set; }
        public string NgayBatDau { get; set; }
        public List<int> DanhSachThuTrongTuan { get; set; }
        public int SoTuanLap { get; set; }
        public int BuocNhayTuan { get; set; }
        public List<string> DanhSachNgayChon { get; set; }
        public List<int> DanhSachID_Tiet { get; set; }
        public int? ID_PhongHoc { get; set; }
        public string LoaiHinhGiangDay { get; set; }
        public bool DayBangNgoaiNgu { get; set; }
        public List<PhanCongGiangVienInput> DanhSachPhanCong { get; set; }
        public bool TuDongGanBaiHoc { get; set; }
        public string GhiChu { get; set; }

        public XepLichLapLaiRequest()
        {
            DanhSachThuTrongTuan = new List<int>();
            DanhSachID_Tiet = new List<int>();
            DanhSachPhanCong = new List<PhanCongGiangVienInput>();
            SoTuanLap = 1;
            BuocNhayTuan = 1;
            LoaiHinhGiangDay = "LT";
            TuDongGanBaiHoc = true;
        }
    }

    public class KetQuaXepLichLapLai
    {
        public bool ThanhCong { get; set; }
        public int SoBuoiThanhCong { get; set; }
        public int SoBuoiBiTrung { get; set; }
        public string ThongBaoTongQuat { get; set; }
        public List<string> ChiTietThanhCong { get; set; }
        public List<string> ChiTietBiTrung { get; set; }

        public KetQuaXepLichLapLai()
        {
            ChiTietThanhCong = new List<string>();
            ChiTietBiTrung = new List<string>();
        }
    }
}
