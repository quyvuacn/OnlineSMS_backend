namespace OnlineSMS.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LimitDate { get; set; }
        public bool Perpetual { get; set; } = false;


        public ICollection<UserService> Users { get; set; }
    }
}
