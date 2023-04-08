namespace OnlineSMS.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string School { get; set; }
        public string College { get; set; }
        public string WorkStatus { get; set; }
        public string Organization { get; set; }
        public string Designation { get; set; }

        public User? User { get; set; }
    }
}
