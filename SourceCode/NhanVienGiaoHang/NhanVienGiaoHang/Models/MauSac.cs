using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class MauSac
    {
        public MauSac()
        {
            ChiTietSanPhams = new HashSet<ChiTietSanPham>();
        }

        public string MaMau { get; set; } = null!;
        public string TenMau { get; set; } = null!;

        public virtual ICollection<ChiTietSanPham> ChiTietSanPhams { get; set; }
    }
}
