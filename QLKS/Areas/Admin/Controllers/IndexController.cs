using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;

namespace QLKS.Areas.Admin.Controllers
{
    public class IndexController : Controller
    {
        // GET: Admin
        dataQLKSEntities db = new dataQLKSEntities();
        public ActionResult Index()
        {
            int soPhongTrong = 0;
            int soPhongDangsd = 0;

            var tblphong = db.tblPhongs.Where(a => a.ma_tinh_trang < 5).ToList();

            foreach (var phong in tblphong) { 
                if (phong.ma_tinh_trang == 1)
                {
                    soPhongTrong += 1;
                } else
                {
                    soPhongDangsd += 1;
                }
            }

            ViewBag.soPhongTrong = soPhongTrong;
            ViewBag.soPhongDangsd = soPhongDangsd;

            return View(db.tblPhongs.ToList());
        }

        public ActionResult PhongCoTheThanhToan()
        {
            var ds = db.tblHoaDons.Where(a => a.ma_tinh_trang == 1)
                .Include(b => b.tblNhanVien)
                .Include(c => c.tblPhieuDatPhong)
                .Include(d => d.tblTinhTrangHoaDon);
            return View(ds.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblNhanVien tblnhanvien) 
        {
            if (ModelState.IsValid) 
            {
                var nv = db.tblNhanViens.Where(a => a.tai_khoan
                .Equals(tblnhanvien.tai_khoan) && a.mat_khau
                .Equals(tblnhanvien.mat_khau))
                .FirstOrDefault();

                if (nv != null)
                {
                    Session["NV"] = nv;

                    if (nv.ma_chuc_vu == 1)
                    {
                        return RedirectToAction("Index", "HoaDon");
                    }
                    return RedirectToAction("Index", "Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid information !");
                }
            }
            return View(tblnhanvien);
        }
        [HttpGet]
        public ActionResult Login() 
        {
            if (Session["NV"] != null)
                return RedirectToAction("Index", "Index");
            return View();
        }
        public ActionResult Logout() 
        {
            Session["NV"] = null;
            return RedirectToAction("Login","Index"); 
        }

        
        
        public ActionResult PhongCoThePhucVu ()
        {
            var ds = db.tblHoaDons.Where(a => a.ma_tinh_trang == 1)
                .Include(b => b.tblNhanVien)
                .Include(c => c.tblPhieuDatPhong)
                .Include(d => d.tblTinhTrangHoaDon);

            return View(ds.ToList());
        }


        public ActionResult PhongCoTheGiaHan ()
        {
            var ds = db.tblHoaDons.Where(a => a.ma_tinh_trang == 1)
                .Include(b => b.tblNhanVien)
                .Include(c => c.tblPhieuDatPhong)
                .Include(d => d.tblTinhTrangHoaDon);

            return View(ds.ToList());
        }

        public ActionResult PhongCoTheDoi()
        {
            var phong = db.tblHoaDons.Where(a => a.ma_tinh_trang == 1)
                .Include(b => b.tblNhanVien)
                .Include(c => c.tblPhieuDatPhong)
                .Include(d => d.tblTinhTrangHoaDon);

            return View(phong.ToList());
        }

        public ActionResult CaNhan() 
        {
            tblNhanVien nv = (tblNhanVien)Session["NV"];
            if (nv != null)
            {
                nv = db.tblNhanViens.Find(nv.ma_nv);
                ViewBag.ma_chuc_vu = new SelectList(db.tblChucVus, "ma_chuc_vu", "chuc_vu", nv.ma_chuc_vu); 
                return View(nv);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult CaNhan([Bind(Include = "ma_nv,ho_ten,ngay_sinh,dia_chi,sdt,tai_khoan,mat_khau,ma_chuc_vu")] tblNhanVien tblNhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblNhanVien).State = EntityState.Modified;
                db.SaveChanges();
                Session["NV"] = tblNhanVien;
                return RedirectToAction("Index");
            }
            ViewBag.ma_chuc_vu = new SelectList(db.tblChucVus, "ma_chuc_vu", "chuc_vu", tblNhanVien.ma_chuc_vu);
            return View(tblNhanVien);
        }

        public ActionResult GiaHanPhong (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblHoaDon tblhoadon = db.tblHoaDons.Find(id);

            if (tblhoadon == null)
            {
                return HttpNotFound();
            }

            tblPhieuDatPhong tblphieudatphong = db.tblPhieuDatPhongs.Find(tblhoadon.ma_pdp);

            String str = null;

            try
            {
                DateTime ngay = (DateTime)db.tblPhieuDatPhongs.Where(a => a.ma_tinh_trang == 1
                && a.ma_phong == tblphieudatphong.tblPhong.ma_phong).Select(a => a.ngay_vao).FirstOrDefault();

                str = ngay.ToString("dddd, dd MMMM yyyy");
            }
            catch
            {

            }
            return View(tblphieudatphong);
        }

        public ActionResult GianHanThanhCong (String ma_pdp, String ngay_ra)
        {
            if (ma_pdp == null || ngay_ra == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(Int32.Parse(ma_pdp));
                DateTime ngayra = (DateTime.Parse(ngay_ra).AddHours(12));
                pdp.ngay_ra = ngayra;
                db.Entry(pdp).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ketqua = "thanhcong";
            }
            catch
            {

            }

            return View();
        }

        



    }
}