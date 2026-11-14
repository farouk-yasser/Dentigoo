using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
namespace Dentigo.Infrastructure.DbContexts
{
    public class DentigoDbContext:DbContext
    {
    
        public DentigoDbContext(DbContextOptions<DentigoDbContext> options)
            : base(options) { } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DentigoDbContext).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
       public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Tooth> Teeth { get; set; }
        public DbSet<ClinicalFinding> ClinicalFindings { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<DentalCase> DentalCases { get; set; }
        public DbSet<CaseFile> CaseFiles { get; set; }
        public DbSet<DesignVersion> DesignVersions { get; set; }
        public DbSet<CaseHistoryEvent> CaseHistoryEvents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }

    }
}
