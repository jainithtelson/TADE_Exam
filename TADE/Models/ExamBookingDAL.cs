using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TADE.Models
{
    public class ExamBookingDAL
    {
        private TADEDBEntities TD = new TADEDBEntities();
        public int UpdateCandidateDetails(CandidateDetail CEB)
        {
            //TD.CandidateDetails.Add(CEB);
            TD.SaveChanges();
            return CEB.CandidateId;
        }
    }
}