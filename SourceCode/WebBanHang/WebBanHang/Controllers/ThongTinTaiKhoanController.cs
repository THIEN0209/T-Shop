using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ThongTinTaiKhoanController : Controller
    {   TshopContext db=new TshopContext();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.LoaiSP = db.LoaiSanPhams.ToList();
            ViewBag.DanhMuc = db.DanhMucs.ToList();

            ViewBag.BaiVietChinhSach = db.BaiViets
   .Where(x => x.Loai == 2)
   .OrderBy(x => x.TenBaiViet)
   .ToList();

            ViewBag.BaiVietThuong = db.BaiViets
                .Where(x => x.Loai == 1)
                .OrderBy(x => x.TenBaiViet)
                .ToList();

            ViewBag.DiaChiCuaHang = db.DiaChiCuaHangs
                          .OrderBy(x => x.Dc)
                          .ToList();
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var tk = db.TaiKhoans
                .Include(x => x.MaVaiTroNavigation)
                .FirstOrDefault(x => x.MaTk == maTK);



            return View(tk);
        }
        public IActionResult DonHang()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var list = db.DonHangs
                .Include(x => x.MaDiaChiNavigation)
                .Include(x => x.MaPtttNavigation)
                .Where(x => x.MaTk == maTK)
                .OrderByDescending(x => x.NgayDat)
                .ToList();

            return View(list);
        }
        public IActionResult ChiTietDonHang(int id)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var donHang = db.DonHangs
                .Include(x => x.MaDiaChiNavigation)
                .Include(x => x.MaPtttNavigation)
                .FirstOrDefault(x => x.MaDh == id
                                  && x.MaTk == maTK);

            if (donHang == null)
                return NotFound();

            var chiTiet = db.ChiTietDonHangs

                .Include(x => x.MaCtspNavigation)

                    .ThenInclude(x => x.MaSpNavigation)

                .Include(x => x.MaCtspNavigation)

                    .ThenInclude(x => x.MaMauNavigation)

                .Include(x => x.MaCtspNavigation)

                    .ThenInclude(x => x.MaSizeNavigation)

                .Where(x => x.MaDh == id)

                .ToList();

            ViewBag.ChiTiet = chiTiet;

            return View(donHang);
        }
        public IActionResult YeuThich()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var list = db.YeuThiches

                .Include(x => x.MaSpNavigation)

                    .ThenInclude(x => x.HinhAnhSanPhams)

                .Where(x => x.MaTk == maTK)

                .OrderByDescending(x => x.NgayThem)

                .ToList();

            return View(list);
        }
        public IActionResult XoaYeuThich(string id)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var item = db.YeuThiches
                .FirstOrDefault(x => x.MaTk == maTK && x.MaSp == id);

            if (item != null)
            {
                db.YeuThiches.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("YeuThich");
        }
        public IActionResult DiaChi()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var list = db.DiaChis

                .Where(x => x.MaTk == maTK)

                .OrderByDescending(x => x.MacDinh)

                .ToList();

            return View(list);
        }
        public IActionResult XoaDiaChi(int id)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var diaChi = db.DiaChis.FirstOrDefault(x => x.MaDiaChi == id && x.MaTk == maTK);

            if (diaChi == null)
            {
                TempData["Error"] = "Không tìm thấy địa chỉ.";
                return RedirectToAction("DiaChi");
            }

            if (diaChi.MacDinh)
            {
                TempData["Error"] = "Không thể xóa địa chỉ mặc định.";
                return RedirectToAction("DiaChi");
            }

            db.DiaChis.Remove(diaChi);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa địa chỉ.";

            return RedirectToAction("DiaChi");
        }
        public IActionResult DatMacDinh(int id)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            // Địa chỉ được chọn
            var diaChi = db.DiaChis.FirstOrDefault(x => x.MaDiaChi == id && x.MaTk == maTK);

            if (diaChi == null)
            {
                TempData["Error"] = "Không tìm thấy địa chỉ.";
                return RedirectToAction("DiaChi");
            }

            // Bỏ mặc định của tất cả địa chỉ của tài khoản
            var listDiaChi = db.DiaChis
                               .Where(x => x.MaTk == maTK)
                               .ToList();

            foreach (var item in listDiaChi)
            {
                item.MacDinh = false;
            }

            // Đặt địa chỉ mới làm mặc định
            diaChi.MacDinh = true;

            // Cập nhật địa chỉ mặc định của tài khoản
            var taiKhoan = db.TaiKhoans.FirstOrDefault(x => x.MaTk == maTK);

            if (taiKhoan != null)
            {
                taiKhoan.MaDiaChi = diaChi.MaDiaChi;
            }

            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật địa chỉ mặc định.";

            return RedirectToAction("DiaChi");
        }
        public IActionResult HuyDon(int id)
        {
            var maTK = HttpContext.Session.GetInt32("MaTK");

            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDh == id && x.MaTk == maTK);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DonHang");
            }

            if (donHang.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Đơn hàng đã bị hủy.";
                return RedirectToAction("DonHang");
            }

            donHang.TrangThai = "Đã hủy";

            db.SaveChanges();

            TempData["Success"] = "Đã hủy đơn hàng.";

            return RedirectToAction("DonHang");
        }
        public IActionResult YeuCauDoiTra(int id)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var donHang = db.DonHangs
                .Include(x => x.MaDiaChiNavigation)
                .Include(x => x.MaPtttNavigation)
                .FirstOrDefault(x => x.MaDh == id
                                  && x.MaTk == maTK);
            if (donHang == null)
                return NotFound();
            ViewBag.DH = donHang;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult YeuCauDoiTra(YeuCauDoiTra model)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDh == model.MaDh
                                                       && x.MaTk == maTK);

            if (donHang == null)
                return NotFound();

            model.NgayYeuCau = DateTime.Now;
            model.TrangThai = "Chờ xử lý";
            donHang.TrangThai = "Chờ xử lý";
            db.YeuCauDoiTras.Add(model);
            db.SaveChanges();

            TempData["Success"] = "Đã gửi yêu cầu đổi trả thành công!";

            return RedirectToAction("ChiTietDonHang", new { id = model.MaDh });
        }
        public IActionResult SuaThongTin()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var tk = db.TaiKhoans
                .Include(x => x.MaVaiTroNavigation)
                .FirstOrDefault(x => x.MaTk == maTK);



            return View(tk);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaThongTin(TaiKhoan model)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var tk = db.TaiKhoans.FirstOrDefault(x => x.MaTk == maTK);

            if (tk == null)
                return NotFound();

            tk.HoTen = model.HoTen;
            tk.Email = model.Email;
            tk.SoDienThoai = model.SoDienThoai;
            tk.MatKhau = model.MatKhau;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";

            return RedirectToAction("index");
        }
    }
}
