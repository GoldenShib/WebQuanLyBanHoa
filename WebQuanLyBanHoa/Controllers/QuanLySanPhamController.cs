using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;

namespace WebQuanLyBanHoa.Controllers
{
    [Authorize(Roles = "QuanTri,QuanLySanPham")]
    public class QuanLySanPhamController : Controller
    {
        // GET: QuanLySanPham
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        public ActionResult Index(int? page)
        {
            //var ls = db.SanPhams.Where(x => x.DaXoa == false);
            var ls = db.SanPhams;
            //Thực hiện chức năng phân trang
            //Tạo biến số sản phẩm trong trang
            int PageSize = 6;
            //Tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            return View(ls.OrderBy(x => x.MaSP).ToPagedList(PageNumber, PageSize));
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            //Load dropdown list nhà cung cấp
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(x => x.TenNCC), "MaNCC", "TenNCC");

            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TaoMoi(SanPham sp, HttpPostedFileBase HinhAnh)
        {
            //Load dropdown list nhà cung cấp
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(x => x.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            //Kiểm tra ảnh tồn tại trong csdl
            if (ModelState.IsValid)
            {
                if (HinhAnh.ContentLength > 0)
                {
                    //Lấy tên hình ảnh
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    //Lấy hình ảnh chuyển vào thư mục hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh"), fileName);
                    //if (System.IO.File.Exists(path))
                    //{
                    //    ViewBag.Upload = "Hình ảnh đã tồn tại";
                    //    return View();
                    //}
                    //else
                    //{
                        HinhAnh.SaveAs(path);
                        Session["tenhinh"] = HinhAnh.FileName;
                        ViewBag.tenhinh = "";
                        sp.HinhAnh = fileName;
                    //}
                }
                db.SanPhams.InsertOnSubmit(sp);
                db.SubmitChanges();
                ViewBag.ThongBao = "Thêm sản phẩm thành công";
            }
            else
            {
                ViewBag.ThongBao = "Thêm sản phẩm thất bại";
                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
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

            //Load dropdown list nhà cung cấp
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(x => x.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);

            return View(sp);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChinhSua(int? id, HttpPostedFileBase HinhAnh)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == id);
            //Load dropdown list nhà cung cấp
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(x => x.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            if (ModelState.IsValid)
            {
                if (HinhAnh.ContentLength > 0)
                {
                    //Lấy tên hình ảnh
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    //Lấy hình ảnh chuyển vào thư mục hình ảnh
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh"), fileName);
                    //if (System.IO.File.Exists(path))
                    //{
                    //    ViewBag.Upload = "Hình ảnh đã tồn tại";
                    //    return View(sp);
                    //}
                    //else
                    //{
                        HinhAnh.SaveAs(path);
                        Session["tenhinh"] = HinhAnh.FileName;
                        ViewBag.tenhinh = "";
                        sp.HinhAnh = fileName;
                    //}
                }
                sp.TenSP = Request.Form["TenSP"];
                sp.NgayCapNhat = DateTime.Parse(Request.Form["NgayCapNhat"]);
                sp.MoTa = Request.Form["MoTa"];
                sp.MaNCC = int.Parse(Request.Form["MaNCC"]);
                sp.Moi = int.Parse(Request.Form["Moi"]);
                sp.SoLuongTon = int.Parse(Request.Form["SoLuongTon"]);
                sp.DonGia = decimal.Parse(Request.Form["DonGia"]);
                sp.DaXoa = bool.Parse(Request.Form["DaXoa"]);
                db.SubmitChanges();
                ViewBag.ThongBao = "Cập nhật thành công";
            }
            else
            {
                ViewBag.ThongBao = "Cập nhật thất bại";
                return View(sp);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
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

            //Load dropdown list nhà cung cấp
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(x => x.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);

            return View(sp);
        }
        [HttpPost]
        public ActionResult Xoa(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            SanPham sp = db.SanPhams.SingleOrDefault(x => x.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            db.SanPhams.DeleteOnSubmit(sp);
            db.SubmitChanges();
            return RedirectToAction("Index");
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