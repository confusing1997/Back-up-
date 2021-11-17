using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;

namespace QLKS.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();
        // GET: KhachHang
        public ActionResult Index()
        {
            return View(db.tblKhachHangs.ToList());
        }

        
        
    }
}
