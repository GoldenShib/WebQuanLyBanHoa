using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net.Mail;

namespace WebQuanLyBanHoa.Controllers
{
    [Authorize(Roles = "QuanTri,QuanLyDonHang")]
    public class QuanLyDonHangController : Controller
    {
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        // GET: QuanLyDonHang
        public ActionResult ChuaThanhToan()
        {
            //Lấy danh sách đơn hàng chưa duyệt
            var lst = db.DonDatHangs.Where(x => x.DaThanhToan == false).OrderBy(x => x.NgayDat);
            return View(lst);
        }
        public ActionResult ChuaGiao()
        {
            //Lấy danh sách đơn hàng chưa giao
            var lst = db.DonDatHangs.Where(x => x.TinhTrangDonHang == false && x.DaThanhToan == true).OrderBy(x => x.NgayGiao);
            return View(lst);
        }
        public ActionResult DaGiaoDaThanhToan()
        {
            //Lấy danh sách đơn hàng đã giao và thanh toán
            var lst = db.DonDatHangs.Where(x => x.TinhTrangDonHang == true && x.DaThanhToan == true);
            return View(lst);
        }
        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonDatHang model = db.DonDatHangs.SingleOrDefault(x => x.MaDDH == id);

            //DonDatHang model = db.DonDatHangs.SingleOrDefault(x => x.MaKH == id);

            if (model == null)
            {
                return HttpNotFound();
            }
            //Lấy danh sách chi tiết đơn hàng
            var lstChiTiet = db.ChiTietDonDatHangs.Where(x => x.MaDDH == id);
            ViewBag.ListChiTietDH = lstChiTiet;
            return View(model);
        }
        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            //Truy vấn lấy ra dữ liệu của đơn hàng đó
            DonDatHang ddhUpdate = db.DonDatHangs.Single(x=>x.MaDDH == ddh.MaDDH);
            ddhUpdate.DaThanhToan = ddh.DaThanhToan;
            ddhUpdate.TinhTrangDonHang = ddh.TinhTrangDonHang;
            db.SubmitChanges();

            //Lấy danh sách chi tiết đơn hàng
            var lstChiTiet = db.ChiTietDonDatHangs.Where(x => x.MaDDH == ddh.MaDDH);
            ViewBag.ListChiTietDH = lstChiTiet;
            return View(ddhUpdate);
        }

        public void GuiEmail(string Title, string ToEmail, string FromEmail, string PassWord, string Content)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmail);//Địa chỉ nhận
            mail.From = new MailAddress(ToEmail);//Địa chỉ gửi
            mail.Subject = Title;//Tiêu đề gửi
            mail.Body = Content;//Nội dung
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //host gửi của Gmail
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(FromEmail, PassWord);//Tài khoản password người gửi
            smtp.EnableSsl = true; //kích hoạt giao tiếp an toàn ssl
            smtp.Send(mail);//Gửi mail đi
        }
        //Giải phóng biến cho vùng nhớ
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}