using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;

namespace QLKS.Controllers.Home
{
    public class CustomerController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();

        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DangKyTaiKhoan(tblKhachHang tblKhach)
        {
            if (ModelState.IsValid)
            {
                if (db.tblKhachHangs.Where(d => d.ma_kh.Equals(tblKhach.ma_kh)).FirstOrDefault() == null 
                    && db.tblKhachHangs.Where(a => a.mail.Equals(tblKhach.mail)).FirstOrDefault() == null
                    && db.tblKhachHangs.Where(b => b.cmt.Equals(tblKhach.cmt)).FirstOrDefault() == null
                    && db.tblKhachHangs.Where(c => c.sdt.Equals(tblKhach.sdt)).FirstOrDefault() == null)
                {
                    db.tblKhachHangs.Add(tblKhach);
                    db.SaveChanges();
                    Session["KH"] = tblKhach;
                    return RedirectToAction("Index", "Home");
                }
                else if (db.tblKhachHangs.Where(d => d.ma_kh.Equals(tblKhach.ma_kh)).FirstOrDefault() != null)
                {
                    ModelState.AddModelError(nameof(tblKhachHang.ma_kh), "Tài khoản đã có người sử dụng !");
                }
                else if (db.tblKhachHangs.Where(b => b.cmt.Equals(tblKhach.cmt)).FirstOrDefault() != null)
                {
                    ModelState.AddModelError(nameof(tblKhachHang.cmt), "Số căn cước công dẫn đã tồn tại !");
                }
                else if (db.tblKhachHangs.Where(c => c.sdt.Equals(tblKhach.sdt)).FirstOrDefault() != null)
                {
                    ModelState.AddModelError(nameof(tblKhach.sdt), "Số điện thoại đã có người sử dụng !");
                }
                else if (db.tblKhachHangs.Where(a => a.mail.Equals(tblKhach.mail)).FirstOrDefault() != null)
                {
                    ModelState.AddModelError(nameof(tblKhachHang.mail), "Địa chỉ email đã có người sử dụng !");
                }
            }
            return View(tblKhach);
        }

        public ActionResult DoiMatKhau()
        {
            tblKhachHang khach = new tblKhachHang();

            if (Session["KH"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                khach = (tblKhachHang)Session["KH"];
            }

            return View(khach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau ([Bind (Include = "ma_kh,mat_khau,ho_ten,cmt,sdt,mail,diem")] tblKhachHang tblkhach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblkhach).State = EntityState.Modified;
                db.SaveChanges();
                Session["KH"] = tblkhach;
                return RedirectToAction("Index", "Home");
            }
            return View(tblkhach);
        }

        public ActionResult DangNhap()
        {
            Session["KH"] = null;

            tblKhachHang khach = (tblKhachHang)Session["KH"];

            if (khach != null)
            {
                return RedirectToAction("XacNhanDatPhong", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(tblKhachHang tblkhach)
        {
            if (ModelState.IsValid)
            {
                var khach = db.tblKhachHangs.Where(k => k.ma_kh.Equals(tblkhach.ma_kh)
                    && k.mat_khau.Equals(tblkhach.mat_khau)).FirstOrDefault();

                if (khach != null)
                {
                    Session["KH"] = khach;
                    return RedirectToAction("Index", "Home");
                }
                else if (db.tblKhachHangs.Where(a => a.ma_kh.Equals(tblkhach.ma_kh)).FirstOrDefault() == null)
                {
                    ModelState.AddModelError(nameof(tblkhach.ma_kh), "Tài khoản không hợp lệ !");
                } 
                else if (db.tblKhachHangs.Where(b => b.mat_khau.Equals(tblkhach.mat_khau)).FirstOrDefault() == null)
                {
                    ModelState.AddModelError(nameof(tblkhach.mat_khau), "Mật khẩu không hợp lệ !");
                }
            }

            return View(tblkhach);
        }

        public ActionResult DangXuat ()
        {
            Session["KH"] = null;
            return RedirectToAction("DangNhap");
        }

        public ActionResult HuyDonDatPhong(int? id)
        {
            tblKhachHang kh = new tblKhachHang();

            if (Session["KH"] != null)
            {
                kh = (tblKhachHang)Session["KH"];
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(id);

            if (pdp == null)
            {
                return HttpNotFound();
            }

            if (pdp.ma_kh != kh.ma_kh)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(pdp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyDonDatPhong (int id)
        {
            tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(id);
            pdp.ma_tinh_trang = 3;
            db.Entry(pdp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DanhSachDonDatPhong", "Home");
        }

        protected override void Dispose (bool dispose)
        {
            if (dispose)
            {
                db.Dispose();
            }
            base.Dispose(dispose);
        }

    }
}