using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Size
    {
        public Size()
        {
            ChiTietSanPhams = new HashSet<ChiTietSanPham>();
        }

        public string MaSize { get; set; } = null!;
        public string TenSize { get; set; } = null!;

        public virtual ICollection<ChiTietSanPham> ChiTietSanPhams { get; set; }
    }
}
