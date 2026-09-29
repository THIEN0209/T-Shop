using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class YeuCauDoiTra
    {
        public int MaDoiTra { get; set; }
        public int MaDh { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayYeuCau { get; set; }

        public virtual DonHang MaDhNavigation { get; set; } = null!;
    }
}
