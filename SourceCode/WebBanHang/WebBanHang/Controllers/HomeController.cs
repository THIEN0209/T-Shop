using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Mymodel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.NetworkInformation;
using System;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        TshopContext db= new TshopContext();
        public class ChiTietGiamGia
        {
            public int MaKm { get; set; }
            public string LoaiKm { get; set; } = null!;
            public string TenKm { get; set; } = null!;
            public double SoTienGiam { get; set; }
            public string? MaSp { get; set; } // chỉ có giá trị với loại "SanPham"
        }

        public class KetQuaKhuyenMai
        {
            public double TongTienGoc { get; set; }
            public double TongGiamGia { get; set; }
            public double TongTienSauGiam { get; set; }
            public string? LoiCode { get; set; }
            public List<ChiTietGiamGia> ChiTietGiam { get; set; } = new();
            // MaCtsp -> số tiền giảm riêng của dòng sản phẩm đó (để hiển thị ngay trên từng item giỏ hàng)
            public Dictionary<int, double> GiamTheoSanPham { get; set; } = new();
        }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.LoaiSP = db.LoaiSanPhams.ToList();
            ViewBag.DanhMuc = db.DanhMucs.ToList();

            ViewBag.BaiVietChinhSach = db.BaiViets
    .Where(x => x.Loai == 1)
    .OrderBy(x => x.TenBaiViet)
    .ToList();

            ViewBag.BaiVietThuong = db.BaiViets
                .Where(x => x.Loai == 2)
                .OrderBy(x => x.TenBaiViet)
                .ToList();

            ViewBag.DiaChiCuaHang = db.DiaChiCuaHangs
                          .OrderBy(x => x.Dc)
                          .ToList();
            base.OnActionExecuting(context);
        }
        public IActionResult Index()
        {
            ViewBag.DanhMuc = db.DanhMucs.ToList();


            ViewBag.SanPhamBanChay = db.SanPhams
                .Where(x => x.TrangThai == "Bán chạy")
                .OrderBy(x => Guid.NewGuid())
                .Take(20)
                .Select(sp => new SanPhamVM
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    MoTa = sp.MoTa,
                    TrangThai = sp.TrangThai,
                    GiaGoc = sp.Gia
                })
                .ToList();

            ViewBag.BannerDauTrang = db.Banners.Include(x => x.MaBaiVietNavigation)
    .Where(x => x.ViTriBanner == "Banner đầu trang")
    .ToList();

            ViewBag.BannerGiuaTrang = db.Banners.Include(x => x.MaBaiVietNavigation)
                .Where(x => x.ViTriBanner == "Banner giữa trang")
                .ToList();

            ViewBag.Banner50 = db.Banners.Include(x => x.MaBaiVietNavigation)
                .Where(x => x.ViTriBanner == "Banner 50 hình 1"
                         || x.ViTriBanner == "Banner 50 hình 2")
                .OrderBy(x => x.ViTriBanner)
                .ToList();
            return View();
        }
        //================ ĐĂNG NHẬP =================

        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(string soDienThoai, string matKhau)
        {
            try
            {
                // 1. Kiểm tra dữ liệu đầu vào trống
                if (string.IsNullOrEmpty(soDienThoai) || string.IsNullOrEmpty(matKhau))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ số điện thoại và mật khẩu!";
                    return RedirectToAction("DangNhap");
                }

                // 2. Tìm kiếm tài khoản trong Database
                var tk = db.TaiKhoans
                           .Include(x => x.MaVaiTroNavigation)
                           .FirstOrDefault(x => x.SoDienThoai == soDienThoai && x.MatKhau == matKhau);

                // 3. Kiểm tra nếu sai tài khoản/mật khẩu
                if (tk == null)
                {
                    TempData["Error"] = "Sai số điện thoại hoặc mật khẩu!";
                    return RedirectToAction("DangNhap");
                }

                // 4. Kiểm tra trạng thái tài khoản bị khóa
                if (tk.TrangThai == false)
                {
                    TempData["Error"] = "Tài khoản của bạn đã bị khóa!";
                    return RedirectToAction("DangNhap");
                }

                // 5. Lưu Session khi đăng nhập thành công
                HttpContext.Session.SetInt32("MaTK", tk.MaTk);
                HttpContext.Session.SetString("HoTen", tk.HoTen);
                HttpContext.Session.SetString("VaiTro", tk.MaVaiTroNavigation.TenVaiTro);

                // 6. Tạo Flash Message thành công
                TempData["Success"] = "Đăng nhập thành công!";

                // Điều hướng về trang chủ Khách hàng
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Error"] = "Đã xảy ra lỗi hệ thống! Vui lòng thử lại sau.";
                return RedirectToAction("DangNhap");
            }
        }
        public IActionResult DangXuat()
        {
            try
            {
                // Xóa toàn bộ Session
                HttpContext.Session.Clear();

                TempData["Success"] = "Đăng xuất thành công!";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi đăng xuất!";
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult DanhSachSanPham(string id, string sapXep)
        {
            ViewBag.CurrentId = id;   // <-- thêm dòng này, controller đã biết chắc chắn giá trị id là gì
            ViewBag.SapXep = sapXep;
            IQueryable<SanPham> query = db.SanPhams
                .Include(x => x.MaLoaiNavigation);

            var baiViet = db.BaiViets.FirstOrDefault(x => x.MaBaiViet.ToString() == id);

            if (baiViet != null)
            {
                var dsMaSP = db.BaiVietSanPhams
                    .Where(x => x.MaBaiViet == baiViet.MaBaiViet)
                    .Select(x => x.MaSp);

                query = query.Where(x => dsMaSP.Contains(x.MaSp));

                ViewBag.BaiViet = baiViet;
                ViewBag.TieuDe = baiViet.TenBaiViet;
            }
            else if (id.Equals("Moi", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.TrangThai.Contains("Mới"));
                ViewBag.TieuDe = "Sản phẩm mới";
            }
            else if (id.Equals("Sale", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.TrangThai.Contains("Sale"));
                ViewBag.TieuDe = "Sản phẩm Sale";
            }
            else if (db.LoaiSanPhams.Any(x => x.MaLoai == id))
            {
                query = query.Where(x => x.MaLoai == id);
                ViewBag.Loai = db.LoaiSanPhams.Find(id);
            }
            else if (db.DanhMucs.Any(x => x.MaDanhMuc == id))
            {
                query = query.Where(x => x.MaLoaiNavigation.MaDanhMuc == id);
                ViewBag.DanhMuc = db.DanhMucs.Find(id);
            }
            else
            {
                query = query.Where(x => x.TenSp.Contains(id));
                ViewBag.TuKhoa = id;
                ViewBag.TieuDe = $"Kết quả tìm kiếm cho \"{id}\"";
            }

            // Sắp xếp theo giá
            if (sapXep == "tang")
            {
                query = query.OrderBy(x => x.Gia);
            }
            else if (sapXep == "giam")
            {
                query = query.OrderByDescending(x => x.Gia);
            }

            ViewBag.SapXep = sapXep; // để view biết đang chọn nút nào

            var listVM = query
                .Select(sp => new SanPhamVM
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    MoTa = sp.MoTa,
                    GioiTinh = sp.GioiTinh,
                    TrangThai = sp.TrangThai,
                    GiaGoc = sp.Gia,
                    MaKm = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                    && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => (int?)spkm.MaKm)
                        .FirstOrDefault(),
                    TenKm = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                    && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => spkm.MaKmNavigation.TenKm)
                        .FirstOrDefault(),
                    GiaTriGiam = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                    && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => (double?)spkm.MaKmNavigation.GiaTriGiam)
                        .FirstOrDefault()
                })
                .ToList();

            return View(listVM);
        }
        public IActionResult ChiTietSanPham(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            var sanPham = db.SanPhams
                            .Include(x => x.MaLoaiNavigation)
                            .FirstOrDefault(x => x.MaSp == id);

            if (sanPham == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Images = db.HinhAnhSanPhams
                               .Where(x => x.MaSp == id)
                               .ToList();

            var listCT = db.ChiTietSanPhams
                           .Include(x => x.MaMauNavigation)
                           .Include(x => x.MaSizeNavigation)
                           .Where(x => x.MaSp == id)
                           .ToList();

            ViewBag.Colors = listCT
                            .GroupBy(x => x.MaMau)
                            .Select(x => x.First())
                            .ToList();

            // Chỉ truyền các dữ liệu JS cần
            ViewBag.JsonBienThe = JsonSerializer.Serialize(
                listCT.Select(x => new
                {
                    maCtsp = x.MaCtsp,
                    maMau = x.MaMau,
                    tenMau = x.MaMauNavigation.TenMau,
                    maSize = x.MaSize,
                    tenSize = x.MaSizeNavigation.TenSize,
                    ton = x.SoLuongTon
                })
            );
            //yêu thích
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK != null)
            {
                ViewBag.DaYeuThich = db.YeuThiches.Any(x =>
                    x.MaTk == maTK &&
                    x.MaSp == id);
            }
            else
            {
                ViewBag.DaYeuThich = false;
            }
            //sản phẩm gợi ý
            ViewBag.SanPhamGoiY = db.SanPhams
                .OrderBy(x => Guid.NewGuid())
                .Take(20)
                .Select(sp => new SanPhamVM
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    MoTa = sp.MoTa,
                    TrangThai = sp.TrangThai,
                    GiaGoc = sp.Gia,

                    MaKm = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                       && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => (int?)spkm.MaKm)
                        .FirstOrDefault(),

                    TenKm = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                       && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => spkm.MaKmNavigation.TenKm)
                        .FirstOrDefault(),

                    GiaTriGiam = sp.SanPhamKhuyenMais
                        .Where(spkm => spkm.MaKmNavigation.TrangThai == true
                                       && spkm.MaKmNavigation.LoaiKm == "SanPham")
                        .Select(spkm => (double?)spkm.MaKmNavigation.GiaTriGiam)
                        .FirstOrDefault()
                })
                .ToList();
            //số sao sản phẩm
            ViewBag.TrungBinhSoSao = db.DanhGia
    .Where(x => x.MaSp == id)
    .Average(x => (double?)x.SoSao) ?? 0;

            return View(sanPham);
        }
        [HttpPost]
        public IActionResult ToggleYeuThich(string maSp)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập."
                });
            }

            var yeuThich = db.YeuThiches
                             .FirstOrDefault(x => x.MaTk == maTK && x.MaSp == maSp);

            if (yeuThich == null)
            {
                db.YeuThiches.Add(new YeuThich
                {
                    MaTk = maTK.Value,
                    MaSp = maSp,
                    NgayThem = DateTime.Now
                });

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    daYeuThich = true
                });
            }

            db.YeuThiches.Remove(yeuThich);
            db.SaveChanges();

            return Json(new
            {
                success = true,
                daYeuThich = false
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDanhGia(string maSp, int soSao, string? noiDung)
        {
            var maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đánh giá.";
                return RedirectToAction("ChiTietSanPham", new { id = maSp });
            }

            DanhGium dg = new DanhGium
            {
                MaSp = maSp,
                MaTk = maTK.Value,
                SoSao = soSao,
                NoiDung = noiDung,
                NgayDanhGia = DateTime.Now
            };

            db.DanhGia.Add(dg);
            db.SaveChanges();

            TempData["Success"] = "Đánh giá thành công.";

            return RedirectToAction("ChiTietSanPham", new { id = maSp });
        }
        public IActionResult XemDanhGia(string id)
        {
            var ds = db.DanhGia
                .Include(x => x.MaTkNavigation)
                .Where(x => x.MaSp == id)
                .OrderByDescending(x => x.NgayDanhGia)
                .ToList();

            return View(ds);
        }
        //==================Đăng ký
        [HttpGet]
        public IActionResult DangKy()
        {
            // Trả về Giao diện Đăng ký tài khoản
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(TaiKhoan tk)
        {
            try
            {


                // Kiểm tra xem Email đã tồn tại trong hệ thống chưa (Do Email có ràng buộc UNIQUE)
                bool isEmailExist = db.TaiKhoans.Any(x => x.Email.ToLower() == tk.Email.ToLower());
                if (isEmailExist)
                {
                    TempData["Error"] = "Email này đã được sử dụng bởi một tài khoản khác!";
                    return View(tk);
                }

                // Gán các giá trị mặc định cho tài khoản mới
                tk.NgayTao = DateTime.Now;
                tk.TrangThai = true; // Mặc định kích hoạt tài khoản

                // Mặc định vai trò Khách hàng. 
                // Bạn nên kiểm tra lại ID của vai trò "Khách hàng" trong bảng VaiTro của bạn là mấy (thường là 2 hoặc 3)
                tk.MaVaiTro = 2;

                // Tiến hành lưu vào Cơ sở dữ liệu
                db.TaiKhoans.Add(tk);
                db.SaveChanges();

                // Tạo Flash Message thông báo thành công
                TempData["Success"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";

                // Điều hướng sang trang Đăng nhập (hoặc trang chủ tùy bạn thiết kế)
                return RedirectToAction("DangNhap", "Home");
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi hệ thống nếu có phát sinh (ví dụ: mất kết nối DB)
                TempData["Error"] = "Vui Lòng Kiểm tra lại số điện thoại!";
                return View(tk);
            }
        }
        //================ Gio hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemVaoGio(int maCtsp, int soLuong = 1)
        {
            // Chưa đăng nhập
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!";
                return RedirectToAction("DangNhap", "Home");
            }

            try
            {
                var gio = db.GioHangs.FirstOrDefault(x =>
                            x.MaTk == maTK &&
                            x.MaCtsp == maCtsp);

                if (gio != null)
                {
                    gio.SoLuong += soLuong;
                }
                else
                {
                    GioHang item = new GioHang();

                    item.MaTk = maTK.Value;
                    item.MaCtsp = maCtsp;
                    item.SoLuong = soLuong;
                    item.NgayThem = DateTime.Now;

                    db.GioHangs.Add(item);
                }

                db.SaveChanges();

                TempData["Success"] = "Đã thêm vào giỏ hàng!";
            }
            catch
            {
                TempData["Error"] = "Không thể thêm vào giỏ hàng!";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult GioHang()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            var list = db.GioHangs
                         .Include(x => x.MaCtspNavigation)
                            .ThenInclude(x => x.MaSpNavigation)
                         .Include(x => x.MaCtspNavigation)
                            .ThenInclude(x => x.MaMauNavigation)
                         .Include(x => x.MaCtspNavigation)
                            .ThenInclude(x => x.MaSizeNavigation)
                         .Where(x => x.MaTk == maTK)
                         .OrderByDescending(x => x.NgayThem)
                         .ToList();

            return View(list);
        }
        public JsonResult LayChiTiet(string maSp, string maMau, string maSize)
        {
            var ct = db.ChiTietSanPhams
                       .FirstOrDefault(x =>
                            x.MaSp == maSp &&
                            x.MaMau == maMau &&
                            x.MaSize == maSize);

            if (ct == null)
                return Json(null);

            return Json(new
            {
                maCtsp = ct.MaCtsp,
                
                ton = ct.SoLuongTon
            });
        }
        public IActionResult XoaKhoiGio(int maCtsp)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            try
            {
                var item = db.GioHangs.FirstOrDefault(x =>
                            x.MaTk == maTK &&
                            x.MaCtsp == maCtsp);

                if (item != null)
                {
                    db.GioHangs.Remove(item);
                    db.SaveChanges();

                    TempData["Success"] = "Đã xóa khỏi giỏ hàng!";
                }
            }
            catch
            {
                TempData["Error"] = "Không thể xóa!";
            }

            return RedirectToAction(nameof(GioHang));
        }
        public IActionResult TangSoLuong(int maCtsp)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            var item = db.GioHangs.FirstOrDefault(x =>
                        x.MaTk == maTK &&
                        x.MaCtsp == maCtsp);

            if (item != null)
            {
                item.SoLuong++;
                db.SaveChanges();
            }

            return RedirectToAction(nameof(GioHang));
        }
        public IActionResult GiamSoLuong(int maCtsp)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            var item = db.GioHangs.FirstOrDefault(x =>
                        x.MaTk == maTK &&
                        x.MaCtsp == maCtsp);

            if (item != null)
            {
                item.SoLuong--;

                if (item.SoLuong <= 0)
                {
                    db.GioHangs.Remove(item);
                }

                db.SaveChanges();
            }

            return RedirectToAction(nameof(GioHang));
        }
        [HttpGet]
        public JsonResult LaySoLuongGio()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                return Json(new
                {
                    cartCount = 0
                });
            }

            int cartCount = db.GioHangs
                              .Where(x => x.MaTk == maTK)
                              .Sum(x => x.SoLuong);

            return Json(new
            {
                cartCount = cartCount
            });
        }
        //============ Thanh Toán
        private double TinhSoTienGiam(string loaiGiam, double giaTriGiam, double soTienGoc)
        {
            // loaiGiam: "%" hoặc "Tien"
            double soTien = loaiGiam == "%" ? soTienGoc * giaTriGiam / 100 : giaTriGiam;
            if (soTien > soTienGoc) soTien = soTienGoc; // không giảm vượt quá phần tiền đang xét
            if (soTien < 0) soTien = 0;
            return soTien;
        }

        private KetQuaKhuyenMai TinhKhuyenMaiApDung(List<GioHang> gioHang, string? codeNhap)
        {
            var ketQua = new KetQuaKhuyenMai();
            var now = DateTime.Now;

            // 1. Tổng tiền gốc
            ketQua.TongTienGoc = gioHang.Sum(x => x.SoLuong * x.MaCtspNavigation.MaSpNavigation.Gia);
            double subTotal = ketQua.TongTienGoc;

            // ====== 2. KM riêng theo SẢN PHẨM (bảng SanPhamKhuyenMai) ======
            var maSpList = gioHang.Select(x => x.MaCtspNavigation.MaSp).Distinct().ToList();

            var kmSanPham = db.SanPhamKhuyenMais
                .Include(x => x.MaKmNavigation)
                .Where(x => maSpList.Contains(x.MaSp)
                         && x.MaKmNavigation.TrangThai == true
                         && x.MaKmNavigation.LoaiKm == "SanPham")
                .ToList();

            foreach (var item in gioHang)
            {
                string maSp = item.MaCtspNavigation.MaSp;
                double thanhTienDong = item.SoLuong * item.MaCtspNavigation.MaSpNavigation.Gia;

                var tot = kmSanPham
                    .Where(x => x.MaSp == maSp)
                    .Select(x => new {
                        x.MaKmNavigation,
                        SoTien = TinhSoTienGiam(x.MaKmNavigation.LoaiGiam, x.MaKmNavigation.GiaTriGiam, thanhTienDong)
                    })
                    .OrderByDescending(x => x.SoTien)
                    .FirstOrDefault();

                if (tot != null && tot.SoTien > 0)
                {
                    ketQua.GiamTheoSanPham[item.MaCtsp] = tot.SoTien;
                    ketQua.ChiTietGiam.Add(new ChiTietGiamGia
                    {
                        MaKm = tot.MaKmNavigation.MaKm,
                        LoaiKm = "SanPham",
                        TenKm = tot.MaKmNavigation.TenKm,
                        SoTienGiam = tot.SoTien,
                        MaSp = maSp
                    });
                    subTotal -= tot.SoTien;
                }
            }

            // subTotal bây giờ = tiền sau khi trừ KM riêng sản phẩm
            // -> Combo, TongDon, Code đều xét điều kiện DonToiThieu / SoLuong trên GIÁ TRỊ NÀY

            // ====== 3. KM TỔNG ĐƠN (DonToiThieu so với subTotal sau KM sản phẩm) ======
            int tongSoLuong = gioHang.Sum(x => x.SoLuong);

            var kmTongDon = db.KhuyenMais
                .Where(x => x.LoaiKm == "TongDon"
                         && x.TrangThai == true
                         && (x.DonToiThieu == null || subTotal >= x.DonToiThieu))
                .OrderByDescending(x => x.GiaTriGiam)
                .FirstOrDefault();

            if (kmTongDon != null)
            {
                double tienGiam = TinhSoTienGiam(kmTongDon.LoaiGiam, kmTongDon.GiaTriGiam, subTotal);
                if (tienGiam > 0)
                {
                    ketQua.ChiTietGiam.Add(new ChiTietGiamGia
                    {
                        MaKm = kmTongDon.MaKm,
                        LoaiKm = "TongDon",
                        TenKm = kmTongDon.TenKm,
                        SoTienGiam = tienGiam
                    });
                    subTotal -= tienGiam;
                }
            }

            // ====== 4. MÃ CODE / MÃ CÁ NHÂN / MÃ COMBO ======
            // Combo giờ không còn tự động áp dụng nữa -> phải nhập mã (codeNhap) giống CN/Code.
            // Chỉ tính khi có codeNhap truyền vào -> nhập mã mới thì hàm chạy lại từ đầu,
            // mã cũ tự động "mất" vì không còn được truyền vào nữa (xử lý ở tầng gọi hàm/Session)
            if (!string.IsNullOrWhiteSpace(codeNhap))
            {
                // ================== MÃ CÁ NHÂN ==================
                if (codeNhap.StartsWith("CN"))
                {
                    int? maTK = HttpContext.Session.GetInt32("MaTK");

                    string maTKStr = codeNhap.Substring(2, 4);
                    int maTKCode = int.Parse(maTKStr);

                    // Khôi phục mã gốc
                    string maCode = codeNhap.Remove(2, 4);

                    if (maTKCode != maTK)
                    {
                        ketQua.LoiCode = "Mã giảm giá này không thuộc tài khoản của bạn.";
                    }
                    else
                    {
                        var kmCaNhan = db.KhuyenMais.FirstOrDefault(x =>
                            x.LoaiKm == "CaNhan"
                            && x.CodeKm == maCode
                            && x.TrangThai == true
                            && (x.DonToiThieu == null || subTotal >= x.DonToiThieu));

                        if (kmCaNhan != null)
                        {
                            bool daSuDung = db.TaiKhoanKhuyenMais.Any(x =>
                                x.MaTk == maTK
                                && x.MaKm == kmCaNhan.MaKm);

                            if (daSuDung)
                            {
                                ketQua.LoiCode = "Mã này đã được sử dụng.";
                            }
                            else
                            {
                                double tienGiam = TinhSoTienGiam(
                                    kmCaNhan.LoaiGiam,
                                    kmCaNhan.GiaTriGiam,
                                    subTotal);

                                if (tienGiam > 0)
                                {
                                    ketQua.ChiTietGiam.Add(new ChiTietGiamGia
                                    {
                                        MaKm = kmCaNhan.MaKm,
                                        LoaiKm = "CaNhan",
                                        TenKm = kmCaNhan.TenKm,
                                        SoTienGiam = tienGiam
                                    });

                                    subTotal -= tienGiam;
                                }
                            }
                        }
                        else
                        {
                            ketQua.LoiCode = "Mã giảm cá nhân không hợp lệ.";
                        }
                    }
                }
                // ================== MÃ COMBO / CODE THƯỜNG ==================
                else
                {
                    // Tìm theo CodeKm trước, chấp nhận cả Combo lẫn Code, rồi rẽ nhánh theo LoaiKm
                    var km = db.KhuyenMais.FirstOrDefault(x =>
                             x.CodeKm == codeNhap
                             && x.TrangThai == true
                             && (x.LoaiKm == "Combo" || x.LoaiKm == "Code"));

                    if (km == null)
                    {
                        ketQua.LoiCode = "Mã giảm giá không hợp lệ, hết hạn hoặc không đủ điều kiện.";
                    }
                    else if (km.LoaiKm == "Combo")
                    {
                        // Combo: điều kiện là đủ SỐ LƯỢNG (SoLuong), không xét DonToiThieu
                        if (km.SoLuong != null && tongSoLuong >= km.SoLuong)
                        {
                            double tienGiam = TinhSoTienGiam(km.LoaiGiam, km.GiaTriGiam, subTotal);

                            if (tienGiam > 0)
                            {
                                ketQua.ChiTietGiam.Add(new ChiTietGiamGia
                                {
                                    MaKm = km.MaKm,
                                    LoaiKm = "Combo",
                                    TenKm = km.TenKm,
                                    SoTienGiam = tienGiam
                                });

                                subTotal -= tienGiam;
                            }
                        }
                        else
                        {
                            ketQua.LoiCode = "Chưa đủ số lượng sản phẩm để áp dụng mã combo này.";
                        }
                    }
                    else // LoaiKm == "Code"
                    {
                        if (km.DonToiThieu == null || subTotal >= km.DonToiThieu)
                        {
                            double tienGiam = TinhSoTienGiam(km.LoaiGiam, km.GiaTriGiam, subTotal);

                            if (tienGiam > 0)
                            {
                                ketQua.ChiTietGiam.Add(new ChiTietGiamGia
                                {
                                    MaKm = km.MaKm,
                                    LoaiKm = "Code",
                                    TenKm = km.TenKm,
                                    SoTienGiam = tienGiam
                                });

                                subTotal -= tienGiam;
                            }
                        }
                        else
                        {
                            ketQua.LoiCode = "Mã giảm giá không hợp lệ, hết hạn hoặc không đủ điều kiện.";
                        }
                    }
                }
            }

            if (subTotal < 0) subTotal = 0;

            ketQua.TongGiamGia = ketQua.ChiTietGiam.Sum(x => x.SoTienGiam);
            ketQua.TongTienSauGiam = subTotal;

            return ketQua;
        }
        public IActionResult ThanhToan()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "Home");
            }

            var taiKhoan = db.TaiKhoans
                             .Include(x => x.MaDiaChiNavigation)
                             .FirstOrDefault(x => x.MaTk == maTK);

            if (taiKhoan == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Địa chỉ giao hàng: ưu tiên địa chỉ đã chọn lưu trong Session, chưa có thì lấy mặc định từ TaiKhoan
            int? maDiaChiGiao = HttpContext.Session.GetInt32("MaDiaChiGiao");
            DiaChi? diaChiGiao = null;

            if (maDiaChiGiao != null)
            {
                diaChiGiao = db.DiaChis.FirstOrDefault(x => x.MaDiaChi == maDiaChiGiao && x.MaTk == maTK);
            }

            if (diaChiGiao == null)
            {
                diaChiGiao = taiKhoan.MaDiaChiNavigation;
                if (diaChiGiao != null)
                {
                    HttpContext.Session.SetInt32("MaDiaChiGiao", diaChiGiao.MaDiaChi);
                }
            }

            if (diaChiGiao == null)
            {
                TempData["Error"] = "Bạn chưa có địa chỉ giao hàng, vui lòng thêm địa chỉ!";
                return RedirectToAction("ChonDiaChi");
            }

            // Giỏ hàng
            var gioHang = db.GioHangs
                            .Include(x => x.MaCtspNavigation)
                                .ThenInclude(x => x.MaSpNavigation)
                            .Include(x => x.MaCtspNavigation)
                                .ThenInclude(x => x.MaMauNavigation)
                            .Include(x => x.MaCtspNavigation)
                                .ThenInclude(x => x.MaSizeNavigation)
                            .Where(x => x.MaTk == maTK)
                            .ToList();

            if (!gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống!";
                return RedirectToAction("GioHang");
            }

            // Mã code đang áp dụng (nếu có) lấy từ Session
            string? maCodeDangDung = HttpContext.Session.GetString("MaCodeKM");

            var ketQuaKm = TinhKhuyenMaiApDung(gioHang, maCodeDangDung);

            // Nếu code trong Session không còn hợp lệ nữa (hết hạn, hết SL...) thì tự xoá khỏi Session
            if (!string.IsNullOrWhiteSpace(maCodeDangDung) && ketQuaKm.LoiCode != null)
            {
                HttpContext.Session.Remove("MaCodeKM");
            }

            ViewBag.TaiKhoan = taiKhoan;
            ViewBag.DiaChiGiao = diaChiGiao;
            ViewBag.GioHang = gioHang;
            ViewBag.KetQuaKm = ketQuaKm;
            ViewBag.MaCodeDangDung = ketQuaKm.LoiCode == null ? maCodeDangDung : null;
            ViewBag.PTTT = db.PhuongThucThanhToans.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApDungMaCode(string? code)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");
            if (maTK == null)
            {
                return Json(new { thanhCong = false, thongBao = "Vui lòng đăng nhập!" });
            }

            // Xoá khoảng trắng, nếu rỗng thì coi như huỷ mã đang dùng
            code = code?.Trim();

            if (string.IsNullOrEmpty(code))
            {
                HttpContext.Session.Remove("MaCodeKM");
            }

            var gioHang = db.GioHangs
                            .Include(x => x.MaCtspNavigation)
                                .ThenInclude(x => x.MaSpNavigation)
                            .Where(x => x.MaTk == maTK)
                            .ToList();

            if (!gioHang.Any())
            {
                return Json(new { thanhCong = false, thongBao = "Giỏ hàng đang trống!" });
            }

            var ketQua = TinhKhuyenMaiApDung(gioHang, string.IsNullOrEmpty(code) ? null : code);

            if (!string.IsNullOrEmpty(code))
            {
                if (ketQua.LoiCode != null)
                {
                    // Mã không hợp lệ -> không lưu Session, giữ nguyên mã cũ (nếu có)
                    return Json(new { thanhCong = false, thongBao = ketQua.LoiCode });
                }

                // Nhập mã mới -> ghi đè Session, mã cũ mất
                HttpContext.Session.SetString("MaCodeKM", code);
            }

            return Json(new
            {
                thanhCong = true,
                tongTienGoc = ketQua.TongTienGoc,
                tongGiamGia = ketQua.TongGiamGia,
                tongTienSauGiam = ketQua.TongTienSauGiam,
                chiTietGiam = ketQua.ChiTietGiam.Select(x => new { x.LoaiKm, x.TenKm, x.SoTienGiam, x.MaSp })
            });
        }
        [HttpGet]
        public IActionResult ChonDiaChi()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");
            if (maTK == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var danhSachDiaChi = db.DiaChis
                                   .Where(x => x.MaTk == maTK)
                                   .OrderByDescending(x => x.MacDinh)
                                   .ThenByDescending(x => x.MaDiaChi)
                                   .ToList();

            int? maDiaChiDangChon = HttpContext.Session.GetInt32("MaDiaChiGiao");

            ViewBag.DanhSachDiaChi = danhSachDiaChi;
            ViewBag.MaDiaChiDangChon = maDiaChiDangChon;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDiaChi(DiaChi model)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");
            if (maTK == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (string.IsNullOrWhiteSpace(model.TinhThanh)
                || string.IsNullOrWhiteSpace(model.QuanHuyen)
                || string.IsNullOrWhiteSpace(model.PhuongXa)
                || string.IsNullOrWhiteSpace(model.DiaChiCuThe))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin địa chỉ!";
                return RedirectToAction("ChonDiaChi");
            }

            try
            {
                var diaChiMoi = new DiaChi
                {
                    MaTk = maTK.Value,
                    TinhThanh = model.TinhThanh.Trim(),
                    QuanHuyen = model.QuanHuyen.Trim(),
                    PhuongXa = model.PhuongXa.Trim(),
                    DiaChiCuThe = model.DiaChiCuThe.Trim(),
                    SdtNguoiNhan = model.SdtNguoiNhan?.Trim(),
                    MacDinh = false
                };

                db.DiaChis.Add(diaChiMoi);
                db.SaveChanges();

                // Thêm xong thì chọn luôn địa chỉ vừa thêm để giao hàng
                HttpContext.Session.SetInt32("MaDiaChiGiao", diaChiMoi.MaDiaChi);

                TempData["Success"] = "Thêm địa chỉ thành công!";
                return RedirectToAction("ThanhToan");
            }
            catch
            {
                TempData["Error"] = "Không thể thêm địa chỉ, vui lòng thử lại!";
                return RedirectToAction("ChonDiaChi");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChonDiaChiGiaoHang(int maDiaChi)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");
            if (maTK == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var diaChi = db.DiaChis.FirstOrDefault(x => x.MaDiaChi == maDiaChi && x.MaTk == maTK);

            if (diaChi == null)
            {
                TempData["Error"] = "Địa chỉ không hợp lệ!";
                return RedirectToAction("ChonDiaChi");
            }

            HttpContext.Session.SetInt32("MaDiaChiGiao", diaChi.MaDiaChi);

            return RedirectToAction("ThanhToan");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DatHang(int maPttt, string? ghiChu)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");
            if (maTK == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            try
            {
                var taiKhoan = db.TaiKhoans.FirstOrDefault(x => x.MaTk == maTK);
                if (taiKhoan == null)
                {
                    TempData["Error"] = "Không tìm thấy tài khoản!";
                    return RedirectToAction("ThanhToan");
                }

                // Địa chỉ giao hàng: lấy từ Session, không dùng taiKhoan.MaDiaChi mặc định nữa
                int? maDiaChiGiao = HttpContext.Session.GetInt32("MaDiaChiGiao");
                var diaChiGiao = maDiaChiGiao != null
                    ? db.DiaChis.FirstOrDefault(x => x.MaDiaChi == maDiaChiGiao && x.MaTk == maTK)
                    : null;

                if (diaChiGiao == null)
                {
                    TempData["Error"] = "Vui lòng chọn địa chỉ giao hàng!";
                    return RedirectToAction("ThanhToan");
                }

                var gioHang = db.GioHangs
                    .Include(x => x.MaCtspNavigation)
                        .ThenInclude(x => x.MaSpNavigation)
                    .Where(x => x.MaTk == maTK)
                    .ToList();

                if (!gioHang.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống!";
                    return RedirectToAction("GioHang");
                }

                // Kiểm tra tồn kho
                foreach (var item in gioHang)
                {
                    if (item.SoLuong > item.MaCtspNavigation.SoLuongTon)
                    {
                        TempData["Error"] = "Sản phẩm " + item.MaCtspNavigation.MaSp + " không đủ số lượng.";
                        return RedirectToAction("ThanhToan");
                    }
                }

                // Tính lại toàn bộ KM (sản phẩm + combo + tổng đơn + code) ngay tại thời điểm đặt hàng
                string? maCodeDangDung = HttpContext.Session.GetString("MaCodeKM");
                var ketQuaKm = TinhKhuyenMaiApDung(gioHang, maCodeDangDung);

                // Nếu code trong Session bỗng không hợp lệ nữa (hết hạn/hết SL đúng lúc đặt) -> báo lỗi, không cho đặt ngầm sai giá
                if (!string.IsNullOrWhiteSpace(maCodeDangDung) && ketQuaKm.LoiCode != null)
                {
                    HttpContext.Session.Remove("MaCodeKM");
                    TempData["Error"] = "Mã giảm giá không còn hiệu lực, vui lòng kiểm tra lại!";
                    return RedirectToAction("ThanhToan");
                }

                DonHang dh = new DonHang();
                dh.MaTk = maTK.Value;
                dh.MaDiaChi = diaChiGiao.MaDiaChi;
                dh.NgayDat = DateTime.Now;
                dh.TongTien = ketQuaKm.TongTienSauGiam;
                dh.MaPttt = maPttt;
                dh.GhiChu = ghiChu;
                dh.TrangThaiThanhToan = "Chưa thanh toán";
                dh.TrangThai = "Chờ xác nhận";
                

                db.DonHangs.Add(dh);
                db.SaveChanges();

                // Chi tiết đơn hàng
                foreach (var item in gioHang)
                {
                    ChiTietDonHang ct = new ChiTietDonHang();
                    ct.MaDh = dh.MaDh;
                    ct.MaCtsp = item.MaCtsp;
                    ct.SoLuong = item.SoLuong;
                    ct.DonGia = item.MaCtspNavigation.MaSpNavigation.Gia;

                    // Thành tiền của dòng = tiền gốc - phần giảm riêng của sản phẩm này (nếu có)
                    double thanhTienGoc = item.SoLuong * item.MaCtspNavigation.MaSpNavigation.Gia;
                    double giamRieng = ketQuaKm.GiamTheoSanPham.ContainsKey(item.MaCtsp)
                        ? ketQuaKm.GiamTheoSanPham[item.MaCtsp]
                        : 0;
                    ct.ThanhTien = thanhTienGoc - giamRieng;

                    db.ChiTietDonHangs.Add(ct);
                }

                // Lưu danh sách khuyến mãi đã áp dụng vào DonHang_KhuyenMai
                // (bao gồm cả loại "SanPham" -> mỗi dòng SP giảm giá cũng được ghi nhận là 1 KM áp dụng cho đơn)
                foreach (var km in ketQuaKm.ChiTietGiam)
                {
                    var dhkm = new DonHangKhuyenMai
                    {
                        MaDh = dh.MaDh,
                        MaKm = km.MaKm,
                        SoTienGiam = km.SoTienGiam
                    };
                    db.DonHangKhuyenMais.Add(dhkm);
                }
                // Lưu lịch sử sử dụng mã giảm giá (chỉ Code và CaNhan)
                foreach (var km in ketQuaKm.ChiTietGiam)
                {
                    if (km.LoaiKm == "Code" || km.LoaiKm == "CaNhan")
                    {
                        db.TaiKhoanKhuyenMais.Add(new TaiKhoanKhuyenMai
                        {
                            MaTk = maTK.Value,
                            MaKm = km.MaKm,
                            NgaySuDung = DateTime.Now
                        });
                    }
                }
                db.SaveChanges();

                // Nếu COD -> trừ kho, xoá giỏ hàng, xoá mã code đã dùng
                if (maPttt == 2)
                {
                    foreach (var item in gioHang)
                    {
                        item.MaCtspNavigation.SoLuongTon -= item.SoLuong;
                    }

                    db.GioHangs.RemoveRange(gioHang);
                    db.SaveChanges();

                    HttpContext.Session.Remove("MaCodeKM");

                    TempData["Success"] = "Đặt hàng thành công!";
                    return RedirectToAction("GioHang");
                }

                // Nếu VNPAY -> chưa xoá giỏ hàng/mã code vội, đợi thanh toán xong (xử lý ở callback MoMo)
                return RedirectToAction("ThanhToanVnPay", "Payment", new { maDH = dh.MaDh });
            }
            catch
            {
                TempData["Error"] = "Không thể đặt hàng!";
            }

            return RedirectToAction("ThanhToan");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
