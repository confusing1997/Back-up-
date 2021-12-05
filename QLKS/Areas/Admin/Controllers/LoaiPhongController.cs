using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;

namespace QLKS.Areas.Admin.Controllers
{
    public class LoaiPhongController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();

        // GET: LoaiPhong
        public ActionResult Index()
        {
            return View(db.tblLoaiPhongs.ToList());
        }

        public ActionResult ThemLoaiPhong ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemLoaiPhong (HttpPostedFileBase file, [Bind(Include = "loai_phong,mo_ta,gia,ti_le_phu_thu,anh")] tblLoaiPhong loaiphong)
        {
            if (ModelState.IsValid)
            {
                if (loaiphong.mo_ta == null || loaiphong.gia == null || loaiphong.ti_le_phu_thu == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                String anh = "/Content/Images/Phong/default.png";

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                           Server.MapPath("~/Content/Images/Phong"), pic);
                    file.SaveAs(path);
                    anh = "/Content/Images/Phong/" + pic;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                loaiphong.anh = anh;
                db.tblLoaiPhongs.Add(loaiphong);
                db.SaveChanges();

                
            }
            return RedirectToAction("Index", "LoaiPhong");
        }

        public ActionResult SuaLoaiPhong (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLoaiPhong tblLoaiPhong = db.tblLoaiPhongs.Find(id);
            if (tblLoaiPhong == null)
            {
                return HttpNotFound();
            }

            return View(tblLoaiPhong);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaLoaiPhong (HttpPostedFileBase file, [Bind(Include = "loai_phong,mo_ta,gia,ti_le_phu_thu,anh")] tblLoaiPhong tblLoaiPhong)
        {
            tblLoaiPhong loaiPhong = db.tblLoaiPhongs.Find(tblLoaiPhong.loai_phong);

            if (ModelState.IsValid)
            {
                if (tblLoaiPhong.mo_ta == null || tblLoaiPhong.gia == null || tblLoaiPhong.ti_le_phu_thu == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                String anh = loaiPhong.anh;

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    String path = System.IO.Path.Combine(
                                           Server.MapPath("~/Content/Images/Phong"), pic);
                    file.SaveAs(path);
                    anh = "/Content/Images/Phong/" + pic;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                loaiPhong.anh = anh;
                loaiPhong.mo_ta = tblLoaiPhong.mo_ta;
                loaiPhong.gia = tblLoaiPhong.gia;
                loaiPhong.ti_le_phu_thu = tblLoaiPhong.ti_le_phu_thu;
                db.Entry(loaiPhong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblLoaiPhong);
        }

        public ActionResult XoaPhong (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblLoaiPhong tblLoaiPhong = db.tblLoaiPhongs.Find(id);

            if (tblLoaiPhong == null)
            {
                return HttpNotFound();
            }
            return View(tblLoaiPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaPhong (int id)
        {
            try
            {
                tblLoaiPhong tblLoaiPhong = db.tblLoaiPhongs.Find(id);
                db.tblLoaiPhongs.Remove(tblLoaiPhong);
                db.SaveChanges();
                return RedirectToAction("Index", "LoaiPhong");
            }
            catch
            {

            }

            return RedirectToAction("Index", "LoaiPhong");
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
