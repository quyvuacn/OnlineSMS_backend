namespace OnlineSMS.RequestModels
{
    public class RequestResult
    {
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
        public object? Data { get; set; } = new {};
    }
}
