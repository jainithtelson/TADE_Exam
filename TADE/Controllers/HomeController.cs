using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TADE.Models;

namespace TADE.Controllers
{
    public class HomeController : Controller
    {
        private TADEDBEntities db = new TADEDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Testimonial()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetTestimonials()
        {
            var testimonials = db.Testimonials.Select(x => new
            { Title = x.Title });
            return Json(testimonials);
        }
        [HttpPost]
        public JsonResult GetLatestNews()
        {
            var latestNews = db.LatestNews.Select(n => new
            { NewsId = n.NewsId, Title = n.Title, NewsScreenshotPath = n.NewsScreenshotPath, dateadded=n.DateAdded }).OrderByDescending(x=>x.dateadded);
            return Json(latestNews);
        }
    }
}