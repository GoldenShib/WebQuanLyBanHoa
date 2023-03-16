using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;

namespace WebQuanLyBanHoa.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: ThongKe
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        public ActionResult Index()
        {
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();//Lấy số lượng người truy cập từ application đã được tạo
            ViewBag.SoNguoiOnline = HttpContext.Application["SoNguoiOnline"].ToString();//Lấy số lượng người đang online từ application đã được tạo
            ViewBag.TongDoanhThu = ThongKeDoanhThu();//Thống kê tổng doanh thu
            ViewBag.TongDonHang = ThongKeDonHang();//Thống kê tổng đơn đặt hàng
            ViewBag.TongThanhVien = ThongKeThanhVien();//Thống kê tổng thành viên
            return View();
        }
        public double ThongKeDonHang()
        {
            double slDDH = db.DonDatHangs.Count();
            return slDDH;
        }
        public double ThongKeThanhVien()
        {
            double slTV = db.ThanhViens.Count();
            return slTV;
        }
        public decimal ThongKeDoanhThu()
        {
            //Thống kê tất cả doanh thu từ khi trang web thành lập
            decimal DoanhThu = db.ChiTietDonDatHangs.
                Sum(x => x.SoLuong * x.DonGia).Value;
            return DoanhThu;
        }
        public decimal ThongKeDoanhThuThang(int Thang, int Nam)
        {
            //Liệt kê những đơn hàng có tháng năm tương ứng
            var lstDDH = db.DonDatHangs.
                Where(x => x.NgayDat.Value.Month == Thang && x.NgayDat.Value.Year == Nam);
            decimal TongTien = 0;
            foreach(var item in lstDDH)
            {
                TongTien += decimal.Parse(item.ChiTietDonDatHangs.
                    Sum(x => x.SoLuong * x.DonGia).Value.ToString());
            }
            return TongTien;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}