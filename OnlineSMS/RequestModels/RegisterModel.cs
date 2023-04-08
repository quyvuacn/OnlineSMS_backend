namespace OnlineSMS.RequestModels
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VerifyCode { get; set; } = "";
    }
}
