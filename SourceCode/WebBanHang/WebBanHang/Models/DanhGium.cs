using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class DanhGium
    {
        public int MaDanhGia { get; set; }
        public string MaSp { get; set; } = null!;
        public int MaTk { get; set; }
        public int SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayDanhGia { get; set; }

        public virtual SanPham MaSpNavigation { get; set; } = null!;
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
    }
}
