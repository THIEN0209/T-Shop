using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class SanPhamKhuyenMai
    {
        public int MaSpkm { get; set; }
        public string MaSp { get; set; } = null!;
        public int MaKm { get; set; }

        public virtual KhuyenMai MaKmNavigation { get; set; } = null!;
        public virtual SanPham MaSpNavigation { get; set; } = null!;
    }
}
