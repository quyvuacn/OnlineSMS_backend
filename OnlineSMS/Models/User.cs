using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSMS.Models
{
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = false)]

    public class User : IdentityUser
    {
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public override string PhoneNumber { 
            get => base.PhoneNumber; 
            set=> base.PhoneNumber = value;
        }

        public override string UserName { get => base.UserName; set => base.UserName=value; }
        public override string Email { get => base.Email; set => base.Email=value; }

        public UserProfile? UserProfile { get; set; }
        public ICollection<UserService>? Services { get; set; }


        [InverseProperty("UserRequest")]
        public ICollection<Friendship>? UserRequests { get; set; }
        [InverseProperty("UserAccept")]
        public ICollection<Friendship>? UserAccepts { get; set; }
        public ICollection<MemberBoxchat>? MemberBoxes { get; set; }
        [InverseProperty("UserSend")]
        public ICollection<Message> Messages { get; set; }
        public ICollection<MessageReact> MessageReacts { get; set; }
        public ICollection<UserCuisine>? UserCuisines { get; set; }
        public ICollection<UserHobbie>? UserHobbies { get; set; }
        public ICollection<UserWorkEducation>? UserWorkEducation { get; set; }
        public ICollection<UserConnection>? UserConnection { get; set; }
        public ICollection<UnreadMessages>? UnreadMessages { get; set; }
    }
}
