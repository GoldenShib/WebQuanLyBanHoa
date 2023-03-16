using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;

namespace WebQuanLyBanHoa.Controllers
{
    [Authorize(Roles ="QuanTri")]
    public class QuanLyPhieuNhapController : Controller
    {
        // GET: QuanLyPhieuNhap
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        [HttpGet]
        public ActionResult NhapHang()
        {
            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            return View();
        }
        [HttpPost]
        public ActionResult NhapHang(PhieuNhap model, IEnumerable<ChiTietPhieuNhap> lstModel)
        {
            ViewBag.MaNCC = db.NhaCungCaps;
            ViewBag.ListSanPham = db.SanPhams;
            model.DaXoa = false;
            db.PhieuNhaps.InsertOnSubmit(model);
            db.SubmitChanges();
            SanPham sp;
            foreach(var item in lstModel)
            {
                //Cập nhật số lượng tồn
                sp = db.SanPhams.Single(x => x.MaSP == item.MaSP);
                sp.SoLuongTon += item.SoLuongNhap;
                //Gán mã phiếu nhập cho các chi tiết phiếu nhập
                item.MaPN = model.MaPN;
            }
            db.ChiTietPhieuNhaps.InsertAllOnSubmit(lstModel);
            db.SubmitChanges();
            ViewBag.ThongBao = "Nhập hàng thành công";
            return View();
        }
        [HttpGet]
        public ActionResult DSSPHetHang(int? page)
        {
            //var ls = db.SanPhams.Where(x => x.DaXoa == false && x.SoLuongTon <= 10);
            var ls = db.SanPhams.Where(x => x.SoLuongTon <= 10);
            //Thực hiện chức năng phân trang
            //Tạo biến số sản phẩm trong trang
            int PageSize = 6;
            //Tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            return View(ls.OrderBy(x => x.MaSP).ToPagedList(PageNumber, PageSize));
        }
        [HttpGet]
        public ActionResult NhapHangDon(int? id)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }
        [HttpPost]
        public ActionResult NhapHangDon(PhieuNhap model, ChiTietPhieuNhap ctpn)
        {
            model.DaXoa = false;
            model.NgayNhap = DateTime.Now;
            db.PhieuNhaps.InsertOnSubmit(model);
            db.SubmitChanges();
            ctpn.MaPN = model.MaPN;
            SanPham sp = db.SanPhams.Single(x => x.MaSP == ctpn.MaSP);
            sp.SoLuongTon += ctpn.SoLuongNhap;
            db.ChiTietPhieuNhaps.InsertOnSubmit(ctpn);
            db.SubmitChanges();
            ViewBag.ThongBao = "Nhập hàng thành công";
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC",model.MaNCC);
            return View(sp);
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