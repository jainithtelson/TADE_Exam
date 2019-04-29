using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TADE.Models
{
    public class CandidateDetailsDAL
    {
        private TADEDBEntities db = new TADEDBEntities();
        public List<CandidateDetail> CandidateDetails()
        {
            return db.CandidateDetails.ToList();
        }
        public CandidateDetail CandidateDetailsById(int? id)
        {
            CandidateDetail candidateDetail = db.CandidateDetails.Find(id);
            return candidateDetail;
        }
        public bool SaveCandidateDetails(CandidateDetail candidateDetail)
        {
           try
            {
                db.CandidateDetails.Add(candidateDetail);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public bool UpdateCandidateDetail(CandidateDetail candidateDetail)
        {
            try
            {
                db.Entry(candidateDetail).State = EntityState.Modified;
                db.SaveChanges();
                return true; }
            catch(Exception ex) { return false; }
        }
        public bool RemoveCandidateDetail(CandidateDetail candidateDetail)
        {
            try
            {
                db.CandidateDetails.Remove(candidateDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }
            
        }
        public AvailableDate AvailableDateByDate(DateTime dtAvailabledate)
        {
            AvailableDate chkAvailabledate = db.AvailableDates.Where(ad => ad.AvailableDate1 == dtAvailabledate).ToList().FirstOrDefault();
            return chkAvailabledate;
        }
        public List<AvailableSeatsForDate> AvailableSeatsForDate(int? availableDatesId)
        {
            List<AvailableSeatsForDate> chkavailableSeatsForDate = db.AvailableSeatsForSlots.Where(ad => ad.AvailableDatesId == availableDatesId).Select(ed => new AvailableSeatsForDate()
            {
                SlotId = ed.SlotId,
                SlotName = ed.ExamSlot.SlotName + " (Seats left: " + ed.RemainingSeats + ")"
            }).ToList();
            return chkavailableSeatsForDate;
        }
        public CandidateExamBookingDetail CandidateExamBookingDetailByCandidateId(int CandidateId)
        {
            CandidateExamBookingDetail candidateExamBookingDetail = db.CandidateExamBookingDetails.Where(a => a.CandidateId.Equals(CandidateId)).FirstOrDefault();
            return candidateExamBookingDetail;
        }
        public bool SaveCandidateExamBookingDetails(CandidateExamBookingDetail candidateExamBookingDetail)
        {
            try
            {
                db.CandidateExamBookingDetails.Add(candidateExamBookingDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }
           
        }
        public bool UpdateCandidateExamBookingDetails(CandidateExamBookingDetail candidateExamBookingDetail)
        {
            try
            {
                db.Entry(candidateExamBookingDetail).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }

        }
        public ExaminerDetail ExaminerDetailByStatus()
        {
            return db.ExaminerDetails.Where(a => a.Status == true && a.Emergency == true).ToList().FirstOrDefault();
        }
        public AvailableSeatsForDate AvailableSeatsForDateBySlotId(int? AvailableDatesId,int SlotId)
        {
            AvailableSeatsForDate chkavailableSeatsForDate = db.AvailableSeatsForSlots.Where(ad => ad.AvailableDatesId == AvailableDatesId && ad.SlotId == SlotId).Select(ed => new AvailableSeatsForDate()
            {
                availableSeatsForSlotsId = ed.AvailableSeatsForSlotsId,
                TotalSeats = ed.TotalSeats,
                RemainingSeats = ed.RemainingSeats
            }).ToList().FirstOrDefault();
            return chkavailableSeatsForDate;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}