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
                db.tblPhongs.Where(t=>t.ma_tinh_trang < 5).
                Include(t => t.tblLoaiPhong).
                Include(t => t.tblTang).
                Include(t => t.tblTinhTrangPhong);
            return View(tblPhongs.ToList());
        }

        public ActionResult ThemPhong ()
        {
            ViewBag.loai_phong = new SelectList(db.tblLoaiPhongs, "loai_phong", "mo_ta");
            ViewBag.ma_tang = new SelectList(db.tblTangs, "ma_tang", "ten_tang");
            ViewBag.ma_tinh_trang = new SelectList(db.tblTinhTrangPhongs, "ma_tinh_trang", "mo_ta");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemPhong (tblPhong tblphong)
        {
            if (ModelState.IsValid)
            {
                db.tblPhongs.Add(tblphong);
                db.SaveChanges();

                return RedirectToAction("Index", "Phong");
            }
            
            return View(tblphong);
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
