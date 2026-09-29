using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            DonHangKhuyenMais = new HashSet<DonHangKhuyenMai>();
            SanPhamKhuyenMais = new HashSet<SanPhamKhuyenMai>();
        }

        public int MaKm { get; set; }
        public string TenKm { get; set; } = null!;
        public string? CodeKm { get; set; }
        public string LoaiGiam { get; set; } = null!;
        public double GiaTriGiam { get; set; }
        public double? DonToiThieu { get; set; }
        public int? SoLuong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public bool? TrangThai { get; set; }
        public string LoaiKm { get; set; } = null!;

        public virtual ICollection<DonHangKhuyenMai> DonHangKhuyenMais { get; set; }
        public virtual ICollection<SanPhamKhuyenMai> SanPhamKhuyenMais { get; set; }
    }
}
