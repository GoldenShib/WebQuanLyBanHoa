using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;
using PagedList;

namespace WebQuanLyBanHoa.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TimKiem
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        [HttpGet]
        public ActionResult KQTimKiem(string sTuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int PageSize = 6;
            //Tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            //Tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(x => x.TenSP.Contains(sTuKhoa));
            ViewBag.TuKhoa = sTuKhoa;
            return View(lstSP.OrderBy(x=>x.TenSP).ToPagedList(PageNumber,PageSize));
        }
        [HttpPost]
        public ActionResult LayTuKhoaTimKiem(string sTuKhoa)
        {

            if (sTuKhoa == "")
            {
                return Redirect("/");
            }
            else
            {
                return RedirectToAction("KQTimKiem", new { @sTuKhoa = sTuKhoa });
            }
        }
        public ActionResult KQTimKiemPartial(string sTuKhoa)
        {
            //Tìm kiếm theo tên sản phẩm
            var lstSP = db.SanPhams.Where(x => x.TenSP.Contains(sTuKhoa));
            ViewBag.TuKhoa = sTuKhoa;
            return PartialView(lstSP.OrderBy(x=>x.DonGia));
        }
    }
}