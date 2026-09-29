using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class Banner
    {
        public int MaBanner { get; set; }
        public string TenBanner { get; set; } = null!;
        public string? LienKetUrl { get; set; }
        public int ThuTu { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
