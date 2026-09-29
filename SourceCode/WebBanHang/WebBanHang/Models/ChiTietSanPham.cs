using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class ChiTietSanPham
    {
        public ChiTietSanPham()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            ChiTietPhieuNhaps = new HashSet<ChiTietPhieuNhap>();
            GioHangs = new HashSet<GioHang>();
            LuuVetDoiTras = new HashSet<LuuVetDoiTra>();
        }

        public int MaCtsp { get; set; }
        public string MaSp { get; set; } = null!;
        public string MaMau { get; set; } = null!;
        public string MaSize { get; set; } = null!;
        public int SoLuongTon { get; set; }

        public virtual MauSac MaMauNavigation { get; set; } = null!;
        public virtual Size MaSizeNavigation { get; set; } = null!;
        public virtual SanPham MaSpNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
        public virtual ICollection<GioHang> GioHangs { get; set; }
        public virtual ICollection<LuuVetDoiTra> LuuVetDoiTras { get; set; }
    }
}
