using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class DanhMuc
    {
        public DanhMuc()
        {
            LoaiSanPhams = new HashSet<LoaiSanPham>();
        }

        public string MaDanhMuc { get; set; } = null!;
        public string TenDanhMuc { get; set; } = null!;

        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; }
    }
}
