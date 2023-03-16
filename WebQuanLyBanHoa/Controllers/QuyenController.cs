using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;

namespace WebQuanLyBanHoa.Controllers
{
    public class QuyenController : Controller
    {
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        // GET: Quyen
        public ActionResult Index()
        {
            return View(db.Quyens.OrderBy(x=>x.TenQuyen));
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