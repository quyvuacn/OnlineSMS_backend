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

            modelBuilder.Entity<Message>()
                .HasOne(f => f.UserSend)
                .WithMany(u => u.Messages)
                .HasForeignKey(f => f.UserSendId);
        }




        public DbSet<VerificationCode> VerificationCode { get; set; } = default!;
        public DbSet<Service> Service { get; set; } = default!;
        public DbSet<UserService> UserService { get; set; } = default!;
        public DbSet<UserProfile> UserProfile { get; set; } = default!;
        public DbSet<UserHobbie> UserHobbie { get; set; } = default!;
        public DbSet<UserCuisine> UserCuisine { get; set; } = default!;
        public DbSet<Friendship> Friendship { get; set; } = default!;
        public DbSet<UserConnection> UserConnection { get; set; } = default!;
        public DbSet<Boxchat> Boxchat { get; set; } = default!;
        public DbSet<MemberBoxchat> MemberBoxchat { get; set; } = default!;
        public DbSet<Message> Message { get; set; } = default!;
        public DbSet<UnreadMessage> UnreadMessage { get; set; } = default!;
    }
}
