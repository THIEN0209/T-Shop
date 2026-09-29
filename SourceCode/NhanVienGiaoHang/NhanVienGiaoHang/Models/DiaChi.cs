using System;
using System.Collections.Generic;

namespace NhanVienGiaoHang.Models
{
    public partial class DiaChi
    {
        public DiaChi()
        {
            DonHangs = new HashSet<DonHang>();
            TaiKhoans = new HashSet<TaiKhoan>();
        }

        public int MaDiaChi { get; set; }
        public int MaTk { get; set; }
        public string TinhThanh { get; set; } = null!;
        public string QuanHuyen { get; set; } = null!;
        public string PhuongXa { get; set; } = null!;
        public string DiaChiCuThe { get; set; } = null!;
        public bool MacDinh { get; set; }
        public string? SdtNguoiNhan { get; set; }

        public virtual TaiKhoan MaTkNavigation { get; set; } = null!;
        public virtual ICollection<DonHang> DonHangs { get; set; }
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
