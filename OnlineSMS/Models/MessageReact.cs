namespace OnlineSMS.Models
{
    public class MessageReact
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MessageId { get; set; }
        public DateTime SendDate { get; set; }
        public Message? Message { get; set; }
        public User? User { get; set; }
    }
}
