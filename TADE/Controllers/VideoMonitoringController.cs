using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TADE.Models;

namespace TADE.Controllers
{
    public class VideoMonitoringController : Controller
    {
        // GET: VideoMonitoring
        private TADEDBEntities db = new TADEDBEntities();
        public ActionResult VideoConferenceBeta(string uniquetoken)
        {
            ViewBag.stratExam = "HidestartExamButton";
            if (Session["ExaminerId"] != null)
            {
                ViewBag.RoomName = "TADE" + Convert.ToString(Session["ExaminerId"]);
                ViewBag.Visibility = true;
            }
            else
            {
                int? id = Convert.ToInt32(Session["CandidateId"]);
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var candidateDetail = db.CandidateDetails.Find(id);
                // ViewBag.CandName = candidateDetail.CandidateId;
                ViewBag.CandName = candidateDetail.FirstName;
                // ViewBag.CandName = candidateDetail.FirstName + " " + candidateDetail.MiddleName + " " + candidateDetail.LastName;
                ViewBag.Visibility = false;
                ViewBag.stratExam = "startExamButton";
            }
            return View();
        }
        public ActionResult InspectCandidates(string uniquetoken)
        {
            ViewBag.Visibility = true;
            ViewBag.stratExam = "HidestartExamButton";
            //var candDetails = db.CandidateDetails.Where(a => a.CandidateId == 1 && a.Status == true).Select(x => new
            //{
            //    id = x.CandidateId,
            //    uniquetoken = x.uniquetoken
            //}).FirstOrDefault();


            if (Session["ExaminerId"] != null)
            {
                ViewBag.RoomName = "TADE" + Convert.ToString(Session["ExaminerId"]);
                ViewBag.Visibility = true;
            }
            else
            {
                if (Convert.ToBoolean(Session["BackCamera"]) == true)
                {
                    ViewBag.BackCamera = true;
                }
                else
                {
                    ViewBag.BackCamera = false;
                }
                int? id = Convert.ToInt32(Session["CandidateId"]);
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var candidateDetail = db.CandidateDetails.Find(id);
                ViewBag.CandID = candidateDetail.CandidateId;
                //ViewBag.CandName = candidateDetail.FirstName;
                ViewBag.CandName = candidateDetail.FirstName + " " + candidateDetail.MiddleName + " " + candidateDetail.LastName;
                ViewBag.Visibility = false;
                ViewBag.stratExam = "startExamButton";
            }
            return View();
        }

        ////[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CandidateDetailVerification(int candId)
        {
            ViewBag.CandidateId = candId;
            ViewBag.Visibility = true;
            CandidateVerification candidateVerification = new CandidateVerification();
            candidateVerification.CandidateId = candId;
            if (Session["ExaminerId"] != null)
            {

                ViewBag.Visibility = true;
            }
            else
            {
                ViewBag.Visibility = false;
            }
            return View(candidateVerification);
        }
        [HttpPost]
        public bool SaveCandidateverification([Bind(Include = "CandidateId,Photo,DrivingLicense,FrontCamera,Microphone,BackCamera,LipMovement")] CandidateVerification candidateVerification)
        {
            // int candidateId = Convert.ToInt32(ViewBag.CandidateId);
            // candidateVerification.CandidateId = candidateId;
            try
            {

               // candidateVerification.CandidateId = CandidateId;
                int examinerId = Convert.ToInt32(Session["ExaminerId"]);
                candidateVerification.VerifiedBy = examinerId;
                candidateVerification.VerifiedOn = DateTime.Now;
                if (candidateVerification.Photo == true && candidateVerification.DrivingLicense == true && candidateVerification.FrontCamera == true && candidateVerification.Microphone == true && candidateVerification.BackCamera == true && candidateVerification.LipMovement == true)
                {
                    candidateVerification.EligibleForStartExam = true;
                }
                else
                {
                    candidateVerification.EligibleForStartExam = false;
                }
                db.CandidateVerifications.Add(candidateVerification);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        [HttpGet]
        public ActionResult GetCandidateDetails()
        {

            int? id = Convert.ToInt32(Session["CandidateId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var candidateDetail = db.CandidateDetails.Find(id);

            return Json(candidateDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CandidateVerificationPartial(string message)
        {
            ViewBag.Message = message;
            return PartialView();
        }
        //public ActionResult ExaminorVideoMonitor()
        //{
        //    Session["Examiner"] = "Examiner";
        //    return RedirectToAction("VideoConferenceBeta", new { uniquetoken = "#123456789" });
        //}
        //public ActionResult StartConferenceUser()
        //{
        //    Session["Examiner"] = "";
        //    return RedirectToAction("VideoConferenceBeta", new { uniquetoken = "#123456789" });
        //}
    }
}