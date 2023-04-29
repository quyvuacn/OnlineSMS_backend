using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace OnlineSMS.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public string Address { get; set; }
        public string? Avatar { get; set; }

        public User? User { get; set; }
        

    }
}
