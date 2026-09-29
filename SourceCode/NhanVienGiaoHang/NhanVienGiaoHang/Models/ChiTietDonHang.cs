using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class ChiTietDonHang
    {
        public int MaCtdh { get; set; }
        public int MaDh { get; set; }
        public int MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public double ThanhTien { get; set; }

        public virtual ChiTietSanPham MaCtspNavigation { get; set; } = null!;
        public virtual DonHang MaDhNavigation { get; set; } = null!;
    }
}
