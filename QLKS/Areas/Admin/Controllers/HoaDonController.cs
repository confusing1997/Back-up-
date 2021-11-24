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
    public class HoaDonController : Controller
    {
        private dataQLKSEntities db = new dataQLKSEntities();

        // GET: HoaDon
        public ActionResult Index() 
        {
            var tblhd = 
                db.tblHoaDons.Where(t=>t.ma_tinh_trang==2).
                Include(t => t.tblNhanVien).
                Include(t => t.tblPhieuDatPhong).
                Include(t => t.tblTinhTrangHoaDon).ToList();

            double tongTien = 0;

            foreach (var hd in tblhd)
            {
                if (hd.ma_tinh_trang == 2)
                {
                    tongTien += (double)hd.tong_tien;
                }
            }

            ViewBag.tongAllHoaDon = tongTien.ToString("C3");
            /*Count*/

            return View(tblhd);
        }

        [HttpPost]
        public ActionResult Index (String ngay_vao, String ngay_ket_thuc)
        {
            List<tblHoaDon> dshd = new List<tblHoaDon>();

            String query = "select * from tblHoaDon where ma_tinh_trang = 2 ";

            if (!ngay_vao.Equals(""))
            {
                query += " and ngay_tra_phong >= '" + ngay_vao + "'";
            }

            if (!ngay_ket_thuc.Equals(""))
            {
                query += " and ngay_tra_phong <= '" + ngay_ket_thuc + "'";
            }

            dshd = db.tblHoaDons.SqlQuery(query).ToList();

            double tongHD = 0;

            foreach (var item in dshd)
            {
                if (item.ma_tinh_trang == 2)
                {
                    tongHD += (double)item.tong_tien;
                }
            }
            ViewBag.tongAllHoaDon = tongHD.ToString("C3");

            return View(dshd);
        }


        public ActionResult XacNhanDonDatPhong (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblPhieuDatPhong tblPhieu = db.tblPhieuDatPhongs.Find(id);

            if (tblPhieu == null)
            {
                return HttpNotFound();
            }

            return View(tblPhieu);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult KetQuaDatPhong (String ma_pdp)
        {
            if (ma_pdp == null)
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                tblHoaDon hd = new tblHoaDon();
                hd.ma_pdp = Int32.Parse(ma_pdp);
                hd.ma_tinh_trang = 1;
                try
                {
                    db.tblHoaDons.Add(hd);
                    tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(Int32.Parse(ma_pdp));
                    if (pdp == null)
                    {
                        return HttpNotFound();
                    }
                    tblPhong p = db.tblPhongs.Find(pdp.ma_phong);
                    if (p == null)
                    {
                        return HttpNotFound();
                    }
                    pdp.ma_tinh_trang = 2;
                    db.Entry(pdp).State = EntityState.Modified;
                    p.ma_tinh_trang = 2;
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Result = "success";
                }
                catch
                {
                    ViewBag.Result = "error";
                }
            }
            return View();
        }

        public ActionResult ThanhToan(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHoaDon tblHoaDon = db.tblHoaDons.Find(id);

            if (tblHoaDon == null)
            {
                return HttpNotFound();
            }

            DateTime ngay_ra = DateTime.Now;
            DateTime ngay_vao = (DateTime)tblHoaDon.tblPhieuDatPhong.ngay_vao;
            DateTime ngay_du_kien = (DateTime)tblHoaDon.tblPhieuDatPhong.ngay_ra;

            DateTime dateS = new DateTime(ngay_vao.Year, ngay_vao.Month, ngay_vao.Day, 12, 0, 0);
            DateTime dateE = new DateTime(ngay_ra.Year, ngay_ra.Month, ngay_ra.Day, 12, 0, 0);

            Double gia = (Double)tblHoaDon.tblPhieuDatPhong.tblPhong.tblLoaiPhong.gia;

            var songay = (dateE - dateS).TotalDays;

            if (dateS > ngay_vao)
                songay++;
            if (ngay_ra > dateE)
                songay++;

            var ti_le_phu_thu = tblHoaDon.tblPhieuDatPhong.tblPhong.tblLoaiPhong.ti_le_phu_thu;
            var so_ngay_phu_thu = Math.Abs(Math.Ceiling((ngay_ra - ngay_du_kien).TotalDays));
            
            var phuthu = so_ngay_phu_thu * gia * ti_le_phu_thu / 100;
            ViewBag.phu_thu = phuthu;

            ViewBag.so_ngay_phu_thu = so_ngay_phu_thu;
            var tien_phong = songay * gia;
            ViewBag.tien_phong = tien_phong;
            ViewBag.so_ngay = songay;

            tblNhanVien nv = (tblNhanVien)Session["NV"];
            if (nv != null)
            {
                ViewBag.ho_ten = nv.ho_ten;
            }

            List<tblDichVuDaDat> dsdv = db.tblDichVuDaDats.Where(u => u.ma_hd == id).ToList();
            ViewBag.list_dv = dsdv;

            double tongtiendv = 0;

            List<double> tt = new List<double>();
            foreach (var item in dsdv)
            {
                double t = (double)(item.so_luong * item.tblDichVu.gia);
                tongtiendv += t;
                tt.Add(t);
            }
            ViewBag.list_tt = tt;
            ViewBag.tien_dich_vu = tongtiendv;
            ViewBag.tong_tien = tien_phong + tongtiendv + phuthu;

            return View(tblHoaDon);
        }
        public ActionResult GoiDichVu(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GoiDichVuModel mod = new GoiDichVuModel();
            mod.dsDichVu = db.tblDichVus.Where(t=>t.ton_kho > 0).ToList();
            mod.dsDvDaDat = db.tblDichVuDaDats.Where(u => u.ma_hd == id).ToList();
            ViewBag.ma_hd = id;
            ViewBag.so_phong = db.tblHoaDons.Find(id).tblPhieuDatPhong.tblPhong.so_phong;
            return View(mod);
        }

        public ActionResult XacNhanGoiDichVu(String ma_hd, String ma_dv, String so_luong)
        {
            if (ma_hd == null || ma_dv == null || so_luong == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int mahd = Int32.Parse(ma_hd);
            int madv = Int32.Parse(ma_dv);
            int sol = Int32.Parse(so_luong);

            var ds = db.tblDichVuDaDats.Where(t => t.ma_hd == mahd).ToList();

            try
            {
                bool check = false;
                foreach (var item in ds)
                {
                    if (item.ma_dv == madv)
                    {
                        item.so_luong += sol;
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    tblDichVuDaDat dv = new tblDichVuDaDat();
                    dv.ma_hd = Int32.Parse(ma_hd);
                    dv.ma_dv = Int32.Parse(ma_dv);
                    dv.so_luong = Int32.Parse(so_luong);
                    db.tblDichVuDaDats.Add(dv);
                }
                tblDichVu dichvu = db.tblDichVus.Find(madv);
                dichvu.ton_kho -= sol;
                db.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("GoiDichVu", "HoaDon", new { id = ma_hd });
        }



        public ActionResult XacNhanThanhToan(String ma_hd, String tien_phong, String tien_dich_vu, String phu_thu, String tong_tien) 
        {
            if (ma_hd == null || tien_phong == null || phu_thu == null || tong_tien == null || tien_dich_vu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                tblHoaDon hd = db.tblHoaDons.Find(Int32.Parse(ma_hd));

                tblNhanVien nv = (tblNhanVien)Session["NV"];
                if (nv != null)
                {
                    hd.ma_nv = nv.ma_nv;
                }

                hd.tien_phong = Double.Parse(tien_phong);
                hd.tien_dich_vu = Double.Parse(tien_dich_vu);
                hd.phu_thu = Double.Parse(phu_thu);
                hd.tong_tien = Double.Parse(tong_tien);
                hd.ma_tinh_trang = 2;
                hd.ngay_tra_phong = DateTime.Now;
                db.Entry(hd).State = EntityState.Modified;

                tblPhong p = db.tblPhongs.Find(hd.tblPhieuDatPhong.ma_phong);
                p.ma_tinh_trang = 1;
                tblPhieuDatPhong pd = db.tblPhieuDatPhongs.Find(hd.tblPhieuDatPhong.ma_pdp);
                pd.ma_tinh_trang = 4;
                db.Entry(p).State = EntityState.Modified;
                db.Entry(pd).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.result = "success";
            }
            catch
            {
                ViewBag.result = "error";
            }
            ViewBag.ma_hd = ma_hd;
            return View();
        }


        public ActionResult DoiPhong (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblHoaDon hd = db.tblHoaDons.Find(id);

            if (hd == null)
            {
                return HttpNotFound();
            }

            tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(hd.ma_pdp);

            var ds = db.tblPhongs.Where(a => a.ma_tinh_trang == 1 && 
                    !(db.tblPhieuDatPhongs.Where(b => (b.ma_tinh_trang == 1 || b.ma_tinh_trang == 2)
                    && b.ngay_ra > DateTime.Now && b.ngay_vao < pdp.ngay_ra))
                    .Select(b => b.ma_phong).ToList().Contains(a.ma_phong));

            ViewBag.ma_phong_moi = new SelectList(ds, "ma_phong", "so_phong");

            return View(pdp);
        }

        public ActionResult DoiPhongThanhCong (String ma_pdp, String ma_phong_cu, String ma_phong_moi)
        {
            if (ma_pdp == null || ma_phong_cu == null || ma_phong_moi == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tblPhieuDatPhong pdp = db.tblPhieuDatPhongs.Find(Int32.Parse(ma_pdp));
                tblPhong phong = db.tblPhongs.Find(pdp.tblPhong.ma_phong);
                phong.ma_tinh_trang = 1;
                db.Entry(phong).State = EntityState.Modified;

                pdp.ma_phong = Int32.Parse(ma_phong_moi);
                phong = db.tblPhongs.Find(Int32.Parse(ma_phong_moi));
                phong.ma_tinh_trang = 2;
                db.Entry(phong).State = EntityState.Modified;
                db.Entry(pdp).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.result = "thanhcong";
            }

            catch (Exception e){

                ViewBag.result = "error" + e;

            }
            
            return View();
        }

        public ActionResult ChiTietHoaDon (int? id)
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

            var tien_phong = (tblhoadon.tblPhieuDatPhong.ngay_ra - tblhoadon.tblPhieuDatPhong.ngay_vao).Value.TotalDays
                                * tblhoadon.tblPhieuDatPhong.tblPhong.tblLoaiPhong.gia;

            ViewBag.tien_phong = tien_phong;
            ViewBag.time_now = DateTime.Now.ToString();

            List<tblDichVuDaDat> dvdd = db.tblDichVuDaDats.Where(a => a.ma_hd == id).ToList();
            ViewBag.list_dv = dvdd;

            double tong_tien_dv = 0;

            List<double> tong_tien = new List<double>();

            foreach (var item in dvdd)
            {
                double tien = (double)(item.so_luong * item.tblDichVu.gia);
                tong_tien_dv += tien;
                tong_tien.Add(tien);
            }

            ViewBag.list_tong_tien_dv = tong_tien;
            ViewBag.tien_dich_vu = tong_tien_dv;
            ViewBag.tong_hoa_don = tien_phong + tong_tien_dv;
            
            return View(tblhoadon);
        }

        

        
    }
}
