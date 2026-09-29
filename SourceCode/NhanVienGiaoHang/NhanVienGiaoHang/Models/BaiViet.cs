using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class BaiViet
    {
        public int MaBaiViet { get; set; }
        public string TenBaiViet { get; set; } = null!;
        public string? LinkBaiViet { get; set; }
        public int Loai { get; set; }
    }
}
