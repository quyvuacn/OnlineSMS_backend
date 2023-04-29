namespace OnlineSMS.Models
{
    public class MemberBoxchat
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string BoxchatId { get; set; }
        public string? Nickname { get; set; }
        public DateTime TimeOfLastMessage { get; set; }
        public User? User { get; set; }
        public Boxchat? Boxchat { get; set; }
    }
}
