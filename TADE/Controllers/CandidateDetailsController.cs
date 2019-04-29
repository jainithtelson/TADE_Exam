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
    public class CandidateDetailsController : Controller
    {
        // private TADEDBEntities db = new TADEDBEntities();
        private CandidateDetailsDAL candDal = new CandidateDetailsDAL();
        private TADESql tsql = new TADESql();
        private TADEDBEntities db = new TADEDBEntities();
        // GET: CandidateDetails
        public ActionResult Index()
        {
            return View(candDal.CandidateDetails());
        }

        // GET: CandidateDetails/Details/5
        public ActionResult Details()
        {
            int? id = Convert.ToInt32(Session["CandidateId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidateDetail candidateDetail = candDal.CandidateDetailsById(id);
            if (candidateDetail == null)
            {
                return HttpNotFound();
            }
            return View(candidateDetail);
        }

        // GET: CandidateDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CandidateDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CandidateId,Email,Password,FirstName,MiddleName,LastName,Year,Month,Day,Phone,AddressLine1,AddressLine2,AddressLine3,PostCode,DrivingLicenseNumber")] CandidateDetail candidateDetail)
        {
            if (ModelState.IsValid)
            {
                //capture video
                //http://www.html5rocks.com/en/tutorials/getusermedia/intro/
                //https://github.com/collab-project/videojs-record
                DateTime dob = new DateTime(candidateDetail.Year, candidateDetail.Month, candidateDetail.Day);
                string ip = Request.UserHostAddress;
                Random rand = new Random();


                candidateDetail.EmailApproved = true;
                candidateDetail.Status = true;
                candidateDetail.DateOfBirth = dob.ToString();
                candidateDetail.IPAddress = ip;
                candidateDetail.ExamId = rand.Next().ToString();
                candDal.SaveCandidateDetails(candidateDetail);
                Session["CandidateId"] = Convert.ToInt32(candidateDetail.CandidateId);

                RegisterViewModel rModel = new RegisterViewModel();
                rModel.Email = candidateDetail.Email;
                rModel.Password = candidateDetail.Password;
                rModel.ConfirmPassword = candidateDetail.Password;
                rModel.Role = "Candidate";
                var registerUser = new AccountController().Register(rModel);

                //   return RedirectToAction("Register", "Account", rModel);
                //  return RedirectToAction("StartExam", "DrivingTest");
            }


            return View(candidateDetail);
        }

        // GET: CandidateDetails/Edit/5
        public ActionResult Edit()
        {
            int? id = Convert.ToInt32(Session["CandidateId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidateDetail candidateDetail = candDal.CandidateDetailsById(id);
            if (candidateDetail == null)
            {
                return HttpNotFound();
            }
            return View(candidateDetail);
        }

        // POST: CandidateDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateId,Email,FirstName,MiddleName,LastName,Phone,AddressLine1,AddressLine2,AddressLine3,PostCode,DrivingLicenseNumber")] CandidateDetail candidateDetail)
        {
            if (ModelState.IsValid)
            {
                candDal.UpdateCandidateDetail(candidateDetail);
                return RedirectToAction("Details");
            }
            return View(candidateDetail);
        }

        // GET: CandidateDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidateDetail candidateDetail = candDal.CandidateDetailsById(id);
            if (candidateDetail == null)
            {
                return HttpNotFound();
            }
            return View(candidateDetail);
        }

        // POST: CandidateDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CandidateDetail candidateDetail = candDal.CandidateDetailsById(id);
            candDal.RemoveCandidateDetail(candidateDetail);
            return RedirectToAction("Details");
        }
        public ActionResult CandidateExamBookingDetails()
        {
            return View();
        }
        public ActionResult RegistrationConfirmation()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetSlotsLists(string AvailableDate)
        {
            //Session["CandidateId"]
            DateTime dtAvailabledate = Convert.ToDateTime(Convert.ToDateTime(AvailableDate).ToShortDateString());
            int AvailableDatesId = 0;
            AvailableDate chkAvailabledate = candDal.AvailableDateByDate(dtAvailabledate);

            if (chkAvailabledate != null)
            {
                AvailableDatesId = chkAvailabledate.AvailableDatesId;

            }
            List< AvailableSeatsForDate> chkavailableSeatsForDate = candDal.AvailableSeatsForDate(AvailableDatesId);
            
            return Json(chkavailableSeatsForDate);
        }
        [HttpPost]
        public JsonResult BookExam([Bind(Include = "BookingId,CandidateId,BookedDate,SlotId")] CandidateExamBookingDetail candidateExamBookingDetail)
        {
            string message = "";
            //Here we will save data to the database
            try
            {
                int candidateId = Convert.ToInt32(Session["CandidateId"]);
                if (candidateId != 0)
                {
                    DateTime exambookdate = Convert.ToDateTime(candidateExamBookingDetail.BookedDate);
                    exambookdate = Convert.ToDateTime(exambookdate.ToShortDateString());
                    //
                    candidateExamBookingDetail.BookedDate = exambookdate;
                    candidateExamBookingDetail.CandidateId = candidateId;
                    candidateExamBookingDetail.Status = true;
                    CandidateExamBookingDetail candidateExamBookingDetl = candDal.CandidateExamBookingDetailByCandidateId(candidateExamBookingDetail.CandidateId);

                    if (candidateExamBookingDetl == null)
                    {
                        candDal.SaveCandidateExamBookingDetails(candidateExamBookingDetail);
                       
                        UpdateSeats(exambookdate, candidateExamBookingDetail.SlotId, -1);
                    }
                    else
                    {
                        if (candidateExamBookingDetl.SlotId == candidateExamBookingDetail.SlotId && candidateExamBookingDetl.BookedDate == candidateExamBookingDetail.BookedDate)
                        {

                        }
                        else
                        {
                            UpdateSeats(Convert.ToDateTime(candidateExamBookingDetl.BookedDate), candidateExamBookingDetl.SlotId, 1);
                            UpdateSeats(exambookdate, candidateExamBookingDetail.SlotId, -1);
                        }

                        candDal.UpdateCandidateExamBookingDetails(candidateExamBookingDetail);


                    }
                    if(exambookdate==Convert.ToDateTime(DateTime.Today.ToShortDateString()))
                    {
                        int ExaminerId = candDal.ExaminerDetailByStatus().ExaminerId;
                        //TADEDBEntities candUpd = new TADEDBEntities();
                        string unidate = DateTime.Today.ToShortDateString();
                        string strunidate = unidate.Replace("/", "");

                        
                        string uniquetoken = strunidate + ExaminerId.ToString();

                        CandidateDetail cd = candDal.CandidateDetailsById(candidateId);
                        cd.CandidateId = candidateId;
                        cd.Status = true;
                        cd.ExaminerId = ExaminerId;
                        cd.uniquetoken = uniquetoken;
                        candDal.UpdateCandidateDetail(cd);

                    }
                    SendMail(candidateExamBookingDetail);
                    message = "Success";
                }
            }
            catch (Exception ex) { message = ex.ToString(); }


            return Json(message);

            // return new JsonResult { Data = message, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public void UpdateSeats(DateTime exambookdate, int SlotId, int seatadd)
        {
            int AvailableDatesId = GetAvailableDateId(exambookdate);
            //int SlotId = candidateExamBookingDetail.SlotId;
            AvailableSeatsForDate chkavailableSeatsForDate = candDal.AvailableSeatsForDateBySlotId(AvailableDatesId, SlotId);
            int availableSeatsForSlotsId = chkavailableSeatsForDate.availableSeatsForSlotsId;
            int? totalSeats = chkavailableSeatsForDate.TotalSeats;
            int? remainingSeats = chkavailableSeatsForDate.RemainingSeats + seatadd;
            if(remainingSeats>totalSeats)
            {
                remainingSeats = totalSeats;
            }
            tsql.UpdateAvailableSeatsForSlot(availableSeatsForSlotsId, totalSeats, remainingSeats);
        }
        public int GetAvailableDateId(DateTime dtAvailabledate)
        {

            int AvailableDatesId = 0;
            var chkAvailabledate = candDal.AvailableDateByDate(dtAvailabledate);
            if (chkAvailabledate != null)
            {
                AvailableDatesId = chkAvailabledate.AvailableDatesId;
            }
            return AvailableDatesId;
        }
        //
        [HttpPost]
        public JsonResult GetSlotsdetails()
        {
            int candidateId = Convert.ToInt32(Session["CandidateId"]);
            if (candidateId == 0)
            {

            }
            var sdetail = db.CandidateExamBookingDetails.Where(a => a.CandidateId.Equals(candidateId)).Select(
                sd => new
                {
                    BookingId = sd.BookingId,
                    CandidateId = sd.CandidateId,
                    BookedDate = sd.BookedDate,
                    SlotId = sd.SlotId,
                    SlotName = sd.ExamSlot.SlotName
                }).FirstOrDefault();

            return Json(sdetail);
        }
        public bool SendMail(CandidateExamBookingDetail candidateExamBookingDetail)
        {
            try
            {
                string message = "";
                //Here we will save data to the database
                DateTime bookedDate = Convert.ToDateTime(candidateExamBookingDetail.BookedDate);
                string slotname = db.ExamSlots.Where(s => s.SlotId == candidateExamBookingDetail.SlotId).FirstOrDefault().SlotName;

                string ToEmail = db.CandidateDetails.Where(c => c.CandidateId == candidateExamBookingDetail.CandidateId).FirstOrDefault().Email;
                //check username available
                byte[] bytes = null;
                MailModel mm = new Models.MailModel();
                mm.To = ToEmail;
                mm.From = "jainith@gmail.com";
                mm.Body = "Dear " + candidateExamBookingDetail.CandidateDetail.FirstName + ", <br /> <br />Thank you for booking exam with TADE. <br /><br />Your exam has been booked on " + bookedDate.ToString("dd/MM/yyyy") + " at " + slotname + ".<br /><br /><u>To attend the exam:</u><br /><br />Please read the registration confirmation email for exam details.<br /><br />  Kind Regards <br /> TADE Admin Team";
                mm.Subject = "TADE exam booking confirmation";
                message = "Exam booking confirmation";
                SendMailBLL sm = new SendMailBLL();
                sm.SendMail(mm, bytes);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
           
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                candDal.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
