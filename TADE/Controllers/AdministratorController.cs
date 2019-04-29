using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TADE.Models;

namespace TADE.Controllers
{
    public class AdministratorController : Controller
    {
       
        private ExamBookingDAL EBDal = new ExamBookingDAL();
        private AdministratorDAL adDal = new AdministratorDAL();
        private TADESql tsql = new TADESql();
        // GET: Administrator
        public ActionResult DistributeCandidatesToExaminers()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult GetExamSlotsForAvailabledate(string AvailableDate)
        {
            adDal.ManageExamDates();
            List < AvailableSeats > availableSeatsForDate = adDal.AvailableSeatsForGivenDate(AvailableDate);
            return Json(availableSeatsForDate);

        }
        [HttpPost]
        public JsonResult GetReleasedDates()
        {
            return Json(adDal.AvailableDates());
        }

        [HttpPost]
        public JsonResult ReleaseSlotsForAvailableDate(List<AvailableSeatsForSlot> availableSeatsForSlot)
        {
            string message = "OK";
            try
            {
                int? availableDateId = availableSeatsForSlot.ToList().FirstOrDefault().AvailableDatesId;
                List<AvailableSeatsForSlot> chkavailableSeatsForDate = adDal.AvailableSeatsForSlots(availableDateId);
                if (chkavailableSeatsForDate.Count() != 0)
                {
                   foreach (var seat in availableSeatsForSlot)
                    {
                        tsql.UpdateAvailableSeatsForSlot(seat.AvailableSeatsForSlotsId, seat.TotalSeats, seat.TotalSeats);
                    }
                }
                else
                {
                    DataTable dtAvailableSeatsForSlots = tsql.MakeTableAvailableSeatsForSlots(availableSeatsForSlot);
                    tsql.MakeBulkCopyTable("dbo.AvailableSeatsForSlots", dtAvailableSeatsForSlots);
                }
                

            }
            catch (Exception ex)
            { message = ex.ToString(); }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DistributeCandidates()
        {
            string message = "";
            try
            {
                int examinerCount = 0;
                int numberOfCandidates = 0;
                List<ExaminerDetail> examinerDetails = adDal.ExaminerDetails();
                if (examinerDetails != null)
                {
                    examinerCount = examinerDetails.Count();
                }
                var todayDate = DateTime.Today.Date;
                List<CandidateExamBookingDetail> candDetails = adDal.CandidateExamBookingDetails(todayDate);

                if (candDetails != null)
                {

                    numberOfCandidates = candDetails.Count();
                }

                if (numberOfCandidates != 0 && examinerCount != 0)
                {
                    //int remainingCand = 0;
                    //if (numberOfCandidates % examinerCount != 0)
                    //{
                    //    remainingCand = numberOfCandidates % examinerCount;
                    //}
                    //int candPerExaminer = numberOfCandidates / examinerCount;
                    int j = 0;
                    foreach (var cand in candDetails)
                    {
                       // TADEDBEntities candUpd = new TADEDBEntities();
                        string unidate = DateTime.Today.ToShortDateString();
                        string strunidate = unidate.Replace("/", "");

                        int examinerId = examinerDetails.ElementAt(j).ExaminerId;
                        string uniquetoken = strunidate + examinerId.ToString();

                        CandidateDetail cd = adDal.GetCandidateDetailsByCandidateId(cand.CandidateId);
                        cd.CandidateId = cand.CandidateId;
                        cd.Status = true;
                        cd.ExaminerId = examinerId;
                        cd.uniquetoken = uniquetoken;
                        bool saveCand = adDal.SaveCandidateDetails(cd);

                        // EBDal.UpdateCandidateDetails(cd);
                        j = j + 1;
                        if (j == examinerCount)
                        {
                            j = 0;
                        }

                    }

                    List<CandidateExamBookingDetail> pastcandDetails = adDal.CandidateExamBookingDetail(todayDate);

                    foreach (var pastcand in pastcandDetails)
                    {
                      
                        CandidateDetail cdpast = adDal.GetCandidateDetailsByCandidateId(pastcand.CandidateId);
                        cdpast.CandidateId = pastcand.CandidateId;
                        cdpast.Status = false;
                        adDal.SaveCandidateDetails(cdpast);
                      
                    }
                    message = "OK";
                }
                else
                {
                    message = "Number of candidates booked for today's test are " + numberOfCandidates.ToString() + " and number of examiners is " + examinerCount.ToString();
                }
                List<DistributeCandidateDetails> distributedCandDetails = adDal.DistributeCandidateDetails();
                return Json(distributedCandDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            
        }
    }
}