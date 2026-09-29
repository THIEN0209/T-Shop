using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhanVienGiaoHang.Models;

namespace NhanVienGiaoHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        TshopContext db= new TshopContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string tinhThanh, string quanHuyen)
        {
            IQueryable<DonHang> query = db.DonHangs
                .Include(d => d.MaDiaChiNavigation)
                .Where(d => d.TrangThai == "Đã xác nhận" && d.MaTaiXe == null);

            if (!string.IsNullOrEmpty(tinhThanh))
            {
                query = query.Where(d => d.MaDiaChiNavigation.TinhThanh == tinhThanh);
            }

            if (!string.IsNullOrEmpty(quanHuyen))
            {
                query = query.Where(d => d.MaDiaChiNavigation.QuanHuyen == quanHuyen);
            }

            // Lấy danh sách tỉnh/thành để đổ vào dropdown (không bị ảnh hưởng bởi filter hiện tại)
            ViewBag.DsTinhThanh = db.DiaChis
                .Select(x => x.TinhThanh)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Danh sách quận/huyện — nếu đã chọn tỉnh thì chỉ lấy huyện thuộc tỉnh đó
            var quanHuyenQuery = db.DiaChis.AsQueryable();
            if (!string.IsNullOrEmpty(tinhThanh))
            {
                quanHuyenQuery = quanHuyenQuery.Where(x => x.TinhThanh == tinhThanh);
            }
            ViewBag.DsQuanHuyen = quanHuyenQuery
                .Select(x => x.QuanHuyen)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.TinhThanhDaChon = tinhThanh;
            ViewBag.QuanHuyenDaChon = quanHuyen;

            var dh = query.ToList();
            return View(dh);
        }
        public IActionResult DaNhan()
        {
            var maTk = HttpContext.Session.GetInt32("MaTk");
            var dh = db.DonHangs
                .Include(d => d.MaDiaChiNavigation)
                .Where(d => d.MaTaiXe == maTk && d.TrangThai == "Đã xác nhận")
                .ToList();
            return View(dh);
        }
        public IActionResult LichSu()
        {
            var maTk = HttpContext.Session.GetInt32("MaTk");
            var dh = db.DonHangs
                .Include(d => d.MaDiaChiNavigation)
                .Where(d => d.MaTaiXe == maTk && d.TrangThai == "Hoàn thành")
                .OrderByDescending(d => d.NgayDat)
                .ToList();
            return View(dh);
        }
        [HttpPost]
        public IActionResult NhanDonHang(int id)
        {
            var maTk = HttpContext.Session.GetInt32("MaTk");

            var donHang = db.DonHangs.FirstOrDefault(d => d.MaDh == id);

            if (donHang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }

            if (donHang.MaTaiXe != null)
            {
                TempData["Error"] = "Đã có người nhận đơn trước.";
                return RedirectToAction("Index");
            }

            donHang.MaTaiXe = maTk;
            db.SaveChanges();

            TempData["Success"] = $"Nhận đơn #{donHang.MaDh} thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult HoanThanhDon(int id)
        {
            var maTk = HttpContext.Session.GetInt32("MaTk");

            var donHang = db.DonHangs.FirstOrDefault(d => d.MaDh == id && d.MaTaiXe == maTk);

            if (donHang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại hoặc không thuộc về bạn.";
                return RedirectToAction("DaNhan");
            }

            donHang.TrangThai = "Hoàn thành";

            if (donHang.TrangThaiThanhToan == "Chưa thanh toán")
            {
                donHang.TrangThaiThanhToan = "Đã thanh toán";
            }

            db.SaveChanges();

            TempData["Success"] = $"Đơn #{donHang.MaDh} đã hoàn thành!";
            return RedirectToAction("DaNhan");
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(string soDienThoai, string matKhau)
        {
            var taiKhoan = await db.TaiKhoans
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(t => t.SoDienThoai == soDienThoai);

            if (taiKhoan == null || taiKhoan.MatKhau != matKhau)
            {
                TempData["Error"] = "Số điện thoại hoặc mật khẩu không đúng.";
                return RedirectToAction("DangNhap");
            }

            if (taiKhoan.TrangThai == false)
            {
                TempData["Error"] = "Tài khoản đã bị khóa.";
                return RedirectToAction("DangNhap");
            }

            if (taiKhoan.MaVaiTro != 3)
            {
                TempData["Error"] = "Tài khoản này không phải là Nhân viên giao hàng.";
                return RedirectToAction("DangNhap");
            }

            HttpContext.Session.SetInt32("MaTk", taiKhoan.MaTk);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetInt32("MaVaiTro", taiKhoan.MaVaiTro);

            TempData["Success"] = $"Chào mừng {taiKhoan.HoTen}!";
            return RedirectToAction("Index", "Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
