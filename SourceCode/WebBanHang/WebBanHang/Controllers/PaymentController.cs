using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Mymodel;
using WebBanHang.service;

namespace WebBanHang.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        TshopContext db = new TshopContext();
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                var maDh = int.Parse(response.OrderId); // vì OrderId chính là vnp_TxnRef = MaDh
                var donHang = db.DonHangs
                    .Include(x => x.ChiTietDonHangs)
                        .ThenInclude(x => x.MaCtspNavigation)
                    .FirstOrDefault(x => x.MaDh == maDh);

                if (donHang != null && donHang.TrangThaiThanhToan == "Chưa thanh toán")
                {
                    donHang.TrangThaiThanhToan = "Đã thanh toán";

                    // Trừ kho + xoá giỏ hàng (tương tự phần COD)
                    foreach (var ct in donHang.ChiTietDonHangs)
                    {
                        ct.MaCtspNavigation.SoLuongTon -= ct.SoLuong;
                    }
                    var gioHang = db.GioHangs.Where(x => x.MaTk == donHang.MaTk);
                    db.GioHangs.RemoveRange(gioHang);

                    db.SaveChanges();
                }

                TempData["Success"] = "Thanh toán VNPay thành công!";
                return RedirectToAction("GioHang", "Home"); // hoặc trang xác nhận đơn hàng
            }

            TempData["Error"] = "Thanh toán VNPay thất bại hoặc bị huỷ.";
            return RedirectToAction("GioHang", "Home");
        }
        [HttpGet]
        public IActionResult ThanhToanVnPay(int maDH)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.MaDh == maDH);
            if (donHang == null)
                return RedirectToAction("GioHang", "Home"); // hoặc trang lỗi phù hợp

            var model = new PaymentInformationModel
            {
                OrderId = donHang.MaDh.ToString(),   // <-- bắt buộc phải có dòng này
                OrderType = "other",
                Amount = donHang.TongTien,
                OrderDescription = $"Thanh toan don hang {donHang.MaDh}",
                Name = "Khach hang"
            };

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
