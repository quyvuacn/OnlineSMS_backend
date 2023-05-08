namespace OnlineSMS.Models
{
    public class Boxchat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string Type { get; set; }

        public string? LastMessageContent { get; set; }
        public string? LastUserSendId { get; set; }
        public string? LastUserSendFullName { get; set; }

        public ICollection<MemberBoxchat>? MemberBoxes { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<UnreadMessage>? UnreadMessages { get; set; }
    }
}
