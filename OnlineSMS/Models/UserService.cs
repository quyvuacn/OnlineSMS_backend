namespace OnlineSMS.Models
{
    public class UserService
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User? User { get; set; }
        public Service? Service { get; set; }
    }
}
