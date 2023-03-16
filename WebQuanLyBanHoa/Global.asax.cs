using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WebQuanLyBanHoa
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["SoNguoiTruyCap"] = 0;
            Application["SoNguoiOnline"] = 0;
        }
        protected void Session_Start()
        {
            Application.Lock();//Dùng để đồng bộ hoá
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["SoNguoiOnline"] = (int)Application["SoNguoiOnline"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();
            Application["SoNguoiOnline"] = (int)Application["SoNguoiOnline"] - 1;
            Application.UnLock();
        }
        //Dang ky Session
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var TaiKhoanCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (TaiKhoanCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(TaiKhoanCookie.Value);
                var Quyen = authTicket.UserData.Split(new Char[] { ',' });
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), Quyen);
                Context.User = userPrincipal;
            }
        }
    }
}
