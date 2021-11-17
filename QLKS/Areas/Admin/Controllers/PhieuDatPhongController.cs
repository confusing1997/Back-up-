using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QLKS.Areas.Admin.Models;
using QLKS.Models;

namespace QLKS.Areas.Admin.Controllers.Admin
{
    public class PhieuDatPhongController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();

        // GET: PhieuDatPhong
        public ActionResult Index()
        {
            HuyDonDatPhongQuaHan();

            var tblPhieuDatPhongs = db.tblPhieuDatPhongs
                .Include(t => t.tblKhachHang)
                .Include(t => t.tblPhong)
                .Include(t => t.tblTinhTrangPhieuDatPhong);
            return View(tblPhieuDatPhongs.ToList());
        }
        

        private void HuyDonDatPhongQuaHan()
        {
            var datenow = DateTime.Now;
            var tblPhieuDatPhongs = db.tblPhieuDatPhongs.Where(u=>u.ma_tinh_trang == 1).Include(t => t.tblKhachHang).Include(t => t.tblPhong).Include(t => t.tblTinhTrangPhieuDatPhong).ToList();
            foreach(var item in tblPhieuDatPhongs)
            {
                System.Diagnostics.Debug.WriteLine((item.ngay_vao - datenow).Value.Days);
                if ((item.ngay_vao - datenow).Value.Days < 0)
                {
                    item.ma_tinh_trang = 3;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public ActionResult DonDatPhong ()
        {
            HuyDonDatPhongQuaHan();

            var tblPhieuDatPhongs =
                db.tblPhieuDatPhongs.Where(t => t.ma_tinh_trang == 1 &&
                t.ngay_vao.Value.Day == DateTime.Now.Day &&
                t.ngay_vao.Value.Month == DateTime.Now.Month &&
                t.ngay_vao.Value.Year == DateTime.Now.Year)
                .Include(t => t.tblKhachHang)
                .Include(t => t.tblPhong)
                .Include(t => t.tblTinhTrangPhieuDatPhong);
            return View(tblPhieuDatPhongs.ToList());
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
