using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PM_Tinhgiogiang_Lib.Models;

namespace PM_Tinhgiogiang_Lib.Helpers
{
    /// <summary>
    /// Lớp tiện ích xuất dữ liệu ra file Excel (.csv UTF-8 BOM), kế thừa chuẩn từ HNKHKTD_Lib.
    /// Không cần cài đặt Microsoft Office trên máy người dùng.
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// Mở hộp thoại chọn nơi lưu và xuất dữ liệu DataGridView ra file CSV hỗ trợ Excel.
        /// </summary>
        public static bool ExportDataGridViewToExcel(DataGridView dgv, string defaultFileName = "DanhSach.csv")
        {
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "File Excel CSV (*.csv)|*.csv|Tất cả các file (*.*)|*.*";
                sfd.FileName = defaultFileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();

                        bool firstCol = true;
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (col.Visible)
                            {
                                if (!firstCol) sb.Append(",");
                                sb.Append(EscapeCsv(col.HeaderText));
                                firstCol = false;
                            }
                        }
                        sb.AppendLine();

                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                firstCol = true;
                                foreach (DataGridViewColumn col in dgv.Columns)
                                {
                                    if (col.Visible)
                                    {
                                        if (!firstCol) sb.Append(",");
                                        object val = row.Cells[col.Index].Value;
                                        string text = (val != null) ? val.ToString() : "";
                                        sb.Append(EscapeCsv(text));
                                        firstCol = false;
                                    }
                                }
                                sb.AppendLine();
                            }
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                        MessageBox.Show("Xuất file Excel thành công:\n" + sfd.FileName, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Xuất bảng tổng hợp giờ chuẩn giảng dạy (BaoCaoGioGiangDto) ra file Excel CSV (UTF-8 BOM) có dòng tiêu đề UMP và dòng Tổng cộng.
        /// </summary>
        public static bool ExportBaoCaoGioGiangToExcel(
            List<BaoCaoGioGiangDto> duLieu,
            string phamVi = "Toàn bộ năm học",
            string defaultFileName = "BaoCao_GioGiang_UMP.csv")
        {
            if (duLieu == null || duLieu.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu giờ giảng để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "File Excel CSV (*.csv)|*.csv|Tất cả các file (*.*)|*.*";
                sfd.FileName = defaultFileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(EscapeCsv("BẢNG TỔNG HỢP GIỜ CHUẨN GIẢNG DẠY - TRƯỜNG DƯỢC (UMP)"));
                        sb.AppendLine(EscapeCsv("Phạm vi: " + phamVi + " | Ngày xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                        sb.AppendLine();

                        string[] headers = new[]
                        {
                            "STT", "Mã GV", "Họ và tên", "Bộ môn",
                            "Định mức gốc", "Miễn giảm", "Định mức thực tế",
                            "Số tiết đứng lớp", "Giờ đứng lớp", "Giờ đề tài/HĐ",
                            "Giờ khảo thí", "Giờ NCKH", "Tổng giờ chuẩn", "Giờ vượt định mức"
                        };
                        sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

                        for (int i = 0; i < duLieu.Count; i++)
                        {
                            BaoCaoGioGiangDto x = duLieu[i];
                            string[] cols = new[]
                            {
                                (i + 1).ToString(),
                                x.MaGV,
                                x.HoTen,
                                x.TenBoMon,
                                x.DinhMucGioChuan.ToString("0.##"),
                                x.GioMienGiam.ToString("0.##"),
                                x.DinhMucThucTe.ToString("0.##"),
                                x.TongSoTietThucTe.ToString(),
                                x.GioDungLop.ToString("0.##"),
                                x.GioDeTaiHoiDong.ToString("0.##"),
                                x.GioKhaoThi.ToString("0.##"),
                                x.GioNCKH.ToString("0.##"),
                                x.TongGioChuanQuyDoi.ToString("0.##"),
                                x.GioVuotDinhMuc.ToString("0.##")
                            };
                            sb.AppendLine(string.Join(",", cols.Select(EscapeCsv)));
                        }

                        string[] totals = new[]
                        {
                            "", "", "TỔNG CỘNG", duLieu.Count + " giảng viên",
                            duLieu.Sum(x => x.DinhMucGioChuan).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.GioMienGiam), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.DinhMucThucTe), 2).ToString("0.##"),
                            duLieu.Sum(x => x.TongSoTietThucTe).ToString(),
                            Math.Round(duLieu.Sum(x => x.GioDungLop), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.GioDeTaiHoiDong), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.GioKhaoThi), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.GioNCKH), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.TongGioChuanQuyDoi), 2).ToString("0.##"),
                            Math.Round(duLieu.Sum(x => x.GioVuotDinhMuc), 2).ToString("0.##")
                        };
                        sb.AppendLine(string.Join(",", totals.Select(EscapeCsv)));

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                        MessageBox.Show("Xuất báo cáo giờ giảng thành công:\n" + sfd.FileName, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất báo cáo Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return false;
        }

        private static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return "\"\"";
            string escaped = input.Replace("\"", "\"\"");
            return "\"" + escaped + "\"";
        }
    }
}
