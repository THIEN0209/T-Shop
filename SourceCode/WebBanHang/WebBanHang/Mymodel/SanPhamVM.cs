namespace WebBanHang.Mymodel
{
    public class SanPhamVM
    {
        public string MaSp { get; set; } = null!;
        public string TenSp { get; set; } = null!;
        public int GioiTinh { get; set; }
        public string? MoTa { get; set; }
        public string? TrangThai { get; set; }
        public double GiaGoc { get; set; }

        // Thông tin khuyến mãi đi kèm (nếu có)
        public int? MaKm { get; set; }
        public string? TenKm { get; set; }
        public double? GiaTriGiam { get; set; }

        // Thuộc tính tự động tính toán Giá Giảm (GiaDaGiam)
        public double GiaDaGiam
        {
            get
            {
                // Nếu không có khuyến mãi, giá đã giảm chính là giá gốc
                if (GiaTriGiam == null || GiaTriGiam <= 0)
                {
                    return GiaGoc;
                }

                // Nếu giá trị giảm dưới 100 -> Tính theo %
                if (GiaTriGiam < 100)
                {
                    return GiaGoc - (GiaGoc * (GiaTriGiam.Value / 100.0));
                }

                // Nếu giá trị giảm từ 100 trở lên -> Trừ thẳng số tiền (Ví dụ: giảm 50K tức là giảm 50,000đ)
                // Lưu ý: Nếu database bạn lưu là 50 (cho 50K) thì nhân với 1000, 
                // còn nếu lưu hẳn là 50000 thì trừ trực tiếp GiaTriGiam.Value. 
                // Ở đây mình ví dụ trừ trực tiếp:
                double giaSauGiam = GiaGoc - GiaTriGiam.Value;

                return giaSauGiam < 0 ? 0 : giaSauGiam; // Đảm bảo giá không bị âm
            }
        }

        // Thuộc tính kiểm tra xem sản phẩm này có đang thực sự được giảm giá hay không
        public bool CoKhuyenMai => GiaTriGiam > 0 && GiaDaGiam < GiaGoc;
    }
}
