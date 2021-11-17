using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;

namespace QLKS.Areas.Admin.Controllers.Admin
{
    public class PhongController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();

        // GET: Phong
        public ActionResult Index()
        {
            var tblPhongs = 
                db.tblPhongs.Where(t=>t.ma_tinh_trang<5).
                Include(t => t.tblLoaiPhong).
                Include(t => t.tblTang).
                Include(t => t.tblTinhTrangPhong);
            return View(tblPhongs.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
