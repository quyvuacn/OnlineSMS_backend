namespace OnlineSMS.Models
{
    public class MessageMedia
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string Path { get; set; }
        public DateTime UploadDate { get; set; }

        public Message? Message { get; set; }
    }
}
