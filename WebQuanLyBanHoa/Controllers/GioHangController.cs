using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MoMo;
using System.IO;




namespace WebQuanLyBanHoa.Controllers
{
    public class GioHangController : Controller
    {

        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        //Lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)//Nếu session giỏ hàng không tồn tại
            {
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
                return lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng thông thường (load lại trang)
        public ActionResult ThemGioHang(int MaSP, string strURL)
        {
            //kiểm tra xem sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp sản phẩm đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(x => x.MaSP == MaSP);
            if (spCheck != null)
            {
                if (sp.SoLuongTon <= spCheck.SoLuong)
                {
                    return View("ThongBao");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                return Redirect(strURL);
            }

            ItemGioHang itemGH = new ItemGioHang(MaSP);
            itemGH.SoLuong++;
            itemGH.ThanhTien = itemGH.SoLuong * itemGH.DonGia;
            if (sp.SoLuongTon <= itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            lstGioHang.Add(itemGH);
            return Redirect(strURL);
        }
        //Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(x => x.SoLuong);
        }
        //Tính tổng tiền
        public decimal TinhTongTien()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(x => x.ThanhTien);
        }
        public ActionResult GioHangPartial()
        {
            if (TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }

        // GET: GioHang
        public ActionResult XemGioHang()
        {
            List<ItemGioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return View(lstGioHang);
        }

        //Chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //kiểm tra xem sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(x => x.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Lấy list giỏ hàng
            ViewBag.GioHang = lstGioHang;
            //Nếu sp tồn tại
            return View(spCheck);
        }
        //Xử lý cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            //Kiểm tra số lượng tồn
            SanPham spCheck = db.SanPhams.SingleOrDefault(n => n.MaSP == itemGH.MaSP);
            if (spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            //Cập nhật số lượng trong session giỏ hàng
            //Bước 1: Lấy List<GioHang> từ session["GioHang"]
            List<ItemGioHang> lstGH = LayGioHang();
            //Bước 2: Lấy sản phẩm cần cập nhật từ trong List<GioHang> ra
            ItemGioHang itemGHUpdate = lstGH.Find(x => x.MaSP == itemGH.MaSP);
            //Bước 3: Cập nhật lại số lượng và thành tiền
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;
            return RedirectToAction("XemGioHang");
        }
        public ActionResult XoaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //kiểm tra xem sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(x => x.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xoá sản phẩm trong giỏ hàng
            lstGioHang.Remove(spCheck);
            return RedirectToAction("XemGioHang");
        }
        //Xây dựng chức năng đặt hàng
        public ActionResult DatHang(KhachHang kh)
        {
            string b;
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang khachHang = new KhachHang();
            if (Session["TaiKhoan"] == null)
            {
                //Thêm một khách hàng mới vào csdl
                khachHang = kh;
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
            }
            else
            {
                //Đối với khách hàng là thành viên
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khachHang.TenKH = tv.HoTen;
                //khachHang.DiaChi = tv.DiaChi;
                khachHang.Email = tv.Email;
                khachHang.SoDienThoai = tv.SoDienThoai;
                //khachHang.MaThanhVien = tv.MaThanhVien;
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
            }
            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khachHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangDonHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            db.DonDatHangs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            //Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();

            foreach (var item in lstGH)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.InsertOnSubmit(ctdh);
                //Cập nhật lại số lượng tồn
                SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == item.MaSP);
                if (sp.MaSP == item.MaSP)
                {
                    sp.SoLuongTon = sp.SoLuongTon - item.SoLuong;
                    db.SubmitChanges();
                }

            }
            db.SubmitChanges();
            Session["GioHang"] = null;
            return View("ThongBaoThanhCong");
            //turn RedirectToAction("XemGioHang");
        }
        //Thêm giỏ hàng ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            //kiểm tra xem sản phẩm có tồn tại trong csdl hay không
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == MaSP);
            if (sp == null)
            {
                //Trang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp sản phẩm đã tồn tại trong giỏ hàng
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(x => x.MaSP == MaSP);
            if (spCheck != null)
            {
                if (sp.SoLuongTon <= spCheck.SoLuong)
                {
                    return Content("<script>alert(\"Sản phẩm đã hết hàng!\")</script>");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }

            ItemGioHang itemGH = new ItemGioHang(MaSP);
            if (sp.SoLuongTon <= itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            lstGioHang.Add(itemGH);
            return Redirect(strURL);
        }

        //Thanh toán MOMO

        public ActionResult Payment()
        {

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOZZ2F20220509";
            string accessKey = "Spq7pu1CuAxf8B34";
            string serectkey = "w19nG0kS9waWi3C9w6gZIqQrzqjpATKI";
            string orderInfo = "Sản phẩm sẽ được giao tới sớm nhất cho Khách hàng";
            string returnUrl = "https://localhost:44308/GioHang/DatHangMomo";
            //string notifyurl = "http://ba1adf48beba.ngrok.io/GioHang/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            string notifyurl = "https://localhost:44308/GioHang/SavePayment";

            string amount = TinhTongTien().ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);


            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i

        public ActionResult DatHangMomo(KhachHang kh)
        {
            string b;
            //Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang khachHang = new KhachHang();
            if (Session["TaiKhoan"] == null)
            {
                //Thêm một khách hàng mới vào csdl
                khachHang = kh;
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
            }
            else
            {
                //Đối với khách hàng là thành viên
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                khachHang.TenKH = tv.HoTen;
                //khachHang.DiaChi = tv.DiaChi;
                khachHang.Email = tv.Email;
                khachHang.SoDienThoai = tv.SoDienThoai;
                //khachHang.MaThanhVien = tv.MaThanhVien;
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();
            }
            //Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khachHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangDonHang = false;
            ddh.DaThanhToan = true;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            db.DonDatHangs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            //Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();

            foreach (var item in lstGH)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.InsertOnSubmit(ctdh);
                //Cập nhật lại số lượng tồn
                SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == item.MaSP);
                if (sp.MaSP == item.MaSP)
                {
                    sp.SoLuongTon = sp.SoLuongTon - item.SoLuong;
                    db.SubmitChanges();
                }

            }
            db.SubmitChanges();
            Session["GioHang"] = null;
            return View("ThongBaoThanhCong");
            //turn RedirectToAction("XemGioHang");
        }


        public ActionResult ConfirmPaymentClient()
        {
            //hiển thị thông báo cho người dùng
            Session["GioHang"] = null;
            return View("ThongBaoThanhCong");
        }

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
            //Thêm đơn hàng
            KhachHang khachHang = new KhachHang();
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = khachHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangDonHang = false;
            ddh.DaThanhToan = true;
            ddh.UuDai = 0;
            ddh.DaHuy = false;
            ddh.DaXoa = false;
            db.DonDatHangs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            //Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();

            foreach (var item in lstGH)
            {
                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia;
                db.ChiTietDonDatHangs.InsertOnSubmit(ctdh);
                //Cập nhật lại số lượng tồn
                SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == item.MaSP);
                if (sp.MaSP == item.MaSP)
                {
                    sp.SoLuongTon = sp.SoLuongTon - item.SoLuong;
                    db.SubmitChanges();
                }

            }
            db.SubmitChanges();


        }





    }
}