using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class VaiTro
    {
        public VaiTro()
        {
            TaiKhoans = new HashSet<TaiKhoan>();
        }

        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; } = null!;

        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
