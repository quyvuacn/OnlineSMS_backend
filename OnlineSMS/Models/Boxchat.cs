namespace OnlineSMS.Models
{
    public class Boxchat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        //public string? Avatar { get; set; }
        public string Type { get; set; }

        public ICollection<MemberBoxchat>? MemberBoxes { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
