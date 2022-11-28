using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Extensions;

namespace Travel.Context.Models.Notification
{
    public class NotificationContext : DbContext
    {
        public NotificationContext()
        {
        }

        public NotificationContext(DbContextOptions<NotificationContext> options)
            : base(options)
        {
        }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<ReportTourBooking> ReportTourBooking { get; set; }
        public DbSet<ReportWeek> ReportWeek { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.IdComment);
                entity.Property(e => e.NameCustomer).HasMaxLength(50);
                entity.Property(e => e.CommentText).HasMaxLength(1000);
                entity.Property(e => e.IdTour).HasMaxLength(50);

            });
            modelBuilder.Entity<ReportTourBooking>(entity =>
            {
                entity.HasKey(e => e.IdReportTourBooking);
                entity.Property(e => e.NameTour).HasMaxLength(100);
                entity.Property(e => e.IdTour).HasMaxLength(50);

            });
            modelBuilder.Entity<ReportWeek>(entity =>
            {
                entity.HasKey(e => e.IdWeek);
            });
            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasKey(e => e.IdNotification);
                entity.Property(e => e.Title).HasMaxLength(50);
                entity.Property(e => e.Content).HasMaxLength(500);
            });
        }

    }
}