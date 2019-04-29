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
    public class TestimonialsController : Controller
    {
        private TADEDBEntities db = new TADEDBEntities();

        // GET: Testimonials
        public ActionResult Index()
        {
            var testimonials = db.Testimonials.Include(t => t.CandidateDetail);
            return View(testimonials.ToList());
        }

        // GET: Testimonials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        // GET: Testimonials/Create
        public ActionResult Create()
        {
            ViewBag.checkCandidateLogin = true;
            int candidateId = Convert.ToInt32(Session["CandidateId"]);
            if (candidateId == 0)
            {
                ViewBag.checkCandidateLogin = false;
            }
            //  ViewBag.CandidateId = new SelectList(db.CandidateDetails, "CandidateId", "Email");
            return View();
        }

        // POST: Testimonials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestimonialId,CandidateId,Name,Title,Description,AddedDate")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                testimonial.CandidateId = Convert.ToInt32(Session["CandidateId"]);
                testimonial.AddedDate = DateTime.Now;
                db.Testimonials.Add(testimonial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CandidateId = new SelectList(db.CandidateDetails, "CandidateId", "Email", testimonial.CandidateId);
            return View(testimonial);
        }

        // GET: Testimonials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            ViewBag.CandidateId = new SelectList(db.CandidateDetails, "CandidateId", "Email", testimonial.CandidateId);
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestimonialId,CandidateId,Name,Title,Description,AddedDate")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testimonial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CandidateId = new SelectList(db.CandidateDetails, "CandidateId", "Email", testimonial.CandidateId);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Testimonial testimonial = db.Testimonials.Find(id);
            if (testimonial == null)
            {
                return HttpNotFound();
            }
            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Testimonial testimonial = db.Testimonials.Find(id);
            db.Testimonials.Remove(testimonial);
            db.SaveChanges();
            return RedirectToAction("Index");
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
