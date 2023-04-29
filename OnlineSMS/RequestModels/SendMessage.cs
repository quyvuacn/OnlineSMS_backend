namespace OnlineSMS.RequestModels
{
    public class SendMessage
    {
        public SendMessage()
        {
            Type = "message";
        }
        public string BoxChatId { get; set; }
        public string DataMessage { get; set; }
        public string Type { get; set; } 
    }
}
