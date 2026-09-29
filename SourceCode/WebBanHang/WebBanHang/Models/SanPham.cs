using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            BaiVietSanPhams = new HashSet<BaiVietSanPham>();
            ChiTietSanPhams = new HashSet<ChiTietSanPham>();
            DanhGia = new HashSet<DanhGium>();
            HinhAnhSanPhams = new HashSet<HinhAnhSanPham>();
            SanPhamKhuyenMais = new HashSet<SanPhamKhuyenMai>();
            YeuThiches = new HashSet<YeuThich>();
        }

        public string MaSp { get; set; } = null!;
        public string TenSp { get; set; } = null!;
        public int GioiTinh { get; set; }
        public string? MoTa { get; set; }
        public string? MaLoai { get; set; }
        public string? TrangThai { get; set; }
        public double Gia { get; set; }

        public virtual LoaiSanPham? MaLoaiNavigation { get; set; }
        public virtual ICollection<BaiVietSanPham> BaiVietSanPhams { get; set; }
        public virtual ICollection<ChiTietSanPham> ChiTietSanPhams { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; }
        public virtual ICollection<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public virtual ICollection<SanPhamKhuyenMai> SanPhamKhuyenMais { get; set; }
        public virtual ICollection<YeuThich> YeuThiches { get; set; }
    }
}
