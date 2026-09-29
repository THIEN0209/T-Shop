using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public string MaLoai { get; set; } = null!;
        public string TenLoai { get; set; } = null!;
        public string MaDanhMuc { get; set; } = null!;

        public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
