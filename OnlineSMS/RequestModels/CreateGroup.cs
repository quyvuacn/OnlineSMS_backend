namespace OnlineSMS.RequestModels
{
    public class CreateGroup
    {
        public List<string> Members { get; set; }
        public string? Avatar { get; set; }
        public string GroupName { get; set; }
    }
}
