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
    public class AdminController : Controller
    {
        DBMAKETMAKETEntities db = new DBMAKETMAKETEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(admin avm)
        {
            admin ad = db.admins.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (ad != null)
            {

                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Create");

            }
            else
            {
                ViewBag.error = "Sai username hoặc passwod";

            }
            return View();
        }
        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult ViewCategory(int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
           // var list = db.categories.Where(x => x.category_fk_admin == 2).OrderByDescending(x => x.cat_id).ToList();
            var list = db.categories.Where(x => x.cat_status == null).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }
        [HttpPost]
        public ActionResult Create(category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Không thể tải lên hình ảnh ....";
            }
            else
            {
                category cat = new category();
                cat.cat_username = cvm.cat_username;
                cat.cat_image = path;
                //not fix
                cat.cat_status = null;
                cat.category_fk_admin = Convert.ToInt32(Session["ad_id"].ToString());
                db.categories.Add(cat);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View();
        } //end,,,,,,,,,,,,,,,,,,,

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

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }
    }
}
