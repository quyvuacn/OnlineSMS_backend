namespace OnlineSMS.RequestModels
{
    public class CallTo
    {
        public string? BoxchatId { get; set; }
        public string? Type { get; set; }
        public bool? IsPickUp { get; set; }
        public string? RoomId { get; set; }
        public string? UserTargetId { get; set; }
        public List<string>? ListUserTargetId { get; set; }
    }
}
