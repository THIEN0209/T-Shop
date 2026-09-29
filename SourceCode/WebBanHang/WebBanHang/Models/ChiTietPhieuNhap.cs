using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class ChiTietPhieuNhap
    {
        public int MaCtpn { get; set; }
        public int MaPn { get; set; }
        public int MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public double DonGiaNhap { get; set; }

        public virtual ChiTietSanPham MaCtspNavigation { get; set; } = null!;
        public virtual PhieuNhap MaPnNavigation { get; set; } = null!;
    }
}
