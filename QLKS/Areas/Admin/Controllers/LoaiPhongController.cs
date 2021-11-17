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
        public ActionResult ThemLoaiPhong (tblLoaiPhong loaiphong)
        {
            string fileName = Path.GetFileNameWithoutExtension(loaiphong.ImageFile.FileName);
            string extension = Path.GetExtension(loaiphong.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            loaiphong.anh = "/Content/Images/Phong/" + fileName;
            fileName = Path.Combine(Server.MapPath("/Content/Images/Phong/"), fileName);
            loaiphong.ImageFile.SaveAs(fileName);
            using (db)
            {
                db.tblLoaiPhongs.Add(loaiphong);
                db.SaveChanges();

            }
            ModelState.Clear();
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
