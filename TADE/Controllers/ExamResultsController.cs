using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using TADE.Models;

namespace TADE.Controllers
{
    public class ExamResultsController : Controller
    {
        private TADEDBEntities TD = new TADEDBEntities();
        private ExamResultsDAL ERDal = new ExamResultsDAL();
       // private TADEDBEntities db = new TADEDBEntities();

        // GET: ExamResults
      
        public ActionResult ExamResultWithGrade()
        {
            if (Session["CandidateId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int CandidateId = Convert.ToInt32(Session["CandidateId"]);
            var jsobj = TD.ExamResults.Include(e => e.CandidateDetail).Include(e => e.ExamType).Where(r => r.CandidateId == CandidateId).Select(x => new
            {
                FirstName = x.CandidateDetail.FirstName,
                MiddleName = x.CandidateDetail.MiddleName,
                LastName = x.CandidateDetail.LastName,
                Email=x.CandidateDetail.Email,
                Date =x.Date,
                IPAddress=x.CandidateDetail.IPAddress,
                ExamId=x.CandidateDetail.ExamId,
                DateOfBirth=x.CandidateDetail.DateOfBirth,
                DrivingLicense = x.CandidateDetail.DrivingLicenseNumber,
                Address=x.CandidateDetail.AddressLine1+ "<br />" + x.CandidateDetail.AddressLine2 + "<br />" + x.CandidateDetail.AddressLine3 + "<br />" + x.CandidateDetail.PostCode,
                TotalScore = x.Score,
                Grade = x.Grade,
                Explanation = x.Explanation
            }).ToList();
            DrivingTestResult DR = new Models.DrivingTestResult();
            DR.Date = jsobj[0].Date;
            DR.IPAddress = jsobj[0].IPAddress;
            DR.ExamId = Convert.ToInt32(jsobj[0].ExamId);
            DR.DateOfBirth = jsobj[0].DateOfBirth;
            DR.FirstName = jsobj[0].FirstName;
            DR.MiddleName= jsobj[0].MiddleName;
            DR.LastName = jsobj[0].LastName;
            DR.Email = jsobj[0].Email;
            DR.Address = jsobj[0].Address;
            DR.DrivingLicense = jsobj[0].DrivingLicense;
            DR.Explanation = jsobj[0].Explanation;
            DR.TotalScore = 0;
            DR.Grade = 0;
            foreach (var item in jsobj)
            {
                DR.TotalScore = DR.TotalScore + item.TotalScore;
                DR.Grade = DR.Grade + item.Grade;
            }
            
            SendMail(DR);
            TADEDBEntities candUpd = new TADEDBEntities();
            CandidateDetail cd = candUpd.CandidateDetails.Find(CandidateId);
            cd.Status = false;
            cd.ExamAttended = true;
            candUpd.Entry(cd).State = EntityState.Modified;
            candUpd.SaveChanges();
            Session["CandidateId"] = null;
            return View(DR);
        }
        public bool SendMail(DrivingTestResult dr)
        {
            string message = "";
            //Here we will save data to the database

            //check username available

            if (dr.Email != null)
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {

                        StringBuilder sb = new StringBuilder();
                        sb.Append("<h1 style='text - align: center; '>TADE EXAM RESULT</h1><p>&nbsp; &nbsp; &nbsp; &nbsp;</p>");
                        sb.Append("<table style='width: 100 % '><tbody><tr><td colspan='2'><h2>APPLICANT DETAILS</h2></td></tr><tr><td>1. FIRST NAME:</td><td>");
                        sb.Append(dr.FirstName);
                        sb.Append("</td></tr><tr><td>2. MIDDLE NAME:</td><td>");
                        sb.Append(dr.MiddleName);

                        sb.Append("</td></tr><tr><td>3. LAST NAME:</td><td>");
                        sb.Append(dr.LastName);

                        sb.Append("</td></tr><tr><td>4. DATE OF BIRTH</td><td>");
                        sb.Append(dr.DateOfBirth);
                        sb.Append("</td></tr><tr><td>5. DRIVING LICENCE No.</td><td>");
                        sb.Append(dr.DrivingLicense);

                        sb.Append("</td></tr><tr><td>6. ADDRESS</td><td>");
                        sb.Append(dr.Address);
                        sb.Append(" </td></tr><tr><td colspan='2'><h2>EXAM DETAILS</h2></td></tr><tr><td>7. REGISTRATION No.</td><td>");
                        sb.Append(dr.ExamId);
                        sb.Append("</td></tr><tr><td>8. IP ADDRESS OF COMPUTER TEST ATTENDED</td><td>");
                        sb.Append(dr.IPAddress);

                        sb.Append("</td></tr><tr><td>9. EXAMINER ID</td><td>");
                        sb.Append("&nbsp;");
                        sb.Append("</td></tr><tr><td>10. TIME OF EXAM</td><td>");
                        sb.Append("&nbsp;");

                        sb.Append("</td></tr><tr><td>11. DURATION OF EXAM</td><td>");
                        sb.Append("&nbsp;");
                        sb.Append(" </td></tr><tr><td>12. EXAM GRADE</td><td>");
                        sb.Append(dr.Grade);

                        sb.Append("</td></tr><tr><td>13. EXAM RESULT VALID TILL</td><td>");
                        sb.Append("&nbsp;");
                        sb.Append(" </td></tr><tr><td>14. COMMENTS</td><td>");
                        sb.Append(dr.Explanation);
                        sb.Append("</td></tr></tbody></table>");


                        StringReader sr = new StringReader(sb.ToString());
                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            htmlparser.Parse(sr);
                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            string thesavePath = Server.MapPath("~/CandidateExamResults") + "\\" + dr.ExamId.ToString() + ".pdf";
                            System.IO.File.WriteAllBytes(thesavePath, bytes);
                            MailModel mm = new Models.MailModel();
                            mm.To = dr.Email;
                            mm.From = "jainith@gmail.com";
                            mm.Body = "Dear " + dr.FirstName + ", <br /> <br />Congratulations! You have successfully completed TADE exam. Please find attached certificate for future reference. <br /><br /> Kind Regards <br /> TADE Admin Team";
                            mm.Subject = "TADE exam result";
                            message = "Success";
                            SendMailBLL sm = new SendMailBLL();
                            sm.SendMail(mm, bytes);
                        }

                    }
                }


            }
            else
            {
                message = "Failed!";
            }
            return true;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TD.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
