using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class DonHangKhuyenMai
    {
        public int MaDh { get; set; }
        public int MaKm { get; set; }
        public double SoTienGiam { get; set; }

        public virtual DonHang MaDhNavigation { get; set; } = null!;
        public virtual KhuyenMai MaKmNavigation { get; set; } = null!;
    }
}
