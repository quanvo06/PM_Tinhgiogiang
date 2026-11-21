using System;
using System.Collections.Generic;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib.Helpers
{
    /// <summary>
    /// Lớp quản lý phiên đăng nhập, phân quyền và ngữ cảnh làm việc (Năm học / Học kỳ / Bộ môn),
    /// kế thừa mô hình SessionHelper từ HNKHKTD_Lib.
    /// </summary>
    public static class SessionHelper
    {
        public const string VAI_TRO_ADMIN = "Admin";
        public const string VAI_TRO_GIAO_VU = "GiaoVu";
        public const string VAI_TRO_TRUONG_BM = "TruongBoMon";
        public const string VAI_TRO_GIANG_VIEN = "GiangVien";

        private static List<string> _userRoles = new List<string>();

        public static NguoiDung LoggedInUser { get; set; }
        public static GiangVien LoggedInGiangVien { get; set; }

        // Ngữ cảnh lọc toàn cục (Context Bar)
        public static int? CurrentNamHocId { get; set; }
        public static int? CurrentHocKyId { get; set; }
        public static int? CurrentBoMonId { get; set; }

        public static List<string> UserRoles
        {
            get { return _userRoles; }
            set { _userRoles = value ?? new List<string>(); }
        }

        public static bool HasRole(string roleName)
        {
            if (LoggedInUser == null) return false;
            if (string.Equals(LoggedInUser.TenDangNhap, "admin", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(LoggedInUser.VaiTro, VAI_TRO_ADMIN, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(LoggedInUser.VaiTro, roleName, StringComparison.OrdinalIgnoreCase)) return true;

            return _userRoles.Exists(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAdmin()
        {
            return HasRole(VAI_TRO_ADMIN);
        }

        public static bool CanXepLich()
        {
            return HasRole(VAI_TRO_ADMIN) || HasRole(VAI_TRO_GIAO_VU) || HasRole(VAI_TRO_TRUONG_BM);
        }

        public static void Logout()
        {
            LoggedInUser = null;
            LoggedInGiangVien = null;
            CurrentNamHocId = null;
            CurrentHocKyId = null;
            CurrentBoMonId = null;
            _userRoles.Clear();
        }
    }
}
