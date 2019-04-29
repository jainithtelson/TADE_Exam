using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TADE.Models;

namespace TADE.Controllers
{
    public class LatestNewsController : Controller
    {
        private TADEDBEntities db = new TADEDBEntities();

        // GET: LatestNews
        public ActionResult Index()
        {
            return View();
            //var latestNews = db.LatestNews.Include(l => l.AspNetUser);
            //return View(latestNews.ToList());
        }
        [HttpPost]
        public JsonResult GetAllLatestNews()
        {
            var latestNews = db.LatestNews.Select(l=> new { Title = l.Title, SourceLink = l.SourceLink, DateAdded = l.DateAdded});
            return Json(latestNews);
        }
        // GET: LatestNews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //  LatestNew latestNew = db.LatestNews.Find(id);

            //if (latestNew == null)
            //{
            //    return HttpNotFound();
            //}
            ViewBag.NewsId = id;
            return View();
        }
        [HttpPost]
        public JsonResult GetLatestNewsDetail(int NewsId)
        {
            var latestNews = db.LatestNews.Where(a => a.NewsId == NewsId).Select(n => new
            { NewsId = n.NewsId, Title = n.Title, SourceLink = n.SourceLink, dateadded = n.DateAdded }).FirstOrDefault();
            // LatestNew latestNews = db.LatestNews.Find(NewsId);
            return Json(latestNews);
        }
        // GET: LatestNews/Create
        public ActionResult Create()
        {
            
            ViewBag.checkCandidateLogin = true;
            string userId = Convert.ToString(Session["Administrator"]);
            if (userId == "")
            {
                ViewBag.checkCandidateLogin = false;
            }
           // ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: LatestNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,NewsScreenshotPath,SourceLink")] LatestNew latestNew)
        {
            if (ModelState.IsValid)
            {
                latestNew.UserId = Convert.ToString(Session["Administrator"]);
                
                latestNew.DateAdded = DateTime.Now;
                db.LatestNews.Add(latestNew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", latestNew.UserId);
            return View(latestNew);
        }

        // GET: LatestNews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LatestNew latestNew = db.LatestNews.Find(id);
            if (latestNew == null)
            {
                return HttpNotFound();
            }
            ViewBag.checkCandidateLogin = true;
            string UserId = Convert.ToString(Session["Administrator"]);
            if (UserId == "")
            {
                ViewBag.checkCandidateLogin = false;
            }
            return View(latestNew);
        }

        // POST: LatestNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsId,UserId,Title,NewsScreenshotPath,SourceLink,DateAdded")] LatestNew latestNew)
        {
            if (ModelState.IsValid)
            {
                latestNew.UserId = Convert.ToString(Session["Administrator"]);
                db.Entry(latestNew).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", latestNew.UserId);
            return View(latestNew);
        }

        // GET: LatestNews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LatestNew latestNew = db.LatestNews.Find(id);
            if (latestNew == null)
            {
                return HttpNotFound();
            }
            return View(latestNew);
        }

        // POST: LatestNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LatestNew latestNew = db.LatestNews.Find(id);
            db.LatestNews.Remove(latestNew);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult OpenWordDocumentLatestNews(string NewsScreenshotPath)
        {
            string fullNewsScreenshotPath = "http://docs.google.com/gview?url=http://tadeexam.co.uk/LatestNews/" + NewsScreenshotPath + ".doc&embedded=true";
            ViewBag.NewsScreenshotPath = fullNewsScreenshotPath;
            return View();
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
