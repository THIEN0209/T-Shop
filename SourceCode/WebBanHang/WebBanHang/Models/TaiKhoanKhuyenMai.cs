using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class TaiKhoanKhuyenMai
    {
        public int MaTkkm { get; set; }
        public int MaTk { get; set; }
        public int MaKm { get; set; }
        public DateTime NgaySuDung { get; set; }

        public virtual KhuyenMai MaKmNavigation { get; set; } = null!;
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
    }
}
