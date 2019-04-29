using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TADE.Models
{
    public class ExamResultsDAL
    {
        private TADEDBEntities TD = new TADEDBEntities();
       
        public int SaveScore(ExamResult ER)
        {
            TD.ExamResults.Add(ER);
            TD.SaveChanges();
            return ER.ResultId;
        }
        public int CalculateGrade(int? score)
        {
            int Grade = 0;
            if (score >= 120)
            {
                Grade = 5;
            }
            if ((score < 30 && score >= 25) || (score < 120 && score >= 100))
            {
                Grade = 4;
            }
            if ((score < 25 && score >= 20) || (score < 100 && score >= 80))
            {
                Grade = 3;
            }
            if ((score < 20 && score >= 10) || (score < 80 && score >= 40))
            {
                Grade = 2;
            }
            if ((score < 10 && score >= 1) || (score < 40 && score >= 1))
            {
                Grade = 1;
            }
            if (score <= 0)
            {
                Grade = 0;
            }
            return Grade;
        }
        public int ExamTypeId(string strExamType)
        {
            var eT = TD.ExamTypes.Where(e => e.ExamType1 == strExamType).ToList();
            int ExamId = eT[0].ExamId;
            return ExamId;
        }
        public decimal? ResponsePercentage(int? AllowedTime, int answeredTime)
        {
            int? timeTaken = AllowedTime - answeredTime;
            decimal? responsePercentage = (Convert.ToDecimal(timeTaken) / AllowedTime) * 100;
            return responsePercentage;
        }
        public int? CalculateScoreByResponseTime(decimal? responsePercentage, int? Answerscore)
        {
            int? ScoreByResponseTime = 0;
            if (responsePercentage <= 40)
            {
                ScoreByResponseTime =  (4 * Answerscore);
            }
            if (responsePercentage > 40 && responsePercentage <= 60)
            {
                ScoreByResponseTime =  (3 * Answerscore);
            }
            if (responsePercentage > 60 && responsePercentage <= 80)
            {
                ScoreByResponseTime =  (2 * Answerscore);
            }
            if (responsePercentage > 80)
            {
                ScoreByResponseTime =  Answerscore;
            }
            return ScoreByResponseTime;
        }
        
    }
}