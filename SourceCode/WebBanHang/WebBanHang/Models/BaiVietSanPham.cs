using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class BaiVietSanPham
    {
        public int MaBvsp { get; set; }
        public int MaBaiViet { get; set; }
        public string MaSp { get; set; } = null!;

        public virtual BaiViet MaBaiVietNavigation { get; set; } = null!;
        public virtual SanPham MaSpNavigation { get; set; } = null!;
    }
}
