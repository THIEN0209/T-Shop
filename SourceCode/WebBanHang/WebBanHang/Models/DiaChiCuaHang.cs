using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class DiaChiCuaHang
    {
        public int MaCh { get; set; }
        public string Dc { get; set; } = null!;
        public string? LinkGgmap { get; set; }
    }
}
