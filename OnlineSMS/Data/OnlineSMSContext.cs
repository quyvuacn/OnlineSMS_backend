using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Models;

namespace OnlineSMS.Data
{
    public class OnlineSMSContext : IdentityDbContext<User>
    {
        public OnlineSMSContext (DbContextOptions<OnlineSMSContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<Friendship>()
               .HasOne(f => f.UserRequest)
               .WithMany(u=> u.UserRequests)
               .HasForeignKey(f => f.UserRequestId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserAccept)
                .WithMany(u=>u.UserAccepts)
                .HasForeignKey(f => f.UserAcceptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageReact>()
                .HasOne(mr => mr.Message)
                .WithMany(m=>m.Reacts)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.NoAction);
        }




        public DbSet<VerificationCode> VerificationCode { get; set; } = default!;
        public DbSet<Service> Service { get; set; } = default!;
        public DbSet<UserService> UserService { get; set; } = default!;
    }
}
