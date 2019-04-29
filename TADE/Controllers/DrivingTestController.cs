using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TADE.Models;
using TADE.CustomResult;
using System.Data.Entity;

namespace TADE.Controllers
{
    public class DrivingTestController : Controller
    {
        private TADEDBEntities TD = new TADEDBEntities();
        private ExamResultsDAL ERDal = new ExamResultsDAL();
        // GET: DrivingTest
        public ActionResult Index()
        {
            return new VideoResult();
        }
        public ActionResult VideoTest()
        {
            //https://www.sitepoint.com/build-a-web-game-in-an-hour-with-visual-studio-and-asp-net/
            return View();
        }
        public ActionResult VideoTestAdvanced()
        {
            //https://www.sitepoint.com/build-a-web-game-in-an-hour-with-visual-studio-and-asp-net/
            return View();
        }
        public ActionResult SetUpVideo()
        {
            if (Session["CandidateId"] != null)
            {
                int candidateId = Convert.ToInt32(Session["CandidateId"]);
                var candDetails = TD.CandidateDetails.Where(a => a.CandidateId == candidateId && a.Status == true).Select(x => new
                {
                    id=x.CandidateId,
                    uniquetoken = x.uniquetoken
                }).FirstOrDefault();
                //CandidateDetail candidateDetails = TD.CandidateDetails.Find(candDetails.id);
                //if (Convert.ToBoolean(Session["FrontCamera"]) == true)
                //{
                //    candidateDetails.FrontCamera = true;
                //}
                //if (Convert.ToBoolean(Session["BackCamera"]) == true)
                //{
                //    candidateDetails.BackCamera = true;
                //}
                //TD.Entry(candidateDetails).State = EntityState.Modified;
                //TD.SaveChanges();

                return RedirectToAction("InspectCandidates", "VideoMonitoring", new { uniquetoken = candDetails.uniquetoken });
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult BackCamera()
        {
            if (Session["CandidateId"] != null)
            {
                Session["BackCamera"] = true;
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
        public ActionResult StartExam()
        {
            //get  candidateId 
            //check logged in user time and booked time.
            ViewBag.stratExam = "HidestartExamButton";
            
            int candidateId = Convert.ToInt32(Session["CandidateId"]);
            if (candidateId != 0)
            {
                var candidateExamBookingDetl = TD.CandidateExamBookingDetails.Where(a => a.CandidateId.Equals(candidateId)).FirstOrDefault();
                if (candidateExamBookingDetl != null)
                {
                    string dtex = DateTime.Now.ToShortDateString();
                    TimeSpan tcurrent = new TimeSpan(DateTime.Now.Hour,DateTime.Now.Minute,0);
                    int thbookedrange = Convert.ToInt32(candidateExamBookingDetl.ExamSlot.Timefrom) -1;
                    int thbookedExact = Convert.ToInt32(candidateExamBookingDetl.ExamSlot.Timefrom);
                    
                    int thbookedExactMinutes = Convert.ToInt32(candidateExamBookingDetl.ExamSlot.TimefromMinutes);
                    TimeSpan tbookedrange = new TimeSpan(thbookedrange, thbookedExactMinutes, 0);
                    TimeSpan tbookedExact = new TimeSpan(thbookedExact, thbookedExactMinutes, 0);
                    DateTime dtbookeddate = Convert.ToDateTime(candidateExamBookingDetl.BookedDate);
                    ViewBag.BookedDate = dtbookeddate.ToString("dd/MM/yyyy") + " and time is " + candidateExamBookingDetl.ExamSlot.SlotName + ". Please login to attend test 30 minutes before booked time. your booked time is " + thbookedExact + ": "+ thbookedExactMinutes;
                    if (candidateExamBookingDetl.BookedDate == Convert.ToDateTime(dtex))
                    {
                        ViewBag.EditExamDate = false;
                        if (tbookedrange <= tcurrent && tcurrent <= tbookedExact)
                        {
                            ViewBag.Message = "This exam has 1 test and each question has seperate time limit and negative " +
     "mark also. Total time for this exam will be 60 minutes and please set your rear and front camera before starting exam.<br /><br />We need to verify your identity before the exam.<br /><br />You need to switch on your front and back camera for identification. <br /><br />Please set your browser option to allow TADE to access your camera and microphone. Eg. If you are using Mozilla browser, you may see a warning window asking your permission to share the camera microphone. Select your camera and microphone and click share selected devises. " + 
    "<h2> If you are ready for verification, please click Ready for video button below. </h2 > ";
                            ViewBag.stratExam = "startExamButton";
                            Session["FrontCamera"] = true;
                        }
                        else
                        {
                            ViewBag.Message = "please login to attend test 30 minutes before booked time. your booked time is " + thbookedExact + ":00 ";
                            ViewBag.stratExam = "HidestartExamButton";
                           // Session["CandidateId"] = null;
                            // message: please login to attend test 30 minutes before booked time. your booked time is
                        }
                    }
                    else
                    {
                        ViewBag.EditExamDate = true;
                       // ViewBag.Message = "please login exact date of exam booked... your bookked date is " + candidateExamBookingDetl.BookedDate;
                        ViewBag.stratExam = "HidestartExamButton";
                        //Session["CandidateId"] = null;
                    }

                }
                
            }
                return View();
        }
        public ActionResult StartVideoIndicatorTest()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CalculateScore(Dictionary<string, string> mouseClickArr)

        {
            //VideoScore vs = new Models.VideoScore();
            // var vs = db.ContentManagements.Include(c => c.AspNetUser).Include(c => c.Category).Where(c => c.UserId == UserId && c.Status == true);
            var vs = TD.VideoScores.Where(v => v.VideoID == 1 && v.Status == true).ToList();
            int startInterval = 0;
            int? score = 0;
            foreach (var item in mouseClickArr)
            {

                decimal k = Convert.ToDecimal(item.Key.ToString());
                string v = item.Value.ToString();
                for (int i = startInterval; i < vs.Count; i++)
                {

                    decimal timeintervalLow = vs[i].TimeInterval - 1;
                    decimal timeintervalUp = vs[i].TimeInterval + 1;
                    if (k >= timeintervalLow && k <= timeintervalUp)
                    {
                        if (vs[i].MouseClick == v)
                        {
                            score = score + Convert.ToInt32(vs[i].Score);
                        }
                        else
                        { score = score - 1; }
                        startInterval = i + 1;
                        break;
                    }
                    if (k < timeintervalLow)
                    { break; }

                }
            }
            int Grade = ERDal.CalculateGrade(score);
            int CandidateId = Convert.ToInt32(Session["CandidateId"]);

            int ExamId = ERDal.ExamTypeId("VideoIndicator");
            ExamResult ER = new ExamResult();
            ER.CandidateId = CandidateId;
            ER.ExamId = ExamId;
            ER.Date = DateTime.Now.ToString("dd/MM/yyyy");
            ER.Score = score;
            ER.Grade = Grade;
            int examResults = ERDal.SaveScore(ER);
            // Session["CandidateId"] = 4;
            //// return RedirectToAction("Index", "ExamResults");
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObjectiveTest()
        {
            if (Session["EndDate"] == null)
            {
                Session["EndDate"] = DateTime.Now.AddMinutes(1).ToString("dd-MM-yyyy h:mm:ss tt");
            }
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.EndDate = Session["EndDate"];
            return View();
            //return View();
        }
        //
        public ActionResult ObjectiveTestAdvancedPrep()
        {
            if (Session["CandidateId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["EndDate"] == null)
            {
                Session["EndDate"] = DateTime.Now.AddMinutes(1).ToString("dd-MM-yyyy h:mm:ss tt");
            }
            return RedirectToAction("ObjectiveTestAdvanced", "DrivingTest", new { candidateId = Convert.ToInt32(Session["CandidateId"]) });
            }
        public ActionResult ObjectiveTestAdvanced(int candidateId)
        {
            //if(Session["CandidateId"] == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            if (Session["EndDate"] == null)
            {
                Session["EndDate"] = DateTime.Now.AddMinutes(1).ToString("dd-MM-yyyy h:mm:ss tt");
            }
            if (Session["ExaminerId"] != null)
            {

                ViewBag.Visibility = true;
            }
            else
            {
                ViewBag.Visibility = false;
            }
            ViewBag.CandidateId = candidateId;
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.EndDate = Session["EndDate"];
            return View();
        }
        [HttpPost]
        public JsonResult GetQuestions()
        {
            var jsobj = TD.ObjectiveTests.Select(x => new
            {
                ObjectiveTestID = x.ObjectiveTestID,
                Question = x.Question,
                AnswerOptions = x.AnswerOptions.Where(b => b.ObjectiveTestID == x.ObjectiveTestID).Select(ao => new { AnswerOptionsID = ao.AnswerOptionsID, AnswerOptionsDescription = ao.AnswerOptionsDescription }),
                CorrectAnswerID = x.CorrectAnswerID,
                Answered = x.Answered,
                Score = x.Score,
                QuestionImage = x.QuestionImage,
                ImageShow = x.ImageShow,
                AllowedTime = x.AllowedTime,
                Priority = x.Priority,
                TimeTaken = 0
            });
            return Json(jsobj);
        }
        [HttpPost]
        public JsonResult Submit(List<ObjectiveTest> answers)
        {
            int answeredTime = 0;
            int? score = 0;
            string explanation = "";
            bool failedImportant = false;
            foreach (var answer in answers)
            {
                answeredTime = answer.TimeTaken;

                if (answer.CorrectAnswerID == answer.Answered)
                {
                    decimal? responsePercentage = ERDal.ResponsePercentage(answer.AllowedTime, answeredTime);
                    int? scoreByResponseTime = ERDal.CalculateScoreByResponseTime(responsePercentage, answer.Score);
                    score = score + scoreByResponseTime;

                }
                else
                {
                    if (answer.Priority == "Important")
                    {
                        failedImportant = true;
                        string CandidateAnswer = "";
                        AnswerOption ansOp = new AnswerOption();
                        if (answer.Answered != null)
                        {
                            ansOp = TD.AnswerOptions.Find(answer.Answered == null ? 0 : answer.Answered);
                            
                            CandidateAnswer = ". <br /> You have answered " + ansOp.AnswerOptionsDescription;
                        }
                       
                        
                        ansOp = TD.AnswerOptions.Find(answer.CorrectAnswerID);
                        string correctAnswer = ansOp.AnswerOptionsDescription;
                        explanation = explanation + "You have failed to answer an important question " + answer.Question + CandidateAnswer + ". <br /> The correct answer is " + correctAnswer + ". <br /> Unfortunately this time you lost your score as you have to ..... ";
                    }
                    score = score - 1;
                }
            }
            // Session["CandidateId"] = 4;
            if (failedImportant == true)
            {
                score = 0;
            }
            int Grade = ERDal.CalculateGrade(score);
            int CandidateId = Convert.ToInt32(Session["CandidateId"]);

            int ExamId = ERDal.ExamTypeId("Objective");
            ExamResult ER = new ExamResult();
            ER.CandidateId = CandidateId;
            ER.ExamId = ExamId;
            ER.Date = DateTime.Now.ToString("dd/MM/yyyy");
            ER.Score = score;
            ER.Grade = Grade;
            ER.Explanation = explanation;
            int examResults = ERDal.SaveScore(ER);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Malpractice()
        {
            int? score = 0;
            string explanation = "Malpractice made. Not eligible for continue TADE exam.";
            
            int Grade = 0;
            int CandidateId = Convert.ToInt32(Session["CandidateId"]);

            int ExamId = ERDal.ExamTypeId("Objective");
            ExamResult ER = new ExamResult();
            ER.CandidateId = CandidateId;
            ER.ExamId = ExamId;
            ER.Date = DateTime.Now.ToString("dd/MM/yyyy");
            ER.Score = score;
            ER.Grade = Grade;
            ER.Explanation = explanation;
            int examResults = ERDal.SaveScore(ER);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}