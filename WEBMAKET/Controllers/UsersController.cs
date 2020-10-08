using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBMAKET.Models;

namespace WEBMAKET.Controllers
{
    public class UsersController : Controller
    {
        DBMAKETMAKETEntities db = new DBMAKETMAKETEntities();
        // GET: Users
        public ActionResult Index(int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
        //    var list = db.categories.Where(x => x.category_fk_admin == 2).OrderByDescending(x => x.cat_id).ToList();
            var list = db.categories.Where(x => x.cat_status == null).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Ảnh không thể đẩy vào...";
            }
            else
            {
                user u = new user();
                u.u_username = uvm.u_username;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.users.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");

            }

            return View();
        }

        public ActionResult login()
        {
            return View();
        }

        // post user
        [HttpPost]
        public ActionResult login(user avm)
        {
            user ad = db.users.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {

                Session["u_id"] = ad.u_id.ToString();
                //  return RedirectToAction("CreateAd");
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Sai username hoặc password";

            }

            return View();
        }
        // upload iamge
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                            ViewBag.Message = "Đẩy lên thành công";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Chỉ các định dạng jpg, jpeg hoặc png mới được chấp nhận ....'); </script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Vui lòng chọn một tệp'); </script>");
                path = "-1";
            }
            return path;
        }
        [HttpGet]
        public ActionResult CreateAd()
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_username");

            return View();
        }
        [HttpPost]
        public ActionResult CreateAd(product pvm, HttpPostedFileBase imgfile)
        {
            List<category> li = db.categories.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_username");


            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                product p = new product();
                p.pro_username = pvm.pro_username;
                p.pro_price = pvm.pro_price;
                p.pro_image = path;
                p.product_fk_cattegory = pvm.product_fk_cattegory;
                p.pro_desc = pvm.pro_desc;
                p.product_fk_users = Convert.ToInt32(Session["u_id"].ToString());
                db.products.Add(p);
                db.SaveChanges();
                Response.Redirect("Index");

            }

            return View();
        }
        //ads
        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.products.Where(x => x.product_fk_cattegory == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }
        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.products.Where(x => x.pro_username.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }


        public ActionResult ViewAd(int? id)
        {
              Adviewmodel ad = new Adviewmodel();
            product p = db.products.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_username;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_desc;
            category cat = db.categories.Where(x => x.cat_id == p.product_fk_cattegory).SingleOrDefault();
            ad.cat_username = cat.cat_username;
            user u = db.users.Where(x => x.u_id == p.product_fk_users).SingleOrDefault();
            ad.u_name = u.u_username;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;




            return View(ad);
        }
        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }
        public ActionResult DeleteAd(int? id)
        {

            product p = db.products.Where(x => x.pro_id == id).SingleOrDefault();
            db.products.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }




}