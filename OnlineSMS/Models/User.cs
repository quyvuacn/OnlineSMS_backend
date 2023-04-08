using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OnlineSMS.Models
{
    public class User : IdentityUser
    {
        public override string Id { get; set; } = Guid.NewGuid().ToString();
        public override string UserName { get; set; }

        public ICollection<UserService>? Services { get; set; }


        [InverseProperty("UserRequest")]
        public ICollection<Friendship>? UserRequests { get; set; }
        [InverseProperty("UserAccept")]
        public ICollection<Friendship>? UserAccepts { get; set; }
        public ICollection<UserProfile>? UserProfiles { get; set; }
        public ICollection<MemberBoxchat>? MemberBoxes { get; set; }
        [InverseProperty("UserSend")]
        public ICollection<Message> Messages { get; set; }
        public ICollection<MessageReact> MessageReacts { get; set; }
    }
}
