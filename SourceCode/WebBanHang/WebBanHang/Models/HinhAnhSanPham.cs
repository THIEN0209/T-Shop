using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class HinhAnhSanPham
    {
        public int MaHa { get; set; }
        public string MaSp { get; set; } = null!;
        public string DuongDanAnh { get; set; } = null!;

        public virtual SanPham MaSpNavigation { get; set; } = null!;
    }
}
