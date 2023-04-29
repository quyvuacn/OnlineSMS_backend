using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSMS.Models
{
    [Table("VerificationCodes")]
    public class VerificationCode
    {
        public VerificationCode(string phoneNumber)
        {
            CreatedTime = DateTime.Now;
            ExpirationTime = DateTime.Now.AddMinutes(5);
            PhoneLockoutEnabled = false;
            VerificationCount = 0;
            PhoneNumber = phoneNumber;
            newVerificationCode();
        }
        public int Id { get; set; }
        public string PhoneNumber { get; set; }

        public string Code { get; set;}
        public int VerificationCount { get; set; } 
        public DateTime? UsedTime { get; set; }
        public DateTime CreatedTime { get; set; } 
        public DateTime ExpirationTime { get; set; } 
        public bool PhoneLockoutEnabled { get; set; } 
        public DateTime PhoneLockoutEnd { get; set; } 

        public void newVerificationCode()
        {
            Random random = new Random();
            Code = random.Next(0, 999999).ToString("D6");
            CreatedTime = DateTime.Now;
            UsedTime = null;
            ExpirationTime = DateTime.Now.AddMinutes(5);
            VerificationCount++;
            if (VerificationCount > 5)
            {
                PhoneLockoutEnabled = true;
            }
            PhoneLockoutEnd = DateTime.Now.AddDays(1);
        }

        public bool phoneUnlockout()
        {
            if(PhoneLockoutEnd < DateTime.Now)
            {
                VerificationCount = 0;
                PhoneLockoutEnabled = false;
                return true;
            }
            return false;
        }
    }
}
