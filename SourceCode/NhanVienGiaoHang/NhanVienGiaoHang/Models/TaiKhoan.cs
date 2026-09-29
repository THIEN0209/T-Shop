using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class TaiKhoan
    {
        public TaiKhoan()
        {
            DanhGia = new HashSet<DanhGium>();
            DiaChis = new HashSet<DiaChi>();
            DonHangMaTaiXeNavigations = new HashSet<DonHang>();
            DonHangMaTkNavigations = new HashSet<DonHang>();
            GioHangs = new HashSet<GioHang>();
            LuuVetDoiTras = new HashSet<LuuVetDoiTra>();
            YeuThiches = new HashSet<YeuThich>();
        }

        public int MaTk { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public int MaVaiTro { get; set; }
        public int? MaDiaChi { get; set; }

        public virtual DiaChi? MaDiaChiNavigation { get; set; }
        public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
        public virtual ICollection<DanhGium> DanhGia { get; set; }
        public virtual ICollection<DiaChi> DiaChis { get; set; }
        public virtual ICollection<DonHang> DonHangMaTaiXeNavigations { get; set; }
        public virtual ICollection<DonHang> DonHangMaTkNavigations { get; set; }
        public virtual ICollection<GioHang> GioHangs { get; set; }
        public virtual ICollection<LuuVetDoiTra> LuuVetDoiTras { get; set; }
        public virtual ICollection<YeuThich> YeuThiches { get; set; }
    }
}
