﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TADE.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TADEDBEntities : DbContext
    {
        public TADEDBEntities()
            : base("name=TADEDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<VideoType> VideoTypes { get; set; }
        public virtual DbSet<VideoScore> VideoScores { get; set; }
        public virtual DbSet<ExamType> ExamTypes { get; set; }
        public virtual DbSet<AnswerOption> AnswerOptions { get; set; }
        public virtual DbSet<ObjectiveTest> ObjectiveTests { get; set; }
        public virtual DbSet<QuestionType> QuestionTypes { get; set; }
        public virtual DbSet<ExamResult> ExamResults { get; set; }
        public virtual DbSet<ExaminorAvailability> ExaminorAvailabilities { get; set; }
        public virtual DbSet<CandidateExamBookingDetail> CandidateExamBookingDetails { get; set; }
        public virtual DbSet<AvailableDate> AvailableDates { get; set; }
        public virtual DbSet<AvailableDatesHistory> AvailableDatesHistories { get; set; }
        public virtual DbSet<AvailableSeatsForSlot> AvailableSeatsForSlots { get; set; }
        public virtual DbSet<ExamSlot> ExamSlots { get; set; }
        public virtual DbSet<ExaminerDetail> ExaminerDetails { get; set; }
        public virtual DbSet<CandidateVerification> CandidateVerifications { get; set; }
        public virtual DbSet<Testimonial> Testimonials { get; set; }
        public virtual DbSet<LatestNew> LatestNews { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<ExamResultsDummy> ExamResultsDummies { get; set; }
        public virtual DbSet<CandidateDetail> CandidateDetails { get; set; }
        public virtual DbSet<CompanyDetail> CompanyDetails { get; set; }
    }
}