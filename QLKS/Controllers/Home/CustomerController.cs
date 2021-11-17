using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                if (db.tblKhachHangs.Find(tblKhach.ma_kh) == null 
                    && db.tblKhachHangs.Where(m => m.mail.Equals(tblKhach.mail)).FirstOrDefault() == null)
                {
                    db.tblKhachHangs.Add(tblKhach);
                    db.SaveChanges();
                    Session["KH"] = tblKhach;
                    return RedirectToAction("XacNhanDatPhong", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên tài khoản hoặc email đã được sử dụng");
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
                else
                {
                    ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ !");
                }
            }

            return View(tblkhach);
        }

        public ActionResult DangXuat ()
        {
            Session["KH"] = null;
            return RedirectToAction("DangNhap");
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