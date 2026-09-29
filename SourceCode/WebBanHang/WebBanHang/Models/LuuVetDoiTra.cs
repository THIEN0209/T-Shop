using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class LuuVetDoiTra
    {
        public int MaLuuVet { get; set; }
        public int MaTk { get; set; }
        public int MaDh { get; set; }
        public int MaCtsp { get; set; }
        public DateTime NgayThem { get; set; }

        public virtual ChiTietSanPham MaCtspNavigation { get; set; } = null!;
        public virtual DonHang MaDhNavigation { get; set; } = null!;
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
    }
}
