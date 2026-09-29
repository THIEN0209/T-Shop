using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class YeuThich
    {
        public int MaTk { get; set; }
        public string MaSp { get; set; } = null!;
        public DateTime NgayThem { get; set; }

        public virtual SanPham MaSpNavigation { get; set; } = null!;
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
    }
}
