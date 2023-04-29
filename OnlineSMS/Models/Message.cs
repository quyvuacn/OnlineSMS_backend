namespace OnlineSMS.Models
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BoxchatId { get; set; }
        public string UserSendId { get; set; }
        public int Status { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Boxchat? Boxchat { get; set; }
        public User? UserSend { get; set; }

        public ICollection<MessageMedia> Media { get; set; }
        public ICollection<MessageReact> Reacts { get; set; }

    }
}
