using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebBanHang.Controllers
{
    public class AdminController : Controller
    {
        TshopContext db = new TshopContext();
        public class KetQuaImport
        {
            public bool ThanhCong { get; set; }
            public string ThongDiep { get; set; } = "";
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(vaiTro))
            {
                context.Result = RedirectToAction("DangNhap", "Home");
                return;
            }

            if (vaiTro != "Admin"
                && vaiTro != "Chủ shop"
                && vaiTro != "Quản lý bán hàng")
            {
                TempData["Error"] = "Bạn không phải thành viên trong quản trị!";
                context.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(context);
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
        //Thống kê
        public IActionResult Index(int? nam)
        {
            int year = nam ?? DateTime.Now.Year;

            ViewBag.Nam = year;

            ViewBag.NamList = db.DonHangs
                .Select(x => x.NgayDat.Year)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            var donHang = db.DonHangs
                .Where(x => x.TrangThai == "Hoàn thành"
                         && x.NgayDat.Year == year)
                .ToList();

            ViewBag.TongDoanhThu = donHang.Sum(x => x.TongTien);
            ViewBag.SoDon = donHang.Count;
            ViewBag.TrungBinh = donHang.Count == 0 ? 0 : donHang.Sum(x => x.TongTien) / donHang.Count;

            double[] doanhThu = new double[12];

            var ds = donHang
                .GroupBy(x => x.NgayDat.Month)
                .Select(x => new
                {
                    Thang = x.Key,
                    TongTien = x.Sum(y => y.TongTien)
                });

            foreach (var item in ds)
            {
                doanhThu[item.Thang - 1] = item.TongTien;
            }

            ViewBag.DoanhThu = doanhThu;

            return View();
        }
        public IActionResult SanPhamBanChay()
        {
            var list = db.ChiTietDonHangs
                .Include(x => x.MaCtspNavigation)
                    .ThenInclude(x => x.MaSpNavigation)
                .Include(x => x.MaCtspNavigation)
                    .ThenInclude(x => x.MaMauNavigation)
                .Include(x => x.MaCtspNavigation)
                    .ThenInclude(x => x.MaSizeNavigation)
                .Include(x => x.MaDhNavigation)
                .Where(x => x.MaDhNavigation.TrangThai == "Hoàn thành")
                .GroupBy(x => x.MaCtsp)
                .Select(g => new
                {
                    MaSP = g.First().MaCtspNavigation.MaSp,
                    TenSP = g.First().MaCtspNavigation.MaSpNavigation.TenSp,
                    Mau = g.First().MaCtspNavigation.MaMauNavigation.TenMau,
                    Size = g.First().MaCtspNavigation.MaSizeNavigation.TenSize,
                    TongBan = g.Sum(x => x.SoLuong),
                    DoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongBan)
                .ToList();

            return View(list);
        }
        //QUẢN LÝ SẢN PHẨM
        public IActionResult QuanLySanPham()
        {
            ViewBag.list = db.SanPhams
                             .Include(x => x.MaLoaiNavigation)
                             .ToList();

            ViewBag.listLoai = db.LoaiSanPhams.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(SanPham sp)
        {
            try
            {
                db.SanPhams.Add(sp);
                db.SaveChanges();

                TempData["Success"] = "Đã thêm sản phẩm thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể thêm sản phẩm!";
            }

            return RedirectToAction("QuanLySanPham");
        }
        [HttpGet]
        public IActionResult SuaSanPham(string id)
        {
            var sp = db.SanPhams.Find(id);

            if (sp == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("QuanLySanPham");
            }

            ViewBag.listLoai = db.LoaiSanPhams.ToList();

            return View(sp);
        }
        //Thêm chi tiết sản phẩm

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(SanPham sp)
        {
            try
            {
                var sanPham = db.SanPhams.Find(sp.MaSp);

                if (sanPham == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("QuanLySanPham");
                }

                sanPham.TenSp = sp.TenSp;
                sanPham.GioiTinh = sp.GioiTinh;
                sanPham.MoTa = sp.MoTa;
                sanPham.MaLoai = sp.MaLoai;

                db.SaveChanges();

                TempData["Success"] = "Cập nhật sản phẩm thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể cập nhật sản phẩm!";
            }

            return RedirectToAction("QuanLySanPham");
        }
        public IActionResult XoaSanPham(string id)
        {
            try
            {
                var sp = db.SanPhams.Find(id);

                if (sp == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("QuanLySanPham");
                }

                db.SanPhams.Remove(sp);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa sản phẩm!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa sản phẩm!";
            }

            return RedirectToAction("QuanLySanPham");
        }
        //QUẢN LÝ DANH MỤC
        public IActionResult QuanLyDanhMuc()
        {
            ViewBag.list = db.DanhMucs.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemDanhMuc(DanhMuc dm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.DanhMucs.Add(dm);
                    db.SaveChanges();

                    TempData["Success"] = "Đã thêm danh mục thành công.";
                }
            }
            catch
            {
                TempData["Error"] = "Không thể thêm danh mục.";
            }

            return RedirectToAction("QuanLyDanhMuc");
        }
        public IActionResult XoaDanhMuc(string id)
        {
            try
            {
                var dm = db.DanhMucs.Find(id);

                if (dm == null)
                {
                    TempData["Error"] = "Không tìm thấy danh mục.";
                    return RedirectToAction("QuanLyDanhMuc");
                }

                db.DanhMucs.Remove(dm);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa danh mục thành công.";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa danh mục này.";
            }

            return RedirectToAction("QuanLyDanhMuc");
        }
        [HttpGet]
        public IActionResult SuaDanhMuc(string id)
        {
            var dm = db.DanhMucs.Find(id);

            if (dm == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục!";
                return RedirectToAction("QuanLyDanhMuc");
            }

            return View(dm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaDanhMuc(DanhMuc dm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.DanhMucs.Update(dm);
                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("QuanLyDanhMuc");
                }
            }
            catch
            {
                TempData["Error"] = "Không thể cập nhật danh mục!";
            }

            return View(dm);
        }
        //QUẢN LÝ LOẠI SẢN PHẨM
        public IActionResult QuanLyLoaiSanPham()
        {
            ViewBag.list = db.LoaiSanPhams
                     .Include(x => x.MaDanhMucNavigation)
                     .ToList();
            ViewBag.DanhMuc = db.DanhMucs.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemLoaiSanPham(LoaiSanPham loai)
        {
            try
            {
                db.LoaiSanPhams.Add(loai);
                db.SaveChanges();

                TempData["Success"] = "Đã thêm loại sản phẩm thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể thêm loại sản phẩm!";
            }

            return RedirectToAction("QuanLyLoaiSanPham");
        }
        [HttpGet]
        public IActionResult SuaLoaiSanPham(string id)
        {
            var loai = db.LoaiSanPhams.Find(id);

            if (loai == null)
            {
                TempData["Error"] = "Không tìm thấy loại sản phẩm!";
                return RedirectToAction("QuanLyLoaiSanPham");
            }

            ViewBag.DanhMuc = db.DanhMucs.ToList();

            return View(loai);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaLoaiSanPham(LoaiSanPham loai)
        {
            try
            {
                var lsp = db.LoaiSanPhams.Find(loai.MaLoai);

                if (lsp == null)
                {
                    TempData["Error"] = "Không tìm thấy loại sản phẩm!";
                    return RedirectToAction("QuanLyLoaiSanPham");
                }

                lsp.TenLoai = loai.TenLoai;
                lsp.MaDanhMuc = loai.MaDanhMuc;

                db.SaveChanges();

                TempData["Success"] = "Cập nhật loại sản phẩm thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể cập nhật loại sản phẩm!";
            }

            return RedirectToAction("QuanLyLoaiSanPham");
        }
        public IActionResult XoaLoaiSanPham(string id)
        {
            try
            {
                var loai = db.LoaiSanPhams.Find(id);

                if (loai == null)
                {
                    TempData["Error"] = "Không tìm thấy loại sản phẩm!";
                    return RedirectToAction("QuanLyLoaiSanPham");
                }

                db.LoaiSanPhams.Remove(loai);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa loại sản phẩm!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa loại sản phẩm!";
            }

            return RedirectToAction("QuanLyLoaiSanPham");
        }
        //QUẢN LÝ MÀU SẮC
        public IActionResult QuanLyMauSac()
        {
            ViewBag.list = db.MauSacs.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemMauSac(MauSac mau)
        {
            try
            {
                db.MauSacs.Add(mau);
                db.SaveChanges();

                TempData["Success"] = "Đã thêm màu sắc thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể thêm màu sắc!";
            }

            return RedirectToAction("QuanLyMauSac");
        }
        [HttpGet]
        public IActionResult SuaMauSac(string id)
        {
            var mau = db.MauSacs.Find(id);

            if (mau == null)
            {
                TempData["Error"] = "Không tìm thấy màu sắc!";
                return RedirectToAction("QuanLyMauSac");
            }

            return View(mau);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaMauSac(MauSac mau)
        {
            try
            {
                var ms = db.MauSacs.Find(mau.MaMau);

                if (ms == null)
                {
                    TempData["Error"] = "Không tìm thấy màu sắc!";
                    return RedirectToAction("QuanLyMauSac");
                }

                ms.TenMau = mau.TenMau;

                db.SaveChanges();

                TempData["Success"] = "Cập nhật màu sắc thành công!";
            }
            catch
            {
                TempData["Error"] = "Không thể cập nhật màu sắc!";
            }

            return RedirectToAction("QuanLyMauSac");
        }
        public IActionResult XoaMauSac(string id)
        {
            try
            {
                var mau = db.MauSacs.Find(id);

                if (mau == null)
                {
                    TempData["Error"] = "Không tìm thấy màu sắc!";
                    return RedirectToAction("QuanLyMauSac");
                }

                db.MauSacs.Remove(mau);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa màu sắc!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa màu sắc!";
            }

            return RedirectToAction("QuanLyMauSac");
        }
        public IActionResult QuanLyHinhAnh()
        {
            var Model = db.SanPhams.ToList();
            return View(Model);
        }
        [HttpGet]
        public IActionResult ThemHinhAnh(string id)
        {
            var sanPham = db.SanPhams.FirstOrDefault(s => s.MaSp == id);
            var danhSachAnh = db.HinhAnhSanPhams
                .Where(h => h.MaSp == id)
                .ToList();
            var danhSachMau = db.MauSacs.ToList();
            ViewBag.DanhSachMau = danhSachMau;
            ViewBag.MaSp = id;
            ViewBag.TenSp = sanPham.TenSp;

            return View(danhSachAnh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(string maSp, string tenMau, IFormFile fileAnh)
        {
            if (fileAnh == null || fileAnh.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file ảnh hợp lệ.";
                return RedirectToAction(nameof(ThemHinhAnh), new { id = maSp });
            }

            if (string.IsNullOrEmpty(tenMau))
            {
                tenMau = "mặc định";
            }

            try
            {
                // Bước 1: Xác định tên file dự kiến dựa theo quy tắc: MaSP + TenMau
                string extension = Path.GetExtension(fileAnh.FileName);
                string safeTenMau = tenMau.Replace(" ", "");
                string fileName = $"{maSp}_{safeTenMau}{extension}";

                // Bước 2: Kiểm tra xem tên file này đã tồn tại trong Database chưa
                bool isExist = db.HinhAnhSanPhams.Any(h => h.DuongDanAnh == fileName);

                if (isExist)
                {
                    // Nếu trùng tên, chặn lại ngay lập tức và bắn thông báo lỗi ra giao diện
                    TempData["Error"] = $"Hình ảnh cho màu '{tenMau}' của sản phẩm này đã tồn tại. Vui lòng chọn màu khác hoặc xóa ảnh cũ trước.";
                    return RedirectToAction(nameof(ThemHinhAnh), new { id = maSp });
                }

                // Bước 3: Nếu không trùng, tiến hành xác định thư mục lưu trữ vật lý
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imageSP");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);

                // Bước 4: Lưu file vật lý vào ổ đĩa server
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await fileAnh.CopyToAsync(stream);
                }

                // Bước 5: Lưu thông tin vào Database sau khi file đã ghi thành công
                var hinhAnh = new WebBanHang.Models.HinhAnhSanPham
                {
                    MaSp = maSp,
                    DuongDanAnh = fileName
                };

                db.HinhAnhSanPhams.Add(hinhAnh);
                db.SaveChanges();

                TempData["Success"] = "Thêm hình ảnh thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction(nameof(ThemHinhAnh), new { id = maSp });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int maHa)
        {
            // Tìm hình ảnh trong database theo MaHA bằng "db"
            var hinhAnh = db.HinhAnhSanPhams.Find(maHa);
            if (hinhAnh == null)
            {
                TempData["Error"] = "Hình ảnh không tồn tại.";
                return RedirectToAction("Index", "SanPham");
            }

            string maSpToRedirect = hinhAnh.MaSp;

            try
            {
                // Xác định đường dẫn file vật lý trong thư mục wwwroot/imageSP
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imageSP", hinhAnh.DuongDanAnh);

                // Nếu có file vật lý thì xóa đi
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Xóa bản ghi trong Database
                db.HinhAnhSanPhams.Remove(hinhAnh);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa hình ảnh thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa file ảnh: " + ex.Message;
            }

            return RedirectToAction(nameof(ThemHinhAnh), new { id = maSpToRedirect });
        }
        // QUẢN LÝ SIZE
        public IActionResult Create()
        {
            ViewBag.l = db.Sizes.ToList();
            return View();
        }

        // POST: Admin/Size/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Size size)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Kiểm tra xem mã size đã tồn tại chưa (vì MaSize là string tự nhập)
                    var existingSize = db.Sizes.Find(size.MaSize);
                    if (existingSize != null)
                    {
                        TempData["Error"] = "Mã kích thước này đã tồn tại trong hệ thống.";
                        return View(size);
                    }

                    // 2. Thêm mới và lưu vào DB
                    db.Sizes.Add(size);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm kích thước mới thành công!";
                    return RedirectToAction(nameof(Create)); // Hoặc tên Action danh sách của bạn
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi thêm dữ liệu: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu nhập vào không hợp lệ.";
            }

            return View(size);
        }
        // POST hoặc GET tùy thuộc vào cách bạn gọi từ View (Khuyên dùng POST để bảo mật)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string maSize)
        {
            // 1. Tìm kích thước trong database theo MaSize
            var size = db.Sizes.Find(maSize);
            if (size == null)
            {
                TempData["Error"] = "Kích thước không tồn tại.";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                // 2. Kiểm tra ràng buộc khóa ngoại: Nếu size đã được gán cho sản phẩm nào đó thì không được xóa
                bool hasProductVariants = db.ChiTietSanPhams.Any(ct => ct.MaSize == maSize);
                if (hasProductVariants)
                {
                    TempData["Error"] = "Không thể xóa kích thước này vì đang có sản phẩm sử dụng!";
                    return RedirectToAction(nameof(Create));
                }

                // 3. Tiến hành xóa trong Database
                db.Sizes.Remove(size);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa kích thước thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa kích thước: " + ex.Message;
            }

            return RedirectToAction(nameof(Create));
        }
        // Quản lý chi tiết sản phẩm
        [HttpGet]
        public IActionResult ThemChiTietSanPham(string id)
        {
            var sp = db.SanPhams.Find(id);

            if (sp == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("QuanLySanPham");
            }

            ViewBag.SanPham = sp;
            ViewBag.MauSac = db.MauSacs.ToList();
            ViewBag.Size = db.Sizes.ToList();

            ViewBag.list = db.ChiTietSanPhams
                             .Include(x => x.MaMauNavigation)
                             .Include(x => x.MaSizeNavigation)
                             .Where(x => x.MaSp == id)
                             .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemChiTietSanPham(ChiTietSanPham ct)
        {
            try
            {
                bool check = db.ChiTietSanPhams.Any(x =>
                            x.MaSp == ct.MaSp &&
                            x.MaMau == ct.MaMau &&
                            x.MaSize == ct.MaSize);

                if (check)
                {
                    TempData["Error"] = "Biến thể này đã tồn tại!";
                    return RedirectToAction("ThemChiTietSanPham", new { id = ct.MaSp });
                }

                db.ChiTietSanPhams.Add(ct);
                db.SaveChanges();

                TempData["Success"] = "Đã thêm biến thể sản phẩm!";
            }
            catch
            {
                TempData["Error"] = "Không thể thêm biến thể!";
            }

            return RedirectToAction("ThemChiTietSanPham", new { id = ct.MaSp });
        }
        public IActionResult XoaChiTietSanPham(int id)
        {
            var ct = db.ChiTietSanPhams.Find(id);

            if (ct == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể!";
                return RedirectToAction("QuanLySanPham");
            }

            try
            {
                bool check =
                    db.ChiTietDonHangs.Any(x => x.MaCtsp == id) ||
                    db.ChiTietPhieuNhaps.Any(x => x.MaCtsp == id) ||
                    db.GioHangs.Any(x => x.MaCtsp == id);

                if (check)
                {
                    TempData["Error"] = "Biến thể đã phát sinh dữ liệu nên không thể xóa!";
                }
                else
                {
                    db.ChiTietSanPhams.Remove(ct);
                    db.SaveChanges();

                    TempData["Success"] = "Đã xóa biến thể!";
                }
            }
            catch
            {
                TempData["Error"] = "Không thể xóa biến thể!";
            }

            return RedirectToAction("ThemChiTietSanPham", new { id = ct.MaSp });
        }
        //====================ĐƠN HÀNG
        public IActionResult QuanLyDonHang(DateTime? ngay)
        {
            var query = db.DonHangs
                .Include(x => x.MaTkNavigation)
                .AsQueryable();

            // Nếu có chọn ngày thì lọc theo ngày
            if (ngay.HasValue)
            {
                query = query.Where(x => x.NgayDat.Date == ngay.Value.Date);
            }

            var list = query
                .OrderBy(x => x.TrangThai == "Chờ xác nhận" ? 1 :
        x.TrangThai == "Đã xác nhận" ? 2 :
        x.TrangThai == "Đang giao" ? 3 :
        x.TrangThai == "Hoàn thành" ? 4 :
        x.TrangThai == "Đã hủy" ? 5 : 99)          // Ưu tiên sắp xếp theo trạng thái
                .ThenByDescending(x => x.NgayDat)   // Trong cùng trạng thái thì mới sắp xếp theo ngày
                .ToList();

            return View(list);
        }
        public IActionResult XacNhanDon(int id)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDh == id);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("QuanLyDonHang");
            }

            if (donHang.TrangThai != "Chờ xác nhận")
            {
                TempData["Error"] = "Đơn hàng đã được xử lý.";
                return RedirectToAction("QuanLyDonHang");
            }

            donHang.TrangThai = "Đã xác nhận";

            db.SaveChanges();

            TempData["Success"] = "Xác nhận đơn hàng thành công.";

            return RedirectToAction("QuanLyDonHang");
        }
        public IActionResult ChiTietDonHang(int id)
        {
            var donHang = db.DonHangs
                .Include(x => x.MaTkNavigation)
                .Include(x => x.MaDiaChiNavigation)
                .Include(x => x.MaPtttNavigation)
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(x => x.MaCtspNavigation)
                        .ThenInclude(x => x.MaSpNavigation)
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(x => x.MaCtspNavigation)
                        .ThenInclude(x => x.MaMauNavigation)
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(x => x.MaCtspNavigation)
                        .ThenInclude(x => x.MaSizeNavigation)
                .FirstOrDefault(x => x.MaDh == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
        //=================tài khoảng
        public IActionResult QuanLyKhachHang(string? tuKhoa)
        {
            var query = db.TaiKhoans
                          .Include(x => x.MaVaiTroNavigation)
                          .Where(x => x.MaVaiTro == 2)
                          .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(x =>
    x.SoDienThoai.Contains(tuKhoa) ||
    EF.Functions.Like(x.HoTen, $"%{tuKhoa}%"));
            }

            var list = query
                .OrderByDescending(x => x.NgayTao)
                .ToList();

            return View(list);
        }
        [HttpPost]
        public IActionResult DoiVaiTro(int maTk, int maVaiTro)
        {
            var tk = db.TaiKhoans.FirstOrDefault(x => x.MaTk == maTk);

            if (tk == null)
            {
                TempData["error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("QuanLyKhachHang");
            }

            // Chỉ cho phép chuyển thành 2,3,4
            if (maVaiTro != 2 && maVaiTro != 3 && maVaiTro != 4)
            {
                TempData["error"] = "Vai trò không hợp lệ.";
                return RedirectToAction("QuanLyKhachHang");
            }

            tk.MaVaiTro = maVaiTro;

            db.SaveChanges();

            TempData["success"] = "Đổi vai trò thành công.";
            if (maVaiTro == 4 || maVaiTro == 3) return RedirectToAction("QuanLyNhanVien");
            return RedirectToAction("QuanLyKhachHang");
        }
        public IActionResult QuanLyNhanVien()
        {
            var list = db.TaiKhoans
                         .Include(x => x.MaVaiTroNavigation)
                         .Where(x => x.MaVaiTro == 3 || x.MaVaiTro == 4)
                         .OrderByDescending(x => x.NgayTao)
                         .ToList();

            return View(list);
        }
        public IActionResult KhoaTaiKhoan(int id)
        {
            var tk = db.TaiKhoans.FirstOrDefault(x => x.MaTk == id);

            if (tk == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            tk.TrangThai = false;

            db.SaveChanges();

            TempData["Success"] = "Đã khóa tài khoản.";

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult MoKhoaTaiKhoan(int id)
        {
            var tk = db.TaiKhoans.FirstOrDefault(x => x.MaTk == id);

            if (tk == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            tk.TrangThai = true;

            db.SaveChanges();

            TempData["Success"] = "Đã mở khóa tài khoản.";

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult DonHangKhachHang(int id)
        {
            var list = db.DonHangs
                .Include(x => x.MaPtttNavigation)
                .Include(x => x.MaTkNavigation)
                .Where(x => x.MaTk == id)
                .OrderByDescending(x => x.NgayDat)
                .ToList();

            ViewBag.KhachHang = db.TaiKhoans
                .FirstOrDefault(x => x.MaTk == id);

            return View(list);
        }
        //===================BANNER
        public IActionResult QuanLyBanner()
        {
            ViewBag.list = db.Banners.ToList();
            ViewBag.BaiViet = db.BaiViets.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ThemBanner(Banner banner, IFormFile fileAnh)
        {
            if (banner.ViTriBanner != "Banner đầu trang")
            {
                bool daTonTai = db.Banners.Any(x => x.ViTriBanner == banner.ViTriBanner);

                if (daTonTai)
                {
                    TempData["Error"] = $"Đã tồn tại {banner.ViTriBanner}, không thể thêm nữa.";
                    return RedirectToAction("QuanLyBanner");
                }
            }

            db.Banners.Add(banner);
            await db.SaveChangesAsync();

            if (fileAnh != null && fileAnh.Length > 0)
            {
                string folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "image",
                    "Banner"
                );

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string filePath = Path.Combine(folder, $"{banner.MaBanner}.png");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileAnh.CopyToAsync(stream);
                }
            }

            TempData["Success"] = "Thêm banner thành công.";

            return RedirectToAction("QuanLyBanner");
        }
        public IActionResult XoaBanner(int id)
        {
            var banner = db.Banners.Find(id);

            if (banner == null)
            {
                TempData["Error"] = "Không tìm thấy banner.";
                return RedirectToAction(nameof(QuanLyBanner));
            }

            try
            {
                string folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "image",
                    "Banner"
                );

                string[] extensions = { ".png", ".jpg", ".jpeg", ".webp" };

                foreach (var ext in extensions)
                {
                    string filePath = Path.Combine(folderPath, $"{banner.MaBanner}{ext}");

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        break;
                    }
                }

                db.Banners.Remove(banner);
                db.SaveChanges();

                TempData["Success"] = "Xóa banner thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction(nameof(QuanLyBanner));
        }  
        //======================footer
        public IActionResult QuanLyBaiViet()
        {
            var dsBaiViet = db.BaiViets
                              .OrderBy(x => x.Loai)
                              .ThenBy(x => x.TenBaiViet)
                              .ToList();
            ViewBag.list = dsBaiViet;
            return View();
        }
        [HttpPost]
        public IActionResult ThemBaiViet(BaiViet baiViet)
        {
            if (string.IsNullOrWhiteSpace(baiViet.TenBaiViet))
            {
                TempData["Error"] = "Vui lòng nhập tên bài viết.";
                return RedirectToAction(nameof(QuanLyBaiViet));
            }

            if (baiViet.Loai != 1 && baiViet.Loai != 2)
            {
                TempData["Error"] = "Loại bài viết không hợp lệ.";
                return RedirectToAction(nameof(QuanLyBaiViet));
            }

            bool tonTai = db.BaiViets.Any(x => x.LinkBaiViet == baiViet.LinkBaiViet);

            if (tonTai)
            {
                TempData["Error"] = "Đường dẫn bài viết đã tồn tại.";
                return RedirectToAction(nameof(QuanLyBaiViet));
            }

            db.BaiViets.Add(baiViet);
            db.SaveChanges();

            TempData["Success"] = "Thêm bài viết thành công.";

            return RedirectToAction(nameof(QuanLyBaiViet));
        }
        public IActionResult XoaBaiViet(int id)
        {
            var baiViet = db.BaiViets.Find(id);

            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction(nameof(QuanLyBaiViet));
            }

            // Xóa các sản phẩm thuộc bài viết
            var dsSanPham = db.BaiVietSanPhams
                .Where(x => x.MaBaiViet == id)
                .ToList();

            db.BaiVietSanPhams.RemoveRange(dsSanPham);

            // Xóa bài viết
            db.BaiViets.Remove(baiViet);

            db.SaveChanges();

            TempData["Success"] = "Xóa bài viết thành công.";

            return RedirectToAction(nameof(QuanLyBaiViet));
        }
        public IActionResult SuaBaiViet(int id)
        {
            var bv = db.BaiViets.FirstOrDefault(x => x.MaBaiViet == id);

            if (bv == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("QuanLyBaiViet");
            }

            return View(bv);
        }
        public IActionResult ThemSanPhamBaiViet(int id)
        {
            var baiViet = db.BaiViets
                .FirstOrDefault(x => x.MaBaiViet == id);

            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("QuanLyBaiViet");
            }

            ViewBag.BaiViet = baiViet;

            ViewBag.SanPham = db.SanPhams
                .OrderBy(x => x.TenSp)
                .ToList();

            ViewBag.DaThem = db.BaiVietSanPhams
                .Where(x => x.MaBaiViet == id)
                .Select(x => x.MaSp)
                .ToList();

            return View();
        }
        [HttpPost]
        public IActionResult ThemSanPhamBaiViet(int MaBaiViet, List<string>? MaSP)
        {
            var baiViet = db.BaiViets.FirstOrDefault(x => x.MaBaiViet == MaBaiViet);

            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("QuanLyBaiViet");
            }

            // Xóa toàn bộ sản phẩm cũ
            var dsCu = db.BaiVietSanPhams
                         .Where(x => x.MaBaiViet == MaBaiViet)
                         .ToList();

            db.BaiVietSanPhams.RemoveRange(dsCu);

            // Thêm lại danh sách mới
            if (MaSP != null)
            {
                foreach (var maSP in MaSP)
                {
                    db.BaiVietSanPhams.Add(new BaiVietSanPham
                    {
                        MaBaiViet = MaBaiViet,
                        MaSp = maSP
                    });
                }
            }

            db.SaveChanges();

            TempData["Success"] = "Cập nhật danh sách sản phẩm thành công.";

            return RedirectToAction("ThemSanPhamBaiViet", new { id = MaBaiViet });
        }
        [HttpPost]
        public IActionResult SuaBaiViet(BaiViet model)
        {
            var bv = db.BaiViets.FirstOrDefault(x => x.MaBaiViet == model.MaBaiViet);

            if (bv == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("QuanLyBaiViet");
            }

            bv.TenBaiViet = model.TenBaiViet;
            bv.LinkBaiViet = model.LinkBaiViet;
            bv.Loai = model.Loai;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật bài viết thành công.";

            return RedirectToAction("QuanLyBaiViet");
        }
        public IActionResult QuanLyDiaChiCuaHang()
        {
            var ds = db.DiaChiCuaHangs.ToList();

            return View(ds);
        }
        [HttpPost]
        public IActionResult ThemDiaChi(DiaChiCuaHang diaChi)
        {
            if (string.IsNullOrWhiteSpace(diaChi.Dc))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ.";
                return RedirectToAction(nameof(QuanLyDiaChiCuaHang));
            }

            db.DiaChiCuaHangs.Add(diaChi);
            db.SaveChanges();

            TempData["Success"] = "Thêm địa chỉ thành công.";

            return RedirectToAction(nameof(QuanLyDiaChiCuaHang));
        }
        public IActionResult XoaDiaChi(int id)
        {
            var diaChi = db.DiaChiCuaHangs.Find(id);

            if (diaChi == null)
            {
                TempData["Error"] = "Không tìm thấy địa chỉ.";
                return RedirectToAction(nameof(QuanLyDiaChiCuaHang));
            }

            db.DiaChiCuaHangs.Remove(diaChi);
            db.SaveChanges();

            TempData["Success"] = "Xóa địa chỉ thành công.";

            return RedirectToAction(nameof(QuanLyDiaChiCuaHang));
        }
        //========================tag
        [HttpGet]
        public IActionResult GanTag()
        {
            var dsSanPham = db.SanPhams
                .OrderBy(x => x.TenSp)
                .ToList();

            return View(dsSanPham);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GanTag(IFormCollection form)
        {
            var dsSanPham = db.SanPhams.ToList();

            foreach (var sp in dsSanPham)
            {
                var tag = Request.Form[$"Tag_{sp.MaSp}"].ToString();

                if (tag == "non")
                {
                    sp.TrangThai = null;
                }
                else if (tag == "Mới")
                {
                    sp.TrangThai = "Mới";
                }
                else if (tag == "Hàng mới về")
                {
                    sp.TrangThai = "Hàng mới về";
                }
                else if (tag == "Bán chạy")
                {
                    sp.TrangThai = "Bán chạy";
                }
                else
                {
                    // Các giá trị khác (tag giảm giá...) thì không làm gì,
                    // giữ nguyên TrangThai hiện tại.
                    continue;
                }
            }

            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật tag sản phẩm.";

            return RedirectToAction(nameof(GanTag));
        }
        //================================KHUYẾN MÃI
        public IActionResult QuanLyKhuyenMai()
        {
            CapNhatTrangThaiKhuyenMai();
            ViewBag.list = db.KhuyenMais
                .OrderByDescending(x => x.MaKm)
                .ToList();

            return View(new KhuyenMai());
        }
        [HttpPost]
        public IActionResult ThemKhuyenMai(KhuyenMai km)
        {

            if (km.CodeKm != null)
            {
                if (db.KhuyenMais.Any(x => x.CodeKm == km.CodeKm))
                {
                    TempData["Error"] = "Code khuyến mãi đã tồn tại.";
                    return RedirectToAction("QuanLyKhuyenMai");
                }
            }


            db.KhuyenMais.Add(km);
            db.SaveChanges();

            TempData["Success"] = "Đã thêm khuyến mãi.";
            return RedirectToAction("QuanLyKhuyenMai");
        }
        [HttpPost]
        public IActionResult SuaKhuyenMai(KhuyenMai km)
        {
            var old = db.KhuyenMais.Find(km.MaKm);

            if (old == null)
            {
                TempData["Error"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("QuanLyKhuyenMai");
            }

            if (db.KhuyenMais.Any(x => x.CodeKm == km.CodeKm && x.MaKm != km.MaKm))
            {
                TempData["Error"] = "Code khuyến mãi đã tồn tại.";
                return RedirectToAction("QuanLyKhuyenMai");
            }

            old.TenKm = km.TenKm;
            old.CodeKm = km.CodeKm;
            old.LoaiGiam = km.LoaiGiam;
            old.GiaTriGiam = km.GiaTriGiam;
            old.DonToiThieu = km.DonToiThieu;
            old.SoLuong = km.SoLuong;
            old.NgayBatDau = km.NgayBatDau;
            old.NgayKetThuc = km.NgayKetThuc;
            old.LoaiKm = km.LoaiKm;
            old.TrangThai = km.TrangThai;

            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật khuyến mãi.";
            return RedirectToAction("QuanLyKhuyenMai");
        }
        public IActionResult SuaKhuyenMai(int id)
        {
            var km = db.KhuyenMais.Find(id);

            if (km == null)
            {
                TempData["Error"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("QuanLyKhuyenMai");
            }

            return View(km);
        }
        public IActionResult XoaKhuyenMai(int id)
        {
            var km = db.KhuyenMais.Find(id);

            if (km == null)
            {
                TempData["Error"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("QuanLyKhuyenMai");
            }

            db.KhuyenMais.Remove(km);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa khuyến mãi.";
            return RedirectToAction("QuanLyKhuyenMai");
        }
        public IActionResult DoiTrangThaiKhuyenMai(int id)
        {
            var km = db.KhuyenMais.Find(id);

            if (km == null)
            {
                TempData["Error"] = "Không tìm thấy khuyến mãi.";
                return RedirectToAction("QuanLyKhuyenMai");
            }

            km.TrangThai = !(km.TrangThai ?? false);

            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật trạng thái.";
            return RedirectToAction("QuanLyKhuyenMai");
        }
        //=========tự động mỗi khi vào trang quản lý khuyến mãi
        private void CapNhatTrangThaiKhuyenMai()
        {
            var now = DateTime.Now;

            var ds = db.KhuyenMais.ToList();

            foreach (var km in ds)
            {
                if (km.NgayBatDau.HasValue &&
                    km.NgayKetThuc.HasValue)
                {
                    km.TrangThai = now >= km.NgayBatDau.Value
                                && now <= km.NgayKetThuc.Value;
                }
            }

            db.SaveChanges();
        }
        //===============gắn mã khuyến mãi cho sản phẩm
        public IActionResult GanSanPhamKhuyenMai(int? id)
        {
            ViewBag.listKM = db.KhuyenMais
                .Where(x => x.LoaiKm == "SanPham")
                .OrderBy(x => x.TenKm)
                .ToList();

            ViewBag.listSP = db.SanPhams.ToList();

            ViewBag.MaKm = id;

            ViewBag.DaGan = new List<string>();

            if (id != null)
            {
                ViewBag.DaGan = db.SanPhamKhuyenMais
                    .Where(x => x.MaKm == id)
                    .Select(x => x.MaSp)
                    .ToList();
            }

            return View();
        }
        [HttpPost]
        public IActionResult GanSanPhamKhuyenMai(int maKm, List<string>? listSanPham)
        {
            // Kiểm tra khuyến mãi
            var km = db.KhuyenMais.FirstOrDefault(x => x.MaKm == maKm);

            if (km == null || km.LoaiKm != "SanPham")
            {
                TempData["Error"] = "Khuyến mãi không hợp lệ.";
                return RedirectToAction("GanSanPhamKhuyenMai");
            }

            // Nếu không chọn sản phẩm nào
            listSanPham ??= new List<string>();

            // Danh sách hiện có của KM này
            var dsCu = db.SanPhamKhuyenMais
                         .Where(x => x.MaKm == maKm)
                         .ToList();

            // Chuỗi tag sale
            string chuoiSale = km.GiaTriGiam < 100
                ? $"sale {km.GiaTriGiam}%"
                : $"sale {km.GiaTriGiam}đ";

            // ====================================================
            // 1. XÓA những sản phẩm bị bỏ tích
            // ====================================================
            foreach (var item in dsCu)
            {
                if (!listSanPham.Contains(item.MaSp))
                {
                    db.SanPhamKhuyenMais.Remove(item);

                    var sp = db.SanPhams.FirstOrDefault(x => x.MaSp == item.MaSp);

                    if (sp != null && sp.TrangThai == chuoiSale)
                    {
                        sp.TrangThai = null;
                    }
                }
            }

            // ====================================================
            // 2. LUÔN XÓA LIÊN KẾT CŨ RỒI THÊM LẠI
            // ====================================================
            foreach (var maSp in listSanPham)
            {
                // Xóa tất cả liên kết cũ của sản phẩm
                var dsLienKetCu = db.SanPhamKhuyenMais
                                    .Where(x => x.MaSp == maSp)
                                    .ToList();

                if (dsLienKetCu.Any())
                {
                    db.SanPhamKhuyenMais.RemoveRange(dsLienKetCu);
                }

                // Thêm lại vào khuyến mãi hiện tại
                db.SanPhamKhuyenMais.Add(new SanPhamKhuyenMai
                {
                    MaKm = maKm,
                    MaSp = maSp
                });

                // Ghi đè tag sale
                var sp = db.SanPhams.FirstOrDefault(x => x.MaSp == maSp);

                if (sp != null)
                {
                    sp.TrangThai = chuoiSale;
                }
            }

            db.SaveChanges();

            TempData["Success"] = "Cập nhật khuyến mãi thành công.";

            return RedirectToAction("GanSanPhamKhuyenMai", new { id = maKm });
        }
        //===============bình luận
        public IActionResult QuanLyBinhLuan()
        {
            var list = db.DanhGia.Include(x => x.MaSpNavigation).Include(x => x.MaTkNavigation).ToList();
            return View(list);
        }
        public IActionResult XoaBinhLuan(int id)
        {
            try
            {
                var sp = db.DanhGia.Find(id);

                if (sp == null)
                {
                    TempData["Error"] = "Không tìm thấy đánh giá!";
                    return RedirectToAction("QuanLyBinhLuan");
                }

                db.DanhGia.Remove(sp);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa đánh giá!";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa đánh giá!";
            }

            return RedirectToAction("QuanLyBinhLuan");
        }
        //===================đổi trả
        public IActionResult QuanLyDoiTra()
        {
            var list = db.YeuCauDoiTras.Where(x => x.TrangThai == "Chờ xử lý").ToList();
            return View(list);
        }
        [HttpPost]
        public IActionResult QuanLyDoiTra(int id)
        {
            var yeuCau = db.YeuCauDoiTras.Find(id);

            if (yeuCau == null)
            {
                TempData["Error"] = "Không tìm thấy yêu cầu đổi trả!";
                return RedirectToAction("QuanLyDoiTra");
            }

            var donHang = db.DonHangs.Find(yeuCau.MaDh);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("QuanLyDoiTra");
            }

            yeuCau.TrangThai = "Đã xử lý đổi trả";
            donHang.TrangThai = "Đã xử lý đổi trả";

            db.SaveChanges();

            TempData["Success"] = "Đã xử lý đổi trả thành công.";

            return RedirectToAction("QuanLyDoiTra");
        }
        public IActionResult XyLyDoiTra(int id)
        {
            ViewBag.MaDH = id;
            ViewBag.mau = db.MauSacs.ToList();
            ViewBag.size = db.Sizes.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult NhanHangTra(int maDH, string maSP, string maMau, string maSize)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var ctsp = db.ChiTietSanPhams.FirstOrDefault(x =>
                x.MaSp == maSP &&
                x.MaMau == maMau &&
                x.MaSize == maSize);

            if (ctsp == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("XyLyDoiTra", new { id = maDH });
            }

            ctsp.SoLuongTon++;

            db.LuuVetDoiTras.Add(new LuuVetDoiTra
            {
                MaTk = maTK.Value,
                MaDh = maDH,
                MaCtsp = ctsp.MaCtsp,
                NgayThem = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã nhận hàng trả.";

            return RedirectToAction("XyLyDoiTra", new { id = maDH });
        }
        [HttpPost]
        public IActionResult GuiHangDoi(int maDH, string maSP, string maMau, string maSize)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTK");

            if (maTK == null)
                return RedirectToAction("DangNhap", "Home");

            var ctsp = db.ChiTietSanPhams.FirstOrDefault(x =>
                x.MaSp == maSP &&
                x.MaMau == maMau &&
                x.MaSize == maSize);

            if (ctsp == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("XyLyDoiTra", new { id = maDH });
            }

            if (ctsp.SoLuongTon <= 0)
            {
                TempData["Error"] = "Sản phẩm đã hết hàng.";
                return RedirectToAction("XyLyDoiTra", new { id = maDH });
            }

            ctsp.SoLuongTon--;

            db.LuuVetDoiTras.Add(new LuuVetDoiTra
            {
                MaTk = maTK.Value,
                MaDh = maDH,
                MaCtsp = ctsp.MaCtsp,
                NgayThem = DateTime.Now
            });

            db.SaveChanges();

            TempData["Success"] = "Đã gửi hàng đổi.";

            return RedirectToAction("XyLyDoiTra", new { id = maDH });
        }
        // =============Danh sách nhà cung cấp
        public IActionResult QuanLyNhaCungCap()
        {
            ViewBag.list = db.NhaCungCaps.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult ThemNhaCungCap(NhaCungCap ncc)
        {
            if (db.NhaCungCaps.Any(x => x.MaNcc == ncc.MaNcc))
            {
                TempData["Error"] = "Mã nhà cung cấp đã tồn tại!";
                return RedirectToAction("QuanLyNhaCungCap");
            }

            db.NhaCungCaps.Add(ncc);
            db.SaveChanges();

            TempData["Success"] = "Thêm nhà cung cấp thành công!";
            return RedirectToAction("QuanLyNhaCungCap");
        }
        // Hiển thị trang sửa
        public IActionResult SuaNhaCungCap(string id)
        {
            var ncc = db.NhaCungCaps.Find(id);

            if (ncc == null)
            {
                TempData["Error"] = "Không tìm thấy nhà cung cấp!";
                return RedirectToAction("QuanLyNhaCungCap");
            }

            return View(ncc);
        }

        // Lưu sửa
        [HttpPost]
        public IActionResult SuaNhaCungCap(NhaCungCap model)
        {
            var ncc = db.NhaCungCaps.Find(model.MaNcc);

            if (ncc == null)
            {
                TempData["Error"] = "Không tìm thấy nhà cung cấp!";
                return RedirectToAction("QuanLyNhaCungCap");
            }

            ncc.TenNcc = model.TenNcc;
            ncc.SoDienThoai = model.SoDienThoai;
            ncc.DiaChi = model.DiaChi;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("QuanLyNhaCungCap");
        }
        [HttpPost]
        public IActionResult XoaNhaCungCap(string id)
        {
            var ncc = db.NhaCungCaps.Find(id);

            if (ncc == null)
            {
                TempData["Error"] = "Không tìm thấy nhà cung cấp!";
                return RedirectToAction("QuanLyNhaCungCap");
            }

            if (db.PhieuNhaps.Any(x => x.MaNcc == id))
            {
                TempData["Error"] = "Nhà cung cấp đã có phiếu nhập, không thể xóa!";
                return RedirectToAction("QuanLyNhaCungCap");
            }

            db.NhaCungCaps.Remove(ncc);
            db.SaveChanges();

            TempData["Success"] = "Xóa thành công!";
            return RedirectToAction("QuanLyNhaCungCap");
        }
        // =============Danh sách phiếu nhập
        public IActionResult QuanLyNhapHang()
        {
            var list = db.PhieuNhaps.OrderBy(x => x.MaNcc).ToList();
            return View(list);
        }
        public IActionResult ChiTietPhieuNhap(int id)
        {
            var list = db.ChiTietPhieuNhaps
                .Where(x => x.MaPn == id)
                .Include(x => x.MaCtspNavigation)
                    .ThenInclude(x => x.MaSpNavigation)
                .Include(x => x.MaCtspNavigation.MaMauNavigation)
                .Include(x => x.MaCtspNavigation.MaSizeNavigation)
                .ToList();

            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file Excel.";
                return RedirectToAction("QuanLyNhapHang");
            }

            var ketQua = await ImportPhieuNhapAsync(file, db);

            if (ketQua.ThanhCong)
                TempData["Success"] = ketQua.ThongDiep;
            else
                TempData["Error"] = ketQua.ThongDiep;

            return RedirectToAction("QuanLyNhapHang");
        }
        public async Task<KetQuaImport> ImportPhieuNhapAsync(IFormFile file, TshopContext db)
        {
            // === 1. Đọc dữ liệu từ Excel ===
            var danhSach = new List<(int Dong, string MaNcc, int MaCtsp, int SoLuong, double DonGiaNhap)>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                // Bỏ 4 dòng đầu: title (1), note (2), note (3), header (4)
                var rows = worksheet.RowsUsed().Skip(4);

                foreach (var row in rows)
                {
                    var maNcc = row.Cell(1).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(maNcc))
                        continue; // bỏ qua dòng trống

                    danhSach.Add((
                        Dong: row.RowNumber(),
                        MaNcc: maNcc,
                        MaCtsp: row.Cell(2).GetValue<int>(),
                        SoLuong: row.Cell(3).GetValue<int>(),
                        DonGiaNhap: row.Cell(4).GetValue<double>()
                    ));
                }
            }

            if (!danhSach.Any())
                return new KetQuaImport { ThanhCong = false, ThongDiep = "File không có dữ liệu hợp lệ." };

            // === 2. Validate ===
            var loi = new List<string>();

            var maNccChung = danhSach.First().MaNcc;
            if (danhSach.Any(x => x.MaNcc != maNccChung))
                loi.Add("Tất cả các dòng phải có cùng MaNcc trong 1 file.");

            var nccTonTai = await db.NhaCungCaps.AnyAsync(n => n.MaNcc == maNccChung);
            if (!nccTonTai)
                loi.Add($"Mã NCC '{maNccChung}' không tồn tại.");

            foreach (var item in danhSach)
            {
                var ctspTonTai = await db.ChiTietSanPhams.AnyAsync(c => c.MaCtsp == item.MaCtsp);
                if (!ctspTonTai)
                    loi.Add($"Dòng {item.Dong}: Mã CTSP '{item.MaCtsp}' không tồn tại.");

                if (item.SoLuong <= 0)
                    loi.Add($"Dòng {item.Dong}: Số lượng phải > 0.");

                if (item.DonGiaNhap <= 0)
                    loi.Add($"Dòng {item.Dong}: Đơn giá nhập phải > 0.");
            }

            if (loi.Any())
                return new KetQuaImport { ThanhCong = false, ThongDiep = string.Join(" | ", loi) };

            // === 3. Tạo Phiếu Nhập ===
            var phieuNhap = new PhieuNhap
            {
                MaNcc = maNccChung,
                NgayNhap = DateTime.Now, // ngày import
                TongTien = danhSach.Sum(x => x.SoLuong * x.DonGiaNhap)
            };
            db.PhieuNhaps.Add(phieuNhap);
            await db.SaveChangesAsync(); // để có MaPn vừa sinh

            // === 4. Tạo Chi Tiết Phiếu Nhập ===
            foreach (var item in danhSach)
            {
                db.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                {
                    MaPn = phieuNhap.MaPn,
                    MaCtsp = item.MaCtsp,
                    SoLuong = item.SoLuong,
                    DonGiaNhap = item.DonGiaNhap
                });
            }

            // === 5. Cộng dồn tồn kho theo MaCtsp ===
            var gopTheoMaCtsp = danhSach
                .GroupBy(x => x.MaCtsp)
                .Select(g => new { MaCtsp = g.Key, TongSoLuong = g.Sum(x => x.SoLuong) });

            foreach (var g in gopTheoMaCtsp)
            {
                var ctsp = await db.ChiTietSanPhams.FirstAsync(c => c.MaCtsp == g.MaCtsp);
                ctsp.SoLuongTon += g.TongSoLuong; // ⚠️ đổi tên field cho đúng model thật của bạn
            }

            await db.SaveChangesAsync();

            return new KetQuaImport
            {
                ThanhCong = true,
                ThongDiep = $"Đã tạo Phiếu Nhập #{phieuNhap.MaPn} với {danhSach.Count} dòng chi tiết."
            };
        }
    }
}
