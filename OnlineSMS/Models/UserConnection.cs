namespace OnlineSMS.Models
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public string SessionId { get; set; }
        
        //public string Status { get; set; }
        public User? User { get; set; }
    }
}
