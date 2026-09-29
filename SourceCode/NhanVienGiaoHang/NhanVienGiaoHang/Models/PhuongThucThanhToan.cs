using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class PhuongThucThanhToan
    {
        public PhuongThucThanhToan()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaPttt { get; set; }
        public string TenPttt { get; set; } = null!;

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
