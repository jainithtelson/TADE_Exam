using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TADE.Models
{
    public class AdministratorDAL
    {
        private TADEDBEntities db = new TADEDBEntities();
        private ExamBookingDAL EBDal = new ExamBookingDAL();
        private TADESql tsql = new TADESql();
        public bool ManageExamDates()
        {
            DateTime? dtAvailableDate = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            var transferdatesEnity = new TADEDBEntities();
            var AvailableDateRemains = transferdatesEnity.AvailableDates.Where(ad => ad.AvailableDate1 < dtAvailableDate).ToList();

            if (AvailableDateRemains != null)
            {
                DataTable dtsourceTable = tsql.SelectAvailableDates();
                tsql.MakeBulkCopyTable("dbo.AvailableDatesHistory", dtsourceTable);
                tsql.DeleteAvailableDates();

            }
            return true;
        }
        public List<AvailableSeats> AvailableSeatsForGivenDate(string AvailableDate)
        {
            DateTime dtAvailabledate = Convert.ToDateTime(Convert.ToDateTime(AvailableDate).ToShortDateString());
            int AvailableDatesId = 0;
            var chkAvailabledate = db.AvailableDates.Where(ad => ad.AvailableDate1 == dtAvailabledate).ToList().FirstOrDefault();
            if (chkAvailabledate == null)
            {
                AvailableDate avDate = new Models.AvailableDate();
                avDate.AvailableDate1 = dtAvailabledate;
                avDate.Status = true;
                db.AvailableDates.Add(avDate);
                db.SaveChanges();
                AvailableDatesId = avDate.AvailableDatesId;
            }
            else
            {
                AvailableDatesId = chkAvailabledate.AvailableDatesId;
            }
            List<AvailableSeats> chkavailableSeatsForDate = db.AvailableSeatsForSlots.Where(ad => ad.AvailableDatesId == AvailableDatesId).Select(ed => new AvailableSeats()
            {
                AvailableSeatsForSlotsId = ed.AvailableSeatsForSlotsId,
                SlotId = ed.SlotId,
                SlotName = ed.ExamSlot.SlotName,
                AvailableDatesId = ed.AvailableDatesId,
                TotalSeats = ed.TotalSeats,
                RemainingSeats = ed.RemainingSeats,
                Status = ed.Status
            }).ToList();


            
            if (chkavailableSeatsForDate.Count() != 0)
            {
                return chkavailableSeatsForDate;
            }
            else
            {
                List<AvailableSeats> examslots = db.ExamSlots.Select(es => new AvailableSeats()
                {
                    AvailableSeatsForSlotsId = 0,
                    SlotId = es.SlotId,
                    SlotName = es.SlotName,
                    AvailableDatesId = AvailableDatesId,
                    TotalSeats = 15,
                    RemainingSeats = 15,
                    Status = true
                }).ToList();
                return examslots;
            }
        }
        public List<string> AvailableDates()
        {
            List<string> releasedDates = db.AvailableDates.Select
                (a => a.AvailableDate1.ToString()).ToList();
            return releasedDates;
        }
        public List<AvailableSeatsForSlot> AvailableSeatsForSlots(int? availableDateId)
        {
            return db.AvailableSeatsForSlots.Where(ad => ad.AvailableDatesId == availableDateId).ToList();
        }
        public List<ExaminerDetail> ExaminerDetails()
        {
            List<ExaminerDetail> examinerDetails = db.ExaminerDetails.Where(a => a.Status == true && a.Emergency != true).ToList();
            return examinerDetails;
        }
        public List<CandidateExamBookingDetail> CandidateExamBookingDetails(DateTime bookedDate)
        {
            List<CandidateExamBookingDetail> candDetails = db.CandidateExamBookingDetails.Where(a => a.BookedDate == bookedDate).ToList();
            return candDetails;
        }
        public CandidateDetail GetCandidateDetailsByCandidateId(int CandidateId)
        {
            CandidateDetail cd = db.CandidateDetails.Find(CandidateId);
            return cd;
        }
        public bool SaveCandidateDetails(CandidateDetail cd)
        {
            try
            {
                db.Entry(cd).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public List<CandidateExamBookingDetail> CandidateExamBookingDetail(DateTime bookedDate)
        {
            List< CandidateExamBookingDetail> pastcandDetails = db.CandidateExamBookingDetails.Where(a => a.BookedDate < bookedDate && a.CandidateDetail.ExamAttended == false).ToList();
            return pastcandDetails;
        }
        public List<DistributeCandidateDetails> DistributeCandidateDetails()
        {
            List<DistributeCandidateDetails> distributedCandDetails = db.CandidateDetails.Include("CandidateExamBookingDetails").Where(a => a.Status == true).Select(x => new DistributeCandidateDetails()
            {
                candidateId = x.CandidateId,
                firstName = x.FirstName,
                lastName = x.LastName,
                bookeddate = x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().BookedDate.ToString(),
                SlotId = x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().SlotId,
                slot = db.ExamSlots.Where(s => s.SlotId == x.CandidateExamBookingDetails.Where(a => a.CandidateId == x.CandidateId).ToList().FirstOrDefault().SlotId).ToList().FirstOrDefault().SlotName
                    ,
                examiner = db.ExaminerDetails.Where(ed => ed.ExaminerId == x.ExaminerId).ToList().FirstOrDefault().FirstName

            }).ToList();
            return distributedCandDetails;
        }
    }
}