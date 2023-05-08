namespace OnlineSMS.Models
{
    public class UnreadMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MessageId { get; set; }
        public string BoxchatId { get; set; }
        public string UserId { get; set; }
    }
}
