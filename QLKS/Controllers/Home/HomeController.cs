using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLKS.Models;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace QLKS.Controllers
{
    public class HomeController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();
        public ActionResult Index()    
        {
            return View();
        }

        public ActionResult About() 
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult TimPhongTrong ()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult TimPhongTrong (String datestart, String dateend)
        {
            Session["ngayVao"] = datestart;
            Session["ngayRa"] = dateend;
            Session["ds_phong"] = null;

            datestart = DateTime.ParseExact(datestart, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
            dateend = DateTime.ParseExact(dateend, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");

            DateTime ngayVao = (DateTime.Parse(datestart)).AddHours(12);
            DateTime ngayRa = (DateTime.Parse(dateend)).AddHours(12);


            var li =
                db.tblPhongs.Where(a => !(db.tblPhieuDatPhongs.Where(b => (b.ma_tinh_trang == 1 || b.ma_tinh_trang == 2)
                && b.ngay_ra > ngayVao && b.ngay_vao < ngayRa))
                .Select(b => b.ma_phong).ToList().Contains(a.ma_phong)).ToList();
            
            return View(li);
        }

        public ActionResult GioiThieuDichVu ()
        {
            return View(db.tblPhongs.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult XacNhanDatPhong()
        {
            if (Session["KH"] == null)
            {
                return RedirectToAction("DangNhap", "Customer");
            }
            HuyDonDatPhongQuaHan();
            tblKhachHang kh = (tblKhachHang)Session["KH"];
            ViewBag.ma_kh = kh.ma_kh;
            ViewBag.ten_kh = kh.ho_ten;
            ViewBag.ngay_dat = DateTime.Now;
            ViewBag.ngay_vao = (String)Session["ngayVao"];
            ViewBag.ngay_ra = (String)Session["ngayRa"];

            String sp = "";

            List<int> ds;
            ds = (List<int>)Session["ds_phong"];
            if (ds == null)
                ds = new List<int>();
            ViewBag.ma_phong = JsonConvert.SerializeObject(ds);
            foreach (var item in ds)
            {
                tblPhong p = (tblPhong)db.tblPhongs.Find(Int32.Parse(item.ToString()));
                sp += p.so_phong.ToString() + " - ";
            }
            ViewBag.so_phong = sp;
            var liP = db.tblPhieuDatPhongs.Where(u => u.ma_kh == kh.ma_kh && u.ma_tinh_trang == 1).ToList();
            return View(liP);

        }

        public ActionResult ChonPhong(string id) 
        {
            try
            {
                List<int> ds;
                ds = (List<int>)Session["ds_phong"];
                if (ds == null)
                    ds = new List<int>();
                ds.Add(Int32.Parse(id));
                Session["ds_phong"] = ds;
                ViewBag.result = "success";
            }
            catch
            {
                ViewBag.result = "error";
            }
            return View();
        }

        public ActionResult HuyChon(string id) 
        {
            try
            {
                List<int> ds;
                ds = (List<int>)Session["ds_phong"];
                if (ds == null)
                    ds = new List<int>();
                ds.Remove(Int32.Parse(id));
                Session["ds_phong"] = ds;
                ViewBag.result = "success";
            }
            catch
            {
                ViewBag.result = "error";
            }
            return View();
        } 

        public void HuyDonDatPhongQuaHan ()
        {
            var now = DateTime.Now;
            var tblPhieuDatPhong = db.tblPhieuDatPhongs.Where(p => p.ma_tinh_trang == 1)
                .Include(o => o.tblKhachHang)
                .Include(q => q.tblPhong)
                .Include(a => a.tblTinhTrangPhieuDatPhong).ToList();

            foreach (var item in tblPhieuDatPhong)
            {
                if ((item.ngay_vao - now).Value.Days < 0)
                {
                    item.ma_tinh_trang = 3;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public ActionResult KetQuaDatPhong (String ma_kh, String ngay_vao, String ngay_ra, String ma_phong)
        {
            if (ma_kh == null || ngay_vao == null || ngay_ra == null || ma_phong == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                tblPhieuDatPhong pdp = new tblPhieuDatPhong();
                List<int> ds = JsonConvert.DeserializeObject<List<int>>(ma_phong);
                pdp.ma_kh = ma_kh;
                pdp.ma_tinh_trang = 1;
                pdp.ngay_dat = DateTime.Now;
                pdp.ngay_vao    = (DateTime.ParseExact(ngay_vao, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(12));
                pdp.ngay_ra     = (DateTime.ParseExact(ngay_ra, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(12));

                try
                {
                    for (int i = 0; i < ds.Count; i++)
                    {
                        pdp.ma_phong = ds[i];
                        db.tblPhieuDatPhongs.Add(pdp);
                        db.SaveChanges();
                        ViewBag.Result = "success";
                    }
                    setNull();
                }
                catch
                {
                    ViewBag.Result = "error";
                }
            }
            return View();
        }


        public ActionResult HuyDondatPhong() 
        {
            setNull();
            return RedirectToAction("Index", "Home");
        }
        private void setNull()
        {
            Session["ngayVao"] = null;
            Session["ngayRa"] = null;
            Session["ma_phong"] = null;
            Session["ds_phong"] = null;
        }
        


    }
}