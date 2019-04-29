using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TADE.Models;

namespace TADE.Controllers
{
    public class ExaminerController : Controller
    {
        // GET: Examiner
        private TADEDBEntities db = new TADEDBEntities();
        public ActionResult AllocatedCandidateDetails()
        {
            if (Session["ExaminerId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
           
            return View();
        }
        [HttpPost]
        public JsonResult GetAllocatedCandDetails()
        {
          
            int examinerId = Convert.ToInt32(Session["ExaminerId"]);
            // var vs = db.ContentManagements.Include(c => c.AspNetUser).Include(c => c.Category).Where(c => c.UserId == UserId && c.Status == true);
            var candDetails = db.CandidateDetails.Include("CandidateExamBookingDetails").Where(a => a.ExaminerId == examinerId && a.Status==true).Select(x => new
            {
                candidateId = x.CandidateId,
                firstName = x.FirstName,
                lastName = x.LastName,
                bookeddate = x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().BookedDate.ToString(),
                SlotId = x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().SlotId,
                slot = db.ExamSlots.Where(s => s.SlotId == x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().SlotId).ToList().FirstOrDefault().SlotName

            });
            return Json(candDetails);
        }
      
        public ActionResult VideoMonitoring()
        {
            if(Session["ExaminerId"]!=null)
            {
                int examinerId = Convert.ToInt32(Session["ExaminerId"]);
                var candDetails = db.CandidateDetails.Where(a => a.ExaminerId == examinerId && a.Status == true).Select(x => new
                {
                    uniquetoken = x.uniquetoken
                }).FirstOrDefault();
                
                return RedirectToAction("InspectCandidates", "VideoMonitoring", new { uniquetoken = candDetails.uniquetoken });
            }
            return View();
        }
    }
}