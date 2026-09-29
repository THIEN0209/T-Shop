using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class PhieuNhap
    {
        public PhieuNhap()
        {
            ChiTietPhieuNhaps = new HashSet<ChiTietPhieuNhap>();
        }

        public int MaPn { get; set; }
        public string MaNcc { get; set; } = null!;
        public DateTime NgayNhap { get; set; }
        public double TongTien { get; set; }

        public virtual NhaCungCap MaNccNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
    }
}
