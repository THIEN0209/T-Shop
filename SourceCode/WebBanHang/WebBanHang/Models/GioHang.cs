using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class GioHang
    {
        public int MaTk { get; set; }
        public int MaCtsp { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayThem { get; set; }

        public virtual ChiTietSanPham MaCtspNavigation { get; set; } = null!;
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
    }
}
