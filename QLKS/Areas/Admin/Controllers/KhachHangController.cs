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
        public ActionResult Index(string searching)
        {
            return View(db.tblKhachHangs.Where(a => searching == null || a.ho_ten.Contains(searching)).ToList());
        }

        public ActionResult ThemKhachHang ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemKhachHang (tblKhachHang tblkhachhang)
        {
            if (ModelState.IsValid)
            {
                if (db.tblKhachHangs.Find(tblkhachhang.ma_kh) == null && 
                    db.tblKhachHangs.Where(x => x.mail == tblkhachhang.mail).FirstOrDefault() == null)
                {
                    db.tblKhachHangs.Add(tblkhachhang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "KhachHang");
                }
            }
            
            
            return View(tblkhachhang);
        }





        
        
    }
}
