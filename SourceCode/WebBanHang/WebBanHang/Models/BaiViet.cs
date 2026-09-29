using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class BaiViet
    {
        public BaiViet()
        {
            BaiVietSanPhams = new HashSet<BaiVietSanPham>();
            Banners = new HashSet<Banner>();
        }

        public int MaBaiViet { get; set; }
        public string TenBaiViet { get; set; } = null!;
        public string? LinkBaiViet { get; set; }
        public int Loai { get; set; }

        public virtual ICollection<BaiVietSanPham> BaiVietSanPhams { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
    }
}
