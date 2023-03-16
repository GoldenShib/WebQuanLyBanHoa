using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQuanLyBanHoa.Models;

namespace WebQuanLyBanHoa.Controllers
{
    [Authorize(Roles ="QuanTri,QuanLyQuyen")]
    public class PhanQuyenController : Controller
    {
        QuanLyBanHoaDataContext db = new QuanLyBanHoaDataContext();
        // GET: PhanQuyen
        public ActionResult Index()
        {
            return View(db.LoaiThanhViens.OrderBy(x=>x.TenLoai));
        }
        [HttpGet]
        public ActionResult PhanQuyen(int id)
        {
            //Lấy mã loại tv dựa vào id
            if(id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiThanhVien ltv = db.LoaiThanhViens.SingleOrDefault(x => x.MaLoaiTV == id);
            if (ltv == null)
            {
                return HttpNotFound();
            }
            //Lấy ra danh sách quyền để load check box
            ViewBag.MaQuyen = db.Quyens;
            //Lấy ra ds quyền của loại thành viên đó
            ViewBag.LoaiTVQuyen = db.LoaiThanhVien_Quyens.Where(x=>x.MaLoaiTV==id);
            return View(ltv);
        }
        [HttpPost]
        public ActionResult PhanQuyen(int MaLTV, IEnumerable<LoaiThanhVien_Quyen> lstPhanQuyen)
        {
            //Trường hợp đã phân quyền rồi nhưng muốn phân quyền lại
            var lstDaPhanQuyen = db.LoaiThanhVien_Quyens.Where(x => x.MaLoaiTV == MaLTV);
            if (lstDaPhanQuyen != null)
            {
                db.LoaiThanhVien_Quyens.DeleteAllOnSubmit(lstDaPhanQuyen);
                db.SubmitChanges();
            }
            if (lstPhanQuyen != null)
            {
                //Kiểm tra list danh sách được check
                foreach (var item in lstPhanQuyen)
                {
                    item.MaLoaiTV = int.Parse(MaLTV.ToString());
                    //Nếu được check thì insert dữ liệu vào bảng phân quyền
                    db.LoaiThanhVien_Quyens.InsertOnSubmit(item);
                }
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}