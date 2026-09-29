using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Banner
    {
        public int MaBanner { get; set; }
        public string TenBanner { get; set; } = null!;
        public DateTime NgayTao { get; set; }
        public int? MaBaiViet { get; set; }
        public string? ViTriBanner { get; set; }

        public virtual BaiViet? MaBaiVietNavigation { get; set; }
    }
}
