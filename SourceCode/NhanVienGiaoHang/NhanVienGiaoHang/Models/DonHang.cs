using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class DonHang
    {
        public DonHang()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            DonHangKhuyenMais = new HashSet<DonHangKhuyenMai>();
            LuuVetDoiTras = new HashSet<LuuVetDoiTra>();
            YeuCauDoiTras = new HashSet<YeuCauDoiTra>();
        }

        public int MaDh { get; set; }
        public int MaTk { get; set; }
        public int MaDiaChi { get; set; }
        public DateTime NgayDat { get; set; }
        public double TongTien { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
        public int? MaPttt { get; set; }
        public int? MaTaiXe { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;

        public virtual DiaChi MaDiaChiNavigation { get; set; } = null!;
        public virtual PhuongThucThanhToan? MaPtttNavigation { get; set; }
        public virtual TaiKhoan? MaTaiXeNavigation { get; set; }
        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual ICollection<DonHangKhuyenMai> DonHangKhuyenMais { get; set; }
        public virtual ICollection<LuuVetDoiTra> LuuVetDoiTras { get; set; }
        public virtual ICollection<YeuCauDoiTra> YeuCauDoiTras { get; set; }
    }
}
