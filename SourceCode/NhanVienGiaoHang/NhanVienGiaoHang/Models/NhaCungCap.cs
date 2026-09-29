using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class NhaCungCap
    {
        public NhaCungCap()
        {
            PhieuNhaps = new HashSet<PhieuNhap>();
        }

        public string MaNcc { get; set; } = null!;
        public string TenNcc { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }

        public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; }
    }
}
